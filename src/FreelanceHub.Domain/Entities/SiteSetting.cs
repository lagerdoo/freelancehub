using FreelanceHub.Domain.Common;

namespace FreelanceHub.Domain.Entities;

public sealed class SiteSetting : EntityBase
{
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
