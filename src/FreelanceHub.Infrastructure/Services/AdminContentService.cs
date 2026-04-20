using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Services;

public sealed class AdminContentService(FreelanceHubDbContext dbContext, EmailDeliveryService emailDeliveryService) : IAdminContentService
{
    public async Task<AdminDashboardSummary> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return new AdminDashboardSummary
        {
            PublishedServices = await dbContext.Services.CountAsync(x => x.IsPublished, cancellationToken),
            PublishedProjects = await dbContext.Projects.CountAsync(x => x.IsPublished, cancellationToken),
            VisibleExperienceEntries = await dbContext.ExperienceEntries.CountAsync(x => x.IsVisible, cancellationToken),
            UnreadContactMessages = await dbContext.ContactMessages.CountAsync(x => !x.IsRead && !x.IsArchived, cancellationToken),
            RecentMessages = await dbContext.ContactMessages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(5)
                .ToListAsync(cancellationToken)
        };
    }

    public async Task<IReadOnlyList<Service>> GetServicesAsync(bool includeUnpublished, CancellationToken cancellationToken = default)
    {
        IQueryable<Service> query = dbContext.Services.AsNoTracking();
        if (!includeUnpublished)
        {
            query = query.Where(x => x.IsPublished);
        }

        return await query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.TitleEn).ToListAsync(cancellationToken);
    }

    public Task<Service?> GetServiceAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Services.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Service> SaveServiceAsync(ServiceEditModel model, CancellationToken cancellationToken = default)
    {
        var entity = model.Id.HasValue
            ? await dbContext.Services.FirstOrDefaultAsync(x => x.Id == model.Id.Value, cancellationToken)
            : null;

        if (entity is null)
        {
            entity = new Service();
            dbContext.Services.Add(entity);
        }

        entity.TitleFr = model.TitleFr.Trim();
        entity.TitleEn = model.TitleEn.Trim();
        entity.SummaryFr = model.SummaryFr.Trim();
        entity.SummaryEn = model.SummaryEn.Trim();
        entity.DescriptionFr = model.DescriptionFr.Trim();
        entity.DescriptionEn = model.DescriptionEn.Trim();
        entity.IconClass = model.IconClass.Trim();
        entity.ImagePath = model.ImagePath?.Trim();
        entity.DisplayOrder = model.DisplayOrder;
        entity.IsPublished = model.IsPublished;
        entity.Slug = await EnsureUniqueSlugAsync<Service>(model.Slug, model.TitleEn, entity.Id, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Services.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        dbContext.Services.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetProjectsAsync(bool includeUnpublished, CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = dbContext.Projects.AsNoTracking();
        if (!includeUnpublished)
        {
            query = query.Where(x => x.IsPublished);
        }

        return await query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.TitleEn).ToListAsync(cancellationToken);
    }

    public Task<Project?> GetProjectAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Project> SaveProjectAsync(ProjectEditModel model, CancellationToken cancellationToken = default)
    {
        var entity = model.Id.HasValue
            ? await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == model.Id.Value, cancellationToken)
            : null;

        if (entity is null)
        {
            entity = new Project();
            dbContext.Projects.Add(entity);
        }

        entity.TitleFr = model.TitleFr.Trim();
        entity.TitleEn = model.TitleEn.Trim();
        entity.SummaryFr = model.SummaryFr.Trim();
        entity.SummaryEn = model.SummaryEn.Trim();
        entity.DescriptionFr = model.DescriptionFr.Trim();
        entity.DescriptionEn = model.DescriptionEn.Trim();
        entity.TechStack = model.TechStack.Trim();
        entity.MainImagePath = model.MainImagePath?.Trim();
        entity.ExternalLink = model.ExternalLink?.Trim();
        entity.IsFeatured = model.IsFeatured;
        entity.DisplayOrder = model.DisplayOrder;
        entity.IsPublished = model.IsPublished;
        entity.Slug = await EnsureUniqueSlugAsync<Project>(model.Slug, model.TitleEn, entity.Id, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Projects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        dbContext.Projects.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExperienceEntry>> GetExperienceEntriesAsync(bool includeHidden, CancellationToken cancellationToken = default)
    {
        IQueryable<ExperienceEntry> query = dbContext.ExperienceEntries.AsNoTracking();
        if (!includeHidden)
        {
            query = query.Where(x => x.IsVisible);
        }

        return await query.OrderByDescending(x => x.StartDate).ThenBy(x => x.DisplayOrder).ToListAsync(cancellationToken);
    }

    public Task<ExperienceEntry?> GetExperienceEntryAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ExperienceEntries.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<ExperienceEntry> SaveExperienceEntryAsync(ExperienceEditModel model, CancellationToken cancellationToken = default)
    {
        var entity = model.Id.HasValue
            ? await dbContext.ExperienceEntries.FirstOrDefaultAsync(x => x.Id == model.Id.Value, cancellationToken)
            : null;

        if (entity is null)
        {
            entity = new ExperienceEntry();
            dbContext.ExperienceEntries.Add(entity);
        }

        entity.RoleFr = model.RoleFr.Trim();
        entity.RoleEn = model.RoleEn.Trim();
        entity.Company = model.Company.Trim();
        entity.Location = model.Location.Trim();
        entity.StartDate = model.StartDate;
        entity.EndDate = model.EndDate;
        entity.DescriptionFr = model.DescriptionFr.Trim();
        entity.DescriptionEn = model.DescriptionEn.Trim();
        entity.Technologies = model.Technologies.Trim();
        entity.DisplayOrder = model.DisplayOrder;
        entity.IsVisible = model.IsVisible;

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteExperienceEntryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.ExperienceEntries.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        dbContext.ExperienceEntries.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContactMessage>> GetContactMessagesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ContactMessages
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<ContactMessage?> GetContactMessageAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateContactMessageAsync(ContactMessageStatusUpdateModel model, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);
        if (message is null)
        {
            return;
        }

        message.IsRead = model.IsRead;
        message.IsArchived = model.IsArchived;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SendContactReplyAsync(ContactMessageReplyModel model, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);
        if (message is null)
        {
            throw new InvalidOperationException("The contact message could not be found.");
        }

        var normalizedToEmail = model.ToEmail.Trim();
        var normalizedSubject = model.Subject.Trim();
        var normalizedBody = NormalizeMultilineText(model.Body);

        try
        {
            await emailDeliveryService.SendAsync(normalizedToEmail, normalizedSubject, normalizedBody, cancellationToken);

            message.IsRead = true;
            message.FollowUpStatus = ContactFollowUpStatus.Replied;
            message.LastReplyAtUtc = DateTime.UtcNow;
            message.LastReplySubject = normalizedSubject;
            message.LastReplyPreview = BuildPreview(normalizedBody, 500);
            message.LastReplyError = null;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            message.FollowUpStatus = ContactFollowUpStatus.ReplyFailed;
            message.LastReplySubject = normalizedSubject;
            message.LastReplyPreview = BuildPreview(normalizedBody, 500);
            message.LastReplyError = BuildPreview(exception.Message, 500);
            await dbContext.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteContactMessageAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await dbContext.ContactMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (message is null)
        {
            return;
        }

        dbContext.ContactMessages.Remove(message);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<SiteSetting> GetSiteSettingAsync(CancellationToken cancellationToken = default)
    {
        var setting = await dbContext.SiteSettings.FirstOrDefaultAsync(cancellationToken);
        if (setting is not null)
        {
            return setting;
        }

        setting = new SiteSetting();
        dbContext.SiteSettings.Add(setting);
        await dbContext.SaveChangesAsync(cancellationToken);
        return setting;
    }

    public async Task<SiteSetting> SaveSiteSettingAsync(SiteSettingEditModel model, CancellationToken cancellationToken = default)
    {
        var entity = model.Id.HasValue
            ? await dbContext.SiteSettings.FirstOrDefaultAsync(x => x.Id == model.Id.Value, cancellationToken)
            : await dbContext.SiteSettings.FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            entity = new SiteSetting();
            dbContext.SiteSettings.Add(entity);
        }

        entity.SiteName = model.SiteName.Trim();
        entity.TaglineFr = model.TaglineFr.Trim();
        entity.TaglineEn = model.TaglineEn.Trim();
        entity.HeroTitleFr = model.HeroTitleFr.Trim();
        entity.HeroTitleEn = model.HeroTitleEn.Trim();
        entity.HeroSubtitleFr = model.HeroSubtitleFr.Trim();
        entity.HeroSubtitleEn = model.HeroSubtitleEn.Trim();
        entity.ContactEmail = model.ContactEmail.Trim();
        entity.Location = model.Location.Trim();
        entity.LinkedInUrl = model.LinkedInUrl.Trim();
        entity.GitHubUrl = model.GitHubUrl.Trim();
        entity.MaltUrl = model.MaltUrl.Trim();
        entity.CalendlyUrl = model.CalendlyUrl.Trim();
        entity.FooterTextFr = model.FooterTextFr.Trim();
        entity.FooterTextEn = model.FooterTextEn.Trim();
        entity.LogoPath = model.LogoPath?.Trim();
        entity.FaviconPath = model.FaviconPath?.Trim();
        entity.MetaTitleFr = model.MetaTitleFr.Trim();
        entity.MetaTitleEn = model.MetaTitleEn.Trim();
        entity.MetaDescriptionFr = model.MetaDescriptionFr.Trim();
        entity.MetaDescriptionEn = model.MetaDescriptionEn.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    private async Task<string> EnsureUniqueSlugAsync<TEntity>(string? explicitSlug, string fallbackTitle, Guid entityId, CancellationToken cancellationToken)
        where TEntity : class
    {
        var baseSlug = SlugGenerator.Generate(string.IsNullOrWhiteSpace(explicitSlug) ? fallbackTitle : explicitSlug);
        var candidate = baseSlug;
        var suffix = 1;

        while (await SlugExistsAsync<TEntity>(candidate, entityId, cancellationToken))
        {
            suffix++;
            candidate = $"{baseSlug}-{suffix}";
        }

        return candidate;
    }

    private Task<bool> SlugExistsAsync<TEntity>(string slug, Guid entityId, CancellationToken cancellationToken)
        where TEntity : class
    {
        if (typeof(TEntity) == typeof(Service))
        {
            return dbContext.Services.AnyAsync(x => x.Slug == slug && x.Id != entityId, cancellationToken);
        }

        return dbContext.Projects.AnyAsync(x => x.Slug == slug && x.Id != entityId, cancellationToken);
    }

    private static string NormalizeMultilineText(string? value)
    {
        return string.Join(
            Environment.NewLine,
            (value ?? string.Empty)
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Split('\n', StringSplitOptions.TrimEntries)
                .Where(line => !string.IsNullOrWhiteSpace(line)));
    }

    private static string BuildPreview(string value, int maxLength)
    {
        var preview = value.Trim();
        if (preview.Length <= maxLength)
        {
            return preview;
        }

        return preview[..(maxLength - 1)] + "…";
    }
}
