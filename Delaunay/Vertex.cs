using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    public class Vertex : ICoord, IDisposable
    {
        internal static readonly Vertex VERTEX_AT_INFINITY = new Vertex(typeof(PrivateConstructorEnforcer), float.NaN, float.NaN);

        private static List<Vertex> _pool = new List<Vertex>();
        private static int _nvertices = 0;
        private PointF _coord;
        private int _vertexIndex;

        public PointF Coord()
        {
            return _coord;
        }

        public int VertexIndex
        {
            get
            {
                return _vertexIndex;
            }
        }

        public float X
        {
            get
            {
                return _coord.X;
            }
        }

        public float Y
        {
            get
            {
                return _coord.Y;
            }
        }

        private static Vertex Create(float x, float y)
        {
            if (float.IsNaN(x) || float.IsNaN(y))
            {
                return VERTEX_AT_INFINITY;
            }
            if (_pool.Count > 0)
            {
                return _pool.Pop().Init(x, y);
            }
            else
            {
                return new Vertex(typeof(PrivateConstructorEnforcer), x, y);
            }
        }

        public Vertex(Type pce, float x, float y)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Vertex static readonlyructor is private");
            }

            Init(x, y);
        }

        private Vertex Init(float x, float y)
        {
            _coord = new PointF(x, y);
            return this;
        }

        public void Dispose()
        {
            _coord = PointF.Empty;
            _pool.Add(this);
        }

        public void SetIndex()
        {
            _vertexIndex = _nvertices++;
        }

        public override string ToString()
        {
            return "Vertex (" + _vertexIndex + ")";
        }

        /**
         * This is the only way to make a Vertex
         * 
         * @param halfedge0
         * @param halfedge1
         * @return 
         * 
         */
        public static Vertex Intersect(Halfedge halfedge0, Halfedge halfedge1)
        {
            Edge edge0, edge1, edge;
            Halfedge halfedge;
            float determinant, intersectionX, intersectionY;
            bool rightOfSite;

            edge0 = halfedge0.edge;
            edge1 = halfedge1.edge;
            if (edge0 == null || edge1 == null)
            {
                return null;
            }
            if (edge0.RightSite == edge1.RightSite)
            {
                return null;
            }

            determinant = edge0.a * edge1.b - edge0.b * edge1.a;
            if (-1.0e-10 < determinant && determinant < 1.0e-10)
            {
                // the edges are parallel
                return null;
            }

            intersectionX = (edge0.c * edge1.b - edge1.c * edge0.b) / determinant;
            intersectionY = (edge1.c * edge0.a - edge0.c * edge1.a) / determinant;

            if (Voronoi.CompareByYThenX(edge0.RightSite, edge1.RightSite) < 0)
            {
                halfedge = halfedge0; edge = edge0;
            }
            else
            {
                halfedge = halfedge1; edge = edge1;
            }
            rightOfSite = intersectionX >= edge.RightSite.X;
            if ((rightOfSite && halfedge.leftRight == LR.LEFT)
            || (!rightOfSite && halfedge.leftRight == LR.RIGHT))
            {
                return null;
            }

            return Vertex.Create(intersectionX, intersectionY);
        }
    }
}