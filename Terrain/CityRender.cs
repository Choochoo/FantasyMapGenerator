using System.Collections.Generic;
using D3Voronoi;

namespace TerrainGenerator
{
    /// <summary>
    /// Contains rendering data and properties for cities and geographical features on the map.
    /// Manages city locations, territories, rivers, coasts, borders, and area properties for map visualization.
    /// </summary>
    public class CityRender
    {
        /// <summary>
        /// Private field storing the area properties for the city render.
        /// </summary>
        private AreaProperties _areaProperties = null;
        
        /// <summary>
        /// Gets or sets the area properties containing geographical and terrain information.
        /// Automatically creates a new AreaProperties instance if null when accessed.
        /// </summary>
        public AreaProperties AreaProperties
        {
            get
            {
                if (_areaProperties == null)
                {
                    _areaProperties = new AreaProperties();
                }
                return _areaProperties;
            }
            set { _areaProperties = value; }
        }
        
        /// <summary>
        /// Array of scores for each vertex, used for city placement and territory calculations.
        /// Higher scores indicate more suitable locations for cities or important features.
        /// </summary>
        public double[] Score;
        
        /// <summary>
        /// Array indicating territory ownership for each vertex.
        /// Values represent which territory or faction controls each area of the map.
        /// </summary>
        public double[] Territories;
        
        /// <summary>
        /// List of vertex indices where cities are located.
        /// Each integer represents the index of a vertex that contains a city.
        /// </summary>
        public List<int> Cities = new List<int>();

        /// <summary>
        /// List of river paths, where each river is represented as a list of points.
        /// Used for rendering water features and determining geographical boundaries.
        /// </summary>
        public List<List<Point>> Rivers;
        
        /// <summary>
        /// List of coastal paths, where each coast is represented as a list of points.
        /// Used for rendering shorelines and determining land-water boundaries.
        /// </summary>
        public List<List<Point>> Coasts;
        
        /// <summary>
        /// List of border paths, where each border is represented as a list of points.
        /// Used for rendering political or territorial boundaries between regions.
        /// </summary>
        public List<List<Point>> Borders;
    }
}
