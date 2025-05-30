using TerrainGenerator;

namespace WorldMap.Layers.Interfaces
{
    /// <summary>
    /// Interface that defines a contract for objects that contain mesh data structures.
    /// Used for layers that need to store and manipulate 3D mesh information for terrain rendering.
    /// </summary>
    public interface IHasMesh
    {
        /// <summary>
        /// Gets or sets the mesh data structure containing vertices, edges, and face information.
        /// This mesh is used for terrain generation, rendering, and geometric calculations.
        /// </summary>
        Mesh Mesh { get; set; }
    }
}
