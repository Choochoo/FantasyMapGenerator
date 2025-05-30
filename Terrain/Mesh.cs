using System.Collections.Generic;
using D3Voronoi;

namespace TerrainGenerator
{
    /// <summary>
    /// Represents a mesh data structure containing vertices, edges, triangles, and adjacency information.
    /// Used for terrain generation, Voronoi diagrams, and geometric calculations in map generation.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// Array of points representing the mesh vertices or cell centers.
        /// These are the primary points used in Voronoi diagram generation.
        /// </summary>
        public Point[] Pts;
        
        /// <summary>
        /// Array of vertices representing the dual points of the mesh.
        /// These are typically the circumcenters of Delaunay triangles.
        /// </summary>
        public Point[] Vxs;
        
        /// <summary>
        /// Dictionary mapping vertex coordinate strings to their integer indices.
        /// Used for efficient vertex lookup and identification.
        /// </summary>
        public Dictionary<string, int> Vxids;
        
        /// <summary>
        /// Dictionary mapping vertex indices to lists of adjacent vertex indices.
        /// Represents the adjacency relationships between vertices in the mesh.
        /// </summary>
        public Dictionary<int, List<int>> Adj;
        
        /// <summary>
        /// List of edges in the mesh, connecting vertices and defining the mesh topology.
        /// Each edge contains information about connected vertices and associated data.
        /// </summary>
        public List<MapEdge> Edges;
        
        /// <summary>
        /// Dictionary mapping vertex indices to lists of triangle points.
        /// Represents the triangular faces of the mesh for rendering and calculations.
        /// </summary>
        public Dictionary<int, List<Point>> Tris;
        
        /// <summary>
        /// The bounding extent of the mesh, defining the spatial boundaries.
        /// Contains minimum and maximum coordinates for the mesh area.
        /// </summary>
        public Extent Extent;
    }
}
