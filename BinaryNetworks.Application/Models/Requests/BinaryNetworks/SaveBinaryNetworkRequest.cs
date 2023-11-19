namespace BinaryNetworks.Application.Models.Requests.BinaryNetworks;

public class SaveBinaryNetworkRequest
{
    public string? Id { get; set; }
    public string NetworkName { get; set; } = null!;
    public string? PreviewImageBase64 { get; set; }
    
    public BinaryNetwork Network { get; set; } = null!;

    public class BinaryNetwork
    {
        public List<BinaryNetworkNode> Nodes { get; set; } = null!;
        public List<BinaryNetworkEdge>? Edges { get; set; }
        
        public class BinaryNetworkNode
        {
            public int Id { get; set; }
            public string Label { get; set; } = null!;
            public NodePosition? Position { get; set; }

            public class NodePosition
            {
                public int X { get; set; }
                public int Y { get; set; }
            }
        }

        public class BinaryNetworkEdge
        {
            public int From { get; set; }
            public int To { get; set; }
        }
    }
}