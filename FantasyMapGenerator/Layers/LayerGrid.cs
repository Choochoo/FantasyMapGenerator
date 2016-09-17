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

        public Point[] MeshPts { get; set; }
        public Point[] MeshVxs { get; set; }
        public Voronoi Voronoi { get; set; }

    }

}
