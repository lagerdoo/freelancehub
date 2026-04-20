using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Contracts;

public interface IContentQueryService
{
    Task<SiteSetting> GetSiteSettingAsync(CancellationToken cancellationToken = default);
    Task<PublicSiteSnapshot> GetHomeSnapshotAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Service>> GetPublishedServicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Project>> GetPublishedProjectsAsync(bool featuredOnly = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExperienceEntry>> GetVisibleExperienceEntriesAsync(CancellationToken cancellationToken = default);
    Task SubmitContactMessageAsync(ContactFormRequest request, CancellationToken cancellationToken = default);
}
