using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Models;

public sealed class ContactFormRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public sealed class PublicSiteSnapshot
{
    public SiteSetting SiteSetting { get; init; } = new();
    public IReadOnlyList<Service> Services { get; init; } = Array.Empty<Service>();
    public IReadOnlyList<Project> FeaturedProjects { get; init; } = Array.Empty<Project>();
    public IReadOnlyList<ExperienceEntry> ExperienceEntries { get; init; } = Array.Empty<ExperienceEntry>();
}
