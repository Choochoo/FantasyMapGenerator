using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    /// <summary>
    /// Singleton class responsible for managing erosion simulation data on the map.
    /// Handles terrain heights, mesh data, downhill flow calculations, and provides reset functionality.
    /// Implements multiple interfaces to provide comprehensive erosion layer management.
    /// </summary>
    public class LayerErosion : IHasHeights, IHasMesh, IHasDownhill, IResetable
    {
        /// <summary>
        /// Private static field holding the singleton instance of LayerErosion.
        /// </summary>
        private static LayerErosion _instance;
        
        /// <summary>
        /// Private field storing the height values for the terrain erosion vertices.
        /// </summary>
        private double[] _heights;

        /// <summary>
        /// Gets the singleton instance of the LayerErosion class.
        /// Creates a new instance if one doesn't already exist.
        /// </summary>
        public static LayerErosion Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LayerErosion();
                return _instance;
            }
        }

        /// <summary>
        /// Gets or sets the downhill flow direction data used for erosion calculations.
        /// Each element indicates the direction of steepest descent for water flow and erosion patterns.
        /// </summary>
        public int[] Downhill { get; set; }
        
        /// <summary>
        /// Gets or sets the height values for terrain vertices used in erosion simulation.
        /// If heights are null and mesh data is available, initializes a new height array with the same length as mesh vertices.
        /// </summary>
        public double[] Heights
        {
            get
            {
                if ((_heights == null) && (Mesh != null) && (Mesh.Vxs != null))
                    _heights = new double[Mesh.Vxs.Length];
                return _heights;
            }
            set { _heights = value; }
        }

        /// <summary>
        /// Gets or sets the mesh data structure containing vertex and edge information for erosion calculations.
        /// </summary>
        public Mesh Mesh { get; set; }
        
        /// <summary>
        /// Resets all erosion layer data by setting mesh, heights, and downhill flow to null.
        /// This clears all cached erosion data and prepares the layer for new simulation.
        /// </summary>
        public void Reset()
        {
            Mesh = null;
            Heights = null;
            Downhill = null;
        }
    }
}