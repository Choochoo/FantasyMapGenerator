using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    public class visibleLineSegmentsClass
    {
        static internal List<LineSegment> VisibleLineSegments(List<Edge> edges)
        {
            List<LineSegment> segments = new List<LineSegment>();
            foreach (Edge edge in edges)
            {
                if (edge.Visible)
                {
                    PointF p1 = edge.ClippedEnds[LR.LEFT];
                    PointF p2 = edge.ClippedEnds[LR.RIGHT];
                    segments.Add(new LineSegment(p1, p2));
                }
            }
            return segments;
        }
    }
}