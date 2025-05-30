namespace TerrainGenerator
{
    /// <summary>
    /// Contains configuration properties for map area generation and rendering.
    /// Defines parameters for point density, city placement, territories, and font sizes for map labels.
    /// </summary>
    public class AreaProperties
    {
        /// <summary>
        /// The number of points to generate for the mesh during map creation.
        /// Higher values create more detailed terrain but require more processing time.
        /// Default value is 16384.
        /// </summary>
        public int NumberOfPoints = 16384;
        
        /// <summary>
        /// The number of cities to place on the generated map.
        /// Cities are positioned based on terrain suitability and spacing algorithms.
        /// Default value is 15.
        /// </summary>
        public int NumberOfCities = 15;
        
        /// <summary>
        /// The number of territories or regions to create on the map.
        /// Territories define political or administrative boundaries around cities.
        /// Default value is 5.
        /// </summary>
        public int NumberOfTerritories = 5;
        
        /// <summary>
        /// Font size for region labels when rendering the map.
        /// Used for displaying territory or large area names.
        /// Default value is 40.
        /// </summary>
        public int FontSizeRegion = 40;
        
        /// <summary>
        /// Font size for city labels when rendering the map.
        /// Used for displaying city names on the map.
        /// Default value is 25.
        /// </summary>
        public int FontSizeCity = 25;
        
        /// <summary>
        /// Font size for town labels when rendering the map.
        /// Used for displaying smaller settlement names on the map.
        /// Default value is 20.
        /// </summary>
        public int FontSizeTown = 20;
    }
}
