using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BinaryNetworks.Application.Interfaces.FileStorage;
using BinaryNetworks.Application.Models.Dtos.Files;
using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using Newtonsoft.Json;

namespace BinaryNetworks.Infrastructure.FileStorage;

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

        // if (!await blobClient.ExistsAsync(cancellationToken)) return null;
        
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