using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    /**
     * The line segment connecting the two Sites is part of the Delaunay triangulation;
     * the line segment connecting the two Vertices is part of the Voronoi diagram
     * @author ashaw
     * 
     */
    public class Edge : IDisposable
    {
        private static List<Edge> _pool = new List<Edge>();

        /**
         * This is the only way to create a new Edge 
         * @param site0
         * @param site1
         * @return 
         * 
         */
        internal static Edge CreateBisectingEdge(Site site0, Site site1)
        {
            float dx, dy, absdx, absdy;
            float a, b, c;

            dx = site1.X - site0.X;
            dy = site1.Y - site0.Y;
            absdx = dx > 0 ? dx : -dx;
            absdy = dy > 0 ? dy : -dy;
            c = site0.X * dx + site0.Y * dy + (dx * dx + dy * dy) * 0.5f;
            if (absdx > absdy)
            {
                a = 1.0f; b = dy / dx; c /= dx;
            }
            else
            {
                b = 1.0f; a = dx / dy; c /= dy;
            }

            Edge edge = Edge.Create();

            edge.LeftSite = site0;
            edge.RightSite = site1;
            site0.AddEdge(edge);
            site1.AddEdge(edge);

            edge._leftVertex = null;
            edge._rightVertex = null;

            edge.a = a; edge.b = b; edge.c = c;
            //trace("createBisectingEdge: a ", edge.a, "b", edge.b, "c", edge.c);

            return edge;
        }

        private static Edge Create()
        {
            Edge edge;
            if (_pool.Count > 0)
            {
                edge = _pool.Pop();
                edge.Init();
            }
            else
            {
                edge = new Edge(typeof(PrivateConstructorEnforcer));
            }
            return edge;
        }


        public LineSegment DelaunayLine()
        {
            // draw a line connecting the input Sites for which the edge is a bisector:
            return new LineSegment(LeftSite.Coord(), RightSite.Coord());
        }

        public LineSegment VoronoiEdge()
        {
            if (!Visible) return new LineSegment(PointF.Empty, PointF.Empty);
            return new LineSegment(_clippedVertices[LR.LEFT],
                                   _clippedVertices[LR.RIGHT]);
        }

        private static int _nedges = 0;

        internal static readonly Edge DELETED = new Edge(typeof(PrivateConstructorEnforcer));

        // the equation of the edge: ax + by = c
        internal float a, b, c;

        // the two Voronoi vertices that the edge connects
        //		(if one of them is null, the edge extends to infinity)
        private Vertex _leftVertex;
        public Vertex LeftVertex
        {
            get
            {
                return _leftVertex;
            }
        }
        private Vertex _rightVertex;
        public Vertex RightVertex
        {
            get
            {
                return _rightVertex;
            }
        }
        internal Vertex GetVertex(LR leftRight)
        {
            return (leftRight == LR.LEFT) ? _leftVertex : _rightVertex;
        }
        internal void SetVertex(LR leftRight, Vertex v)
        {
            if (leftRight == LR.LEFT)
            {
                _leftVertex = v;
            }
            else
            {
                _rightVertex = v;
            }
        }

        internal bool IsPartOfConvexHull()
        {
            return (_leftVertex == null || _rightVertex == null);
        }

        public float SitesDistance()
        {
            return Utilities.Distance(LeftSite.Coord(), RightSite.Coord());
        }

        public static float CompareSitesDistancesMax(Edge edge0, Edge edge1)
        {
            float length0 = edge0.SitesDistance();
            float length1 = edge1.SitesDistance();
            if (length0 < length1)
            {
                return 1;
            }
            if (length0 > length1)
            {
                return -1;
            }
            return 0;
        }

        public static float CompareSitesDistances(Edge edge0, Edge edge1)
        {
            return -CompareSitesDistancesMax(edge0, edge1);
        }

        // Once clipVertices() is called, this Dictionary will hold two PointFs
        // representing the clipped coordinates of the left and right ends...
        private Dictionary<LR, PointF> _clippedVertices;
        public Dictionary<LR, PointF> ClippedEnds
        {
            get
            {
                return _clippedVertices;
            }
        }
        // unless the entire Edge is outside the bounds.
        // In that case visible will be false:
        internal bool Visible
        {
            get
            {
                return _clippedVertices != null;
            }
        }

        // the two input Sites for which this Edge is a bisector:
        private Dictionary<LR, Site> _sites;

        public Site LeftSite
        {
            set
            {
                _sites[LR.LEFT] = value;
            }
            get
            {
                return _sites[LR.LEFT];
            }
        }
        public Site RightSite
        {
            set
            {
                _sites[LR.RIGHT] = value;
            }
            get
            {
                return _sites[LR.RIGHT];
            }
        }
        internal Site GetSite(LR leftRight)
        {
            return _sites[leftRight] as Site;
        }

        private int _edgeIndex;

        /// <summary>
        /// Disposes of this edge by clearing its vertices, clipped vertices, and sites, then returns it to the object pool.
        /// </summary>
        public void Dispose()
        {
            _leftVertex = null;
            _rightVertex = null;
            if (_clippedVertices != null)
            {
                _clippedVertices[LR.LEFT] = PointF.Empty;
                _clippedVertices[LR.RIGHT] = PointF.Empty;
                _clippedVertices = null;
            }
            _sites[LR.LEFT] = null;
            _sites[LR.RIGHT] = null;
            _sites = null;

            _pool.Add(this);
        }

        /// <summary>
        /// Initializes a new Edge instance with enforced private construction.
        /// </summary>
        /// <param name="pce">Private constructor enforcer to prevent external instantiation.</param>
        public Edge(Type pce)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Edge: static readonlyructor is private");
            }

            _edgeIndex = _nedges++;
            Init();
        }

        /// <summary>
        /// Initializes this edge by creating the sites dictionary.
        /// </summary>
        private void Init()
        {
            _sites = new Dictionary<LR, Site>();
        }

        public override string ToString()
        {
            return "Edge " + _edgeIndex + "; sites " + _sites[LR.LEFT] + ", " + _sites[LR.RIGHT]
                    + "; endVertices " + (_leftVertex != null ? _leftVertex.VertexIndex.ToString() : "null") + ", "
                     + (_rightVertex != null ? _rightVertex.VertexIndex.ToString() : "null") + "::";
        }

        /// <summary>
        /// Clips the edge vertices to be within the specified bounds rectangle.
        /// Sets the clipped vertices to contain the visible portion of the edge within bounds.
        /// </summary>
        /// <param name="bounds">The rectangular bounds to clip the edge to.</param>
        internal void ClipVertices(RectangleF bounds)
        {
            float xmin = bounds.X;
            float ymin = bounds.Y;
            float xmax = bounds.Right;
            float ymax = bounds.Bottom;

            Vertex vertex0, vertex1;
            float x0, x1, y0, y1;

            if (a == 1.0 && b >= 0.0)
            {
                vertex0 = _rightVertex;
                vertex1 = _leftVertex;
            }
            else
            {
                vertex0 = _leftVertex;
                vertex1 = _rightVertex;
            }

            if (a == 1.0)
            {
                y0 = ymin;
                if (vertex0 != null && vertex0.Y > ymin)
                {
                    y0 = vertex0.Y;
                }
                if (y0 > ymax)
                {
                    return;
                }
                x0 = c - b * y0;

                y1 = ymax;
                if (vertex1 != null && vertex1.Y < ymax)
                {
                    y1 = vertex1.Y;
                }
                if (y1 < ymin)
                {
                    return;
                }
                x1 = c - b * y1;

                if ((x0 > xmax && x1 > xmax) || (x0 < xmin && x1 < xmin))
                {
                    return;
                }

                if (x0 > xmax)
                {
                    x0 = xmax; y0 = (c - x0) / b;
                }
                else if (x0 < xmin)
                {
                    x0 = xmin; y0 = (c - x0) / b;
                }

                if (x1 > xmax)
                {
                    x1 = xmax; y1 = (c - x1) / b;
                }
                else if (x1 < xmin)
                {
                    x1 = xmin; y1 = (c - x1) / b;
                }
            }
            else
            {
                x0 = xmin;
                if (vertex0 != null && vertex0.X > xmin)
                {
                    x0 = vertex0.X;
                }
                if (x0 > xmax)
                {
                    return;
                }
                y0 = c - a * x0;

                x1 = xmax;
                if (vertex1 != null && vertex1.X < xmax)
                {
                    x1 = vertex1.X;
                }
                if (x1 < xmin)
                {
                    return;
                }
                y1 = c - a * x1;

                if ((y0 > ymax && y1 > ymax) || (y0 < ymin && y1 < ymin))
                {
                    return;
                }

                if (y0 > ymax)
                {
                    y0 = ymax; x0 = (c - y0) / a;
                }
                else if (y0 < ymin)
                {
                    y0 = ymin; x0 = (c - y0) / a;
                }

                if (y1 > ymax)
                {
                    y1 = ymax; x1 = (c - y1) / a;
                }
                else if (y1 < ymin)
                {
                    y1 = ymin; x1 = (c - y1) / a;
                }
            }

            _clippedVertices = new Dictionary<LR, PointF>();
            if (vertex0 == _leftVertex)
            {
                _clippedVertices[LR.LEFT] = new PointF(x0, y0);
                _clippedVertices[LR.RIGHT] = new PointF(x1, y1);
            }
            else
            {
                _clippedVertices[LR.RIGHT] = new PointF(x0, y0);
                _clippedVertices[LR.LEFT] = new PointF(x1, y1);
            }
        }
    }
}