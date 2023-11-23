using BinaryNetworks.Domain.Entities.Abstract;

namespace BinaryNetworks.Domain.Entities;

public class BinaryNetwork : BaseEntity
{
    public string Name { get; set; } = null!;
    public string BinaryNetworkJson { get; set; } = null!;
    public byte[]? PreviewImage { get; set; }
    
    public string? NetworkBlobName { get; set; }
    public string? NetworkFileUrl { get; set; }
    
    public string? PreviewImageBlobName { get; set; }
    public string? PreviewImageUrl { get; set; }
    
    public string? AuthorId { get; set; }
    public string NetworkFileId { get; set; } = null!;
    public string? PreviewImageFileId { get; set; }
}