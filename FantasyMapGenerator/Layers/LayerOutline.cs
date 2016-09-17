

using D3Voronoi;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerOutline : IHasHeights, IHasMesh, IHasVoronoi
    {
        public static LayerOutline Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerOutline();
                }
                return _instance;
            }
        }

        private static LayerOutline _instance;
        private double[] _heights;

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

        public Mesh Mesh { get; set; }
        public Voronoi Voronoi { get; set; }
    }

}
