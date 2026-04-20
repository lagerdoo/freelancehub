using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Models;

public sealed class LoginRequest
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class AdminIdentity
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
}

public sealed class AdminDashboardSummary
{
    public int PublishedServices { get; init; }
    public int PublishedProjects { get; init; }
    public int VisibleExperienceEntries { get; init; }
    public int UnreadContactMessages { get; init; }
    public IReadOnlyList<ContactMessage> RecentMessages { get; init; } = Array.Empty<ContactMessage>();
}

public sealed class ServiceEditModel
{
    public Guid? Id { get; set; }
    public string TitleFr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string SummaryFr { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string DescriptionFr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public string? Slug { get; set; }
}

public sealed class ProjectEditModel
{
    public Guid? Id { get; set; }
    public string TitleFr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string SummaryFr { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string DescriptionFr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;
    public string? MainImagePath { get; set; }
    public string? ExternalLink { get; set; }
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public string? Slug { get; set; }
}

public sealed class ExperienceEditModel
{
    public Guid? Id { get; set; }
    public string RoleFr { get; set; } = string.Empty;
    public string RoleEn { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string DescriptionFr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; }
}

public sealed class ContactMessageStatusUpdateModel
{
    public Guid Id { get; set; }
    public bool IsRead { get; set; }
    public bool IsArchived { get; set; }
}

public sealed class ContactMessageReplyModel
{
    public Guid Id { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public sealed class SiteSettingEditModel
{
    public Guid? Id { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string TaglineFr { get; set; } = string.Empty;
    public string TaglineEn { get; set; } = string.Empty;
    public string HeroTitleFr { get; set; } = string.Empty;
    public string HeroTitleEn { get; set; } = string.Empty;
    public string HeroSubtitleFr { get; set; } = string.Empty;
    public string HeroSubtitleEn { get; set; } = string.Empty;
    public string AboutSummaryFr { get; set; } = string.Empty;
    public string AboutSummaryEn { get; set; } = string.Empty;
    public string SkillsFr { get; set; } = string.Empty;
    public string SkillsEn { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string MaltUrl { get; set; } = string.Empty;
    public string CalendlyUrl { get; set; } = string.Empty;
    public string FooterTextFr { get; set; } = string.Empty;
    public string FooterTextEn { get; set; } = string.Empty;
    public string? LogoPath { get; set; }
    public string? FaviconPath { get; set; }
    public string MetaTitleFr { get; set; } = string.Empty;
    public string MetaTitleEn { get; set; } = string.Empty;
    public string MetaDescriptionFr { get; set; } = string.Empty;
    public string MetaDescriptionEn { get; set; } = string.Empty;
}

public sealed class MediaUploadRequest
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string AltTextFr { get; set; } = string.Empty;
    public string AltTextEn { get; set; } = string.Empty;
    public Stream Content { get; set; } = Stream.Null;
}
