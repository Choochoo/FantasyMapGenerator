using D3Voronoi;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerErosion : IHasHeights, IHasMesh, IHasDownhill, IHasVoronoi
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
        public Voronoi VoronoiGenerator { get; set; }
        public double[] Heights
        {
            get
            {
                if ((_heights == null) && (Mesh != null) && (Mesh.Vxs != null))
                    _heights = new double[Mesh.Vxs.Count];
                return _heights;
            }
            set { _heights = value; }
        }

        public Mesh Mesh { get; set; }

        public static void GenerateUneroded()
        {
            Instance.Mesh = Terrain.GenerateGoodMesh(Instance, 4096, Extent.DefaultExtent);
            Instance.Heights = Terrain.Add(Instance.Heights.Length, Terrain.Slope(Instance.Mesh, Terrain.RandomVector(4)),
                Terrain.Cone(Instance.Mesh, Terrain.Runif(-1, 1)),
                Terrain.Mountains(Instance.Mesh, 50));
            Instance.Heights = Terrain.Peaky(Instance.Heights);
            Instance.Heights = Terrain.FillSinks(Instance.Mesh, Instance.Heights);
            Instance.Heights = Terrain.SetSeaLevel(Instance.Heights, 0.5f);
        }
    }
}