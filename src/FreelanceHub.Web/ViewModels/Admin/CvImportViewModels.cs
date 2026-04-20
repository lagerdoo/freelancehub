using System.ComponentModel.DataAnnotations;
using FreelanceHub.Application.Models;

namespace FreelanceHub.Web.ViewModels.Admin;

public sealed class CvImportIndexViewModel
{
    public CvImportUploadViewModel Upload { get; init; } = new();
    public IReadOnlyList<CvImportRecordSummary> Imports { get; init; } = Array.Empty<CvImportRecordSummary>();
}

public sealed class CvImportUploadViewModel
{
    [Required]
    public IFormFile? File { get; set; }
}

public sealed class CvImportReviewViewModel
{
    public Guid ImportId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ExtractedTextPreview { get; set; } = string.Empty;
    public bool DeleteFileAfterApply { get; set; }
    public CvImportProfileSuggestionViewModel Profile { get; set; } = new();
    public List<CvImportExperienceSuggestionViewModel> ExperienceEntries { get; set; } = [];
    public List<CvImportServiceSuggestionViewModel> Services { get; set; } = [];
    public List<CvImportProjectSuggestionViewModel> Projects { get; set; } = [];
}

public sealed class CvImportProfileSuggestionViewModel
{
    public bool Selected { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string TaglineFr { get; set; } = string.Empty;
    public string TaglineEn { get; set; } = string.Empty;
    public string AboutSummaryFr { get; set; } = string.Empty;
    public string AboutSummaryEn { get; set; } = string.Empty;
    public string SkillsFr { get; set; } = string.Empty;
    public string SkillsEn { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
}

public sealed class CvImportExperienceSuggestionViewModel
{
    public bool Selected { get; set; }
    public string RoleFr { get; set; } = string.Empty;
    public string RoleEn { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string DescriptionFr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
}

public sealed class CvImportServiceSuggestionViewModel
{
    public bool Selected { get; set; }
    public string TitleFr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string SummaryFr { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string DescriptionFr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string IconClass { get; set; } = "bi bi-briefcase";
}

public sealed class CvImportProjectSuggestionViewModel
{
    public bool Selected { get; set; }
    public string TitleFr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string SummaryFr { get; set; } = string.Empty;
    public string SummaryEn { get; set; } = string.Empty;
    public string DescriptionFr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;
}
