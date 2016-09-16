using System.Collections.Generic;
using System.Drawing;

namespace Delaunay
{
    public class selectEdgesForSitePointFClass
    {
        static PointF _coord;
        internal static List<Edge> SelectEdgesForSitePointF(PointF coord, List<Edge> edgesToTest)
        {
            _coord = coord;
            return edgesToTest.Filter(MyTest);
        }

        static bool MyTest(Edge edge, int index, List<Edge> vector)
        {
            return ((edge.LeftSite != null && edge.LeftSite.Coord() == _coord)
            || (edge.RightSite != null && edge.RightSite.Coord() == _coord));
        }
    }
}