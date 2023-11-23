using BinaryNetworks.Domain.Entities.Abstract;

namespace BinaryNetworks.Domain.Entities;

public class BinaryNetwork : BaseEntity
{
    public string Name { get; set; } = null!;
    public string BinaryNetworkJson { get; set; } = null!;
    public byte[]? PreviewImage { get; set; }
    
    public string? AuthorId { get; set; }
}