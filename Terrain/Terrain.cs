using System;
using System.Collections.Generic;
using System.Linq;
using D3Voronoi;
using MathNet.Numerics.Statistics;
using Priority_Queue;

namespace TerrainGenerator
{
    /// <summary>
    /// Main class responsible for generating terrain, coastlines, and geographical features for fantasy maps.
    /// Provides methods for mesh generation, height map creation, erosion simulation, and various terrain algorithms.
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// Random number generator instance used for all terrain generation operations.
        /// </summary>
        private Random _random;

        /// <summary>
        /// Initializes a new instance of the Terrain class with a specified random seed.
        /// </summary>
        /// <param name="seed">The seed value for the random number generator to ensure reproducible terrain generation.</param>
        public Terrain(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Generates a random double value within the specified range using uniform distribution.
        /// </summary>
        /// <param name="lo">The lower bound (inclusive) of the range.</param>
        /// <param name="hi">The upper bound (exclusive) of the range.</param>
        /// <returns>A random double value between lo and hi.</returns>
        public double Runif(double lo, double hi)
        {
            return lo + _random.NextDouble() * (hi - lo);
        }

        /// <summary>
        /// Generates two normally distributed random numbers using the Box-Muller transform.
        /// </summary>
        /// <param name="x1">Output parameter for the first normally distributed random number.</param>
        /// <param name="x2">Output parameter for the second normally distributed random number.</param>
        public void RNorm(out double x1, out double x2)
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

        /// <summary>
        /// Generates an array of random points within the specified extent.
        /// </summary>
        /// <param name="n">The number of points to generate.</param>
        /// <param name="extent">The bounding area within which to generate points.</param>
        /// <returns>An array of randomly distributed points.</returns>
        public Point[] GeneratePoints(int n, Extent extent)
        {
            var pts = new Point[n];
            for (var i = 0; i < n; i++)
            {
                var x = (_random.NextDouble() - 0.5) * extent.Width;
                var y = (_random.NextDouble() - 0.5) * extent.Height;
                pts[i] = new Point(x, y);
            }
            return pts;
        }

        /// <summary>
        /// Generates a high-quality mesh with well-distributed points using Lloyd relaxation.
        /// </summary>
        /// <param name="n">The number of points to generate for the mesh.</param>
        /// <param name="extent">The bounding area for the mesh generation.</param>
        /// <returns>A Mesh object with optimally distributed vertices and connectivity information.</returns>
        public Mesh GenerateGoodMesh(int n, Extent extent)
        {
            var pts = GenerateGoodPoints(n, extent);
            return MakeMesh(pts, extent);
        }

        /// <summary>
        /// Generates well-distributed points by applying Lloyd relaxation to improve point distribution.
        /// </summary>
        /// <param name="n">The number of points to generate.</param>
        /// <param name="extent">The bounding area for point generation.</param>
        /// <returns>An array of optimally distributed points.</returns>
        private Point[] GenerateGoodPoints(int n, Extent extent)
        {
            var pts = GeneratePoints(n, extent);
            Array.Sort(pts, (a, b) => a.X.CompareTo(b.X));
            return ImprovePoints(pts, extent, 1);
        }

        /// <summary>
        /// Generates a complete coastline with terrain features including mountains, erosion, and water bodies.
        /// </summary>
        /// <param name="downhill">Output parameter containing downhill flow directions for each vertex.</param>
        /// <param name="mesh">Output parameter containing the generated mesh structure.</param>
        /// <param name="npts">The number of points to use for mesh generation.</param>
        /// <param name="extent">The bounding area for terrain generation.</param>
        /// <returns>An array of height values representing the final terrain elevation.</returns>
        public double[] GenerateCoast(ref int[] downhill, ref Mesh mesh, int npts, Extent extent)
        {
            mesh = GenerateGoodMesh(npts, extent);
            var h = Add(mesh.Vxs.Length,
                Slope(mesh, RandomVector(4)),
                Cone(mesh, Runif(-1, -1)),
                Mountains(mesh, 50)
                );
            for (var i = 0; i < 10; i++)
            {
                h = Relax(mesh, h);
            }
            h = Peaky(h);
            h = DoErosion(ref mesh, ref downhill, h, Runif(0, 0.1d), 5);
            h = SetSeaLevel(h, Runif(0.2d, 0.6d));
            h = FillSinks(ref mesh, h);
            h = CleanCoast(mesh, h, 3);
            return h;
        }

        /// <summary>
        /// Calculates the centroid (geometric center) of a collection of points.
        /// </summary>
        /// <param name="pts">The list of points to calculate the centroid for.</param>
        /// <returns>A Point representing the centroid of the input points.</returns>
        private Point Centroid(List<Point> pts)
        {
            var x = 0d;
            var y = 0d;
            foreach (Point t in pts)
            {
                x += t.X;
                y += t.Y;
            }
            return new Point(x / pts.Count, y / pts.Count);
        }

        /// <summary>
        /// Improves point distribution using Lloyd relaxation algorithm by moving points toward Voronoi cell centroids.
        /// </summary>
        /// <param name="pts">The array of points to improve.</param>
        /// <param name="extent">The bounding area for the points.</param>
        /// <param name="n">The number of relaxation iterations to perform (default is 1).</param>
        /// <returns>An array of points with improved distribution.</returns>
        public Point[] ImprovePoints(Point[] pts, Extent extent, int n = 1)
        {
            extent = extent == null ? Extent.DefaultExtent : extent;
            for (var i = 0; i < n; i++)
            {
                pts = Voronoi(extent).Polygons(pts).Select(p => Centroid(p.Points)).ToArray();
            }
            return pts;
        }
        
        /// <summary>
        /// Creates a Voronoi diagram generator configured for the specified extent.
        /// </summary>
        /// <param name="extent">The bounding area for the Voronoi diagram.</param>
        /// <returns>A Voronoi instance configured with the appropriate extent.</returns>
        public Voronoi Voronoi(Extent extent)
        {
            extent = extent == null ? Extent.DefaultExtent : extent;
            var w = extent.Width / 2d;
            var h = extent.Height / 2d;
            var newExtent = new Extent(-w, -h, w, h);
            return new Voronoi(newExtent);
        }

        /// <summary>
        /// Creates a mesh data structure from an array of points using Voronoi diagram generation.
        /// </summary>
        /// <param name="pts">The array of points to create the mesh from.</param>
        /// <param name="extent">The bounding area for the mesh.</param>
        /// <returns>A Mesh object containing vertices, edges, adjacency information, and triangulation data.</returns>
        public Mesh MakeMesh(Point[] pts, Extent extent)
        {
            var vor = Voronoi(extent).VoronoiDiagram(pts);
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
                Vxs = vxs.ToArray(),
                Adj = adj,
                Tris = tris,
                Edges = edges,
                Extent = extent
            };


            return mesh;
        }

        /// <summary>
        /// Adds multiple height arrays together element-wise to create a composite height map.
        /// </summary>
        /// <param name="count">The number of elements in each height array.</param>
        /// <param name="values">Variable number of height arrays to add together.</param>
        /// <returns>A new array with the sum of all input arrays at each position.</returns>
        public double[] Add(int count, params double[][] values)
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

        /// <summary>
        /// Generates mountain-like features by placing Gaussian peaks at random locations.
        /// Each mountain contributes height based on distance from its center using a Gaussian distribution.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions.</param>
        /// <param name="n">The number of mountain peaks to generate.</param>
        /// <param name="r">The radius parameter controlling mountain width (default is 0.05).</param>
        /// <returns>An array of height values representing mountain features.</returns>
        public double[] Mountains(Mesh mesh, int n, double r = 0.05f)
        {
            var mounts = new List<Point>();
            for (var i = 0; i < n; i++)
            {
                mounts.Add(new Point(mesh.Extent.Width * (_random.NextDouble() - 0.5f), mesh.Extent.Height * (_random.NextDouble() - 0.5f)));
            }
            var newvals = new double[mesh.Vxs.Length];
            for (var i = 0; i < mesh.Vxs.Length; i++)
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

        /// <summary>
        /// Creates a linear slope across the terrain in the specified direction.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions.</param>
        /// <param name="direction">A vector indicating the direction and magnitude of the slope.</param>
        /// <returns>An array of height values representing the linear slope.</returns>
        public double[] Slope(Mesh mesh, Point direction)
        {
            var output = new List<double>();
            foreach (var f in mesh.Vxs)
            {
                output.Add(f.X * direction.X + f.Y * direction.Y);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Creates a conical height distribution with maximum height at the center.
        /// Height decreases radially from the center based on distance.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions.</param>
        /// <param name="slope">The slope factor controlling the cone steepness.</param>
        /// <returns>An array of height values representing the conical distribution.</returns>
        public double[] Cone(Mesh mesh, double slope)
        {
            var output = new List<double>();
            foreach (var f in mesh.Vxs)
            {
                output.Add((double)Math.Pow((double)(f.X * f.X + f.Y * f.Y), 0.5) * slope);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Normalizes height values to the range [0, 1] based on the minimum and maximum values.
        /// </summary>
        /// <param name="heights">The array of height values to normalize.</param>
        /// <returns>An array of normalized height values between 0 and 1.</returns>
        public double[] Normalize(double[] heights)
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

        /// <summary>
        /// Applies a square root transformation to normalized heights to create more pronounced peaks.
        /// </summary>
        /// <param name="heights">The array of height values to transform.</param>
        /// <returns>An array of height values with enhanced peak characteristics.</returns>
        public double[] Peaky(double[] heights)
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

        /// <summary>
        /// Smooths height values by averaging each vertex with its neighbors.
        /// Helps reduce noise and create more natural terrain transitions.
        /// </summary>
        /// <param name="mesh">The mesh containing adjacency information.</param>
        /// <param name="heights">The array of height values to smooth.</param>
        /// <returns>An array of smoothed height values.</returns>
        public double[] Relax(Mesh mesh, double[] heights)
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

        /// <summary>
        /// Gets the list of neighboring vertex indices for a given vertex in the mesh.
        /// </summary>
        /// <param name="mesh">The mesh containing adjacency information.</param>
        /// <param name="index">The vertex index to find neighbors for.</param>
        /// <returns>A list of neighboring vertex indices.</returns>
        public List<int> Neighbours(Mesh mesh, int index)
        {
            var onbs = mesh.Adj[index];
            var nbs = new List<int>();
            for (var i = 0; i < onbs.Count; i++)
            {
                nbs.Add(onbs[i]);
            }
            return nbs;
        }

        /// <summary>
        /// Generates a random direction vector using normal distribution for both X and Y components.
        /// </summary>
        /// <param name="scale">The scaling factor for the vector magnitude.</param>
        /// <returns>A Point representing a random direction vector.</returns>
        public Point RandomVector(int scale)
        {
            var xOut = 0d;
            var yOut = 0d;
            RNorm(out xOut, out yOut);
            return new Point(scale * xOut, scale * yOut);
        }

        /// <summary>
        /// Generates a random vector and returns it as an array of doubles.
        /// </summary>
        /// <param name="scale">The scaling factor for the vector magnitude.</param>
        /// <returns>An array containing the X and Y components of the random vector.</returns>
        public double[] RandomDoubles(int scale)
        {
            var vector = RandomVector(scale);
            return new[] { (double)vector.X, (double)vector.Y };
        }

        /// <summary>
        /// Generates an uneroded terrain with basic height features including slopes, cones, and mountains.
        /// </summary>
        /// <param name="mesh">Output parameter for the generated mesh.</param>
        /// <param name="h">Output parameter for the generated height array.</param>
        public void GenerateUneroded(ref Mesh mesh, ref double[] h)
        {
            mesh = GenerateGoodMesh(4096, Extent.DefaultExtent);
            h = Add(mesh.Vxs.Length, Slope(mesh, RandomVector(4)),
                Cone(mesh, Runif(-1, 1)),
                Mountains(mesh, 50));
            h = Peaky(h);
            h = FillSinks(ref mesh, h);
            h = SetSeaLevel(h, 0.5f);
        }

        /// <summary>
        /// Generates contour lines at a specified elevation level by finding edges that cross the contour threshold.
        /// </summary>
        /// <param name="mesh">The mesh containing edge information.</param>
        /// <param name="heights">Array of height values for vertices.</param>
        /// <param name="level">The elevation level for contour generation (default is 0).</param>
        /// <returns>A list of contour paths, where each path is a list of connected points.</returns>
        public List<List<Point>> Contour(ref Mesh mesh, ref double[] heights, int level = 0)
        {
            var edges = new List<Point[]>();
            for (var i = 0; i < mesh.Edges.Count; i++)
            {
                var e = mesh.Edges[i];
                if (e.Right == null) continue;
                if (IsNearEdge(mesh, e.Spot1) || IsNearEdge(mesh, e.Spot2)) continue;
                if ((heights[e.Spot1] > level && heights[e.Spot2] <= level) || (heights[e.Spot2] > level && heights[e.Spot1] <= level))
                {
                    edges.Add(new[] { e.Left, e.Right });
                }
            }
            return MergeSegments(edges);
        }

        /// <summary>
        /// Determines if a vertex is near the edge of the mesh boundary.
        /// </summary>
        /// <param name="mesh">The mesh containing the vertex and extent information.</param>
        /// <param name="i">The index of the vertex to check.</param>
        /// <returns>True if the vertex is near the mesh boundary, false otherwise.</returns>
        public bool IsNearEdge(Mesh mesh, int i)
        {
            var x = mesh.Vxs[i].X;
            var y = mesh.Vxs[i].Y;
            var w = mesh.Extent.Width;
            var h = mesh.Extent.Height;
            return x < -0.45f * w || x > 0.45f * w || y < -0.45f * h || y > 0.45f * h;
        }

        /// <summary>
        /// Merges line segments into continuous paths by connecting endpoints.
        /// Used for creating contour lines and other continuous features from discrete segments.
        /// </summary>
        /// <param name="segs">List of line segments to merge, where each segment is defined by two points.</param>
        /// <returns>A list of continuous paths formed by connecting the segments.</returns>
        public List<List<Point>> MergeSegments(List<Point[]> segs)
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

        /// <summary>
        /// Sets the sea level by subtracting a quantile-based threshold from all height values.
        /// Areas below the new sea level will have negative heights (representing water).
        /// </summary>
        /// <param name="heights">The array of height values to adjust.</param>
        /// <param name="q">The quantile value (0.0 to 1.0) to use as the sea level threshold.</param>
        /// <returns>An array of height values adjusted relative to the new sea level.</returns>
        public double[] SetSeaLevel(double[] heights, double q)
        {
            var output = new double[heights.Length];
            var doubleHeights = heights.Select(h => h);
            var delta = doubleHeights.Quantile(q);
            for (var i = 0; i < heights.Length; i++)
            {
                output[i] = heights[i] - delta;
            }
            return output.ToArray();
        }

        /// <summary>
        /// Fills sink areas in the terrain to ensure proper water flow by raising low areas to allow drainage.
        /// Uses an iterative algorithm to eliminate local minima that would trap water.
        /// </summary>
        /// <param name="mesh">The mesh containing terrain topology.</param>
        /// <param name="heights">Array of height values to process.</param>
        /// <param name="epsilon">Small value to ensure proper drainage flow (default is 1e-5).</param>
        /// <returns>An array of height values with sinks filled to enable proper water flow.</returns>
        public double[] FillSinks(ref Mesh mesh, double[] heights, double epsilon = 1e-5f)
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

        /// <summary>
        /// Performs erosion simulation on the terrain by iteratively applying erosion processes.
        /// Combines sink filling and erosion to create realistic terrain weathering effects.
        /// </summary>
        /// <param name="mesh">The mesh containing terrain topology.</param>
        /// <param name="downhill">Output parameter for downhill flow directions.</param>
        /// <param name="heights">Array of height values to erode.</param>
        /// <param name="amount">The amount of erosion to apply in each iteration.</param>
        /// <param name="n">The number of erosion iterations to perform (default is 1).</param>
        /// <returns>An array of height values after erosion processing.</returns>
        public double[] DoErosion(ref Mesh mesh, ref int[] downhill, double[] heights, double amount, int n = 1)
        {
            heights = FillSinks(ref mesh, heights);
            for (var i = 0; i < n; i++)
            {
                heights = Erode(mesh, ref downhill, heights, amount);
                heights = FillSinks(ref mesh, heights);
            }
            return heights;
        }

        /// <summary>
        /// Applies a single erosion step to the terrain based on calculated erosion rates.
        /// </summary>
        /// <param name="mesh">The mesh containing terrain topology.</param>
        /// <param name="downhill">Array of downhill flow directions for each vertex.</param>
        /// <param name="h">Array of height values to erode.</param>
        /// <param name="amount">The erosion amount scaling factor.</param>
        /// <returns>An array of height values after one erosion step.</returns>
        private double[] Erode(Mesh mesh, ref int[] downhill, double[] h, double amount)
        {
            var er = ErosionRate(mesh, ref downhill, h);
            var output = new double[h.Length];
            var maxr = er.Max();
            for (var i = 0; i < h.Length; i++)
            {
                output[i] = h[i] - amount * (er[i] / maxr);
            }
            return output;
        }

        /// <summary>
        /// Calculates the erosion rate at each vertex based on water flux and terrain slope.
        /// Combines river erosion (flux-based) and creep erosion (slope-based) effects.
        /// </summary>
        /// <param name="mesh">The mesh containing terrain topology.</param>
        /// <param name="downhill">Array of downhill flow directions for each vertex.</param>
        /// <param name="h">Array of height values for slope calculation.</param>
        /// <returns>An array of erosion rates for each vertex.</returns>
        public double[] ErosionRate(Mesh mesh, ref int[] downhill, double[] h)
        {
            var flux = GetFlux(ref mesh, ref downhill, h);
            var slope = GetSlope(mesh, h);
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

        /// <summary>
        /// Calculates water flux (flow accumulation) at each vertex by following downhill flow paths.
        /// Higher flux values indicate areas with more water flow, such as valley bottoms and river channels.
        /// </summary>
        /// <param name="mesh">The mesh containing terrain topology.</param>
        /// <param name="downhill">Array of downhill flow directions for each vertex.</param>
        /// <param name="h">Array of height values for flow calculation.</param>
        /// <returns>An array of flux values indicating water flow accumulation at each vertex.</returns>
        private double[] GetFlux(ref Mesh mesh, ref int[] downhill, double[] h)
        {
            var dh = Downhill(mesh, ref downhill, h);
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
                if (j >= 0 && j < dh.Length && dh[j] >= 0 && dh[j] < output.Length)
                {
                    output[dh[j]] += output[j];
                }
            }
            return output;
        }

        /// <summary>
        /// Calculates the slope magnitude at each vertex using triangular slope estimation.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions and connectivity.</param>
        /// <param name="h">Array of height values for slope calculation.</param>
        /// <returns>An array of slope magnitudes for each vertex.</returns>
        private double[] GetSlope(Mesh mesh, double[] h)
        {
            var output = new double[h.Length];
            for (var i = 0; i < h.Length; i++)
            {
                var s = Trislope(mesh, h, i);
                output[i] = (double)Math.Sqrt((double)(s.X * s.X + s.Y * s.Y));
            }
            return output;
        }
        
        /// <summary>
        /// Calculates the slope vector at a vertex using the three neighboring vertices.
        /// Uses linear interpolation across the triangle to determine the gradient.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions and connectivity.</param>
        /// <param name="h">Array of height values.</param>
        /// <param name="i">The vertex index to calculate slope for.</param>
        /// <returns>A Point representing the slope vector (gradient) at the vertex.</returns>
        public Point Trislope(Mesh mesh, double[] h, int i)
        {
            var nbs = Neighbours(mesh, i);
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

        /// <summary>
        /// Calculates downhill flow directions for each vertex by finding the steepest descent neighbor.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex connectivity.</param>
        /// <param name="downhill">Reference to the downhill array to populate or return if already calculated.</param>
        /// <param name="h">Array of height values for flow direction calculation.</param>
        /// <returns>An array of vertex indices indicating the downhill flow direction for each vertex.</returns>
        private int[] Downhill(Mesh mesh, ref int[] downhill, double[] h)
        {
            if (downhill != null) return downhill;

            var downs = new int[h.Length];
            for (var i = 0; i < h.Length; i++)
            {
                if (Isedge(mesh, i))
                {
                    downs[i] = -2;
                    continue;
                }
                var best = -1;
                var besth = h[i];
                var nbs = Neighbours(mesh, i);
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
            downhill = downs;
            return downs;
        }

        /// <summary>
        /// Determines if a vertex is on the edge of the mesh by checking its neighbor count.
        /// </summary>
        /// <param name="mesh">The mesh containing adjacency information.</param>
        /// <param name="i">The vertex index to check.</param>
        /// <returns>True if the vertex is on the mesh edge (has fewer than 3 neighbors), false otherwise.</returns>
        private bool Isedge(Mesh mesh, int i)
        {
            return (mesh.Adj[i].Count < 3);
        }

        /// <summary>
        /// Cleans up coastline features by smoothing irregular water-land boundaries.
        /// Removes isolated water cells and fills isolated land cells near the coast.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex connectivity.</param>
        /// <param name="h">Array of height values representing land (positive) and water (negative).</param>
        /// <param name="iters">The number of cleanup iterations to perform.</param>
        /// <returns>An array of height values with cleaned coastlines.</returns>
        public double[] CleanCoast(Mesh mesh, double[] h, int iters)
        {
            for (var iter = 0; iter < iters; iter++)
            {
                //var changed = 0;
                var newh = new double[h.Length];
                for (var i = 0; i < h.Length; i++)
                {
                    newh[i] = h[i];
                    var nbs = Neighbours(mesh, i);
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
                    var nbs = Neighbours(mesh, i);
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
        
        /// <summary>
        /// Generates river paths by following water flow from high-flux areas downhill to the sea.
        /// Creates connected path segments that represent river networks based on water flow accumulation.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions and connectivity.</param>
        /// <param name="downhill">Array of downhill flow directions for each vertex.</param>
        /// <param name="h">Array of height values to determine land/water boundaries.</param>
        /// <param name="limit">The minimum flux threshold for creating rivers.</param>
        /// <returns>A list of river paths, where each path is a list of connected points.</returns>
        public List<List<Point>> GetRivers(ref Mesh mesh, ref int[] downhill, ref double[] h, double limit)
        {
            var dh = Downhill(mesh, ref downhill, h);
            var flux = GetFlux(ref mesh, ref downhill, h);
            var links = new List<Point[]>();
            var above = 0;
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
        
        /// <summary>
        /// Smooths a path by applying weighted averaging to interior points.
        /// Helps create more natural-looking curved paths for rivers and other features.
        /// </summary>
        /// <param name="path">The path to smooth, represented as a list of connected points.</param>
        /// <returns>A smoothed version of the input path.</returns>
        private List<Point> RelaxPath(List<Point> path)
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
        
        /// <summary>
        /// Creates a new CityRender object with generated terrain data and initialized city collections.
        /// </summary>
        /// <param name="downhill">Reference to downhill flow directions array.</param>
        /// <param name="mesh">Reference to the mesh data structure.</param>
        /// <param name="heights">Reference to height values array.</param>
        /// <param name="extent">The spatial extent for terrain generation.</param>
        /// <returns>A new CityRender object with initialized properties and empty city list.</returns>
        public CityRender NewCityRender(ref int[] downhill, ref Mesh mesh, ref double[] heights, Extent extent)
        {
            heights = heights != null ? heights : GenerateCoast(ref downhill, ref mesh, 4096, extent);
            return new CityRender()
            {
                AreaProperties = new AreaProperties(),
                Cities = new List<int>()
            };
        }
        
        /// <summary>
        /// Calculates suitability scores for city placement at each vertex based on multiple factors.
        /// Considers water flow, distance from edges, and proximity to existing cities.
        /// </summary>
        /// <param name="cities">Reference to the list of existing city vertex indices.</param>
        /// <param name="downhill">Reference to downhill flow directions array.</param>
        /// <param name="mesh">Reference to the mesh data structure.</param>
        /// <param name="h">Reference to height values array.</param>
        /// <returns>An array of suitability scores for city placement at each vertex.</returns>
        public double[] CityScore(ref List<int> cities, ref int[] downhill, ref Mesh mesh, ref double[] h)
        {
            var score = GetFlux(ref mesh, ref downhill, h).ToList();
            for (var i = 0; i < score.Count; i++)
                score[i] = Math.Sqrt(score[i]);

            for (var i = 0; i < h.Length; i++)
            {
                if (h[i] <= 0 || IsNearEdge(mesh, i))
                {
                    score[i] = -double.MaxValue;
                    continue;
                }
                score[i] += 0.01d / (1e-9d + Math.Abs(mesh.Vxs[i].X) - mesh.Extent.Width / 2d);
                score[i] += 0.01d / (1e-9d + Math.Abs(mesh.Vxs[i].Y) - mesh.Extent.Height / 2d);


                for (var j = 0; j < cities.Count; j++)
                {
                    score[i] -= 0.02d / (Distance(mesh, cities[j], i) + 1e-9d);
                }
            }
            return score.ToArray();
        }
        
        /// <summary>
        /// Calculates the Euclidean distance between two vertices in the mesh.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions.</param>
        /// <param name="i">The index of the first vertex.</param>
        /// <param name="j">The index of the second vertex.</param>
        /// <returns>The distance between the two vertices.</returns>
        private double Distance(Mesh mesh, int i, int j)
        {
            var p = mesh.Vxs[i];
            var q = mesh.Vxs[j];
            return Math.Sqrt((double)((p.X - q.X) * (p.X - q.X) + (p.Y - q.Y) * (p.Y - q.Y)));
        }
        
        /// <summary>
        /// Calculates a penalty score for label placement based on overlap with existing labels and map features.
        /// Used in label optimization to avoid overlapping text and ensure readable map labels.
        /// </summary>
        /// <param name="label">The potential label location to evaluate.</param>
        /// <param name="mesh">The mesh containing spatial boundaries.</param>
        /// <param name="citylabels">List of existing city labels to avoid overlapping.</param>
        /// <param name="cities">List of city locations to avoid covering.</param>
        /// <param name="avoids">Array of path lists (rivers, borders, etc.) to avoid overlapping.</param>
        /// <returns>An integer penalty score, where higher values indicate worse placement.</returns>
        public int Penalty(PossibleLabelLocation label, Mesh mesh, List<PossibleLabelLocation> citylabels, List<int> cities, List<List<Point>>[] avoids)
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
        
        /// <summary>
        /// Calculates the centroid (geometric center) of a territory's land area.
        /// Used for determining optimal placement of territory labels and markers.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions.</param>
        /// <param name="h">Array of height values to distinguish land from water.</param>
        /// <param name="terr">Array of territory assignments for each vertex.</param>
        /// <param name="city">The territory identifier to calculate center for.</param>
        /// <param name="landOnly">Whether to include only land vertices (above sea level) in the calculation.</param>
        /// <returns>A Point representing the centroid of the specified territory.</returns>
        public Point TerrCenter(Mesh mesh, double[] h, double[] terr, int city, bool landOnly)
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
        
        /// <summary>
        /// Generates border paths between different territories by finding edges that cross territory boundaries.
        /// Creates connected line segments that represent political or administrative boundaries.
        /// </summary>
        /// <param name="mesh">The mesh containing edge connectivity information.</param>
        /// <param name="cityRender">The CityRender object containing territory assignments.</param>
        /// <param name="h">Array of height values to exclude underwater areas.</param>
        /// <returns>A list of border paths, where each path is a list of connected points.</returns>
        public List<List<Point>> GetBorders(ref Mesh mesh, ref CityRender cityRender, ref double[] h)
        {
            var terr = cityRender.Territories;
            var edges = new List<Point[]>();
            for (var i = 0; i < mesh.Edges.Count; i++)
            {
                var e = mesh.Edges[i];
                if (e.Right == null) continue;
                if (IsNearEdge(mesh, e.Spot1) || IsNearEdge(mesh, e.Spot2)) continue;
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
        
        /// <summary>
        /// Assigns territories to vertices using a flood-fill algorithm based on distance from cities.
        /// Creates territories by expanding from city centers until all land vertices are assigned.
        /// </summary>
        /// <param name="cityRender">The CityRender object containing city and territory information.</param>
        /// <param name="downhill">Reference to downhill flow directions array.</param>
        /// <param name="mesh">Reference to the mesh data structure.</param>
        /// <param name="h">Reference to height values array.</param>
        /// <returns>An array of territory assignments for each vertex.</returns>
        public double[] GetTerritories(ref CityRender cityRender, ref int[] downhill, ref Mesh mesh, ref double[] h)
        {
            var cities = cityRender.Cities;
            var n = cityRender.AreaProperties.NumberOfTerritories;
            if (n > cities.Count) n = cities.Count;
            var flux = GetFlux(ref mesh, ref downhill, h);
            var territories = new double[h.Length];
            var queue = new SimplePriorityQueue<CityProperty>();
            for (var i = 0; i < n; i++)
            {
                territories[cities[i]] = cities[i];
                var nbs = Neighbours(mesh, cities[i]);
                for (var j = 0; j < nbs.Count; j++)
                {
                    var score = Weight(mesh, h, flux, cities[i], nbs[j]);
                    queue.Enqueue(new CityProperty()
                    {
                        Score = score,
                        City = cities[i],
                        Vx = nbs[j]
                    }, (float)score);
                }
            }
            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                if (territories[u.Vx] != 0) continue;
                territories[u.Vx] = u.City;
                var nbs = Neighbours(mesh, u.Vx);
                for (var i = 0; i < nbs.Count; i++)
                {
                    var v = nbs[i];
                    if (territories[v] != 0) continue;
                    var newdist = Weight(mesh, h, flux, u.Vx, v);
                    var score = u.Score + newdist;
                    queue.Enqueue(new CityProperty()
                    {
                        Score = score,
                        City = u.City,
                        Vx = v
                    }, (float)score);
                }
            }
            return territories;
        }
        
        /// <summary>
        /// Calculates the movement cost between two adjacent vertices for territory expansion.
        /// Considers terrain height differences, water flow, and land/water boundaries.
        /// </summary>
        /// <param name="mesh">The mesh containing vertex positions.</param>
        /// <param name="h">Array of height values.</param>
        /// <param name="flux">Array of water flux values.</param>
        /// <param name="u">The source vertex index.</param>
        /// <param name="v">The destination vertex index.</param>
        /// <returns>A cost value for moving from vertex u to vertex v.</returns>
        private double Weight(Mesh mesh, double[] h, double[] flux, int u, int v)
        {
            var horiz = Distance(mesh, u, v);
            var vert = h[v] - h[u];
            if (vert > 0) vert /= 10;
            var diff = 1f + 0.25f * (double)Math.Pow((double)(vert / horiz), 2);
            diff += 100f * (double)Math.Sqrt((double)flux[u]);
            if (h[u] <= 0) diff = 100;
            if ((h[u] > 0) != (h[v] > 0)) return 1000f;
            return horiz * diff;
        }
        
        /// <summary>
        /// Places a single city at the location with the highest suitability score.
        /// Updates the city list with the new city location.
        /// </summary>
        /// <param name="cities">Reference to the list of existing city vertex indices.</param>
        /// <param name="downhill">Reference to downhill flow directions array.</param>
        /// <param name="mesh">Reference to the mesh data structure.</param>
        /// <param name="h">Reference to height values array.</param>
        public void PlaceCity(ref List<int> cities, ref int[] downhill, ref Mesh mesh, ref double[] h)
        {
            var score = CityScore(ref cities, ref downhill, ref mesh, ref h);
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
            cities.Add(newcity);
        }
        
        /// <summary>
        /// Places multiple cities on the map according to the specified number in area properties.
        /// Iteratively places cities at optimal locations while considering existing city positions.
        /// </summary>
        /// <param name="cityRender">The CityRender object containing city configuration and list.</param>
        /// <param name="downhill">Reference to downhill flow directions array.</param>
        /// <param name="mesh">Reference to the mesh data structure.</param>
        /// <param name="h">Reference to height values array.</param>
        public void PlaceCities(ref CityRender cityRender, ref int[] downhill, ref Mesh mesh, ref double[] h)
        {
            var n = cityRender.AreaProperties.NumberOfCities;
            var cities = cityRender.Cities;
            for (var i = 0; i < n; i++)
            {
                PlaceCity(ref cities, ref downhill, ref mesh, ref h);
            }
            cityRender.Cities = cities;
        }
    }
}
