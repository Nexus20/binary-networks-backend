using BinaryNetworks.Application.Exceptions;
using BinaryNetworks.Application.Interfaces.Persistence;
using BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;
using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using BinaryNetworks.Application.Models.Results.BinaryNetworks;
using BinaryNetworks.Domain.Entities;
using Newtonsoft.Json;

namespace BinaryNetworks.Application.Services;

public class BinaryNetworksService : IBinaryNetworksService
{
    private readonly IUnitOfWork _unitOfWork;

    public BinaryNetworksService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
                PreviewImageUrl = ConvertBytesToPreviewImageBase64(network.PreviewImage),
                CreatedAt = network.CreatedAt,
                UpdatedAt = network.UpdatedAt
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
            
        var networkData = JsonConvert.DeserializeObject<BinaryNetworkResult.BinaryNetwork>(network.BinaryNetworkJson);
            
        var result = new BinaryNetworkResult
        {
            Id = network.Id,
            NetworkName = network.Name,
            PreviewImageUrl = ConvertBytesToPreviewImageBase64(network.PreviewImage),
            CreatedAt = network.CreatedAt,
            UpdatedAt = network.UpdatedAt,
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
        var entity = new BinaryNetwork()
        {
            Name = request.NetworkName,
            BinaryNetworkJson = JsonConvert.SerializeObject(request.Network),
            PreviewImage = ConvertImageBase64ToBytes(request.PreviewImageBase64)
        };

        await _unitOfWork.BinaryNetworks.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateAsync(SaveBinaryNetworkRequest request, CancellationToken cancellationToken = default)
    {
        var binaryNetwork = await _unitOfWork.BinaryNetworks.GetByIdAsync(request.Id!, cancellationToken);

        if (binaryNetwork is null)
            throw new NotFoundException(nameof(BinaryNetwork), request.Id!);
        
        binaryNetwork.Name = request.NetworkName;
        binaryNetwork.BinaryNetworkJson = JsonConvert.SerializeObject(request.Network);
        binaryNetwork.PreviewImage = ConvertImageBase64ToBytes(request.PreviewImageBase64);
        
        _unitOfWork.BinaryNetworks.Update(binaryNetwork);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    private string? ConvertBytesToPreviewImageBase64(byte[]? bytes)
    {
        if (bytes is null)
            return null;
        
        var imageBase64 = Convert.ToBase64String(bytes);
        return $"data:image/png;base64,{imageBase64}";
    }

    private byte[]? ConvertImageBase64ToBytes(string? imageBase64)
    {
        if (string.IsNullOrWhiteSpace(imageBase64))
            return null;
        
        var base64EncodedBytes = imageBase64.Replace("data:image/png;base64,", "");
        return Convert.FromBase64String(base64EncodedBytes);
    }
}