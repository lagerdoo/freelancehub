using FreelanceHub.Domain.Common;

namespace FreelanceHub.Domain.Entities;

public sealed class CvImportRecord : EntityBase
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string ExtractedText { get; set; } = string.Empty;
    public DateTime? AppliedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
