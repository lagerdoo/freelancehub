namespace FreelanceHub.Infrastructure.Options;

public sealed class CvImportOptions
{
    public const string SectionName = "CvImport";

    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    public string StorageFolder { get; set; } = "App_Data/CvImports";
}
