using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.Statistics;
using Voronoi2;
using WorldMap.Geom;
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
        //public double[] Heights;
        public double[] Score;
        public double[] Territories;//has to be double for voronoi render
        public List<int> Cities = new List<int>();

        // public Mesh HeightMesh;
        // public Mesh TerritoryMesh;

        public List<List<Vector2f>> Rivers;
        public List<List<Vector2f>> Coasts;
        public List<List<Vector2f>> Borders;
    }

    public class Mesh
    {
        public List<Vector2f> Pts;
        public List<Vector2f> Vxs;
        public Dictionary<string, int> Vxids;
        public Dictionary<int, List<int>> Adj;
        public List<MapEdge> Edges;
        public Dictionary<int, List<Vector2f>> Tris;
        public Extent Extent;
    }

    public class MapEdge
    {
        public int Spot1;
        public int Spot2;
        public Vector2f Left;
        public Vector2f Right;
    }


    public class Terrain
    {
        private static Random random = new Random(200);

        public static double Runif(double lo, double hi)
        {
            var randomdouble = 0.3423424234f; //random.Nextdouble();
            return lo + randomdouble * (hi - lo);
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
                w = (double)(x1 * x1 + x2 * x2);
            }
            w = Math.Sqrt(-2f * Math.Log(w) / w);
            x2 *= (double)w;
            x1 *= (double)w;
        }

        public static List<Vector2f> GeneratePoints(int n, Extent extent)
        {
            var pts = new List<Vector2f>();
            for (var i = 0; i < n; i++)
            {
                var x = (double)random.NextDouble() * extent.width;
                var y = (double)random.NextDouble() * extent.height;
                pts.Add(new Vector2f(x, y));
            }
            return pts;
        }

        public static Mesh GenerateGoodMesh(Voronoi voronoi, int n, Extent extent)
        {
            var pts = GenerateGoodPoints(voronoi, n, extent);
            return MakeMesh(voronoi, pts, extent);
        }

        private static List<Vector2f> GenerateGoodPoints(Voronoi voronoi, int n, Extent extent)
        {

            var pts = GeneratePoints(n, extent);
            pts = pts.OrderBy(point => point.x).ToList();
            return ImprovePoints(pts, extent);
        }

        public static double[] GenerateCoast<T>(Voronoi voronoi, T instance, int npts, Extent extent) where T : IHasDownhill, IHasMesh
        {
            instance.Mesh = Terrain.GenerateGoodMesh(voronoi, npts, extent);
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

        private static List<Vector2f> ImprovePoints(List<Vector2f> pts, Extent extent, int n = 1)
        {
            /*
            for (var i = 0; i < n; i++)
            {
                pts = Voronoi(pts, extent)
                    .polygons(pts)
                    .map(centroid);
            }*/
            return pts;
        }

        private static void Voronoi(List<Vector2f> pts, Extent extent)
        {
            /*
            var w = extent.width / 2f;
            var h = extent.height / 2f;
            return d3.voronoi()
                .extent([


                    [-w, -h],

            [w, h]
		])(pts);*/
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
                    Site1 = new Vector2f(Convert.ToDouble(zero.Split(',')[0]), Convert.ToDouble(zero.Split(',')[1])),
                    Site2 = new Vector2f(Convert.ToDouble(one.Split(',')[0]), Convert.ToDouble(one.Split(',')[1])),
                    Left = new Vector2f(Convert.ToDouble(left.Split(',')[0]), Convert.ToDouble(left.Split(',')[1]))
                };
                if (anotherSplit.Length > 7)
                {
                    var right = anotherSplit[7];
                    newEdge.Right = new Vector2f(Convert.ToDouble(right.Split(',')[0]),
                        Convert.ToDouble(right.Split(',')[1]));
                }
                outputEdges.Add(newEdge);
            }
            return outputEdges;
        }


        public static Mesh MakeMesh(Voronoi voronoi, List<Vector2f> pts, Extent extent)
        {
            var primEdges = ParseEdges(StringValues.EdgesString);
            var vxs = new List<Vector2f>();
            var vxids = new Dictionary<Vector2f, int>();
            var adj = new Dictionary<int, List<int>>();
            var edges = new List<MapEdge>();
            var tris = new Dictionary<int, List<Vector2f>>();

            //var variance = 5.5f;
            foreach (var e in primEdges)
            {
                //if (e == null || e.x1 == 0 || e.y1 == 0 || e.x2 == 0 || e.y2 == 0) continue;
                //if (Equals4DigitPrecision((double)e.x1, (double)e.x2) && Equals4DigitPrecision((double)e.y1, (double)e.y2)) continue;

                //if (e.LeftSite.x < variance || e.RightSite.x < variance || e.LeftSite.y < variance || e.RightSite.y < variance)
                //  continue;
                //if (e.LeftSite.x > extent.width - variance || e.RightSite.x > extent.width - variance || e.LeftSite.y > extent.height - variance || e.RightSite.y > extent.height - variance)
                //  continue;
                if (e == null)
                    continue;

                int e0 = 0;
                var foundE0 = vxids.TryGetValue(e.Site1, out e0);
                if (!foundE0)
                {
                    e0 = vxs.Count;
                    vxids.Add(e.Site1, e0);
                    vxs.Add(e.Site1);
                }
                List<Vector2f> firstTri = null;
                bool foundFirst = tris.TryGetValue(e0, out firstTri);
                if (!foundFirst)
                {
                    firstTri = new List<Vector2f>();
                    tris.Add(e0, firstTri);
                }

                int e1 = 0;
                var foundE1 = vxids.TryGetValue(e.Site2, out e1);
                if (!foundE1)
                {
                    e1 = vxs.Count;
                    vxids.Add(e.Site2, e1);
                    vxs.Add(e.Site2);
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


                if (!firstTri.Contains(e.Left))
                {
                    firstTri.Add(e.Left);
                    tris[e0] = firstTri;
                }
                if (e.Right != null && e.Right.x != 0 && e.Right.y != 0 && !firstTri.Contains(e.Right))
                {
                    firstTri.Add(e.Right);
                    tris[e0] = firstTri;
                }

                List<Vector2f> secondTri = null;
                var foundSecond = tris.TryGetValue(e1, out secondTri);
                if (!foundSecond)
                {
                    secondTri = new List<Vector2f>();
                    tris.Add(e1, secondTri);
                }
                if (!secondTri.Contains(e.Left))
                {
                    secondTri.Add(e.Left);
                    tris[e1] = secondTri;
                }
                if (e.Right != null && e.Right.x != 0 && e.Right.y != 0 && !secondTri.Contains(e.Right))
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
            var mounts = new List<Vector2f>();
            var randOne = .3f; //Math.random();
            var randTwo = .5f; //Math.random();
            for (var i = 0; i < n; i++)
            {
                mounts.Add(new Vector2f(mesh.Extent.width * (randOne - 0.5f), mesh.Extent.height * (randTwo - 0.5f)));
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
                            Math.Pow(Math.Exp((double)(-((p.x - m.x) * (p.x - m.x) + (p.y - m.y) * (p.y - m.y)) / (2 * r * r))),
                                2);
                }
            }
            return newvals;
        }

        public static double[] Slope(Mesh mesh, Vector2f direction)
        {
            var output = new List<double>();
            foreach (var f in mesh.Vxs)
            {
                output.Add(f.x * direction.x + f.y * direction.y);
            }
            return output.ToArray();
        }

        public static double[] Cone(Mesh mesh, double slope)
        {
            var output = new List<double>();
            foreach (var f in mesh.Vxs)
            {
                output.Add((double)Math.Pow((double)(f.x * f.x + f.y * f.y), 0.5) * slope);
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

        public static Vector2f RandomVector(int scale)
        {
            var xOut = 0.03661885595999543f;
            var yOut = -0.9259299039132616f;
            //RNorm(out xOut, out yOut);
            Debug.WriteLine($"x:{xOut} y:{yOut}");
            return new Vector2f(scale * xOut, scale * yOut);
        }

        public static double[] Randomdoubles(int scale)
        {
            var vector = RandomVector(scale);
            return new[] { (double)vector.x, (double)vector.y };
        }


        public static List<List<Vector2f>> Contour<T>(T instance, int level = 0) where T : IHasMesh, IHasHeights
        {
            var edges = new List<Vector2f[]>();
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
            var x = mesh.Vxs[i].x;
            var y = mesh.Vxs[i].y;
            var w = mesh.Extent.width;
            var h = mesh.Extent.height;
            return x < -0.45f * w || x > 0.45f * w || y < -0.45f * h || y > 0.45f * h;
        }

        public static List<List<Vector2f>> MergeSegments(List<Vector2f[]> segs)
        {
            var adj = new Dictionary<Vector2f, List<Vector2f>>();
            for (var i = 0; i < segs.Count; i++)
            {
                var seg = segs[i];
                List<Vector2f> a0 = null;
                var foundA0 = adj.TryGetValue(seg[0], out a0);
                if (!foundA0)
                {
                    a0 = new List<Vector2f>();
                    adj[seg[0]] = a0;
                }

                List<Vector2f> a1 = null;
                var foundA1 = adj.TryGetValue(seg[0], out a1);
                if (!foundA1)
                {
                    a1 = new List<Vector2f>();
                    adj[seg[1]] = a1;
                }
                a0.Add(seg[1]);
                a1.Add(seg[0]);
                adj[seg[0]] = a0;
                adj[seg[1]] = a1;
            }
            var done = new bool[segs.Count];
            var paths = new List<List<Vector2f>>();
            List<Vector2f> path = null;
            while (true)
            {
                if (path == null)
                {
                    for (var i = 0; i < segs.Count; i++)
                    {
                        if (done[i]) continue;
                        done[i] = true;
                        path = new List<Vector2f> { segs[i].ElementAt(0), segs[i].ElementAt(1) };
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
                output[i] = (double)Math.Sqrt((double)(s.x * s.x + s.y * s.y));
            }
            return output;
        }
        public static Vector2f Trislope(Mesh mesh, double[] h, int i)
        {
            var nbs = Terrain.Neighbours(mesh, i);
            if (nbs.Count != 3) return Vector2f.zero;
            var p0 = mesh.Vxs[nbs[0]];
            var p1 = mesh.Vxs[nbs[1]];
            var p2 = mesh.Vxs[nbs[2]];

            var x1 = p1.x - p0.x;
            var x2 = p2.x - p0.x;
            var y1 = p1.y - p0.y;
            var y2 = p2.y - p0.y;

            var det = x1 * y2 - x2 * y1;
            var h1 = h[nbs[1]] - h[nbs[0]];
            var h2 = h[nbs[2]] - h[nbs[0]];

            return new Vector2f((y2 * h1 - y1 * h2) / det, (-x2 * h1 + x1 * h2) / det);
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
                    //changed++;
                }
                h = newh;
            }
            return h;
        }

        public static List<List<Vector2f>> GetRivers<T>(T instance, double limit) where T : IHasMesh, IHasDownhill, IHasHeights
        {
            var dh = Terrain.Downhill(instance.Mesh, instance, instance.Heights);
            var flux = Terrain.GetFlux(instance.Mesh, instance, instance.Heights);
            var links = new List<Vector2f[]>();
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
                        var downV = new Vector2f((up.x + down.x) / 2d, (up.y + down.y) / 2d);
                        links.Add(new[] { up, downV });
                    }
                }
            }

            var output = new List<List<Vector2f>>();
            var mergedSegments = MergeSegments(links);
            foreach (var segment in mergedSegments)
                output.Add(RelaxPath(segment));

            return output;
        }

        private static List<Vector2f> RelaxPath(List<Vector2f> path)
        {
            var newpath = new List<Vector2f>() { path[0] };
            for (var i = 1; i < path.Count - 1; i++)
            {
                var newpt = new Vector2f(0.25f * path[i - 1].x + 0.5f * path[i].x + 0.25f * path[i + 1].x,
                    0.25f * path[i - 1].y + 0.5f * path[i].y + 0.25f * path[i + 1].y);

                newpath.Add(newpt);
            }
            newpath.Add(path[path.Count - 1]);
            return newpath;
        }

        public static CityRender NewCityRender<T>(Voronoi voronoi, T instance, Extent extent) where T : IHasDownhill, IHasMesh, IHasHeights
        {
            instance.Heights = instance.Heights != null ? instance.Heights : GenerateCoast(voronoi, instance, 4096, extent);
            return new CityRender()
            {
                //Heights = instance.Heights,
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
                score[i] += 0.01d / (1e-9d + Math.Abs(mesh.Vxs[i].x) - mesh.Extent.width / 2d);
                score[i] += 0.01d / (1e-9d + Math.Abs(mesh.Vxs[i].y) - mesh.Extent.height / 2d);


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
            return (double)Math.Sqrt((double)((p.x - q.x) * (p.x - q.x) + (p.y - q.y) * (p.y - q.y)));
        }
        public static int Penalty(PossibleLabelLocation label, Mesh mesh, List<PossibleLabelLocation> citylabels, List<int> cities, List<List<Vector2f>>[] avoids)
        {
            var pen = 0;
            if (label.X0 < -0.45d * mesh.Extent.width) pen += 100;
            if (label.X1 > 0.45d * mesh.Extent.width) pen += 100;
            if (label.Y0 < -0.45d * mesh.Extent.height) pen += 100;
            if (label.Y1 > 0.45d * mesh.Extent.height) pen += 100;
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
                if (label.X0 < c.x && label.X1 > c.x && label.Y0 < c.y && label.Y1 > c.y)
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
                        if (pt.x > label.X0 && pt.x < label.X1 && pt.y > label.Y0 && pt.y < label.Y1)
                        {
                            pen++;
                        }
                    }
                }
            }
            return pen;
        }

        public static Vector2f TerrCenter(Mesh mesh, double[] h, double[] terr, int city, bool landOnly)
        {
            var x = 0d;
            var y = 0d;
            var n = 0;
            for (var i = 0; i < terr.Length; i++)
            {
                if (terr[i] != city) continue;
                if (landOnly && h[i] <= 0) continue;
                x += mesh.Vxs[i].x;
                y += mesh.Vxs[i].y;
                n++;
            }
            return new Vector2f(x / n, y / n);
        }

        public static List<List<Vector2f>> GetBorders<T>(T instance) where T : IHasMesh, IHasCityRender, IHasHeights
        {
            var cityRender = instance.CityRender;
            var mesh = instance.Mesh;
            var terr = cityRender.Territories;
            var h = instance.Heights;
            var edges = new List<Vector2f[]>();
            for (var i = 0; i < mesh.Edges.Count; i++)
            {
                var e = mesh.Edges[i];
                if (e.Right == null) continue;
                if (Terrain.IsNearEdge(mesh, e.Spot1) || IsNearEdge(mesh, e.Spot2)) continue;
                if (h[e.Spot1] < 0 || h[e.Spot2] < 0) continue;
                if (terr[e.Spot1] != terr[e.Spot2])
                {
                    edges.Add(new Vector2f[] { e.Left, e.Right });
                }
            }

            var output = new List<List<Vector2f>>();
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
