using FreelanceHub.Application.Models;

namespace FreelanceHub.Application.Contracts;

public interface ICvImportService
{
    Task<IReadOnlyList<CvImportRecordSummary>> GetImportsAsync(CancellationToken cancellationToken = default);
    Task<CvImportDraft> CreateDraftAsync(CvImportCreateRequest request, CancellationToken cancellationToken = default);
    Task<CvImportDraft?> GetDraftAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAppliedAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteImportAsync(Guid id, CancellationToken cancellationToken = default);
}
