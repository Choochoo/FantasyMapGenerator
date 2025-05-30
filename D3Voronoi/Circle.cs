using System;
using System.Collections.Generic;
using System.Linq;

namespace D3Voronoi
{
    public static class Circle
    {
        public static List<RedBlackTree> CirclePool = new List<RedBlackTree>();

        public static RedBlackTree FirstCircle;
        public static void AttachCircle(ref RedBlackTree circles, RedBlackTree arc)
        {
            RedBlackTree lArc = arc.P;
            RedBlackTree rArc = arc.N;

            if (lArc == null || rArc == null) return;

            Point lSite = lArc.Site;
            Point cSite = arc.Site;
            Point rSite = rArc.Site;

            if (lSite == rSite) return;

            double bx = cSite.X;
            double by = cSite.Y;
            double ax = lSite.X - bx;
            double ay = lSite.Y - by;
            double cx = rSite.X - bx;
            double cy = rSite.Y - by;

            double d = 2 * (ax * cy - ay * cx);
            if (d >= -Diagram.Epsilon2) return;

            double ha = ax * ax + ay * ay;
            double hc = cx * cx + cy * cy;
            double x = (cy * ha - ay * hc) / d;
            double y = (ax * hc - cx * ha) / d;

            RedBlackTree circle = null;
            if (Circle.CirclePool.Any())
            {
                circle = Circle.CirclePool.Last();
                Circle.CirclePool.Remove(circle);
            }
            else
            {
                circle = new RedBlackTree();
            }

            circle.Arc = arc;
            circle.Site = cSite;
            circle.X = x + bx;
            circle.Y = (circle.CY = y + by) + Math.Sqrt(x * x + y * y); // y bottom

            arc.Circle = circle;

            RedBlackTree before = null;
            RedBlackTree node = circles._;

            while (node != null)
            {
                if (circle.Y < node.Y || (circle.Y == node.Y && circle.X <= node.X))
                {
                    if (node.L != null) node = node.L;
                    else { before = node.P; break; }
                }
                else
                {
                    if (node.R != null) node = node.R;
                    else { before = node; break; }
                }
            }

            circles.Insert(before, circle);
            if (before == null) FirstCircle = circle;
        }

        public static void DetachCircle(ref RedBlackTree circles, RedBlackTree arc)
        {
            RedBlackTree circle = arc.Circle;
            if (circle != null)
            {
                if (circle.P == null)
                    FirstCircle = circle.N;

                circles.Remove(circle);
                CirclePool.Add(circle);
                circle.Reset();
                arc.Circle = null;
            }
        }
    }
}
