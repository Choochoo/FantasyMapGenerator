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

        public Diagram(List<Point> sites, Extent extent)
        {
            //Lexicographic
            sites = sites.OrderByDescending(p => p.Y).ThenByDescending(p => p.X).ToList();

            var site = sites.Last();
            sites.Remove(site);
            double x = double.MaxValue;
            double y = double.MaxValue;
            RedBlackTree circle;

            Edges = new List<Edge>();
            Cells = new Dictionary<int, Cell>(sites.Count);
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
                    site = sites.LastOrDefault();
                    if (site != null)
                        sites.Remove(site);
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

        public void Polygons()
        {
            var edges = Edges;
            /*
            return Diagram.Cells.map(function(cell) {
                var polygon = cell.halfedges.map(function(i) { return cellHalfedgeStart(cell, edges[i]); });
                polygon.data = cell.site.data;
                return polygon;
            });
            */
        }

        public void Triangles()
        {
            //var triangles = [];
            var edges = Edges;

            for (var i = 0; i < Cells.Count; i++)
            {
                var cell = Cells[i];
                var site = cell.Site;
                var halfedges = cell.HalfEdges;
                var j = -1;
                var m = halfedges.Count;
                Point s0;
                var e1 = new Edge();//DON'T KNOW edges[halfedges[m - 1]];
                var s1 = e1.Left == site ? e1.Right : e1.Left;

                while (++j < m)
                {
                    s0 = s1;
                    e1 = new Edge();//DON'T KNOW edges[halfedges[j]];
                    s1 = e1.Left == site ? e1.Right : e1.Left;
                    if (i < s0.Index && i < s1.Index && TriangleArea(site, s0, s1) < 0)
                    {
                        //triangles.push([site.data, s0.data, s1.data]);
                    }
                }
            }

            return; //triangles;
        }

        public void Links()
        { /*
            return Diagram.Edges.filter(function(edge) {
                return edge.right;
            }).map(function(edge) {
                return {
                    source: edge.left.data,
        target: edge.right.data
                        };
            });
        }*/
        }
    }
}
