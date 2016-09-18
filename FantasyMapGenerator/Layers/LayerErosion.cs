using TerrainGenerator;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerErosion : IHasHeights, IHasMesh, IHasDownhill, IResetable
    {
        private static LayerErosion _instance;
        private double[] _heights;

        public static LayerErosion Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LayerErosion();
                return _instance;
            }
        }

        public int[] Downhill { get; set; }
        public double[] Heights
        {
            get
            {
                if ((_heights == null) && (Mesh != null) && (Mesh.Vxs != null))
                    _heights = new double[Mesh.Vxs.Length];
                return _heights;
            }
            set { _heights = value; }
        }

        public Mesh Mesh { get; set; }
        public void Reset()
        {
            Mesh = null;
            Heights = null;
            Downhill = null;
        }

    }
}