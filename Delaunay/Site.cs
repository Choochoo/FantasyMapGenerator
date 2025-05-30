using System;
using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    /// <summary>
    /// Represents a site (point) in a Voronoi diagram with associated edges and region information.
    /// Sites are the fundamental elements used to generate Voronoi diagrams and Delaunay triangulations.
    /// </summary>
    public class Site : ICoord
    {
        private static List<Site> _pool = new List<Site>();
        
        /// <summary>
        /// Creates a new Site instance from the object pool or constructs a new one if the pool is empty.
        /// </summary>
        /// <param name="p">The position of the site as a PointF.</param>
        /// <param name="index">The unique index identifier for this site.</param>
        /// <param name="weight">The weight value associated with this site.</param>
        /// <param name="color">The color value associated with this site.</param>
        /// <returns>A new or recycled Site instance initialized with the provided values.</returns>
        public static Site Create(PointF p, int index, float weight, uint color)
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop().Init(p, index, weight, color);
            }
            else
            {
                return new Site(typeof(PrivateConstructorEnforcer), p, index, weight, color);
            }
        }

        /// <summary>
        /// Sorts a list of sites first by Y coordinate, then by X coordinate, and updates site indices accordingly.
        /// </summary>
        /// <param name="sites">The list of sites to sort in-place.</param>
        internal static void SortSites(List<Site> sites)
        {
            sites.SortFunc(Site.Compare);
        }

        /// <summary>
        /// Compares two sites for sorting purposes, prioritizing Y coordinate then X coordinate.
        /// Also swaps site indices as needed to maintain consistency with the new ordering.
        /// </summary>
        /// <param name="s1">The first site to compare.</param>
        /// <param name="s2">The second site to compare.</param>
        /// <returns>-1 if s1 should come before s2, 1 if s2 should come before s1, 0 if equal.</returns>
        private static float Compare(Site s1, Site s2)
        {
            int returnValue = (int)Voronoi.CompareByYThenX(s1, s2);

            // swap _siteIndex values if necessary to match new ordering:
            int tempIndex;
            if (returnValue == -1)
            {
                if (s1._siteIndex > s2._siteIndex)
                {
                    tempIndex = (int)s1._siteIndex;
                    s1._siteIndex = s2._siteIndex;
                    s2._siteIndex = (uint)tempIndex;
                }
            }
            else if (returnValue == 1)
            {
                if (s2._siteIndex > s1._siteIndex)
                {
                    tempIndex = (int)s2._siteIndex;
                    s2._siteIndex = s1._siteIndex;
                    s1._siteIndex = (uint)tempIndex;
                }

            }

            return returnValue;
        }


        private static readonly float EPSILON = .005f;
        
        /// <summary>
        /// Determines if two points are close enough to be considered the same location.
        /// </summary>
        /// <param name="p0">The first point to compare.</param>
        /// <param name="p1">The second point to compare.</param>
        /// <returns>True if the points are within the epsilon distance threshold.</returns>
        private static bool CloseEnough(PointF p0, PointF p1)
        {
            return Utilities.Distance(p0, p1) < EPSILON;
        }

        private PointF _coord;

        /// <summary>
        /// Gets the coordinate position of this site.
        /// </summary>
        /// <returns>The PointF representing the site's position.</returns>
        public PointF Coord()
        {
            return _coord;
        }

        internal uint color;
        internal float weight;

        private uint _siteIndex;

        // the edges that define this Site's Voronoi region:
        private List<Edge> _edges;

        /// <summary>
        /// Gets the list of edges that define this site's Voronoi region.
        /// </summary>
        internal List<Edge> Edges
        {
            get
            {
                return _edges;
            }
        }
        // which end of each edge hooks up with the previous edge in _edges:
        private List<LR> _edgeOrientations;
        // ordered list of PointFs that define the region clipped to bounds:
        private List<PointF> _region;

        /// <summary>
        /// Initializes a new Site instance with enforced private construction.
        /// </summary>
        /// <param name="pce">Private constructor enforcer to prevent external instantiation.</param>
        /// <param name="p">The position of the site.</param>
        /// <param name="index">The site's index identifier.</param>
        /// <param name="weight">The weight associated with this site.</param>
        /// <param name="color">The color associated with this site.</param>
        public Site(Type pce, PointF p, int index, float weight, uint color)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Site static readonlyructor is private");
            }
            Init(p, index, weight, color);
        }

        /// <summary>
        /// Initializes or reinitializes this site with new values.
        /// </summary>
        /// <param name="p">The position of the site.</param>
        /// <param name="index">The site's index identifier.</param>
        /// <param name="weight">The weight associated with this site.</param>
        /// <param name="color">The color associated with this site.</param>
        /// <returns>This site instance for method chaining.</returns>
        private Site Init(PointF p, int index, float weight, uint color)
        {
            _coord = p;
            _siteIndex = (uint)index;
            this.weight = weight;
            this.color = color;
            _edges = new List<Edge>();
            _region = null;
            return this;
        }

        /// <summary>
        /// Returns a string representation of this site including its index and coordinates.
        /// </summary>
        /// <returns>A descriptive string of this site.</returns>
        public override string ToString()
        {
            return "Site " + _siteIndex + ": " + Coord().ToString();
        }

        /// <summary>
        /// Moves this site to a new position and clears any cached data.
        /// </summary>
        /// <param name="p">The new position for this site.</param>
        private void Move(PointF p)
        {
            Clear();
            _coord = p;
        }

        /// <summary>
        /// Disposes of this site by clearing its data and returning it to the object pool.
        /// </summary>
        public void Dispose()
        {
            _coord = PointF.Empty;
            Clear();
            _pool.Add(this);
        }

        /// <summary>
        /// Clears all cached data associated with this site including edges and region information.
        /// </summary>
        private void Clear()
        {
            if (_edges != null)
            {
                _edges.Clear();
                _edges = null;
            }
            if (_edgeOrientations != null)
            {
                _edgeOrientations.Clear();
                _edgeOrientations = null;
            }
            if (_region != null)
            {
                _region.Clear();
                _region = null;
            }
        }

        /// <summary>
        /// Adds an edge to this site's list of defining edges.
        /// </summary>
        /// <param name="edge">The edge to add to this site.</param>
        internal void AddEdge(Edge edge)
        {
            _edges.Add(edge);
        }

        /// <summary>
        /// Finds the nearest edge to this site by comparing distances.
        /// </summary>
        /// <returns>The edge that is closest to this site.</returns>
        internal Edge NearestEdge()
        {
            _edges.SortFunc(Edge.CompareSitesDistances);
            return _edges[0];
        }

        /// <summary>
        /// Gets all sites that are neighbors to this site (connected by edges).
        /// </summary>
        /// <returns>A list of neighboring sites.</returns>
        internal List<Site> NeighborSites()
        {
            if (_edges == null || _edges.Count == 0)
            {
                return new List<Site>();
            }
            if (_edgeOrientations == null)
            {
                ReorderEdges();
            }
            List<Site> list = new List<Site>();
            foreach (Edge edge in _edges)
            {
                list.Add(NeighborSite(edge));
            }
            return list;
        }

        /// <summary>
        /// Gets the neighboring site connected by the specified edge.
        /// </summary>
        /// <param name="edge">The edge connecting this site to its neighbor.</param>
        /// <returns>The neighboring site, or null if this site is not connected to the edge.</returns>
        private Site NeighborSite(Edge edge)
        {
            if (this == edge.LeftSite)
            {
                return edge.RightSite;
            }
            if (this == edge.RightSite)
            {
                return edge.LeftSite;
            }
            return null;
        }

        /// <summary>
        /// Gets the Voronoi region for this site clipped to the specified bounds.
        /// </summary>
        /// <param name="clippingBounds">The rectangular bounds to clip the region to.</param>
        /// <returns>A list of points defining the clipped Voronoi region.</returns>
        internal List<PointF> Region(RectangleF clippingBounds)
        {
            if (_edges == null || _edges.Count == 0)
            {
                return new List<PointF>();
            }
            if (_edgeOrientations == null)
            {
                ReorderEdges();
                _region = ClipToBounds(clippingBounds);
                if ((new Polygon(_region)).GetWinding() == Winding.CLOCKWISE)
                {
                    _region.Reverse();
                }
            }
            return _region;
        }

        /// <summary>
        /// Reorders the edges around this site to form a proper polygon boundary.
        /// </summary>
        private void ReorderEdges()
        {
            EdgeReorderer reorderer = new EdgeReorderer(_edges, typeof(Vertex));
            _edges = reorderer.Edges;
            _edgeOrientations = reorderer.EdgeOrientations;
            reorderer.Dispose();
        }

        /// <summary>
        /// Clips the Voronoi region to the specified rectangular bounds.
        /// </summary>
        /// <param name="bounds">The bounds to clip the region to.</param>
        /// <returns>A list of points representing the clipped region.</returns>
        private List<PointF> ClipToBounds(RectangleF bounds)
        {
            List<PointF> PointFs = new List<PointF>();
            int n = _edges.Count;
            int i = 0;
            Edge edge;
            while (i < n && ((_edges[i] as Edge).Visible == false))
            {
                ++i;
            }

            if (i == n)
            {
                // no edges visible
                return new List<PointF>();
            }
            edge = _edges[i];
            LR orientation = _edgeOrientations[i];
            PointFs.Add(edge.ClippedEnds[orientation]);
            PointFs.Add(edge.ClippedEnds[LR.Other(orientation)]);

            for (int j = i + 1; j < n; ++j)
            {
                edge = _edges[j];
                if (edge.Visible == false)
                {
                    continue;
                }
                Connect(PointFs, j, bounds);
            }
            // close up the polygon by adding another corner PointF of the bounds if needed:
            Connect(PointFs, i, bounds, true);

            return PointFs;
        }

        /// <summary>
        /// Connects points in the region by adding boundary points as needed.
        /// </summary>
        /// <param name="PointFs">The list of points to connect.</param>
        /// <param name="j">The index of the edge to connect.</param>
        /// <param name="bounds">The bounds for clipping.</param>
        private void Connect(List<PointF> PointFs, int j, RectangleF bounds)
        {
            Connect(PointFs, j, bounds, false);
        }

        /// <summary>
        /// Connects points in the region by adding boundary points as needed, with option to close the polygon.
        /// </summary>
        /// <param name="PointFs">The list of points to connect.</param>
        /// <param name="j">The index of the edge to connect.</param>
        /// <param name="bounds">The bounds for clipping.</param>
        /// <param name="closingUp">Whether this connection is closing up the polygon.</param>
        private void Connect(List<PointF> PointFs, int j, RectangleF bounds, bool closingUp)
        {
            PointF rightPointF = PointFs[PointFs.Count - 1];
            Edge newEdge = _edges[j] as Edge;
            LR newOrientation = _edgeOrientations[j];
            // the PointF that  must be connected to rightPointF:
            PointF newPointF = newEdge.ClippedEnds[newOrientation];
            if (!CloseEnough(rightPointF, newPointF))
            {
                // The PointFs do not coincide, so they must have been clipped at the bounds;
                // see if they are on the same border of the bounds:
                if (rightPointF.X != newPointF.X
                && rightPointF.Y != newPointF.Y)
                {
                    // They are on different borders of the bounds;
                    // insert one or two corners of bounds as needed to hook them up:
                    // (NOTE this will not be corRectangleF if the region should take up more than
                    // half of the bounds RectangleF, for then we will have gone the wrong way
                    // around the bounds and included the smaller part rather than the larger)
                    int rightCheck = BoundsCheck.Check(rightPointF, bounds);
                    int newCheck = BoundsCheck.Check(newPointF, bounds);
                    float px, py;
                    if (rightCheck == BoundsCheck.RIGHT)
                    {
                        px = bounds.Right;
                        if (newCheck == BoundsCheck.BOTTOM)
                        {
                            py = bounds.Bottom;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.TOP)
                        {
                            py = bounds.Top;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.LEFT)
                        {
                            if (rightPointF.Y - bounds.Y + newPointF.Y - bounds.Y < bounds.Height)
                            {
                                py = bounds.Top;
                            }
                            else
                            {
                                py = bounds.Bottom;
                            }
                            PointFs.Add(new PointF(px, py));
                            PointFs.Add(new PointF(bounds.Left, py));
                        }
                    }
                    else if (rightCheck == BoundsCheck.LEFT)
                    {
                        px = bounds.Left;
                        if (newCheck == BoundsCheck.BOTTOM)
                        {
                            py = bounds.Bottom;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.TOP)
                        {
                            py = bounds.Top;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.RIGHT)
                        {
                            if (rightPointF.Y - bounds.Y + newPointF.Y - bounds.Y < bounds.Height)
                            {
                                py = bounds.Top;
                            }
                            else
                            {
                                py = bounds.Bottom;
                            }
                            PointFs.Add(new PointF(px, py));
                            PointFs.Add(new PointF(bounds.Right, py));
                        }
                    }
                    else if (rightCheck == BoundsCheck.TOP)
                    {
                        py = bounds.Top;
                        if (newCheck == BoundsCheck.RIGHT)
                        {
                            px = bounds.Right;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.LEFT)
                        {
                            px = bounds.Left;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.BOTTOM)
                        {
                            if (rightPointF.X - bounds.X + newPointF.X - bounds.X < bounds.Width)
                            {
                                px = bounds.Left;
                            }
                            else
                            {
                                px = bounds.Right;
                            }
                            PointFs.Add(new PointF(px, py));
                            PointFs.Add(new PointF(px, bounds.Bottom));
                        }
                    }
                    else if (rightCheck == BoundsCheck.BOTTOM)
                    {
                        py = bounds.Bottom;
                        if (newCheck == BoundsCheck.RIGHT)
                        {
                            px = bounds.Right;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.LEFT)
                        {
                            px = bounds.Left;
                            PointFs.Add(new PointF(px, py));
                        }
                        else if (newCheck == BoundsCheck.TOP)
                        {
                            if (rightPointF.X - bounds.X + newPointF.X - bounds.X < bounds.Width)
                            {
                                px = bounds.Left;
                            }
                            else
                            {
                                px = bounds.Right;
                            }
                            PointFs.Add(new PointF(px, py));
                            PointFs.Add(new PointF(px, bounds.Top));
                        }
                    }
                }
                if (closingUp)
                {
                    // newEdge's ends have already been added
                    return;
                }
                PointFs.Add(newPointF);
            }
            PointF newRightPointF = newEdge.ClippedEnds[LR.Other(newOrientation)];
            if (!CloseEnough(PointFs[0], newRightPointF))
            {
                PointFs.Add(newRightPointF);
            }
        }


        /// <summary>
        /// Gets the X coordinate of this site.
        /// </summary>
        internal float X
        {
            get
            {
                return _coord.X;
            }
        }

        /// <summary>
        /// Gets the Y coordinate of this site.
        /// </summary>
        internal float Y
        {
            get
            {
                return _coord.Y;
            }
        }

        /// <summary>
        /// Calculates the distance from this site to another coordinate.
        /// </summary>
        /// <param name="p">The coordinate to measure distance to.</param>
        /// <returns>The Euclidean distance to the specified coordinate.</returns>
        internal float Dist(ICoord p)
        {
            return Utilities.Distance(Coord(), p.Coord());
        }
    }

    /// <summary>
    /// Utility class for checking which boundary a point lies on within a rectangle.
    /// Provides constants and methods for boundary detection during region clipping.
    /// </summary>
    class BoundsCheck
    {
        public static readonly int TOP = 1;
        public static readonly int BOTTOM = 2;
        public static readonly int LEFT = 4;
        public static readonly int RIGHT = 8;

        /// <summary>
        /// Determines which boundary or boundaries of the rectangle the point lies on.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="bounds">The rectangle to check against.</param>
        /// <returns>A bitfield indicating which boundaries the point is on.</returns>
        public static int Check(PointF point, RectangleF bounds)
        {
            int value = 0;
            if (point.X == bounds.Left)
            {
                value |= LEFT;
            }
            if (point.X == bounds.Right)
            {
                value |= RIGHT;
            }
            if (point.Y == bounds.Top)
            {
                value |= TOP;
            }
            if (point.Y == bounds.Bottom)
            {
                value |= BOTTOM;
            }
            return value;
        }

        /// <summary>
        /// Initializes a new instance of the BoundsCheck utility class.
        /// </summary>
        public BoundsCheck()
        {
            throw new Exception("BoundsCheck is a static class");
        }
    }
}