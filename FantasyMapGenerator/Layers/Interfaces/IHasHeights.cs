namespace WorldMap.Layers.Interfaces
{
    /// <summary>
    /// Interface that defines a contract for objects that contain terrain height data.
    /// Used for layers that need to store and manipulate elevation information.
    /// </summary>
    public interface IHasHeights
    {
        /// <summary>
        /// Gets or sets an array of double values representing height/elevation data for terrain vertices.
        /// Each element corresponds to the elevation at a specific point in the terrain mesh.
        /// </summary>
        double[] Heights { get; set; }
    }
}
