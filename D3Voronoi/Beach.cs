using System;
using System.Collections.Generic;
using System.Linq;

namespace D3Voronoi
{
    public static class Beach
    {

        public static List<RedBlackTree> BeachPool = new List<RedBlackTree>();

        public static RedBlackTree CreateBeach(Point site)
        {
            RedBlackTree beach = null;
            if (BeachPool.Any())
            {
                beach = BeachPool.Last();
                BeachPool.Remove(beach);
            }
            if (beach == null)
                beach = new RedBlackTree();

            beach.Site = site;
            return beach;
        }

        public static void DetachBeach(ref RedBlackTree circles, ref RedBlackTree beaches, RedBlackTree beach)
        {
            Circle.DetachCircle(ref circles, beach);
            beaches.Remove(beach);
            Beach.BeachPool.Add(beach);
            beach.Reset();
        }

        public static void RemoveBeach(ref Dictionary<int, Cell> cells, ref List<Edge> edges, ref RedBlackTree beaches, ref RedBlackTree circles, RedBlackTree beach)
        {
            var circle = beach.Circle;
            var x = circle.X;
            var y = circle.CY;
            var vertex = new Point(x, y);
            var previous = beach.P;
            var next = beach.N;
            var disappearing = new List<RedBlackTree>() { beach };

            DetachBeach(ref circles, ref beaches, beach);

            var lArc = previous;
            while (lArc.Circle != null && Math.Abs(x - lArc.Circle.X) < Diagram.Epsilon && Math.Abs(y - lArc.Circle.CY) < Diagram.Epsilon)
            {
                previous = lArc.P;
                disappearing.Insert(0, lArc);
                DetachBeach(ref circles, ref beaches, lArc);
                lArc = previous;
            }

            disappearing.Insert(0, lArc);
            Circle.DetachCircle(ref circles, lArc);

            var rArc = next;
            while (rArc.Circle != null && Math.Abs(x - rArc.Circle.X) < Diagram.Epsilon && Math.Abs(y - rArc.Circle.CY) < Diagram.Epsilon)
            {
                next = rArc.N;
                disappearing.Add(rArc);
                DetachBeach(ref circles, ref beaches, rArc);
                rArc = next;
            }

            disappearing.Add(rArc);
            Circle.DetachCircle(ref circles, rArc);

            int nArcs = disappearing.Count;
            int iArc;

            for (iArc = 1; iArc < nArcs; ++iArc)
            {
                rArc = disappearing[iArc];
                lArc = disappearing[iArc - 1];
                rArc.Edge = Edge.SetEdgeEnd(rArc.Edge, lArc.Site, rArc.Site, vertex);
            }

            lArc = disappearing[0];
            rArc = disappearing[nArcs - 1];
            rArc.Edge = Edge.CreateEdge(ref cells, ref edges, lArc.Site, rArc.Site, null, vertex);

            Circle.AttachCircle(ref circles, lArc);
            Circle.AttachCircle(ref circles, rArc);
        }

        public static void AddBeach(ref Dictionary<int, Cell> cells, ref RedBlackTree circles, ref RedBlackTree beaches, ref List<Edge> edges, Point site)
        {
            var x = site.X;
            var directrix = site.Y;
            RedBlackTree lArc = null;
            RedBlackTree rArc = null;
            var dxl = 0d;
            var dxr = 0d;
            var node = beaches._;

            while (node != null)
            {
                dxl = LeftBreakPoint(node, directrix) - x;
                if (dxl > Diagram.Epsilon) node = node.L;
                else
                {
                    dxr = x - RightBreakPoint(node, directrix);
                    if (dxr > Diagram.Epsilon)
                    {
                        if (node.R == null)
                        {
                            lArc = node;
                            break;
                        }
                        node = node.R;
                    }
                    else
                    {
                        if (dxl > -Diagram.Epsilon)
                        {
                            lArc = node.P;
                            rArc = node;
                        }
                        else if (dxr > -Diagram.Epsilon)
                        {
                            lArc = node;
                            rArc = node.N;
                        }
                        else
                        {
                            lArc = rArc = node;
                        }
                        break;
                    }
                }
            }

            Cell.CreateCell(ref cells, site);
            var newArc = CreateBeach(site);
            beaches.Insert(lArc, newArc);

            if (lArc == null && rArc == null)
                return;

            if (lArc == rArc)
            {
                Circle.DetachCircle(ref circles, lArc);
                rArc = CreateBeach(lArc.Site);
                beaches.Insert(newArc, rArc);
                newArc.Edge = rArc.Edge = Edge.CreateEdge(ref cells, ref edges, lArc.Site, newArc.Site);
                Circle.AttachCircle(ref circles, lArc);
                Circle.AttachCircle(ref circles, rArc);
                return;
            }

            if (rArc == null)
            { // && lArc
                newArc.Edge = Edge.CreateEdge(ref cells, ref edges, lArc.Site, newArc.Site);
                return;
            }

            // else lArc !== rArc
            Circle.DetachCircle(ref circles, lArc);
            Circle.DetachCircle(ref circles, rArc);

            var lSite = lArc.Site;
            var ax = lSite.X;
            var ay = lSite.Y;
            var bx = site.X - ax;
            var by = site.Y - ay;
            var rSite = rArc.Site;
            var cx = rSite.X - ax;
            var cy = rSite.Y - ay;
            var d = 2 * (bx * cy - by * cx);
            var hb = bx * bx + by * by;
            var hc = cx * cx + cy * cy;
            var vertex = new Point((cy * hb - by * hc) / d + ax, (bx * hc - cx * hb) / d + ay);

            rArc.Edge = Edge.SetEdgeEnd(rArc.Edge, lSite, rSite, vertex);
            newArc.Edge = Edge.CreateEdge(ref cells, ref edges, lSite, site, null, vertex);
            rArc.Edge = Edge.CreateEdge(ref cells, ref edges, site, rSite, null, vertex);
            Circle.AttachCircle(ref circles, lArc);
            Circle.AttachCircle(ref circles, rArc);
        }

        public static double LeftBreakPoint(RedBlackTree arc, double directrix)
        {
            var site = arc.Site;
            var rfocx = site.X;
            var rfocy = site.Y;
            var pby2 = rfocy - directrix;

            if (pby2 == 0) return rfocx;

            var lArc = arc.P;
            if (lArc == null) return double.MinValue;

            site = lArc.Site;
            var lfocx = site.X;
            var lfocy = site.Y;
            var plby2 = lfocy - directrix;

            if (plby2 == 0) return lfocx;

            var hl = lfocx - rfocx;
            var aby2 = 1 / pby2 - 1 / plby2;
            var b = hl / plby2;

            if (aby2 != 0) return (-b + Math.Sqrt(b * b - 2 * aby2 * (hl * hl / (-2 * plby2) - lfocy + plby2 / 2 + rfocy - pby2 / 2))) / aby2 + rfocx;

            return (rfocx + lfocx) / 2;
        }

        private static double RightBreakPoint(RedBlackTree arc, double directrix)
        {
            var rArc = arc.N;
            if (rArc != null) return LeftBreakPoint(rArc, directrix);
            var site = arc.Site;
            return site.Y == directrix ? site.X : double.MaxValue;
        }
    }
}
