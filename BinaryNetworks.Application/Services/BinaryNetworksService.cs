using System.Text;
using BinaryNetworks.Application.Exceptions;
using BinaryNetworks.Application.Interfaces.FileStorage;
using BinaryNetworks.Application.Interfaces.Persistence;
using BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;
using BinaryNetworks.Application.Models.Dtos.Files;
using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using BinaryNetworks.Application.Models.Results.BinaryNetworks;
using BinaryNetworks.Domain.Entities;
using Newtonsoft.Json;

namespace BinaryNetworks.Application.Services;

public class BinaryNetworksService : IBinaryNetworksService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileStorageService2 _fileStorageService2;
    private readonly IUnitOfWork _unitOfWork;

    public BinaryNetworksService(IFileStorageService fileStorageService, IUnitOfWork unitOfWork, IFileStorageService2 fileStorageService2)
    {
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _fileStorageService2 = fileStorageService2;
    }

    public async Task<IEnumerable<BinaryNetworkShortResult>> GetAsync(CancellationToken cancellationToken = default)
    {
        var networks = await _unitOfWork.BinaryNetworks.GetAllAsync(cancellationToken);

        if (networks.Count == 0)
            return Enumerable.Empty<BinaryNetworkShortResult>();

        var result = new List<BinaryNetworkShortResult>();
        
        foreach (var network in networks)
        {
            var resultItem = new BinaryNetworkShortResult
            {
                Id = network.Id,
                NetworkName = network.Name,
                PreviewImageUrl = network.PreviewImageUrl,
                CreatedAt = network.CreatedAt,
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

        var networkJson = await _fileStorageService2.DownloadAsync(network.NetworkFileId);
            
        var networkData = JsonConvert.DeserializeObject<BinaryNetworkResult.BinaryNetwork>(networkJson);
            
        var result = new BinaryNetworkResult
        {
            Id = network.Id,
            NetworkName = network.Name,
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
        var networkFileId = await UploadNetworkFileAsync(request);

        var entity = new BinaryNetwork()
        {
            Name = request.NetworkName,
            NetworkFileId = networkFileId,
            CreatedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(request.PreviewImageBase64))
        {
            var previewImageInfo = await UploadPreviewImageAsync(request);
            entity.PreviewImageFileId = previewImageInfo.Item1;
            entity.PreviewImageUrl = previewImageInfo.Item2;
        }

        await _unitOfWork.BinaryNetworks.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateAsync(SaveBinaryNetworkRequest request, CancellationToken cancellationToken = default)
    {
        var binaryNetwork = await _unitOfWork.BinaryNetworks.GetByIdAsync(request.Id!, cancellationToken);

        if (binaryNetwork is null)
            throw new NotFoundException(nameof(BinaryNetwork), request.Id!);
        
        await UploadNetworkFileAsync(request, binaryNetwork.NetworkFileId);
        
        if (!string.IsNullOrWhiteSpace(request.PreviewImageBase64))
        {
            var previewImageInfo = await UploadPreviewImageAsync(request, binaryNetwork.PreviewImageFileId);
            binaryNetwork.PreviewImageUrl = previewImageInfo.Item2;
        }
        
        _unitOfWork.BinaryNetworks.Update(binaryNetwork);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> UploadNetworkFileAsync(SaveBinaryNetworkRequest request, string? existingFileId = null)
    {
        var json = JsonConvert.SerializeObject(request.Network);

        using var jsonMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        
        var dto = new FileDto()
        {
            ContentType = "application/json",
            Content = jsonMemoryStream,
            Name = request.NetworkName + ".json"
        };
        
       var fileId = await _fileStorageService2.UploadAsync(dto, "networks", existingFileId);

        return fileId;
    }
    
    private async Task<(string, string)> UploadPreviewImageAsync(SaveBinaryNetworkRequest request, string? existingFileId = null)
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
            
        var fileId = await _fileStorageService2.UploadAsync(dto, "previews", existingFileId);

        var url = await _fileStorageService2.ShareAsync(fileId);

        return (fileId, url);
    }
}