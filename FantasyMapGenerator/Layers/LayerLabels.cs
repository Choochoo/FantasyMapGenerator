using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    /// <summary>
    /// Singleton class responsible for managing label rendering data on the map.
    /// Handles terrain heights, mesh data, downhill flow, city rendering, and provides reset functionality.
    /// Implements multiple interfaces to provide comprehensive label layer management.
    /// </summary>
    public class LayerLabels : IHasHeights, IHasMesh, IHasDownhill, IHasCityRender, IResetable
    {
        /// <summary>
        /// Gets the singleton instance of the LayerLabels class.
        /// Creates a new instance if one doesn't already exist.
        /// </summary>
        public static LayerLabels Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerLabels();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private static field holding the singleton instance of LayerLabels.
        /// </summary>
        private static LayerLabels _instance;
        
        /// <summary>
        /// Private field storing the mesh data structure used for label rendering.
        /// </summary>
        private Mesh _mesh;

        /// <summary>
        /// Gets or sets the height values for terrain vertices used in label positioning calculations.
        /// </summary>
        public double[] Heights { get; set; }

        /// <summary>
        /// Gets or sets the mesh data structure containing vertex and edge information for label placement.
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        /// <summary>
        /// Gets or sets the downhill flow direction data used for label placement optimization.
        /// </summary>
        public int[] Downhill { get; set; }
        
        /// <summary>
        /// Gets or sets the city rendering data containing city information for label display.
        /// </summary>
        public CityRender CityRender { get; set; }

        /// <summary>
        /// Resets all label layer data by setting mesh, heights, downhill flow, and city render data to null.
        /// This clears all cached label data and prepares the layer for new information.
        /// </summary>
        public void Reset()
        {
            _mesh = null;
            Heights = null;
            Downhill = null;
            CityRender = null;
        }
    }
}
