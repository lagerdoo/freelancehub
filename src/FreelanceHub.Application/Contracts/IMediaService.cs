using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Contracts;

public interface IMediaService
{
    Task<IReadOnlyList<UploadedMedia>> GetMediaAsync(CancellationToken cancellationToken = default);
    Task<UploadedMedia> UploadAsync(MediaUploadRequest request, CancellationToken cancellationToken = default);
}
