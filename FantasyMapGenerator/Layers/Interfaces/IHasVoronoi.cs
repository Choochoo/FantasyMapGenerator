


using D3Voronoi;

namespace WorldMap.Layers.Interfaces
{
    public interface IHasVoronoi
    {
        Voronoi VoronoiGenerator { get; set; }
    }
}
