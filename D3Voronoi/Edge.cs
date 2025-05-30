using System;
using System.Collections.Generic;

namespace D3Voronoi
{
    public class Edge
    {
        public Point[] Points = new Point[2];
        public Point Left = null;
        public Point Right = null;
        public Edge() { }
        public static Edge CreateEdge(ref Dictionary<int, Cell> cells, ref List<Edge> edges, Point left, Point right, Point v0 = null, Point v1 = null)
        {
            Edge edge = new Edge()
            {
                Points = new Point[2]
            };

            edges.Add(edge);
            int index = edges.Count - 1;
            edge.Left = left;
            edge.Right = right;
            if (v0 != null)
                edge = SetEdgeEnd(edge, left, right, v0);
            if (v1 != null)
                edge = SetEdgeEnd(edge, right, left, v1);
            cells[left.Index].HalfEdges.Add(index);
            cells[right.Index].HalfEdges.Add(index);
            return edge;
        }

        public static Edge CreateBorderEdge(Point left, Point v0, Point v1)
        {
            Edge edge = new Edge()
            {
                Points = new Point[] { v0, v1 }
            };
            edge.Left = left;
            return edge;
        }

        public static Edge SetEdgeEnd(Edge edge, Point left, Point right, Point vertex)
        {
            if (edge.Points[0] == null && edge.Points[1] == null)
            {
                edge.Points[0] = vertex;
                edge.Left = left;
                edge.Right = right;
            }
            else if (edge.Left == right)
            {
                edge.Points[1] = vertex;
            }
            else
            {
                edge.Points[0] = vertex;
            }
            return edge;
        }

        // Liang–Barsky line clipping.
        public static bool ClipEdge(Edge edge, double x0, double y0, double x1, double y1)
        {
            Point a = edge.Points[0];
            Point b = edge.Points[1];
            double ax = a.X;
            double ay = a.Y;
            double bx = b.X;
            double by = b.Y;
            double t0 = 0d;
            double t1 = 1d;
            double? dx = bx - ax;
            double dy = by - ay;
            double r;
            //DX IS MESSED UP HERE
            r = x0 - ax;
            if (dx == null && r > 0) return false;
            r /= dx.Value;
            if (dx < 0)
            {
                if (r < t0) return false;
                if (r < t1) t1 = r;
            }
            else if (dx > 0)
            {
                if (r > t1) return false;
                if (r > t0) t0 = r;
            }

            r = x1 - ax;
            if (dx == null && r < 0) return false;
            r /= dx.Value;
            if (dx < 0)
            {
                if (r > t1) return false;
                if (r > t0) t0 = r;
            }
            else if (dx > 0)
            {
                if (r < t0) return false;
                if (r < t1) t1 = r;
            }

            r = y0 - ay;
            if (dy == null && r > 0) return false;
            r /= dy;
            if (dy < 0)
            {
                if (r < t0) return false;
                if (r < t1) t1 = r;
            }
            else if (dy > 0)
            {
                if (r > t1) return false;
                if (r > t0) t0 = r;
            }

            r = y1 - ay;
            if (dy == null && r < 0) return false;
            r /= dy;
            if (dy < 0)
            {
                if (r > t1) return false;
                if (r > t0) t0 = r;
            }
            else if (dy > 0)
            {
                if (r < t0) return false;
                if (r < t1) t1 = r;
            }

            if (!(t0 > 0) && !(t1 < 1)) return true; // TODO Better check?

            if (t0 > 0) edge.Points[0] = new Point(ax + t0 * dx.Value, ay + t0 * dy);
            if (t1 < 1) edge.Points[1] = new Point(ax + t1 * dx.Value, ay + t1 * dy);
            return true;
        }

        public static bool ConnectEdge(Edge edge, double x0, double y0, double x1, double y1)
        {
            Point v1 = edge.Points[1];
            if (v1 != null) return true;

            Point v0 = edge.Points[0];
            Point left = edge.Left;
            Point right = edge.Right;
            double lx = left.X;
            double ly = left.Y;
            double rx = right.X;
            double ry = right.Y;
            double fx = (lx + rx) / 2;
            double fy = (ly + ry) / 2;
            double fm;
            double fb;

            if (ry == ly)
            {
                if (fx < x0 || fx >= x1) return false;
                if (lx > rx)
                {
                    if (v0 == null) v0 = new Point(fx, y0);
                    else if (v0.Y >= y1) return false;
                    v1 = new Point(fx, y1);
                }
                else
                {
                    if (v0 == null) v0 = new Point(fx, y1);
                    else if (v0.Y < y0) return false;
                    v1 = new Point(fx, y0);
                }
            }
            else
            {
                fm = (lx - rx) / (ry - ly);
                fb = fy - fm * fx;
                if (fm < -1 || fm > 1)
                {
                    if (lx > rx)
                    {
                        if (v0 == null) v0 = new Point((y0 - fb) / fm, y0);
                        else if (v0.Y >= y1) return false;
                        v1 = new Point((y1 - fb) / fm, y1);
                    }
                    else
                    {
                        if (v0 == null) v0 = new Point((y1 - fb) / fm, y1);
                        else if (v0.Y < y0) return false;
                        v1 = new Point((y0 - fb) / fm, y0);
                    }
                }
                else
                {
                    if (ly < ry)
                    {
                        if (v0 == null) v0 = new Point(x0, fm * x0 + fb);
                        else if (v0.X >= x1) return false;
                        v1 = new Point(x1, fm * x1 + fb);
                    }
                    else
                    {
                        if (v0 == null) v0 = new Point(x1, fm * x1 + fb);
                        else if (v0.X < x0) return false;
                        v1 = new Point(x0, fm * x0 + fb);
                    }
                }
            }

            edge.Points[0] = v0;
            edge.Points[1] = v1;
            return true;
        }

        public static void ClipEdges(ref List<Edge> edges, double x0, double y0, double x1, double y1)
        {
            int i = edges.Count;
            Edge edge;

            while (i-- > 0)
            {
                edge = edges[i];
                if (!ConnectEdge(edge, x0, y0, x1, y1) || !ClipEdge(edge, x0, y0, x1, y1) || !(Math.Abs(edge.Points[0].X - edge.Points[1].X) > Diagram.Epsilon || Math.Abs(edge.Points[0].Y - edge.Points[1].Y) > Diagram.Epsilon))
                {
                    edges[i] = null;
                }
            }
        }
    }
}
