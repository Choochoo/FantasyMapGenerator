using System.Collections.Generic;
using Voronoi2;
using WorldMap.Layers.Interfaces;

namespace WorldMap.Layers
{
    public class LayerOutline : IHasHeights, IHasMesh, IHasEdges
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
        private Mesh _mesh;
        private List<GraphEdge> _edges;

        public double[] Heights
        {
            get
            {
                if (_heights == null && Mesh != null && Mesh.Vxs != null)
                {
                    _heights = new double[Mesh.Vxs.Count];
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

        public List<GraphEdge> Edges
        {
            get { return _edges; }
            set { _edges = value; }
        }
    }

}
