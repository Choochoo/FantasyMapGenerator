using TerrainGenerator;

namespace WorldMap.Layers.Interfaces
{
    /// <summary>
    /// Interface that defines a contract for objects that contain city rendering data.
    /// Used for layers that need to manage and display city information on the map.
    /// </summary>
    public interface IHasCityRender
    {
        /// <summary>
        /// Gets or sets the city rendering data containing information about cities and their visual representation.
        /// This includes city locations, sizes, names, and other properties needed for map display.
        /// </summary>
        CityRender CityRender { get; set; }
    }
}
