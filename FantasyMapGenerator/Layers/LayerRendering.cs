using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerRendering : IHasHeights, IHasMesh, IHasDownhill, IResetable
    {
        public static LayerRendering Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LayerRendering();
                }
                return _instance;
            }
        }

        private static LayerRendering _instance;
        private double[] _heights;
        private Mesh _mesh;

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

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public int[] Downhill { get; set; }
        public void Reset()
        {
            _mesh = null;
            Heights = null;
            Downhill = null;
        }
    }

}
