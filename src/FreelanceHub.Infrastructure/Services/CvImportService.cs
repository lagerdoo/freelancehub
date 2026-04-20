using System.Text.RegularExpressions;
using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Infrastructure.Data;
using FreelanceHub.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UglyToad.PdfPig;

namespace FreelanceHub.Infrastructure.Services;

public sealed partial class CvImportService(
    FreelanceHubDbContext dbContext,
    IHostEnvironment hostEnvironment,
    IOptions<CvImportOptions> options) : ICvImportService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/x-pdf",
        "application/acrobat",
        "applications/vnd.pdf",
        "text/pdf",
        "text/x-pdf"
    };

    private static readonly string[] SummaryHeadings =
    [
        "summary",
        "profile",
        "about",
        "professional summary",
        "executive summary"
    ];

    private static readonly string[] SkillsHeadings =
    [
        "skills",
        "technical skills",
        "core skills",
        "technologies",
        "competencies"
    ];

    private static readonly string[] ExperienceHeadings =
    [
        "experience",
        "professional experience",
        "work experience",
        "employment history"
    ];

    private static readonly string[] ProjectHeadings =
    [
        "projects",
        "selected projects",
        "key projects",
        "engagements"
    ];

    public async Task<IReadOnlyList<CvImportRecordSummary>> GetImportsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.CvImportRecords
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new CvImportRecordSummary
            {
                Id = x.Id,
                OriginalFileName = x.OriginalFileName,
                FileSize = x.FileSize,
                CreatedAtUtc = x.CreatedAtUtc,
                AppliedAtUtc = x.AppliedAtUtc,
                IsDeleted = x.DeletedAtUtc.HasValue
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CvImportDraft> CreateDraftAsync(CvImportCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var extension = Path.GetExtension(request.OriginalFileName);
        var storageRoot = GetStorageRoot();
        var yearMonthFolder = Path.Combine(storageRoot, DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        Directory.CreateDirectory(yearMonthFolder);

        var storedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var absolutePath = Path.Combine(yearMonthFolder, storedFileName);

        await EnsureValidPdfSignatureAsync(request.Content, cancellationToken);

        await using (var output = File.Create(absolutePath))
        {
            await request.Content.CopyToAsync(output, cancellationToken);
        }

        var extractedText = ExtractTextFromPdf(absolutePath);
        if (string.IsNullOrWhiteSpace(extractedText))
        {
            File.Delete(absolutePath);
            throw new InvalidOperationException("The uploaded PDF did not contain extractable text. Scanned PDFs are not supported in this first version.");
        }

        var entity = new CvImportRecord
        {
            OriginalFileName = request.OriginalFileName.Trim(),
            StoredFileName = storedFileName,
            ContentType = request.ContentType.Trim(),
            FileSize = request.FileSize,
            StoragePath = Path.GetRelativePath(hostEnvironment.ContentRootPath, absolutePath),
            ExtractedText = extractedText.Trim()
        };

        dbContext.CvImportRecords.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return BuildDraft(entity);
    }

    public async Task<CvImportDraft?> GetDraftAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.CvImportRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAtUtc == null, cancellationToken);

        return entity is null ? null : BuildDraft(entity);
    }

    public async Task MarkAppliedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.CvImportRecords.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.AppliedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteImportAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.CvImportRecords.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || entity.DeletedAtUtc.HasValue)
        {
            return;
        }

        var absolutePath = Path.Combine(hostEnvironment.ContentRootPath, entity.StoragePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        entity.DeletedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private CvImportDraft BuildDraft(CvImportRecord entity)
    {
        var text = entity.ExtractedText;
        var lines = SplitLines(text);
        var profile = BuildProfileSuggestion(lines, text);
        var skills = ExtractSkills(lines, text);
        var experiences = ExtractExperienceSuggestions(text, skills);
        var services = BuildServiceSuggestions(text, skills);
        var projects = ExtractProjectSuggestions(text);

        return new CvImportDraft
        {
            ImportId = entity.Id,
            OriginalFileName = entity.OriginalFileName,
            ExtractedTextPreview = BuildPreview(text, 3500),
            Profile = profile,
            Skills = skills,
            ExperienceEntries = experiences,
            Services = services,
            Projects = projects
        };
    }

    private CvImportProfileSuggestion BuildProfileSuggestion(IReadOnlyList<string> lines, string text)
    {
        var headerLines = lines.Take(12).ToList();
        var siteName = headerLines.FirstOrDefault(IsLikelyNameLine) ?? "Freelance Profile";
        var taglineEn = headerLines.FirstOrDefault(line => line != siteName && !LooksLikeContactLine(line) && line.Length <= 100)
            ?? "Freelance developer and technical consultant";
        var aboutSummary = ExtractSectionParagraph(lines, SummaryHeadings)
            ?? ExtractFirstParagraph(text)
            ?? "Replace this summary with the consultant profile extracted from the CV.";

        var skills = ExtractSkills(lines, text);
        var location = headerLines.FirstOrDefault(IsLikelyLocationLine) ?? string.Empty;
        var email = EmailRegex().Match(text) is { Success: true } emailMatch ? emailMatch.Value : string.Empty;
        var linkedInUrl = UrlRegex().Matches(text).Select(x => x.Value).FirstOrDefault(x => x.Contains("linkedin", StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
        var githubUrl = UrlRegex().Matches(text).Select(x => x.Value).FirstOrDefault(x => x.Contains("github", StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
        var skillsCsv = string.Join(", ", skills);

        return new CvImportProfileSuggestion
        {
            SiteName = siteName,
            TaglineEn = taglineEn,
            TaglineFr = taglineEn,
            AboutSummaryEn = aboutSummary,
            AboutSummaryFr = aboutSummary,
            SkillsEn = skillsCsv,
            SkillsFr = skillsCsv,
            ContactEmail = email,
            Location = location,
            LinkedInUrl = linkedInUrl,
            GitHubUrl = githubUrl
        };
    }

    private IReadOnlyList<string> ExtractSkills(IReadOnlyList<string> lines, string text)
    {
        var section = ExtractSectionLines(lines, SkillsHeadings);
        var source = section.Count > 0 ? section : lines.Where(line => line.Contains(",") && line.Length <= 180).Take(5).ToList();
        var skills = new List<string>();

        foreach (var line in source)
        {
            foreach (var part in line.Split([",", "•", "|", ";"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (part.Length is < 2 or > 40)
                {
                    continue;
                }

                if (LooksLikeSectionHeading(part) || LooksLikeContactLine(part))
                {
                    continue;
                }

                if (!skills.Contains(part, StringComparer.OrdinalIgnoreCase))
                {
                    skills.Add(part);
                }
            }
        }

        if (skills.Count == 0)
        {
            foreach (Match match in SkillTokenRegex().Matches(text))
            {
                var skill = match.Value.Trim();
                if (!skills.Contains(skill, StringComparer.OrdinalIgnoreCase))
                {
                    skills.Add(skill);
                }
            }
        }

        return skills.Take(12).ToList();
    }

    private IReadOnlyList<CvImportExperienceSuggestion> ExtractExperienceSuggestions(string text, IReadOnlyList<string> skills)
    {
        var sectionText = ExtractSectionText(text, ExperienceHeadings) ?? text;
        var blocks = SplitBlocks(sectionText)
            .Where(block => YearRegex().IsMatch(block))
            .Take(6)
            .ToList();

        var results = new List<CvImportExperienceSuggestion>();
        foreach (var block in blocks)
        {
            var blockLines = SplitLines(block);
            if (blockLines.Count < 2)
            {
                continue;
            }

            var roleLine = blockLines[0];
            var companyLine = blockLines.Skip(1).FirstOrDefault() ?? string.Empty;
            var (startDate, endDate) = ParseDateRange(block);
            var descriptionLines = blockLines.Skip(2).Where(line => !LooksLikeDateLine(line)).ToList();
            if (descriptionLines.Count == 0)
            {
                descriptionLines = blockLines.Skip(1).Where(line => !LooksLikeDateLine(line)).ToList();
            }

            var description = string.Join(" ", descriptionLines).Trim();
            if (string.IsNullOrWhiteSpace(description))
            {
                description = block.Trim();
            }

            var (company, location) = SplitCompanyAndLocation(companyLine);
            var technologies = string.Join(", ", skills.Where(skill => block.Contains(skill, StringComparison.OrdinalIgnoreCase)).Take(8));

            results.Add(new CvImportExperienceSuggestion
            {
                RoleEn = roleLine,
                RoleFr = roleLine,
                Company = company,
                Location = location,
                StartDate = startDate ?? DateOnly.FromDateTime(DateTime.Today),
                EndDate = endDate,
                DescriptionEn = BuildPreview(description, 1200),
                DescriptionFr = BuildPreview(description, 1200),
                Technologies = technologies
            });
        }

        return results;
    }

    private IReadOnlyList<CvImportServiceSuggestion> BuildServiceSuggestions(string text, IReadOnlyList<string> skills)
    {
        var lowerText = text.ToLowerInvariant();
        var scored = new List<(int Score, CvImportServiceSuggestion Suggestion)>
        {
            (ScoreKeywords(lowerText, "architecture", "api", "microservice", "backend", "asp.net"), new CvImportServiceSuggestion
            {
                TitleEn = "Backend architecture and delivery support",
                TitleFr = "Architecture backend et accompagnement delivery",
                SummaryEn = "Architecture guidance, API structure, service boundaries, and practical engineering decisions.",
                SummaryFr = "Cadrage d'architecture, structure d'API, frontieres de services et decisions techniques pragmatiques.",
                DescriptionEn = "Suggested from the CV profile for consultants who work on backend architecture, maintainability, and delivery structure.",
                DescriptionFr = "Suggestion issue du CV pour les profils qui interviennent sur l'architecture backend, la maintenabilite et la structuration du delivery.",
                IconClass = "bi bi-diagram-3"
            }),
            (ScoreKeywords(lowerText, "migration", "legacy", "modernization", "upgrade", "refactor", "monolith"), new CvImportServiceSuggestion
            {
                TitleEn = "Application modernization",
                TitleFr = "Modernisation applicative",
                SummaryEn = "Incremental modernization of legacy applications without blocking delivery.",
                SummaryFr = "Modernisation progressive d'applications existantes sans bloquer le delivery.",
                DescriptionEn = "Suggested because the CV mentions migrations, recovery work, or restructuring legacy systems.",
                DescriptionFr = "Suggestion retenue car le CV mentionne des migrations, des reprises ou de la restructuration d'existant.",
                IconClass = "bi bi-arrow-repeat"
            }),
            (ScoreKeywords(lowerText, "performance", "reliability", "incident", "observability", "postgresql", "sql", "database"), new CvImportServiceSuggestion
            {
                TitleEn = "Reliability and performance review",
                TitleFr = "Audit performance et fiabilite",
                SummaryEn = "Review of production bottlenecks, delivery risks, and high-impact operational issues.",
                SummaryFr = "Analyse des points de friction de production, des risques delivery et des enjeux d'exploitation prioritaires.",
                DescriptionEn = "Suggested when the CV points to performance tuning, data-heavy platforms, or production stabilization work.",
                DescriptionFr = "Suggestion retenue lorsque le CV met en avant la performance, les sujets data ou la stabilisation de production.",
                IconClass = "bi bi-speedometer2"
            }),
            (ScoreKeywords(lowerText, "integration", "synchronization", "crm", "erp", "queue", "message", "event"), new CvImportServiceSuggestion
            {
                TitleEn = "System integration and workflow automation",
                TitleFr = "Integration systemes et automatisation",
                SummaryEn = "Design of integration APIs, synchronization flows, and resilient business automation.",
                SummaryFr = "Conception d'APIs d'integration, de flux de synchronisation et d'automatisations resilientes.",
                DescriptionEn = "Suggested from integration-related technologies or projects identified in the CV.",
                DescriptionFr = "Suggestion basee sur les integrations, flux ou outils de synchronisation identifies dans le CV.",
                IconClass = "bi bi-shuffle"
            })
        };

        return scored
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Suggestion.TitleEn)
            .Take(3)
            .Select(x => x.Suggestion)
            .ToList();
    }

    private IReadOnlyList<CvImportProjectSuggestion> ExtractProjectSuggestions(string text)
    {
        var sectionText = ExtractSectionText(text, ProjectHeadings);
        if (string.IsNullOrWhiteSpace(sectionText))
        {
            return Array.Empty<CvImportProjectSuggestion>();
        }

        var blocks = SplitBlocks(sectionText)
            .Where(block => block.Length > 80)
            .Take(3)
            .ToList();

        return blocks.Select(block =>
        {
            var lines = SplitLines(block);
            var title = lines.FirstOrDefault() ?? "Imported project suggestion";
            var details = string.Join(" ", lines.Skip(1)).Trim();
            var techStack = string.Join(", ", SkillTokenRegex().Matches(block).Select(x => x.Value).Distinct(StringComparer.OrdinalIgnoreCase).Take(8));

            return new CvImportProjectSuggestion
            {
                TitleEn = title,
                TitleFr = title,
                SummaryEn = BuildPreview(details, 320),
                SummaryFr = BuildPreview(details, 320),
                DescriptionEn = BuildPreview(details, 1200),
                DescriptionFr = BuildPreview(details, 1200),
                TechStack = techStack
            };
        }).ToList();
    }

    private void ValidateRequest(CvImportCreateRequest request)
    {
        if (request.FileSize <= 0 || request.FileSize > options.Value.MaxFileSizeBytes)
        {
            throw new InvalidOperationException("The CV file size is invalid.");
        }

        var extension = Path.GetExtension(request.OriginalFileName);
        if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only PDF resumes are supported in this first version.");
        }

        if (!AllowedContentTypes.Contains(request.ContentType) && !string.IsNullOrWhiteSpace(request.ContentType))
        {
            throw new InvalidOperationException("The uploaded file must be a PDF document.");
        }
    }

    private string GetStorageRoot()
    {
        var folder = options.Value.StorageFolder.Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(hostEnvironment.ContentRootPath, folder);
    }

    private static async Task EnsureValidPdfSignatureAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (!stream.CanSeek)
        {
            throw new InvalidOperationException("The uploaded CV stream must be seekable.");
        }

        var buffer = new byte[5];
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        stream.Position = 0;

        if (bytesRead < 5 || buffer[0] != 0x25 || buffer[1] != 0x50 || buffer[2] != 0x44 || buffer[3] != 0x46 || buffer[4] != 0x2D)
        {
            throw new InvalidOperationException("The uploaded file does not look like a valid PDF.");
        }
    }

    private static string ExtractTextFromPdf(string absolutePath)
    {
        using var document = PdfDocument.Open(absolutePath);
        return string.Join(Environment.NewLine + Environment.NewLine, document.GetPages().Select(page => page.Text));
    }

    private static IReadOnlyList<string> SplitLines(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }

    private static IEnumerable<string> SplitBlocks(string text)
    {
        return text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(block => !string.IsNullOrWhiteSpace(block));
    }

    private static List<string> ExtractSectionLines(IReadOnlyList<string> lines, string[] headings)
    {
        var startIndex = FindSectionStart(lines, headings);
        if (startIndex < 0)
        {
            return [];
        }

        var results = new List<string>();
        for (var index = startIndex + 1; index < lines.Count; index++)
        {
            if (LooksLikeSectionHeading(lines[index]))
            {
                break;
            }

            results.Add(lines[index]);
        }

        return results;
    }

    private static string? ExtractSectionText(string text, string[] headings)
    {
        var lines = SplitLines(text);
        var sectionLines = ExtractSectionLines(lines, headings);
        return sectionLines.Count == 0 ? null : string.Join(Environment.NewLine, sectionLines);
    }

    private static string? ExtractSectionParagraph(IReadOnlyList<string> lines, string[] headings)
    {
        var sectionLines = ExtractSectionLines(lines, headings);
        return sectionLines.Count == 0 ? null : BuildPreview(string.Join(" ", sectionLines), 900);
    }

    private static int FindSectionStart(IReadOnlyList<string> lines, string[] headings)
    {
        for (var index = 0; index < lines.Count; index++)
        {
            if (headings.Any(heading => NormalizeLine(lines[index]).Equals(heading, StringComparison.OrdinalIgnoreCase)))
            {
                return index;
            }
        }

        return -1;
    }

    private static string? ExtractFirstParagraph(string text)
    {
        return SplitBlocks(text)
            .Select(block => BuildPreview(block.Trim(), 900))
            .FirstOrDefault(block => block.Length > 80 && !LooksLikeContactLine(block));
    }

    private static string NormalizeLine(string line)
    {
        return line.Trim().Trim(':').ToLowerInvariant();
    }

    private static bool LooksLikeSectionHeading(string line)
    {
        var normalized = NormalizeLine(line);
        return SummaryHeadings.Contains(normalized) ||
               SkillsHeadings.Contains(normalized) ||
               ExperienceHeadings.Contains(normalized) ||
               ProjectHeadings.Contains(normalized) ||
               normalized is "education" or "contact" or "certifications";
    }

    private static bool LooksLikeContactLine(string line)
    {
        return EmailRegex().IsMatch(line) || UrlRegex().IsMatch(line) || PhoneRegex().IsMatch(line);
    }

    private static bool IsLikelyNameLine(string line)
    {
        if (line.Length is < 4 or > 60 || LooksLikeContactLine(line))
        {
            return false;
        }

        return line.All(character => char.IsLetter(character) || character is ' ' or '-' or '\'');
    }

    private static bool IsLikelyLocationLine(string line)
    {
        return !LooksLikeContactLine(line) &&
               line.Length <= 80 &&
               (line.Contains("remote", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("france", StringComparison.OrdinalIgnoreCase) ||
                line.Contains("europe", StringComparison.OrdinalIgnoreCase) ||
                line.Contains(','));
    }

    private static bool LooksLikeDateLine(string line)
    {
        return YearRegex().IsMatch(line);
    }

    private static (DateOnly? StartDate, DateOnly? EndDate) ParseDateRange(string text)
    {
        var years = YearRegex().Matches(text).Select(match => int.Parse(match.Value)).ToList();
        if (years.Count == 0)
        {
            return (null, null);
        }

        var start = new DateOnly(years[0], 1, 1);
        DateOnly? end = years.Count > 1 ? new DateOnly(years[1], 12, 1) : null;
        if (Regex.IsMatch(text, "present|current|aujourd'hui|presente?", RegexOptions.IgnoreCase))
        {
            end = null;
        }

        return (start, end);
    }

    private static (string Company, string Location) SplitCompanyAndLocation(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return (string.Empty, string.Empty);
        }

        var parts = input.Split(["|", "•", " - "], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => (string.Empty, string.Empty),
            1 => (parts[0], string.Empty),
            _ => (parts[0], parts[^1])
        };
    }

    private static int ScoreKeywords(string lowerText, params string[] keywords)
    {
        return keywords.Sum(keyword => lowerText.Contains(keyword, StringComparison.Ordinal) ? 1 : 0);
    }

    private static string BuildPreview(string text, int maxLength)
    {
        var normalized = Regex.Replace(text, @"\s+", " ").Trim();
        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        return normalized[..(maxLength - 1)] + "…";
    }

    [GeneratedRegex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"https?://[^\s]+", RegexOptions.IgnoreCase)]
    private static partial Regex UrlRegex();

    [GeneratedRegex(@"\+?\d[\d\s().-]{7,}\d", RegexOptions.IgnoreCase)]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"\b(19|20)\d{2}\b")]
    private static partial Regex YearRegex();

    [GeneratedRegex(@"\b(?:\.NET|ASP\.NET Core|ASP\.NET|C#|SQL Server|PostgreSQL|Redis|Azure|AWS|Docker|Kubernetes|RabbitMQ|Entity Framework|EF Core|OpenTelemetry|Hangfire|REST|GraphQL|React|Angular|Vue|TypeScript|JavaScript|CI/CD)\b", RegexOptions.IgnoreCase)]
    private static partial Regex SkillTokenRegex();
}
