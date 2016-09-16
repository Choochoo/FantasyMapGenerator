using System.Collections.Generic;

namespace Delaunay
{
    public static class DelaunayLinesForEdgesClass
    {
        internal static List<LineSegment> DelaunayLinesForEdges(List<Edge> edges)
        {
            List<LineSegment> segments = new List<LineSegment>();
            foreach (Edge edge in edges)
            {
                segments.Add(edge.DelaunayLine());
            }
            return segments;
        }
    }
}