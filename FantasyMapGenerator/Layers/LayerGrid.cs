using D3Voronoi;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerGrid : IResetable
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
        public void Reset()
        {
            MeshPts = null;
            MeshVxs = null;
        }

    }

}
