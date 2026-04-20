namespace FreelanceHub.Infrastructure.Options;

public sealed class UploadOptions
{
    public const string SectionName = "Uploads";

    public long MaxFileSizeBytes { get; set; } = 3 * 1024 * 1024;
}
