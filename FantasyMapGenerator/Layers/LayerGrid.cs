using System.Collections.Generic;
using D3Voronoi;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerGrid : IHasVoronoi
    {
        public static LayerGrid Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerGrid();
                }
                return _instance;
            }
        }

        private static LayerGrid _instance;

        public List<Point> MeshPts { get; set; }
        public List<Point> MeshVxs { get; set; }
        public Voronoi VoronoiGenerator { get; set; }

    }

}
