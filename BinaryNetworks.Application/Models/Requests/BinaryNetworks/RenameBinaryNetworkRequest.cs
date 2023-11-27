using System.ComponentModel.DataAnnotations;

namespace BinaryNetworks.Application.Models.Requests.BinaryNetworks;

public class RenameBinaryNetworkRequest
{
    [Required]
    public string NetworkName { get; set; } = null!;
}