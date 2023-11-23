using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BinaryNetworks.Application.Interfaces.FileStorage;
using BinaryNetworks.Application.Models.Dtos.Files;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace BinaryNetworks.Infrastructure.FileStorage;

public class GoogleDriveService : IFileStorageService2
{
    private readonly DriveService _driveService;

    public GoogleDriveService(DriveService driveService)
    {
        _driveService = driveService;
    }
    
    public async Task<string> DownloadAsync(string fileId)
    {
        var request = _driveService.Files.Get(fileId);
        var stream = new MemoryStream();
        await request.DownloadAsync(stream);
        stream.Position = 0;
        
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();
        return json;
    }
    
    public Task DeleteAsync(string fileId)
    {
        return _driveService.Files.Delete(fileId).ExecuteAsync();
    }
    
    public async Task<string> UploadAsync(FileDto file, string folderName, string? fileId = null)
    {
        var folderId = await GetOrCreateFolderAsync(folderName);
        
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = file.Name,
        };

        if (fileId is not null)
        {
            await using (var stream = file.Content)
            {
                var updateRequest = _driveService.Files.Update(fileMetadata, fileId, stream, file.ContentType);
                await updateRequest.UploadAsync();
                
                return fileId;
            }
        }
        
        await using (var stream = file.Content)
        {
            fileMetadata.Parents = new List<string>() { folderId };
            var createRequest = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            createRequest.Fields = "id";
            await createRequest.UploadAsync();
            
            return createRequest.ResponseBody?.Id;
        }
    }

    public async Task<string> ShareAsync(string fileId)
    {
        var permission = new Permission
        {
            Type = "anyone",
            Role = "reader"
        };
        
        await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();
        
        var request = _driveService.Files.Get(fileId);
        request.Fields = "webViewLink";
        var file = await request.ExecuteAsync();

        return file.WebViewLink;
    }

    private async Task<string> GetOrCreateFolderAsync(string folderName)
    {
        var request = _driveService.Files.List();
        request.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and trashed=false";
        request.Fields = "files(id, name)";
        var response = await request.ExecuteAsync();

        if (response.Files.Count > 0)
            return response.Files[0].Id;
        
        var folderMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder"
        };

        var createRequest = _driveService.Files.Create(folderMetadata);
        createRequest.Fields = "id";
        var folder = await createRequest.ExecuteAsync();
        
        return folder.Id;
    }
}

public class BlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<UrlsDto?> UploadAsync(List<FileDto>? files, string containerName, string? blobName = null)
    {
        if (files == null || files.Count == 0)
            return null;

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var urls = new List<FileNameWithUrlDto>();

        foreach (var file in files)
        {
            blobName ??= file.GetPathWithFileName();
            var blobClient = containerClient.GetBlobClient(blobName);
            
            await blobClient.UploadAsync(file.Content, new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            });

            urls.Add(new FileNameWithUrlDto()
            {
                Url = blobClient.Uri.ToString(),
                Name = file.Name,
                BlobName = blobClient.Name
            });
        }

        return new UrlsDto(urls);
    }

    public async Task<bool> DeleteAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url can't be null or whitespace");
        
        var containerName = url.Split('/').Skip(3).Take(1).First();
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        
        var fileName = Path.Combine(url.Split('/').Skip(4).ToArray());

        var blobClient = containerClient.GetBlobClient(fileName);
        var response = await blobClient.DeleteAsync();

        if (response.IsError)
            return false;
        
        return true;
    }

    public async Task<string> DownloadAsync(string blobName, string containerName, CancellationToken cancellationToken = default)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainer.GetBlobClient(blobName);
        
        var response = await blobClient.DownloadAsync(cancellationToken);
        
        using var reader = new StreamReader(response.Value.Content);
        var json = await reader.ReadToEndAsync();
        return json;
    }

    public async Task<bool> DeleteAsync(UrlsDto urls)
    {
        if (urls.Urls?.Any() == false)
            return true;
        
        var containerClient = _blobServiceClient.GetBlobContainerClient("datasets");
        
        foreach (var fileUrl in urls.Urls)
        {
            var fileName = Path.Combine(fileUrl.Url.Split('/').Skip(4).ToArray());

            var blobClient = containerClient.GetBlobClient(fileName);
            var response = await blobClient.DeleteAsync();

            if (response.IsError)
                return false;
        }

        return true;
    }
}