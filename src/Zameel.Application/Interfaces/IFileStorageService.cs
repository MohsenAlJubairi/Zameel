using Zameel.Application.Common;

namespace Zameel.Application.Interfaces;

public interface IFileStorageService
{
    Task<ServiceResult<string>> SaveFileAsync(Stream fileStream, string originalFileName, string folderName, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> DeleteFileAsync(string relativeFilePath, CancellationToken cancellationToken = default);
}
