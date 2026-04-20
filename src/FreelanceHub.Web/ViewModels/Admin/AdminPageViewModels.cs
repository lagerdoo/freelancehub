using System.ComponentModel.DataAnnotations;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FreelanceHub.Web.ViewModels.Admin;

public sealed class LoginViewModel
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public sealed class DashboardViewModel
{
    public AdminDashboardSummary Summary { get; init; } = new();
    public AdminListPageViewModel<ContactMessage> RecentMessagesTable { get; init; } = new();
}

public sealed class ServiceFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(160)]
    public string TitleFr { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string TitleEn { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string SummaryFr { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string SummaryEn { get; set; } = string.Empty;

    [Required]
    public string DescriptionFr { get; set; } = string.Empty;

    [Required]
    public string DescriptionEn { get; set; } = string.Empty;

    [StringLength(120)]
    public string IconClass { get; set; } = string.Empty;

    [StringLength(300)]
    public string? ImagePath { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;

    [StringLength(180)]
    public string? Slug { get; set; }
}

public sealed class ProjectFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(160)]
    public string TitleFr { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string TitleEn { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string SummaryFr { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string SummaryEn { get; set; } = string.Empty;

    [Required]
    public string DescriptionFr { get; set; } = string.Empty;

    [Required]
    public string DescriptionEn { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string TechStack { get; set; } = string.Empty;

    [StringLength(300)]
    public string? MainImagePath { get; set; }

    [Url, StringLength(300)]
    public string? ExternalLink { get; set; }

    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;

    [StringLength(180)]
    public string? Slug { get; set; }
}

public sealed class ExperienceFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(160)]
    public string RoleFr { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string RoleEn { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Company { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Location { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    [Required]
    public string DescriptionFr { get; set; } = string.Empty;

    [Required]
    public string DescriptionEn { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Technologies { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}

public sealed class SiteSettingFormViewModel
{
    public Guid? Id { get; set; }

    [Required, StringLength(150)]
    public string SiteName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string TaglineFr { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string TaglineEn { get; set; } = string.Empty;

    [Required]
    public string HeroTitleFr { get; set; } = string.Empty;

    [Required]
    public string HeroTitleEn { get; set; } = string.Empty;

    [Required]
    public string HeroSubtitleFr { get; set; } = string.Empty;

    [Required]
    public string HeroSubtitleEn { get; set; } = string.Empty;

    [Required]
    public string AboutSummaryFr { get; set; } = string.Empty;

    [Required]
    public string AboutSummaryEn { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string SkillsFr { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string SkillsEn { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Location { get; set; } = string.Empty;

    [Url, StringLength(300)]
    public string LinkedInUrl { get; set; } = string.Empty;

    [Url, StringLength(300)]
    public string GitHubUrl { get; set; } = string.Empty;

    [Url, StringLength(300)]
    public string MaltUrl { get; set; } = string.Empty;

    [Url, StringLength(300)]
    public string CalendlyUrl { get; set; } = string.Empty;

    [Required]
    public string FooterTextFr { get; set; } = string.Empty;

    [Required]
    public string FooterTextEn { get; set; } = string.Empty;

    [StringLength(300)]
    public string? LogoPath { get; set; }

    [StringLength(300)]
    public string? FaviconPath { get; set; }

    [Required, StringLength(180)]
    public string MetaTitleFr { get; set; } = string.Empty;

    [Required, StringLength(180)]
    public string MetaTitleEn { get; set; } = string.Empty;

    [Required, StringLength(320)]
    public string MetaDescriptionFr { get; set; } = string.Empty;

    [Required, StringLength(320)]
    public string MetaDescriptionEn { get; set; } = string.Empty;
}

public sealed class MediaUploadViewModel
{
    [Required]
    public IFormFile? File { get; set; }

    [StringLength(250)]
    public string AltTextFr { get; set; } = string.Empty;

    [StringLength(250)]
    public string AltTextEn { get; set; } = string.Empty;

    public IReadOnlyList<UploadedMedia> ExistingMedia { get; init; } = Array.Empty<UploadedMedia>();
}
