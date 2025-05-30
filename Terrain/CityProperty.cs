using Priority_Queue;

namespace TerrainGenerator
{
    /// <summary>
    /// Represents properties and characteristics of a city in the map generation system.
    /// Contains scoring information and vertex associations for city placement and evaluation.
    /// </summary>
    public class CityProperty
    {
        /// <summary>
        /// The score value indicating the suitability or importance of this city location.
        /// Higher scores typically indicate better locations for city placement.
        /// </summary>
        public double Score;
        
        /// <summary>
        /// The identifier or index of the city.
        /// Used to reference and distinguish between different cities on the map.
        /// </summary>
        public int City;
        
        /// <summary>
        /// The vertex index where this city is located in the mesh.
        /// References a specific point in the terrain mesh structure.
        /// </summary>
        public int Vx;
    }
}
