using BinaryNetworks.Application.Models.Dtos.Files;

namespace BinaryNetworks.Application.Interfaces.FileStorage;

public interface IFileStorageService
{
    Task<UrlsDto?> UploadAsync(List<FileDto>? files, string containerName, string? blobName = null);
    Task<bool> DeleteAsync(UrlsDto urls);
    Task<bool> DeleteAsync(string url);
    Task<string> DownloadAsync(string blobName, string containerName, CancellationToken cancellationToken = default);
}