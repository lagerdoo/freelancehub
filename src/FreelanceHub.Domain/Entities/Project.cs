using FreelanceHub.Domain.Common;

namespace FreelanceHub.Domain.Entities;

public sealed class Project : EntityBase
{
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
    public bool IsPublished { get; set; } = true;
    public string Slug { get; set; } = string.Empty;
}
