using BinaryNetworks.Application.Models.Results.Abstract;

namespace BinaryNetworks.Application.Models.Results.BinaryNetworks;

public class BinaryNetworkShortResult : BaseResult
{
    public string NetworkName { get; set; } = null!;
    public string? PreviewImageUrl { get; set; }
}