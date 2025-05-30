using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    /// <summary>
    /// Singleton class responsible for managing layer rendering data including heights, mesh, and downhill flow information.
    /// Implements multiple interfaces to provide height data, mesh data, downhill flow data, and reset functionality.
    /// </summary>
    public class LayerRendering : IHasHeights, IHasMesh, IHasDownhill, IResetable
    {
        /// <summary>
        /// Gets the singleton instance of the LayerRendering class.
        /// Creates a new instance if one doesn't already exist.
        /// </summary>
        public static LayerRendering Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerRendering();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private static field holding the singleton instance of LayerRendering.
        /// </summary>
        private static LayerRendering _instance;
        
        /// <summary>
        /// Private field storing the height values for the terrain mesh vertices.
        /// </summary>
        private double[] _heights;
        
        /// <summary>
        /// Private field storing the mesh data structure used for rendering.
        /// </summary>
        private Mesh _mesh;

        /// <summary>
        /// Gets or sets the height values for the terrain mesh vertices.
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
        /// Gets or sets the mesh data structure containing vertex and edge information for terrain rendering.
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        /// <summary>
        /// Gets or sets the downhill flow direction data for each vertex in the mesh.
        /// Used for water flow and erosion calculations.
        /// </summary>
        public int[] Downhill { get; set; }
        
        /// <summary>
        /// Resets all layer rendering data by setting mesh, heights, and downhill flow to null.
        /// This clears all cached rendering data and prepares the layer for new data.
        /// </summary>
        public void Reset()
        {
            _mesh = null;
            Heights = null;
            Downhill = null;
        }
    }

}
