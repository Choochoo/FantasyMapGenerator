using System.Collections.Generic;

namespace D3Voronoi
{
    /// <summary>
    /// Represents a polygon formed by a collection of connected points in a Voronoi diagram.
    /// Used to define the boundaries of Voronoi cells and other geometric shapes.
    /// </summary>
    public class Polygon
    {
        /// <summary>
        /// List of points that define the vertices of the polygon in order.
        /// </summary>
        public List<Point> Points;
        
        /// <summary>
        /// Associated data point that this polygon represents or is centered around.
        /// </summary>
        public Point Data;
    }
}
