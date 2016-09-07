using System.Collections.Generic;
using Voronoi2;

namespace WorldMap.Layers.Interfaces
{
    public interface IHasEdges
    {
        List<GraphEdge> Edges { get; set; }
    }
}
