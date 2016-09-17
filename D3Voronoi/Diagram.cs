using System.Collections.Generic;
using System.Linq;

namespace D3Voronoi
{
    public class Diagram
    {
        public const double Epsilon = 1e-6;
        public const double Epsilon2 = 1e-12;

        public RedBlackTree Beaches;
        public RedBlackTree Circles;
        public Dictionary<int, Cell> Cells;
        public List<Edge> Edges;

        public Diagram(List<Point> sitesList, Extent extent)
        {
            //Lexicographic
            sitesList = sitesList.OrderByDescending(p => p.Y).ThenByDescending(p => p.X).ToList();
            var sitesLinkedList = new LinkedList<Point>(sitesList.ToArray());
            var site = sitesLinkedList.Last();
            sitesLinkedList.Remove(site);
            double x = double.MaxValue;
            double y = double.MaxValue;
            RedBlackTree circle;

            Edges = new List<Edge>();
            Cells = new Dictionary<int, Cell>(sitesLinkedList.Count);
            Beaches = new RedBlackTree();
            Circles = new RedBlackTree();

            while (true)
            {
                circle = Circle.FirstCircle;
                if (site != null && (circle == null || site.Y < circle.Y || (site.Y == circle.Y && site.X < circle.X)))
                {
                    if (site.X != x || site.Y != y)
                    {
                        Beach.AddBeach(ref Cells, ref Circles, ref Beaches, ref Edges, site);
                        x = site.X;
                        y = site.Y;
                    }
                    site = sitesLinkedList.LastOrDefault();
                    if (site != null)
                        sitesLinkedList.Remove(site);
                }
                else if (circle != null)
                {
                    Beach.RemoveBeach(ref Cells, ref Edges, ref Beaches, ref Circles, circle.Arc);
                }
                else
                {
                    break;
                }
            }

            Cell.SortCellHalfedges(ref Cells, Edges);

            if (extent != null)
            {
                var x0 = +extent.X;
                var y0 = +extent.Y;
                var x1 = +extent.Width;
                var y1 = +extent.Height;
                Edge.ClipEdges(ref Edges, x0, y0, x1, y1);
                Cell.ClipCells(ref Cells, ref Edges, x0, y0, x1, y1);
            }


        }

        public double TriangleArea(Point a, Point b, Point c)
        {
            return (a.X - c.X) * (b.Y - a.Y) - (a.X - b.X) * (c.Y - a.Y);
        }

        public List<Polygon> Polygons()
        {
            var edges = Edges;
            return Cells.Select(cell => new Polygon()
            {
                Points = cell.Value.HalfEdges.Select((i, h) => Cell.CellHalfedgeStart(cell.Value, edges[i])).ToList(),
                Data = cell.Value.Site.Data
            }).ToList();
        }

        public List<Point[]> Triangles()
        {
            List<Point[]> triangles = new List<Point[]>();
            var edges = Edges;

            for (var i = 0; i < Cells.Count; i++)
            {
                var cell = Cells[i];
                var site = cell.Site;
                var halfedges = cell.HalfEdges;
                var j = -1;
                var m = halfedges.Count;
                Point s0;
                var e1 = edges[halfedges[m - 1]];
                var s1 = e1.Left == site ? e1.Right : e1.Left;

                while (++j < m)
                {
                    s0 = s1;
                    e1 = edges[halfedges[j]];
                    s1 = e1.Left == site ? e1.Right : e1.Left;
                    if (i < s0.Index && i < s1.Index && TriangleArea(site, s0, s1) < 0)
                    {
                        triangles.Add(new Point[] {
                            site.Data,
                            s0.Data,
                            s1.Data
                        });
                    }
                }
            }

            return triangles;
        }

        public List<Link> Links()
        {
            return Edges.Where(e => e.Right != null)
                .Select(e => new Link()
                {
                    Source = e.Left.Data,
                    Target = e.Right.Data
                }).ToList();
        }
    }
}
