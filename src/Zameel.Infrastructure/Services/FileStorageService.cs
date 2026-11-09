using Microsoft.AspNetCore.Hosting;
using Zameel.Application.Common;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;

namespace Zameel.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<ServiceResult<string>> SaveFileAsync(Stream fileStream, string originalFileName, string folderName, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return ServiceResult<string>.Fail(AppMessages.InvalidImageFormat);
        }

        if (fileStream.Length > SystemConstants.MaxImageFileSizeInBytes)
        {
            return ServiceResult<string>.Fail(AppMessages.ImageSizeExceeded);
        }

        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            return ServiceResult<string>.Fail(AppMessages.InvalidImageFormat);
        }

        // WebRootPath fallback if running outside standard web host
        var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var targetDirectory = Path.Combine(webRoot, "uploads", folderName);

        if (!Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        var uniqueFileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(targetDirectory, uniqueFileName);

        using (var outputStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(outputStream, cancellationToken);
        }

        var relativePath = $"/uploads/{folderName}/{uniqueFileName}";
        return ServiceResult<string>.Success(relativePath);
    }

    public Task<ServiceResult<bool>> DeleteFileAsync(string relativeFilePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativeFilePath))
        {
            return Task.FromResult(ServiceResult<bool>.Success(true));
        }

        try
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var cleanRelative = relativeFilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRoot, cleanRelative);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.FromResult(ServiceResult<bool>.Success(true));
        }
        catch (Exception)
        {
            // Logging can be done here, return success to not block user flow
            return Task.FromResult(ServiceResult<bool>.Success(false));
        }
    }
}
