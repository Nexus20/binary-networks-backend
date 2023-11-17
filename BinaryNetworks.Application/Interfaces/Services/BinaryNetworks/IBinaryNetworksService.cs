using BinaryNetworks.Application.Models.Requests.BinaryNetworks;
using BinaryNetworks.Application.Models.Results.BinaryNetworks;

namespace BinaryNetworks.Application.Interfaces.Services.BinaryNetworks;

public interface IBinaryNetworksService
{
    Task SaveAsync(SaveBinaryNetworkRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<BinaryNetworkResult>> GetAsync(CancellationToken cancellationToken = default);
    Task<BinaryNetworkResult> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}