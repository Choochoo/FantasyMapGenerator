using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    public class Halfedge : IDisposable
    {
        private static List<Halfedge> _pool = new List<Halfedge>();
        public static Halfedge Create(Edge edge, LR lr)
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop().Init(edge, lr);
            }
            else
            {
                return new Halfedge(typeof(PrivateConstructorEnforcer), edge, lr);
            }
        }

        public static Halfedge CreateDummy()
        {
            return Create(null, null);
        }

        public Halfedge edgeListLeftNeighbor, edgeListRightNeighbor;
        public Halfedge nextInPriorityQueue;

        public Edge edge;
        public LR leftRight;
        public Vertex vertex;

        // the vertex's y-coordinate in the transformed Voronoi space V*
        public float ystar;

        public Halfedge(Type pce, Edge edge, LR lr)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Halfedge static readonlyructor is private");
            }

            Init(edge, lr);
        }

        private Halfedge Init(Edge edge, LR lr)
        {
            this.edge = edge;
            leftRight = lr;
            nextInPriorityQueue = null;
            vertex = null;
            return this;
        }

        public override string ToString()
        {
            return "Halfedge (leftRight: " + leftRight + "; vertex: " + vertex + ")";
        }

        public void Dispose()
        {
            if (edgeListLeftNeighbor != null || edgeListRightNeighbor != null)
            {
                // still in EdgeList
                return;
            }
            if (nextInPriorityQueue != null)
            {
                // still in PriorityQueue
                return;
            }
            edge = null;
            leftRight = null;
            vertex = null;
            _pool.Add(this);
        }

        public void ReallyDispose()
        {
            edgeListLeftNeighbor = null;
            edgeListRightNeighbor = null;
            nextInPriorityQueue = null;
            edge = null;
            leftRight = null;
            vertex = null;
            _pool.Add(this);
        }

        internal bool IsLeftOf(PointF p)
        {
            Site topSite;
            bool rightOfSite, above, fast;
            float dxp, dyp, dxs, t1, t2, t3, yl;

            topSite = edge.RightSite;
            rightOfSite = p.X > topSite.X;
            if (rightOfSite && this.leftRight == LR.LEFT)
            {
                return true;
            }
            if (!rightOfSite && this.leftRight == LR.RIGHT)
            {
                return false;
            }

            if (edge.a == 1.0)
            {
                dyp = p.Y - topSite.Y;
                dxp = p.X - topSite.X;
                fast = false;
                if ((!rightOfSite && edge.b < 0.0) || (rightOfSite && edge.b >= 0.0))
                {
                    above = dyp >= edge.b * dxp;
                    fast = above;
                }
                else
                {
                    above = p.X + p.Y * edge.b > edge.c;
                    if (edge.b < 0.0)
                    {
                        above = !above;
                    }
                    if (!above)
                    {
                        fast = true;
                    }
                }
                if (!fast)
                {
                    dxs = topSite.X - edge.LeftSite.X;
                    above = edge.b * (dxp * dxp - dyp * dyp) <
                            dxs * dyp * (1.0 + 2.0 * dxp / dxs + edge.b * edge.b);
                    if (edge.b < 0.0)
                    {
                        above = !above;
                    }
                }
            }
            else  /* edge.b == 1.0 */
            {
                yl = edge.c - edge.a * p.X;
                t1 = p.Y - yl;
                t2 = p.X - topSite.X;
                t3 = yl - topSite.Y;
                above = t1 * t1 > t2 * t2 + t3 * t3;
            }
            return this.leftRight == LR.LEFT ? above : !above;
        }
    }
}