using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    /// <summary>
    /// Singleton class responsible for managing outline rendering data on the map.
    /// Handles terrain heights and mesh data for drawing map outlines and boundaries.
    /// Implements interfaces for height and mesh data management with reset functionality.
    /// </summary>
    public class LayerOutline : IHasHeights, IHasMesh, IResetable
    {
        /// <summary>
        /// Gets the singleton instance of the LayerOutline class.
        /// Creates a new instance if one doesn't already exist.
        /// </summary>
        public static LayerOutline Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerOutline();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private static field holding the singleton instance of LayerOutline.
        /// </summary>
        private static LayerOutline _instance;
        
        /// <summary>
        /// Private field storing the height values for the terrain outline vertices.
        /// </summary>
        private double[] _heights;

        /// <summary>
        /// Gets or sets the height values for terrain vertices used in outline generation.
        /// If heights are null and mesh data is available, initializes a new height array with the same length as mesh vertices.
        /// </summary>
        public double[] Heights
        {
            get
            {
                if (_heights == null && Mesh != null && Mesh.Vxs != null)
                {
                    _heights = new double[Mesh.Vxs.Length];
                }
                return _heights;
            }
            set { _heights = value; }
        }

        /// <summary>
        /// Gets or sets the mesh data structure containing vertex and edge information for outline rendering.
        /// </summary>
        public Mesh Mesh { get; set; }
        
        /// <summary>
        /// Resets all outline layer data by setting mesh and heights to null.
        /// This clears all cached outline data and prepares the layer for new information.
        /// </summary>
        public void Reset()
        {
            Mesh = null;
            Heights = null;
        }
    }
}
