using System.Collections.Generic;
using D3Voronoi;

namespace TerrainGenerator
{
    public class Mesh
    {
        public Point[] Pts;
        public Point[] Vxs;
        public Dictionary<string, int> Vxids;
        public Dictionary<int, List<int>> Adj;
        public List<MapEdge> Edges;
        public Dictionary<int, List<Point>> Tris;
        public Extent Extent;
    }
}
