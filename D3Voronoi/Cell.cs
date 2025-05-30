using System;
using System.Collections.Generic;
using System.Linq;

namespace D3Voronoi
{
    public class Cell
    {
        public Point Site;
        public List<int> HalfEdges;
        public static Cell CreateCell(ref Dictionary<int, Cell> cells, Point site)
        {
            Cell cell = new Cell()
            {
                Site = site,
                HalfEdges = new List<int>()
            };
            cells[site.Index] = cell;

            return cell;
        }

        private static double CellHalfEdgeAngle(Cell cell, Edge edge)
        {
            Point site = cell.Site;
            Point va = edge.Left;
            Point vb = edge.Right;

            if (site == vb)
            {
                vb = va;
                va = site;
            }

            if (vb != null)
                return Math.Atan2(vb.Y - va.Y, vb.X - va.X);

            if (site == va)
            {
                va = edge.Points[1];
                vb = edge.Points[0];
            }
            else
            {
                va = edge.Points[0];
                vb = edge.Points[1];
            }

            return Math.Atan2(va.X - vb.X, vb.Y - va.Y);
        }

        public static Point CellHalfedgeStart(Cell cell, Edge edge)
        {
            return edge.Points[edge.Left != cell.Site ? 1 : 0];
        }

        public static Point CellHalfEdgeEnd(Cell cell, Edge edge)
        {
            return edge.Points[edge.Left == cell.Site ? 1 : 0];
        }

        public static void SortCellHalfedges(ref Dictionary<int, Cell> cells, List<Edge> edges)
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                Cell cell = cells[i];
                List<int> halfedges = cell.HalfEdges;
                int j = 0;
                int m = halfedges.Count;

                if (cell != null && m > 0)
                {
                    int[] index = new int[m];
                    double[] array = new double[m];
                    for (j = 0; j < m; ++j)
                    {
                        index[j] = j;
                        array[j] = CellHalfEdgeAngle(cell, edges[halfedges[j]]);
                    }
                    Array.Sort(array, index);
                    Array.Reverse(index);
                    int[] arrayD = new int[m];
                    for (j = 0; j < m; ++j)
                    {
                        arrayD[j] = halfedges[index[j]];
                    }
                    for (j = 0; j < m; ++j)
                    {
                        halfedges[j] = arrayD[j];
                    }
                }
                cell.HalfEdges = halfedges;
            }
        }

        public static void ClipCells(ref Dictionary<int, Cell> cells, ref List<Edge> edges, double x0, double y0, double x1, double y1)
        {
            int nCells = cells.Count;
            int iCell;
            Cell cell;
            Point site;
            int iHalfedge;
            List<int> halfedges;
            int nHalfedges;
            Point start;
            double startX;
            double startY;
            Point end;
            double endX;
            double endY;
            bool cover = true;

            for (iCell = 0; iCell < nCells; ++iCell)
            {
                cell = cells[iCell];
                if (cell != null)
                {
                    site = cell.Site;
                    halfedges = cell.HalfEdges;
                    iHalfedge = halfedges.Count;

                    // Remove any dangling clipped edges.
                    while (iHalfedge-- > 0)
                    {
                        if (edges[halfedges[iHalfedge]] == null)
                        {
                            halfedges.RemoveAt(iHalfedge);
                        }
                    }

                    // Insert any border edges as necessary.
                    iHalfedge = 0;
                    nHalfedges = halfedges.Count;
                    while (iHalfedge < nHalfedges)
                    {
                        end = CellHalfEdgeEnd(cell, edges[halfedges[iHalfedge]]);
                        endX = end.X;
                        endY = end.Y;
                        start = CellHalfedgeStart(cell, edges[halfedges[++iHalfedge % nHalfedges]]);
                        startX = start.X;
                        startY = start.Y;
                        if (Math.Abs(endX - startX) > Diagram.Epsilon || Math.Abs(endY - startY) > Diagram.Epsilon)
                        {
                            Point point = Math.Abs(endX - x0) < Diagram.Epsilon && y1 - endY > Diagram.Epsilon ? new Point(x0, Math.Abs(startX - x0) < Diagram.Epsilon ? startY : y1) :
                            Math.Abs(endY - y1) < Diagram.Epsilon && x1 - endX > Diagram.Epsilon ? new Point(Math.Abs(startY - y1) < Diagram.Epsilon ? startX : x1, y1) :
                            Math.Abs(endX - x1) < Diagram.Epsilon && endY - y0 > Diagram.Epsilon ? new Point(x1, Math.Abs(startX - x1) < Diagram.Epsilon ? startY : y0) :
                            Math.Abs(endY - y0) < Diagram.Epsilon && endX - x0 > Diagram.Epsilon ? new Point(Math.Abs(startY - y0) < Diagram.Epsilon ? startX : x0, y0) :
                            null;

                            Edge newEdge = Edge.CreateBorderEdge(site, end, point);
                            edges.Add(newEdge);
                            halfedges.Insert(iHalfedge, edges.Count - 1);
                            ++nHalfedges;
                        }
                    }

                    if (nHalfedges > 0)
                        cover = false;
                }
            }

            // If there weren't any edges, have the closest site cover the extent.
            // It doesn't matter which corner of the extent we measure!
            if (cover)
            {
                double dx;
                double dy;
                double d2;
                double dc = double.MaxValue;
                Cell coverCell = null;
                for (iCell = 0; iCell < nCells; ++iCell)
                {
                    cell = cells[iCell];
                    if (cell != null)
                    {
                        site = cell.Site;
                        dx = site.X - x0;
                        dy = site.Y - y0;
                        d2 = dx * dx + dy * dy;
                        if (d2 < dc)
                        {
                            dc = d2;
                            coverCell = cell;
                        }
                    }
                }

                if (coverCell != null)
                {
                    Point v00 = new Point(x0, y0);
                    Point v01 = new Point(x0, y1);
                    Point v11 = new Point(x1, y1);
                    Point v10 = new Point(x1, y0);


                    edges.Add(Edge.CreateBorderEdge(site = coverCell.Site, v00, v01));
                    coverCell.HalfEdges.Add(edges.Count - 1);
                    edges.Add(Edge.CreateBorderEdge(site, v01, v11));
                    coverCell.HalfEdges.Add(edges.Count - 1);
                    edges.Add(Edge.CreateBorderEdge(site, v11, v10));
                    coverCell.HalfEdges.Add(edges.Count - 1);
                    edges.Add(Edge.CreateBorderEdge(site, v10, v00));
                    coverCell.HalfEdges.Add(edges.Count - 1);

                }
            }

            // Lastly delete any cells with no edges; these were entirely clipped.
            for (iCell = 0; iCell < nCells; ++iCell)
            {
                cell = cells[iCell];
                if (cell != null)
                {
                    if (!cell.HalfEdges.Any())
                    {
                        cells[iCell] = null;
                    }
                }
            }
        }
    }
}
