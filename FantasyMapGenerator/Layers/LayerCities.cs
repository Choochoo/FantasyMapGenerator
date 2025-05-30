using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    /// <summary>
    /// Singleton class responsible for managing city rendering data on the map.
    /// Handles terrain heights, mesh data, downhill flow, city rendering information, and provides reset functionality.
    /// Implements multiple interfaces to provide comprehensive city layer management.
    /// </summary>
    public class LayerCities : IHasHeights, IHasMesh, IHasDownhill, IHasCityRender, IResetable
    {
        /// <summary>
        /// Gets the singleton instance of the LayerCities class.
        /// Creates a new instance if one doesn't already exist.
        /// </summary>
        public static LayerCities Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerCities();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private static field holding the singleton instance of LayerCities.
        /// </summary>
        private static LayerCities _instance;
        
        /// <summary>
        /// Private field storing the mesh data structure used for city rendering.
        /// </summary>
        private Mesh _mesh;

        /// <summary>
        /// Gets or sets the height values for terrain vertices used in city placement calculations.
        /// </summary>
        public double[] Heights { get; set; }

        /// <summary>
        /// Gets or sets the mesh data structure containing vertex and edge information for city placement.
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        /// <summary>
        /// Gets or sets the downhill flow direction data used for city placement optimization.
        /// </summary>
        public int[] Downhill { get; set; }
        
        /// <summary>
        /// Gets or sets the city rendering data containing detailed city information for display.
        /// </summary>
        public CityRender CityRender { get; set; }
        
        /// <summary>
        /// Resets all city layer data by setting mesh, heights, downhill flow, and city render data to null.
        /// This clears all cached city data and prepares the layer for new information.
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
