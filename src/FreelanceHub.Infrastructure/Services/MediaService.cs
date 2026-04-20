using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Infrastructure.Data;
using FreelanceHub.Infrastructure.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FreelanceHub.Infrastructure.Services;

public sealed class MediaService(FreelanceHubDbContext dbContext, IWebHostEnvironment environment, IOptions<UploadOptions> options) : IMediaService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    public async Task<IReadOnlyList<UploadedMedia>> GetMediaAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.UploadedMedia
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<UploadedMedia> UploadAsync(MediaUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FileSize <= 0 || request.FileSize > options.Value.MaxFileSizeBytes)
        {
            throw new InvalidOperationException("The uploaded file size is invalid.");
        }

        var extension = Path.GetExtension(request.OriginalFileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Only image uploads are allowed.");
        }

        if (!AllowedContentTypes.Contains(request.ContentType))
        {
            throw new InvalidOperationException("The uploaded file type is not supported.");
        }

        await EnsureValidSignatureAsync(request.Content, extension, cancellationToken);

        var folderSegment = Path.Combine("uploads", "media", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
        var webRootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var absoluteFolder = Path.Combine(webRootPath, folderSegment);
        Directory.CreateDirectory(absoluteFolder);

        var storedFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var absolutePath = Path.Combine(absoluteFolder, storedFileName);

        await using (var fileStream = File.Create(absolutePath))
        {
            await request.Content.CopyToAsync(fileStream, cancellationToken);
        }

        var entity = new UploadedMedia
        {
            OriginalFileName = request.OriginalFileName,
            StoredFileName = storedFileName,
            ContentType = request.ContentType,
            FileSize = request.FileSize,
            RelativePath = "/" + Path.Combine(folderSegment, storedFileName).Replace("\\", "/"),
            AltTextFr = request.AltTextFr.Trim(),
            AltTextEn = request.AltTextEn.Trim()
        };

        dbContext.UploadedMedia.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    private static async Task EnsureValidSignatureAsync(Stream stream, string extension, CancellationToken cancellationToken)
    {
        if (!stream.CanSeek)
        {
            throw new InvalidOperationException("The upload stream must be seekable.");
        }

        var buffer = new byte[12];
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        stream.Position = 0;

        var isValid = extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => bytesRead >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF,
            ".png" => bytesRead >= 8 && buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                      buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A,
            ".gif" => bytesRead >= 6 &&
                      ((buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38 && buffer[4] == 0x37 && buffer[5] == 0x61) ||
                       (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38 && buffer[4] == 0x39 && buffer[5] == 0x61)),
            ".webp" => bytesRead >= 12 &&
                       buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
                       buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50,
            _ => false
        };

        if (!isValid)
        {
            throw new InvalidOperationException("The uploaded file signature is invalid.");
        }
    }
}
