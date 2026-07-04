namespace UniversityMaintenance.Application.Common.Interfaces;

/// <summary>Persists uploaded evidence images and returns a relative path to serve them from.</summary>
public interface IFileStorageService
{
    /// <summary>Validates and saves the file, returning a relative web path (e.g. /uploads/xxx.jpg).</summary>
    Task<string> SaveAsync(Stream content, string fileName, string contentType, long length,
        CancellationToken cancellationToken = default);
}
