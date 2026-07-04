using Microsoft.Extensions.Options;
using UniversityMaintenance.Application.Common.Exceptions;
using UniversityMaintenance.Application.Common.Interfaces;

namespace UniversityMaintenance.Infrastructure.Storage;

/// <summary>Saves evidence images to local disk and returns a web-servable relative path.</summary>
public class FileStorageService(IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly FileStorageOptions _options = options.Value;

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType,
        long length, CancellationToken cancellationToken = default)
    {
        if (length <= 0)
            throw new BadRequestException("The uploaded file is empty.");

        if (length > _options.MaxBytes)
            throw new BadRequestException($"File exceeds the {_options.MaxBytes / (1024 * 1024)} MB limit.");

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_options.AllowedExtensions.Contains(ext))
            throw new BadRequestException("Unsupported file type. Allowed: " +
                string.Join(", ", _options.AllowedExtensions));

        Directory.CreateDirectory(_options.RootPath);

        var storedName = $"{Guid.NewGuid():N}{ext}";
        var physicalPath = Path.Combine(_options.RootPath, storedName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await content.CopyToAsync(stream, cancellationToken);
        }

        // e.g. "/uploads/ab12....jpg"
        return $"{_options.RequestPath.TrimEnd('/')}/{storedName}";
    }
}
