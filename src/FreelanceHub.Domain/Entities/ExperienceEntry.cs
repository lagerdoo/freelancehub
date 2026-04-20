using FreelanceHub.Domain.Common;

namespace FreelanceHub.Domain.Entities;

public sealed class ExperienceEntry : EntityBase
{
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
    public bool IsVisible { get; set; } = true;
}
