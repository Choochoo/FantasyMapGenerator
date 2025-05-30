using D3Voronoi;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    /// <summary>
    /// Singleton class responsible for managing grid data on the map.
    /// Handles mesh points and vertices for grid-based rendering and calculations.
    /// Implements the IResetable interface to provide reset functionality.
    /// </summary>
    public class LayerGrid : IResetable
    {
        /// <summary>
        /// Gets the singleton instance of the LayerGrid class.
        /// Creates a new instance if one doesn't already exist.
        /// </summary>
        public static LayerGrid Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerGrid();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private static field holding the singleton instance of LayerGrid.
        /// </summary>
        private static LayerGrid _instance;

        /// <summary>
        /// Gets or sets the array of mesh points used for grid calculations and rendering.
        /// These points define the grid structure for map generation.
        /// </summary>
        public Point[] MeshPts { get; set; }
        
        /// <summary>
        /// Gets or sets the array of mesh vertices used for grid calculations and rendering.
        /// These vertices define the grid vertex positions for map generation.
        /// </summary>
        public Point[] MeshVxs { get; set; }
        
        /// <summary>
        /// Resets all grid layer data by setting mesh points and vertices to null.
        /// This clears all cached grid data and prepares the layer for new information.
        /// </summary>
        public void Reset()
        {
            MeshPts = null;
            MeshVxs = null;
        }
    }
}
