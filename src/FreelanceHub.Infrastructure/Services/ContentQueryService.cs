using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Services;

public sealed class ContentQueryService(FreelanceHubDbContext dbContext) : IContentQueryService
{
    public async Task<SiteSetting> GetSiteSettingAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SiteSettings.AsNoTracking().OrderBy(x => x.CreatedAtUtc).FirstOrDefaultAsync(cancellationToken)
            ?? new SiteSetting
            {
                SiteName = "FreelanceHub",
                TaglineFr = "Consultant backend .NET",
                TaglineEn = "Freelance .NET backend consultant",
                HeroTitleFr = "Architecture .NET et delivery fiable",
                HeroTitleEn = "Reliable .NET architecture and delivery",
                HeroSubtitleFr = "J'aide les entreprises a concevoir, moderniser et fiabiliser leurs plateformes.",
                HeroSubtitleEn = "I help companies design, modernize, and stabilize their platforms.",
                ContactEmail = "contact@example.com",
                Location = "Paris, France",
                FooterTextFr = "Concu pour des missions backend et architecture exigeantes.",
                FooterTextEn = "Built for demanding backend and architecture engagements.",
                MetaTitleFr = "Consultant .NET freelance",
                MetaTitleEn = "Freelance .NET consultant",
                MetaDescriptionFr = "Developpement backend, architecture, performance et accompagnement technique.",
                MetaDescriptionEn = "Backend engineering, architecture, performance, and technical consulting."
            };
    }

    public async Task<PublicSiteSnapshot> GetHomeSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var siteSetting = await GetSiteSettingAsync(cancellationToken);
        var services = await GetPublishedServicesAsync(cancellationToken);
        var featuredProjects = await GetPublishedProjectsAsync(true, cancellationToken);
        var experienceEntries = await GetVisibleExperienceEntriesAsync(cancellationToken);

        return new PublicSiteSnapshot
        {
            SiteSetting = siteSetting,
            Services = services.Take(3).ToList(),
            FeaturedProjects = featuredProjects.Take(3).ToList(),
            ExperienceEntries = experienceEntries.Take(3).ToList()
        };
    }

    public async Task<IReadOnlyList<Service>> GetPublishedServicesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Services
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.TitleEn)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetPublishedProjectsAsync(bool featuredOnly = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = dbContext.Projects.AsNoTracking().Where(x => x.IsPublished);
        if (featuredOnly)
        {
            query = query.Where(x => x.IsFeatured);
        }

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.TitleEn)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExperienceEntry>> GetVisibleExperienceEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ExperienceEntries
            .AsNoTracking()
            .Where(x => x.IsVisible)
            .OrderByDescending(x => x.StartDate)
            .ThenBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task SubmitContactMessageAsync(ContactFormRequest request, CancellationToken cancellationToken = default)
    {
        dbContext.ContactMessages.Add(new ContactMessage
        {
            Name = ContactInputSanitizer.SanitizeSingleLine(request.Name, 120),
            Email = ContactInputSanitizer.SanitizeSingleLine(request.Email, 256),
            Subject = ContactInputSanitizer.SanitizeSingleLine(request.Subject, 140),
            Message = ContactInputSanitizer.SanitizeMultiLine(request.Message, 3000)
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
