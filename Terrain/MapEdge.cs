using D3Voronoi;

namespace TerrainGenerator
{
    /// <summary>
    /// Represents an edge in the map mesh, connecting two vertices and defining adjacent regions.
    /// Contains information about the connected vertices and the points on either side of the edge.
    /// </summary>
    public class MapEdge
    {
        /// <summary>
        /// Index of the first vertex connected by this edge.
        /// References a vertex in the mesh vertex array.
        /// </summary>
        public int Spot1;
        
        /// <summary>
        /// Index of the second vertex connected by this edge.
        /// References a vertex in the mesh vertex array.
        /// </summary>
        public int Spot2;
        
        /// <summary>
        /// Point representing the left side of the edge.
        /// Used for determining adjacency and spatial relationships.
        /// </summary>
        public Point Left;
        
        /// <summary>
        /// Point representing the right side of the edge.
        /// Used for determining adjacency and spatial relationships.
        /// </summary>
        public Point Right;
    }
}
