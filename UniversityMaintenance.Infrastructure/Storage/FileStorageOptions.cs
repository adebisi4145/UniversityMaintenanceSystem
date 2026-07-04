namespace UniversityMaintenance.Infrastructure.Storage;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>Absolute path on disk where uploads are written (set by the host at startup).</summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>Public URL prefix the saved files are served from.</summary>
    public string RequestPath { get; set; } = "/uploads";

    public long MaxBytes { get; set; } = 5 * 1024 * 1024; // 5 MB

    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
}
