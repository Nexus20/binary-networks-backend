using BinaryNetworks.Application.Models.Results.Abstract;

namespace BinaryNetworks.Application.Models.Results.BinaryNetworks;

public class BinaryNetworkResult : BaseResult
{
    public string Name { get; set; } = null!;
    public string? PreviewImageUrl { get; set; }
    
    public BinaryNetwork Network { get; set; } = null!;

    public class BinaryNetwork
    {
        public List<BinaryNetworkNode> Nodes { get; set; } = null!;
        public List<BinaryNetworkEdge> Edges { get; set; } = null!;
        
        public class BinaryNetworkNode
        {
            public int Id { get; set; }
            public string Label { get; set; } = null!;
            public NodePosition Position { get; set; } = null!;

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