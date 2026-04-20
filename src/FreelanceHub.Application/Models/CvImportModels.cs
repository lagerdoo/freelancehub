namespace FreelanceHub.Application.Models;

public sealed class CvImportCreateRequest
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Stream Content { get; set; } = Stream.Null;
}

public sealed class CvImportRecordSummary
{
    public Guid Id { get; init; }
    public string OriginalFileName { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? AppliedAtUtc { get; init; }
    public bool IsDeleted { get; init; }
}

public sealed class CvImportDraft
{
    public Guid ImportId { get; init; }
    public string OriginalFileName { get; init; } = string.Empty;
    public string ExtractedTextPreview { get; init; } = string.Empty;
    public CvImportProfileSuggestion Profile { get; init; } = new();
    public IReadOnlyList<string> Skills { get; init; } = Array.Empty<string>();
    public IReadOnlyList<CvImportExperienceSuggestion> ExperienceEntries { get; init; } = Array.Empty<CvImportExperienceSuggestion>();
    public IReadOnlyList<CvImportServiceSuggestion> Services { get; init; } = Array.Empty<CvImportServiceSuggestion>();
    public IReadOnlyList<CvImportProjectSuggestion> Projects { get; init; } = Array.Empty<CvImportProjectSuggestion>();
}

public sealed class CvImportProfileSuggestion
{
    public string SiteName { get; init; } = string.Empty;
    public string TaglineFr { get; init; } = string.Empty;
    public string TaglineEn { get; init; } = string.Empty;
    public string AboutSummaryFr { get; init; } = string.Empty;
    public string AboutSummaryEn { get; init; } = string.Empty;
    public string SkillsFr { get; init; } = string.Empty;
    public string SkillsEn { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string LinkedInUrl { get; init; } = string.Empty;
    public string GitHubUrl { get; init; } = string.Empty;
}

public sealed class CvImportExperienceSuggestion
{
    public string RoleFr { get; init; } = string.Empty;
    public string RoleEn { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? EndDate { get; init; }
    public string DescriptionFr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public string Technologies { get; init; } = string.Empty;
}

public sealed class CvImportServiceSuggestion
{
    public string TitleFr { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string SummaryFr { get; init; } = string.Empty;
    public string SummaryEn { get; init; } = string.Empty;
    public string DescriptionFr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public string IconClass { get; init; } = "bi bi-briefcase";
}

public sealed class CvImportProjectSuggestion
{
    public string TitleFr { get; init; } = string.Empty;
    public string TitleEn { get; init; } = string.Empty;
    public string SummaryFr { get; init; } = string.Empty;
    public string SummaryEn { get; init; } = string.Empty;
    public string DescriptionFr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public string TechStack { get; init; } = string.Empty;
}
