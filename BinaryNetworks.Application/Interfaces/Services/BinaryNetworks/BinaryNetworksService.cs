using System.Text;
using BinaryNetworks.Application.Exceptions;
using BinaryNetworks.Application.Interfaces.FileStorage;
using BinaryNetworks.Application.Interfaces.Persistence;
using BinaryNetworks.Application.Models.Dtos.Files;
using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using BinaryNetworks.Application.Models.Results.BinaryNetworks;
using BinaryNetworks.Domain.Entities;
using Newtonsoft.Json;

namespace BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;

public class BinaryNetworksService : IBinaryNetworksService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;

    public BinaryNetworksService(IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
    {
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BinaryNetworkResult>> GetAsync(CancellationToken cancellationToken = default)
    {
        var networks = await _unitOfWork.BinaryNetworks.GetAllAsync(cancellationToken);

        if (networks.Count == 0)
            return Enumerable.Empty<BinaryNetworkResult>();

        var result = new List<BinaryNetworkResult>();
        
        foreach (var network in networks)
        {
            var networkJson = await _fileStorageService.DownloadAsync(network.NetworkBlobName, "networks", cancellationToken);
            
            var networkData = JsonConvert.DeserializeObject<BinaryNetworkResult.BinaryNetwork>(networkJson);
            
            var resultItem = new BinaryNetworkResult
            {
                Id = network.Id,
                Name = network.Name,
                PreviewImageUrl = network.PreviewImageUrl,
                CreatedAt = network.CreatedAt,
                Network = networkData
            };

            result.Add(resultItem);
        }
        
        return result;
    }
    
    public async Task<BinaryNetworkResult> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var network = await _unitOfWork.BinaryNetworks.GetByIdAsync(id, cancellationToken);

        if (network is null)
            throw new NotFoundException(nameof(BinaryNetwork), id);

        var networkJson = await _fileStorageService.DownloadAsync(network.NetworkBlobName, "networks", cancellationToken);
            
        var networkData = JsonConvert.DeserializeObject<BinaryNetworkResult.BinaryNetwork>(networkJson);
            
        var result = new BinaryNetworkResult
        {
            Id = network.Id,
            Name = network.Name,
            PreviewImageUrl = network.PreviewImageUrl,
            CreatedAt = network.CreatedAt,
            Network = networkData
        };

        return result;
    }
    
    public async Task SaveAsync(SaveBinaryNetworkRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(request.Id))
        {
            await UpdateAsync(request, cancellationToken);
            return;
        }
        
        await CreateAsync(request, cancellationToken);
    }

    private async Task CreateAsync(SaveBinaryNetworkRequest request, CancellationToken cancellationToken = default)
    {
        var networkFileUrl = await UploadNetworkFileAsync(request);

        var entity = new BinaryNetwork()
        {
            Name = request.NetworkName,
            NetworkFileUrl = networkFileUrl.Urls.First().Url,
            NetworkBlobName = networkFileUrl.Urls.First().BlobName,
            CreatedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(request.PreviewImageBase64))
        {
            var previewImageUrl = await UploadPreviewImageAsync(request);
            entity.PreviewImageUrl = previewImageUrl.Urls.First().Url;
            entity.PreviewImageBlobName = previewImageUrl.Urls.First().BlobName;
        }

        await _unitOfWork.BinaryNetworks.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateAsync(SaveBinaryNetworkRequest request, CancellationToken cancellationToken = default)
    {
        var binaryNetwork = await _unitOfWork.BinaryNetworks.GetByIdAsync(request.Id!, cancellationToken);

        if (binaryNetwork is null)
            throw new NotFoundException(nameof(BinaryNetwork), request.Id!);
        
        var networkFileUrl = await UploadNetworkFileAsync(request, binaryNetwork.NetworkBlobName);
        
        binaryNetwork.Name = request.NetworkName;
        binaryNetwork.NetworkFileUrl = networkFileUrl.Urls.First().Url;
        
        if (!string.IsNullOrWhiteSpace(request.PreviewImageBase64))
        {
            var previewImageUrl = await UploadPreviewImageAsync(request, binaryNetwork.PreviewImageBlobName);
            binaryNetwork.PreviewImageUrl = previewImageUrl.Urls.First().Url;
        }
        
        _unitOfWork.BinaryNetworks.Update(binaryNetwork);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<UrlsDto> UploadNetworkFileAsync(SaveBinaryNetworkRequest request, string? blobName = null)
    {
        var json = JsonConvert.SerializeObject(request.Network);

        using var jsonMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        var dto = new FileDto()
        {
            ContentType = "application/json",
            Content = jsonMemoryStream,
            Name = request.NetworkName + ".json"
        };
            
        var result = await _fileStorageService.UploadAsync(new List<FileDto> { dto }, "networks", blobName);
            
        if (result is null)
            throw new Exception("Error while uploading file to storage");

        return result;
    }
    
    private async Task<UrlsDto> UploadPreviewImageAsync(SaveBinaryNetworkRequest request, string? blobName = null)
    {
        var base64EncodedBytes = request.PreviewImageBase64!.Replace("data:image/png;base64,", "");
        var imageBytes = Convert.FromBase64String(base64EncodedBytes);
            
        using var imageMemoryStream = new MemoryStream(imageBytes);
        
        var dto = new FileDto()
        {
            ContentType = "image/png",
            Content = imageMemoryStream,
            Name = request.NetworkName + ".png"
        };
            
        var result = await _fileStorageService.UploadAsync(new List<FileDto> { dto }, "previews", blobName);
            
        if (result is null)
            throw new Exception("Error while uploading file to storage");

        return result;
    }
}