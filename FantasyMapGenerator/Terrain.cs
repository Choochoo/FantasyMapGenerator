using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using D3Voronoi;
using MathNet.Numerics.Statistics;
using WorldMap.Layers.Interfaces;

namespace WorldMap
{
    public class CityProperty
    {
        public double Score;
        public int City;
        public int Vx;
    }

    public class AreaProperties
    {
        public int NumberOfPoints = 16384;
        public int NumberOfCities = 15;
        public int NumberOfTerritories = 5;
        public int FontSizeRegion = 40;
        public int FontSizeCity = 25;
        public int FontSizeTown = 20;
    }

    public class CityRender
    {
        private AreaProperties _areaProperties = null;
        public AreaProperties AreaProperties
        {
            get
            {
                if (_areaProperties == null)
                {
                    _areaProperties = new AreaProperties();
                }
                return _areaProperties;
            }
            set { _areaProperties = value; }
        }
        public double[] Score;
        public double[] Territories;
        public List<int> Cities = new List<int>();

        public List<List<Point>> Rivers;
        public List<List<Point>> Coasts;
        public List<List<Point>> Borders;
    }

    public class Mesh
    {
        public List<Point> Pts;
        public List<Point> Vxs;
        public Dictionary<string, int> Vxids;
        public Dictionary<int, List<int>> Adj;
        public List<MapEdge> Edges;
        public Dictionary<int, List<Point>> Tris;
        public Extent Extent;
    }

    public class Polygon
    {
        public List<Point> Points = new List<Point>();
    }

    public class MapEdge
    {
        public int Spot1;
        public int Spot2;
        public Point Left;
        public Point Right;
    }


    public class Terrain
    {
        private static Random random = new Random(200);

        public static double Runif(double lo, double hi)
        {
            return lo + random.NextDouble() * (hi - lo);
        }

        public static void RNorm(out double x1, out double x2)
        {
            x1 = 0;
            x2 = 0;
            var w = 2.0;
            while (w >= 1)
            {
                x1 = Runif(-1f, 1f);
                x2 = Runif(-1f, 1f);
                w = x1 * x1 + x2 * x2;
            }
            w = Math.Sqrt(-2f * Math.Log(w) / w);
            x2 *= w;
            x1 *= w;
        }

        public static List<Point> GeneratePoints(int n, Extent extent)
        {
            var pts = new List<Point>();
            for (var i = 0; i < n; i++)
            {
                var x = (random.NextDouble() - 0.5) * extent.Width;
                var y = (random.NextDouble() - 0.5) * extent.Height;
                pts.Add(new Point(x, y));
            }
            return pts;
        }

        public static Mesh GenerateGoodMesh(IHasVoronoi instance, int n, Extent extent)
        {
            var pts = GenerateGoodPoints(instance, n, extent);
            return MakeMesh(pts, extent);
        }

        private static List<Point> GenerateGoodPoints(IHasVoronoi instance, int n, Extent extent)
        {
            var pts = GeneratePoints(n, extent);
            pts = pts.OrderBy(point => point.X).ToList();
            return pts;
            //return ImprovePoints(instance, pts, extent, 1);
        }

        public static double[] GenerateCoast<T>(T instance, int npts, Extent extent) where T : IHasDownhill, IHasMesh, IHasVoronoi
        {
            instance.Mesh = Terrain.GenerateGoodMesh(instance, npts, extent);
            var h = Terrain.Add(instance.Mesh.Vxs.Count,
                Terrain.Slope(instance.Mesh, Terrain.RandomVector(4)),
                Terrain.Cone(instance.Mesh, Terrain.Runif(-1, -1)),
                Terrain.Mountains(instance.Mesh, 50)
                );
            for (var i = 0; i < 10; i++)
            {
                h = Terrain.Relax(instance.Mesh, h);
            }
            h = Terrain.Peaky(h);
            h = Terrain.DoErosion(instance.Mesh, instance, h, Terrain.Runif(0, 0.1d), 5);
            h = Terrain.SetSeaLevel(h, Terrain.Runif(0.2d, 0.6d));
            h = Terrain.FillSinks(instance.Mesh, h);
            h = Terrain.CleanCoast(instance.Mesh, h, 3);
            return h;
        }
        /*

        private static Point Centroid(Point site, int siteIndex, VoronoiGraph graph)
        {
            var x = 0d;
            var y = 0d;
            var edge = graph.Edges.ElementAt(siteIndex);
            var startEdge = edge.ID;
            var count = 0;
            while (startEdge != edge.ID || count == 0)
            {
                if (edge.Vertex == -1 || graph.Edges[edge.Opposite].Vertex == -1)
                    continue;

                var vert1 = graph.Vertices[edge.Vertex].Position;
                //var vert2 = graph.Vertices[graph.Edges[edge.Opposite].Vertex].Position;
                x += vert1.X;
                y += vert1.Y;
                edge = graph.Edges.ElementAt(edge.Next);
                count++;
            }
            var newSite = new Point(x / count, y / count);
            return newSite;
        }


        public static List<Point> ImprovePoints(IHasVoronoi instance, List<Point> sites, Extent extent, int n = 1)
        {
            var w = extent.width / 2d;
            var h = extent.height / 2d;
            var rect = new Extent(-w, -h, w, h);
            sites.ForEach(s => Debug.Write(s + ","));
            for (var i = 0; i < n; i++)
            {
                var newSites = new List<Point>();
                // Compute the voronoi graph
                instance.Graph = instance.Voronoi.CreateVoronoi(sites.ToArray());
                //cap infinite edges.
                instance.Graph.Complete(20000f);
                for (int j = 0; j < sites.Count; j++)
                {
                    var newSite = Terrain.Centroid(sites[j], j, instance.Graph);
                    newSites.Add(newSite);
                }
                sites = newSites;
                
            }
            Debug.WriteLine("space");
            sites.ForEach(s => Debug.Write(s + ","));
            return sites;
        }


        /*
        private static List<Polygon> GetPolygons(List<GraphEdge> edges)
        {
            var site1 = edges.Select(e => e.site1);
            var site2 = edges.Select(e => e.site2);
            var allSites = new List<int>(site1);
            allSites.AddRange(site2);
            allSites = allSites.Distinct().ToList();

            var polgyons = new List<Polygon>();
            foreach (var site in allSites)
            {
                var currentEdges = edges.Where(e => e.site1 == site || e.site2 == site).ToList();
                var vectors = currentEdges.Select(e => new Point(e.Position.X1, e.Position.Y1)).ToList();
                vectors.AddRange(currentEdges.Select(e => new Point(e.Position.X2, e.Position.Y2)).ToList());
                polgyons.Add(new Polygon()
                {
                    Points = vectors.Distinct().ToList()
                });
            }

            return polgyons;
        }
        
        private static List<Vertex> VoronoiRange(IHasVoronoi instance, List<Point> pts, Extent extent)
        {
            var w = extent.width / 2d;
            var h = extent.height / 2d;
            instance.Voronoi.Construct(pts, new Extent(-w, -h, w, h));
            return instance.Voronoi.Vertices;
        }
        
        public static List<GraphEdge> ParseEdges(string edges)
        {
            var delimiter = "{";
            var delimiter2 = "}";

            var outputEdges = new List<GraphEdge>();
            var arrays =
                edges.Split(new[] { delimiter, delimiter2 }, StringSplitOptions.None).Where(a => a.Length > 10).ToList();

            var delimiter44 = "[";
            var delimiter55 = "]";
            foreach (var array in arrays)
            {
                var anotherSplit = array.Split(new[] { delimiter44, delimiter55 }, StringSplitOptions.None);
                var zero = anotherSplit[1];
                var one = anotherSplit[3];
                var left = anotherSplit[5];
                var newEdge = new GraphEdge()
                {
                    Site1 = new Point(Convert.ToDouble(zero.Split(',')[0]), Convert.ToDouble(zero.Split(',')[1])),
                    Site2 = new Point(Convert.ToDouble(one.Split(',')[0]), Convert.ToDouble(one.Split(',')[1])),
                    Left = new Point(Convert.ToDouble(left.Split(',')[0]), Convert.ToDouble(left.Split(',')[1]))
                };
                if (anotherSplit.Length > 7)
                {
                    var right = anotherSplit[7];
                    newEdge.Right = new Point(Convert.ToDouble(right.Split(',')[0]),
                        Convert.ToDouble(right.Split(',')[1]));
                }
                outputEdges.Add(newEdge);
            }
            return outputEdges;
        }
        */

        public static Diagram Voronoi(Voronoi vorInstance, List<Point> pts, Extent extent)
        {
            extent = extent == null ? Extent.DefaultExtent : extent;
            var w = extent.Width / 2d;
            var h = extent.Height / 2d;
            return vorInstance.VoronoiDiagram(pts.Select(p => new Point(p.X, p.Y)).ToList(), new Extent(-w, -h, w, h));
        }

        public static Mesh MakeMesh(List<Point> pts, Extent extent)
        {
            var vorInstance = new Voronoi();
            var vor = Voronoi(vorInstance, pts, extent);
            var vxs = new List<Point>();
            var vxids = new Dictionary<string, int>();
            var adj = new Dictionary<int, List<int>>();
            var edges = new List<MapEdge>();

            var tris = new Dictionary<int, List<Point>>();
            var counter = 0;
            foreach (var e in vor.Edges)
            {
                if (e == null)
                    continue;

                int e0 = 0;
                var foundE0 = vxids.TryGetValue(e.Points[0].ToString(), out e0);
                if (!foundE0)
                {
                    e0 = vxs.Count;
                    vxids.Add(e.Points[0].ToString(), e0);
                    vxs.Add(e.Points[0]);
                }
                List<Point> firstTri = null;
                bool foundFirst = tris.TryGetValue(e0, out firstTri);
                if (!foundFirst)
                {
                    firstTri = new List<Point>();
                    tris.Add(e0, firstTri);
                }

                int e1 = 0;
                var foundE1 = vxids.TryGetValue(e.Points[1].ToString(), out e1);
                if (!foundE1)
                {
                    e1 = vxs.Count;
                    vxids.Add(e.Points[1].ToString(), e1);
                    vxs.Add(e.Points[1]);
                }

                List<int> adj0 = null;
                var foundadj0 = adj.TryGetValue(e0, out adj0);
                if (!foundadj0)
                {
                    adj0 = new List<int>();
                    adj.Add(e0, adj0);
                }
                adj0.Add(e1);
                adj[e0] = adj0;

                List<int> adj1 = null;
                var foundadj1 = adj.TryGetValue(e1, out adj1);
                if (!foundadj1)
                {
                    adj1 = new List<int>();
                    adj.Add(e1, adj1);
                }
                adj1.Add(e0);
                adj[e1] = adj1;


                if (!firstTri.Any(t => t.X == e.Left.X && t.Y == e.Left.Y))
                {
                    firstTri.Add(e.Left);
                    tris[e0] = firstTri;
                }
                if (e.Right != null && !firstTri.Any(t => t.X == e.Right.X && t.Y == e.Right.Y))
                {
                    firstTri.Add(e.Right);
                    tris[e0] = firstTri;
                }

                List<Point> secondTri = null;
                var foundSecond = tris.TryGetValue(e1, out secondTri);
                if (!foundSecond)
                {
                    secondTri = new List<Point>();
                    tris.Add(e1, secondTri);
                }
                if (!secondTri.Any(t => t.X == e.Left.X && t.Y == e.Left.Y))
                {
                    secondTri.Add(e.Left);
                    tris[e1] = secondTri;
                }
                if (e.Right != null && !secondTri.Any(t => t.X == e.Right.X && t.Y == e.Right.Y))
                {
                    secondTri.Add(e.Right);
                    tris[e1] = secondTri;
                }
                edges.Add(new MapEdge()
                {
                    Spot1 = e0,
                    Spot2 = e1,
                    Left = e.Left,
                    Right = e.Right
                });
                counter++;
            }
            var mesh = new Mesh()
            {
                Pts = pts,
                Vxs = vxs,
                Adj = adj,
                Tris = tris,
                Edges = edges,
                Extent = extent
            };


            return mesh;
        }

        public static double[] Add(int count, params double[][] values)
        {
            var newvals = new double[count];
            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j < values.Length; j++)
                {
                    newvals[i] += values[j].ElementAt(i);
                }
            }
            return newvals;
        }

        public static double[] Mountains(Mesh mesh, int n, double r = 0.05f)
        {
            var mounts = new List<Point>();
            var randOne = .3f; //Math.random();
            var randTwo = .5f; //Math.random();
            for (var i = 0; i < n; i++)
            {
                mounts.Add(new Point(mesh.Extent.Width * (randOne - 0.5f), mesh.Extent.Height * (randTwo - 0.5f)));
            }
            var newvals = new double[mesh.Vxs.Count];
            for (var i = 0; i < mesh.Vxs.Count; i++)
            {
                var p = mesh.Vxs[i];
                for (var j = 0; j < n; j++)
                {
                    var m = mounts[j];
                    newvals[i] +=
                        (double)
                            Math.Pow(Math.Exp((double)(-((p.X - m.X) * (p.X - m.X) + (p.Y - m.Y) * (p.Y - m.Y)) / (2 * r * r))),
                                2);
                }
            }
            return newvals;
        }

        public static double[] Slope(Mesh mesh, Point direction)
        {
            var output = new List<double>();
            foreach (var f in mesh.Vxs)
            {
                output.Add(f.X * direction.X + f.Y * direction.Y);
            }
            return output.ToArray();
        }

        public static double[] Cone(Mesh mesh, double slope)
        {
            var output = new List<double>();
            foreach (var f in mesh.Vxs)
            {
                output.Add((double)Math.Pow((double)(f.X * f.X + f.Y * f.Y), 0.5) * slope);
            }
            return output.ToArray();
        }

        public static double[] Normalize(double[] heights)
        {
            var lo = heights.Min();
            var hi = heights.Max();
            var output = new List<double>();
            foreach (var f in heights)
            {
                output.Add((f - lo) / (hi - lo));
            }
            return output.ToArray();
        }

        public static double[] Peaky(double[] heights)
        {
            var output = new List<double>();
            var normalized = Normalize(heights);
            for (int i = 0; i < normalized.Length; i++)
            {
                var f = normalized[i];
                output.Add((double)Math.Sqrt((double)f));
            }
            return output.ToArray();
        }

        public static double[] Relax(Mesh mesh, double[] heights)
        {
            var output = new double[heights.Length];
            for (var i = 0; i < heights.Length; i++)
            {
                var nbs = Neighbours(mesh, i);
                if (nbs.Count < 3)
                {
                    output[i] = 0;
                    continue;
                }
                var avg = new List<double>();
                foreach (var nb in nbs)
                    avg.Add(heights[nb]);

                output[i] = avg.Average();
            }
            return output.ToArray();
        }

        public static List<int> Neighbours(Mesh mesh, int index)
        {
            var onbs = mesh.Adj[index];
            var nbs = new List<int>();
            for (var i = 0; i < onbs.Count; i++)
            {
                nbs.Add(onbs[i]);
            }
            return nbs;
        }

        public static Point RandomVector(int scale)
        {
            var xOut = 0.03661885595999543f;
            var yOut = -0.9259299039132616f;
            //RNorm(out xOut, out yOut);
            Debug.WriteLine($"x:{xOut} y:{yOut}");
            return new Point(scale * xOut, scale * yOut);
        }

        public static double[] Randomdoubles(int scale)
        {
            var vector = RandomVector(scale);
            return new[] { (double)vector.X, (double)vector.Y };
        }


        public static List<List<Point>> Contour<T>(T instance, int level = 0) where T : IHasMesh, IHasHeights
        {
            var edges = new List<Point[]>();
            for (var i = 0; i < instance.Mesh.Edges.Count; i++)
            {
                var e = instance.Mesh.Edges[i];
                if (e.Right == null) continue;
                if (IsNearEdge(instance.Mesh, e.Spot1) || IsNearEdge(instance.Mesh, e.Spot2)) continue;
                if ((instance.Heights[e.Spot1] > level && instance.Heights[e.Spot2] <= level) || (instance.Heights[e.Spot2] > level && instance.Heights[e.Spot1] <= level))
                {
                    edges.Add(new[] { e.Left, e.Right });
                }
            }
            return MergeSegments(edges);
        }

        public static bool IsNearEdge(Mesh mesh, int i)
        {
            var x = mesh.Vxs[i].X;
            var y = mesh.Vxs[i].Y;
            var w = mesh.Extent.Width;
            var h = mesh.Extent.Height;
            return x < -0.45f * w || x > 0.45f * w || y < -0.45f * h || y > 0.45f * h;
        }

        public static List<List<Point>> MergeSegments(List<Point[]> segs)
        {
            var adj = new Dictionary<Point, List<Point>>();
            for (var i = 0; i < segs.Count; i++)
            {
                var seg = segs[i];
                List<Point> a0 = null;
                var foundA0 = adj.TryGetValue(seg[0], out a0);
                if (!foundA0)
                {
                    a0 = new List<Point>();
                    adj[seg[0]] = a0;
                }

                List<Point> a1 = null;
                var foundA1 = adj.TryGetValue(seg[0], out a1);
                if (!foundA1)
                {
                    a1 = new List<Point>();
                    adj[seg[1]] = a1;
                }
                a0.Add(seg[1]);
                a1.Add(seg[0]);
                adj[seg[0]] = a0;
                adj[seg[1]] = a1;
            }
            var done = new bool[segs.Count];
            var paths = new List<List<Point>>();
            List<Point> path = null;
            while (true)
            {
                if (path == null)
                {
                    for (var i = 0; i < segs.Count; i++)
                    {
                        if (done[i]) continue;
                        done[i] = true;
                        path = new List<Point> { segs[i].ElementAt(0), segs[i].ElementAt(1) };
                        break;
                    }
                    if (path == null) break;
                }
                var changed = false;
                for (var i = 0; i < segs.Count; i++)
                {
                    if (done[i]) continue;
                    if (adj[path[0]].Count == 2 && segs[i][0] == path[0])
                    {
                        path.Insert(0, segs[i][1]);
                    }
                    else if (adj[path[0]].Count == 2 && segs[i][1] == path[0])
                    {
                        path.Insert(0, segs[i][0]);
                    }
                    else if (adj[path[path.Count - 1]].Count == 2 && segs[i][0] == path[path.Count - 1])
                    {
                        path.Add(segs[i][1]);
                    }
                    else if (adj[path[path.Count - 1]].Count == 2 && segs[i][1] == path[path.Count - 1])
                    {
                        path.Add(segs[i][0]);
                    }
                    else
                    {
                        continue;
                    }
                    done[i] = true;
                    changed = true;
                    break;
                }
                if (!changed)
                {
                    paths.Add(path);
                    path = null;
                }
            }
            return paths;
        }

        public static double[] SetSeaLevel(double[] heights, double q)
        {
            var output = new double[heights.Length];
            var doubleHeights = heights.Select(h => (double)h);
            var delta = doubleHeights.Quantile((double)q);
            for (var i = 0; i < heights.Length; i++)
            {
                output[i] = heights[i] - (double)delta;
            }
            return output.ToArray();
        }

        public static double[] FillSinks(Mesh mesh, double[] heights, double epsilon = 1e-5f)
        {
            var infinity = int.MaxValue;
            var output = new double[heights.Length];
            for (var i = 0; i < output.Length; i++)
            {
                if (IsNearEdge(mesh, i))
                {
                    output[i] = heights[i];
                }
                else
                {
                    output[i] = infinity;
                }
            }
            while (true)
            {
                var changed = false;
                for (var i = 0; i < output.Length; i++)
                {
                    if (output[i] == heights[i]) continue;
                    var nbs = Neighbours(mesh, i);
                    for (var j = 0; j < nbs.Count; j++)
                    {
                        if (heights[i] >= output[nbs[j]] + epsilon)
                        {
                            output[i] = heights[i];
                            changed = true;
                            break;
                        }
                        var oh = output[nbs[j]] + epsilon;
                        if ((output[i] > oh) && (oh > heights[i]))
                        {
                            output[i] = oh;
                            changed = true;
                        }
                    }
                }
                if (!changed) return output;
            }
        }

        public static double[] DoErosion(Mesh mesh, IHasDownhill downhillInstance, double[] heights, double amount, int n = 1)
        {
            heights = Terrain.FillSinks(mesh, heights);
            for (var i = 0; i < n; i++)
            {
                heights = Terrain.Erode(mesh, downhillInstance, heights, amount);
                heights = Terrain.FillSinks(mesh, heights);
            }
            return heights;
        }

        private static double[] Erode(Mesh mesh, IHasDownhill downhillInstance, double[] h, double amount)
        {
            var er = Terrain.ErosionRate(mesh, downhillInstance, h);
            var output = new double[h.Length];
            var maxr = er.Max();
            for (var i = 0; i < h.Length; i++)
            {
                output[i] = h[i] - amount * (er[i] / maxr);
            }
            return output;
        }

        public static double[] ErosionRate(Mesh mesh, IHasDownhill downhillInstance, double[] h)
        {
            var flux = Terrain.GetFlux(mesh, downhillInstance, h);
            var slope = Terrain.GetSlope(mesh, downhillInstance, h);
            var output = new double[h.Length];
            for (var i = 0; i < h.Length; i++)
            {
                var river = Math.Sqrt(flux[i]) * slope[i];
                var creep = slope[i] * slope[i];
                var total = 1000 * river + creep;
                total = total > 200 ? 200 : total;
                output[i] = total;
            }
            return output;
        }

        private static double[] GetFlux(Mesh mesh, IHasDownhill downhillInstance, double[] h)
        {
            var dh = Terrain.Downhill(mesh, downhillInstance, h);
            var idxs = new List<int>();
            var output = new double[h.Length];
            for (var i = 0; i < h.Length; i++)
            {
                idxs.Add(i);
                output[i] = 1f / h.Length;
            }
            idxs.Sort((a, b) =>
            {
                if (h[b] - h[a] > h[a] - h[b])
                    return 1;
                if (h[b] - h[a] < h[a] - h[b])
                    return -1;
                return 0;
            });
            for (var i = 0; i < h.Length; i++)
            {
                var j = idxs[i];
                if (dh[j] >= 0)
                {
                    output[dh[j]] += output[j];
                }
            }
            return output;
        }

        private static double[] GetSlope(Mesh mesh, IHasDownhill downhillInstance, double[] h)
        {
            var output = new double[h.Length];
            for (var i = 0; i < h.Length; i++)
            {
                var s = Terrain.Trislope(mesh, h, i);
                output[i] = (double)Math.Sqrt((double)(s.X * s.X + s.Y * s.Y));
            }
            return output;
        }
        public static Point Trislope(Mesh mesh, double[] h, int i)
        {
            var nbs = Terrain.Neighbours(mesh, i);
            if (nbs.Count != 3) return Point.Zero;
            var p0 = mesh.Vxs[nbs[0]];
            var p1 = mesh.Vxs[nbs[1]];
            var p2 = mesh.Vxs[nbs[2]];

            var x1 = p1.X - p0.X;
            var x2 = p2.X - p0.X;
            var y1 = p1.Y - p0.Y;
            var y2 = p2.Y - p0.Y;

            var det = x1 * y2 - x2 * y1;
            var h1 = h[nbs[1]] - h[nbs[0]];
            var h2 = h[nbs[2]] - h[nbs[0]];

            return new Point((y2 * h1 - y1 * h2) / det, (-x2 * h1 + x1 * h2) / det);
        }

        private static int[] Downhill(Mesh mesh, IHasDownhill downhillInstance, double[] h)
        {
            if (downhillInstance.Downhill != null) return downhillInstance.Downhill;

            var downs = new int[h.Length];
            for (var i = 0; i < h.Length; i++)
            {
                if (Terrain.Isedge(mesh, i))
                {
                    downs[i] = -2;
                    continue;
                }
                var best = -1;
                var besth = h[i];
                var nbs = Terrain.Neighbours(mesh, i);
                for (var j = 0; j < nbs.Count; j++)
                {
                    if (h[nbs[j]] < besth)
                    {
                        besth = h[nbs[j]];
                        best = nbs[j];
                    }
                }
                downs[i] = best;
            }
            downhillInstance.Downhill = downs;
            return downs;
        }

        private static bool Isedge(Mesh mesh, int i)
        {
            return (mesh.Adj[i].Count < 3);
        }

        public static double[] CleanCoast(Mesh mesh, double[] h, int iters)
        {
            for (var iter = 0; iter < iters; iter++)
            {
                //var changed = 0;
                var newh = new double[h.Length];
                for (var i = 0; i < h.Length; i++)
                {
                    newh[i] = h[i];
                    var nbs = Terrain.Neighbours(mesh, i);
                    if (h[i] <= 0 || nbs.Count != 3) continue;
                    var count = 0;
                    double best = -double.MaxValue;
                    for (var j = 0; j < nbs.Count; j++)
                    {
                        if (h[nbs[j]] > 0)
                        {
                            count++;
                        }
                        else if (h[nbs[j]] > best)
                        {
                            best = h[nbs.ElementAt(j)];
                        }
                    }
                    if (count > 1) continue;
                    newh[i] = best / 2;
                    //changed++;
                }
                h = newh;
                newh = new double[h.Length];
                for (var i = 0; i < h.Length; i++)
                {
                    newh[i] = h[i];
                    var nbs = Terrain.Neighbours(mesh, i);
                    if (h[i] > 0 || nbs.Count != 3) continue;
                    var count = 0;
                    double best = double.MaxValue;
                    for (var j = 0; j < nbs.Count; j++)
                    {
                        if (h[nbs[j]] <= 0)
                        {
                            count++;
                        }
                        else if (h[nbs[j]] < best)
                        {
                            best = h[nbs[j]];
                        }
                    }
                    if (count > 1) continue;
                    newh[i] = best / 2f;
                }
                h = newh;
            }
            return h;
        }
        public static List<List<Point>> GetRivers<T>(T instance, double limit) where T : IHasMesh, IHasDownhill, IHasHeights
        {
            var dh = Terrain.Downhill(instance.Mesh, instance, instance.Heights);
            var flux = Terrain.GetFlux(instance.Mesh, instance, instance.Heights);
            var links = new List<Point[]>();
            var above = 0;
            var h = instance.Heights;
            var mesh = instance.Mesh;
            for (var i = 0; i < h.Length; i++)
            {
                if (h[i] > 0) above++;
            }
            limit *= above / (double)h.Length;
            for (var i = 0; i < dh.Length; i++)
            {
                if (IsNearEdge(mesh, i)) continue;
                if (flux[i] > limit && h[i] > 0 && dh[i] >= 0)
                {
                    var up = mesh.Vxs[i];
                    var down = mesh.Vxs[dh[i]];
                    if (h[dh[i]] > 0)
                    {
                        links.Add(new[] { up, down });
                    }
                    else
                    {
                        var downV = new Point((up.X + down.X) / 2d, (up.Y + down.Y) / 2d);
                        links.Add(new[] { up, downV });
                    }
                }
            }

            var output = new List<List<Point>>();
            var mergedSegments = MergeSegments(links);
            foreach (var segment in mergedSegments)
                output.Add(RelaxPath(segment));

            return output;
        }
        private static List<Point> RelaxPath(List<Point> path)
        {
            var newpath = new List<Point>() { path[0] };
            for (var i = 1; i < path.Count - 1; i++)
            {
                var newpt = new Point(0.25f * path[i - 1].X + 0.5f * path[i].X + 0.25f * path[i + 1].X,
                    0.25f * path[i - 1].Y + 0.5f * path[i].Y + 0.25f * path[i + 1].Y);

                newpath.Add(newpt);
            }
            newpath.Add(path[path.Count - 1]);
            return newpath;
        }
        public static CityRender NewCityRender<T>(T instance, Extent extent) where T : IHasDownhill, IHasMesh, IHasHeights, IHasVoronoi
        {
            instance.Heights = instance.Heights != null ? instance.Heights : GenerateCoast(instance, 4096, extent);
            return new CityRender()
            {
                AreaProperties = new AreaProperties(),
                Cities = new List<int>()
            };
        }
        public static double[] CityScore<T>(T instance) where T : IHasCityRender, IHasDownhill, IHasMesh, IHasHeights
        {
            var h = instance.Heights;
            var mesh = instance.Mesh;
            var cities = instance.CityRender.Cities;
            var score = GetFlux(instance.Mesh, instance, h).ToList();
            for (var i = 0; i < score.Count; i++)
                score[i] = Math.Sqrt(score[i]);

            for (var i = 0; i < h.Length; i++)
            {
                if (h[i] <= 0 || Terrain.IsNearEdge(mesh, i))
                {
                    score[i] = -double.MaxValue;
                    continue;
                }
                score[i] += 0.01d / (1e-9d + Math.Abs(mesh.Vxs[i].X) - mesh.Extent.Width / 2d);
                score[i] += 0.01d / (1e-9d + Math.Abs(mesh.Vxs[i].Y) - mesh.Extent.Height / 2d);


                for (var j = 0; j < cities.Count; j++)
                {
                    score[i] -= 0.02d / (Terrain.Distance(mesh, cities[j], i) + 1e-9d);
                }
            }
            return score.ToArray();
        }
        private static double Distance(Mesh mesh, int i, int j)
        {
            var p = mesh.Vxs[i];
            var q = mesh.Vxs[j];
            return (double)Math.Sqrt((double)((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y)));
        }
        public static int Penalty(PossibleLabelLocation label, Mesh mesh, List<PossibleLabelLocation> citylabels, List<int> cities, List<List<Point>>[] avoids)
        {
            var pen = 0;
            if (label.X0 < -0.45d * mesh.Extent.Width) pen += 100;
            if (label.X1 > 0.45d * mesh.Extent.Width) pen += 100;
            if (label.Y0 < -0.45d * mesh.Extent.Height) pen += 100;
            if (label.Y1 > 0.45d * mesh.Extent.Height) pen += 100;
            for (var i = 0; i < citylabels.Count; i++)
            {
                var olabel = citylabels[i];
                if (label.X0 < olabel.X1 && label.X1 > olabel.X0 &&
                    label.Y0 < olabel.Y1 && label.Y1 > olabel.Y0)
                {
                    pen += 100;
                }
            }

            for (var i = 0; i < cities.Count; i++)
            {
                var c = mesh.Vxs[cities[i]];
                if (label.X0 < c.X && label.X1 > c.X && label.Y0 < c.Y && label.Y1 > c.Y)
                {
                    pen += 100;
                }
            }
            for (var i = 0; i < avoids.Length; i++)
            {
                var avoid = avoids[i];
                for (var j = 0; j < avoid.Count; j++)
                {
                    var avpath = avoid[j];
                    for (var k = 0; k < avpath.Count; k++)
                    {
                        var pt = avpath[k];
                        if (pt.X > label.X0 && pt.X < label.X1 && pt.Y > label.Y0 && pt.Y < label.Y1)
                        {
                            pen++;
                        }
                    }
                }
            }
            return pen;
        }
        public static Point TerrCenter(Mesh mesh, double[] h, double[] terr, int city, bool landOnly)
        {
            var x = 0d;
            var y = 0d;
            var n = 0;
            for (var i = 0; i < terr.Length; i++)
            {
                if (terr[i] != city) continue;
                if (landOnly && h[i] <= 0) continue;
                x += mesh.Vxs[i].X;
                y += mesh.Vxs[i].Y;
                n++;
            }
            return new Point(x / n, y / n);
        }
        public static List<List<Point>> GetBorders<T>(T instance) where T : IHasMesh, IHasCityRender, IHasHeights
        {
            var cityRender = instance.CityRender;
            var mesh = instance.Mesh;
            var terr = cityRender.Territories;
            var h = instance.Heights;
            var edges = new List<Point[]>();
            for (var i = 0; i < mesh.Edges.Count; i++)
            {
                var e = mesh.Edges[i];
                if (e.Right == null) continue;
                if (Terrain.IsNearEdge(mesh, e.Spot1) || IsNearEdge(mesh, e.Spot2)) continue;
                if (h[e.Spot1] < 0 || h[e.Spot2] < 0) continue;
                if (terr[e.Spot1] != terr[e.Spot2])
                {
                    edges.Add(new Point[] { e.Left, e.Right });
                }
            }

            var output = new List<List<Point>>();
            var mergedSegments = MergeSegments(edges);
            foreach (var segment in mergedSegments)
                output.Add(RelaxPath(segment));

            return output;
        }
        public static double[] GetTerritories<T>(T instance) where T : IHasCityRender, IHasDownhill, IHasMesh, IHasHeights
        {
            var cityRender = instance.CityRender;
            var h = instance.Heights;
            var cities = cityRender.Cities;
            var n = cityRender.AreaProperties.NumberOfTerritories;
            if (n > cities.Count) n = cities.Count;
            var flux = Terrain.GetFlux(instance.Mesh, instance, h);
            var territories = new double[h.Length];
            var queue = new List<CityProperty>();
            for (var i = 0; i < n; i++)
            {
                territories[cities[i]] = cities[i];
                var nbs = Terrain.Neighbours(instance.Mesh, cities[i]);
                for (var j = 0; j < nbs.Count; j++)
                {
                    queue.Add(new CityProperty()
                    {
                        Score = Terrain.Weight(instance.Mesh, h, flux, cities[i], nbs[j]),
                        City = cities[i],
                        Vx = nbs[j]
                    });
                    queue = queue.OrderBy(q => q.Score).ToList();
                }
            }
            while (queue.Count > 0)
            {
                var u = queue.ElementAt(0);
                queue.Remove(u);
                if (territories[u.Vx] != 0) continue;
                territories[u.Vx] = u.City;
                var nbs = Terrain.Neighbours(instance.Mesh, u.Vx);
                for (var i = 0; i < nbs.Count; i++)
                {
                    var v = nbs[i];
                    if (territories[v] != 0) continue;
                    var newdist = Terrain.Weight(instance.Mesh, h, flux, u.Vx, v);
                    queue.Add(new CityProperty()
                    {
                        Score = u.Score + newdist,
                        City = u.City,
                        Vx = v
                    });
                    queue = queue.OrderBy(q => q.Score).ToList();
                }
            }
            return territories;
        }
        private static double Weight(Mesh mesh, double[] h, double[] flux, int u, int v)
        {
            var horiz = Terrain.Distance(mesh, u, v);
            var vert = h[v] - h[u];
            if (vert > 0) vert /= 10;
            var diff = 1f + 0.25f * (double)Math.Pow((double)(vert / horiz), 2);
            diff += 100f * (double)Math.Sqrt((double)flux[u]);
            if (h[u] <= 0) diff = 100;
            if ((h[u] > 0) != (h[v] > 0)) return 1000f;
            return horiz * diff;
        }
        public static void PlaceCity<T>(T instance) where T : IHasCityRender, IHasDownhill, IHasMesh, IHasHeights
        {
            var score = Terrain.CityScore(instance);
            var newcity = -1;
            var highestScore = -1d;
            for (int i = 0; i < score.Length; i++)
            {
                if (score[i] > highestScore)
                {
                    highestScore = score[i];
                    newcity = i;
                }
            }
            instance.CityRender.Cities.Add(newcity);
        }
        public static void PlaceCities<T>(T instance) where T : IHasCityRender, IHasDownhill, IHasMesh, IHasHeights
        {
            var n = instance.CityRender.AreaProperties.NumberOfCities;
            for (var i = 0; i < n; i++)
            {
                Terrain.PlaceCity(instance);
            }
        }
    }
}
