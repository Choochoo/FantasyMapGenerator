namespace WorldMap.Layers.Interfaces
{
    /// <summary>
    /// Interface that defines a contract for objects that contain downhill flow direction data.
    /// Used for terrain features that need to track water flow and erosion patterns.
    /// </summary>
    public interface IHasDownhill
    {
        /// <summary>
        /// Gets or sets an array of integers representing downhill flow directions for each vertex.
        /// Each element indicates the direction of steepest descent for water flow calculations.
        /// </summary>
        int[] Downhill { get; set; }
    }
}
