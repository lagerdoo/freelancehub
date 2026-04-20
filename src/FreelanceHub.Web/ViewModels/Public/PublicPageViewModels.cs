using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Web.ViewModels.Public;

public sealed class HomePageViewModel
{
    public PublicSiteSnapshot Snapshot { get; init; } = new();
}

public sealed class AboutPageViewModel
{
    public IReadOnlyList<ExperienceEntry> ExperienceEntries { get; init; } = Array.Empty<ExperienceEntry>();
}

public sealed class ServicesPageViewModel
{
    public IReadOnlyList<Service> Services { get; init; } = Array.Empty<Service>();
}

public sealed class ProjectsPageViewModel
{
    public IReadOnlyList<Project> Projects { get; init; } = Array.Empty<Project>();
}

public sealed class ExperiencePageViewModel
{
    public IReadOnlyList<ExperienceEntry> ExperienceEntries { get; init; } = Array.Empty<ExperienceEntry>();
}

public sealed class ContactPageViewModel
{
    public ContactFormViewModel Form { get; init; } = new();
    public bool IsSubmitted { get; init; }
}

public sealed class ContactFormViewModel
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? Website { get; set; }

    public string SubmissionToken { get; set; } = string.Empty;
}
