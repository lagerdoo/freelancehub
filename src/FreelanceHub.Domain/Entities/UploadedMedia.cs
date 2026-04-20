using FreelanceHub.Domain.Common;

namespace FreelanceHub.Domain.Entities;

public sealed class UploadedMedia : EntityBase
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string RelativePath { get; set; } = string.Empty;
    public string AltTextFr { get; set; } = string.Empty;
    public string AltTextEn { get; set; } = string.Empty;
}
