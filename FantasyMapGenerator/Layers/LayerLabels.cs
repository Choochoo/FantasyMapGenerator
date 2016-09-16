
using D3Voronoi;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerLabels : IHasHeights, IHasMesh, IHasDownhill, IHasCityRender, IHasVoronoi
    {
        public static LayerLabels Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerLabels();
                }
                return _instance;
            }
        }

        private static LayerLabels _instance;
        private Mesh _mesh;

        public double[] Heights { get; set; }

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public int[] Downhill { get; set; }
        public CityRender CityRender { get; set; }
        public Voronoi VoronoiGenerator { get; set; }
    }

}
