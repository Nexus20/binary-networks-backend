using BinaryNetworks.Application.Models.Dtos.Files;

namespace BinaryNetworks.Application.Interfaces.FileStorage;

public interface IFileStorageService2
{
    Task<string> DownloadAsync(string fileId);
    Task DeleteAsync(string fileId);
    Task<string> UploadAsync(FileDto file, string folderName, string? fileId = null);
    Task<string> ShareAsync(string fileId);
}