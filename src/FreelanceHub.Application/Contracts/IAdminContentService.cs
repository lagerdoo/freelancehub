using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Contracts;

public interface IAdminContentService
{
    Task<AdminDashboardSummary> GetDashboardAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Service>> GetServicesAsync(bool includeUnpublished, CancellationToken cancellationToken = default);
    Task<Service?> GetServiceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Service> SaveServiceAsync(ServiceEditModel model, CancellationToken cancellationToken = default);
    Task DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> GetProjectsAsync(bool includeUnpublished, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project> SaveProjectAsync(ProjectEditModel model, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExperienceEntry>> GetExperienceEntriesAsync(bool includeHidden, CancellationToken cancellationToken = default);
    Task<ExperienceEntry?> GetExperienceEntryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExperienceEntry> SaveExperienceEntryAsync(ExperienceEditModel model, CancellationToken cancellationToken = default);
    Task DeleteExperienceEntryAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactMessage>> GetContactMessagesAsync(CancellationToken cancellationToken = default);
    Task<ContactMessage?> GetContactMessageAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateContactMessageAsync(ContactMessageStatusUpdateModel model, CancellationToken cancellationToken = default);
    Task SendContactReplyAsync(ContactMessageReplyModel model, CancellationToken cancellationToken = default);
    Task DeleteContactMessageAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SiteSetting> GetSiteSettingAsync(CancellationToken cancellationToken = default);
    Task<SiteSetting> SaveSiteSettingAsync(SiteSettingEditModel model, CancellationToken cancellationToken = default);
}
