using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using D3Voronoi;
using Language;
using WorldMap.Layers;
using WorldMap.Layers.Interfaces;
using Point = D3Voronoi.Point;
using TerrainGenerator;

namespace WorldMap
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public class DrawPanel : Panel
    {
        private Bitmap _drawnBitmap = new Bitmap(1000, 1000);
        private readonly Color _coastColor = Color.Black;
        private readonly Color _riverColor = Color.DarkBlue;
        private readonly Color _borderColor = Color.Red;
        private readonly Color _slopeColor = Color.Black;


        private readonly int _coastSize = 4;
        private readonly int _riverSize = 2;
        private readonly int _borderSize = 5;

        private PrivateFontCollection fantasyFont;
        private int[] _drawQueue;
        public int[] DrawQueue
        {
            get { return _drawQueue; }
            set
            {
                _drawQueue = value;
                Invalidate();
            }
        }

        private Terrain _terrain;
        public Terrain Terrain
        {
            get
            {
                return _terrain;
            }
        }



        private string[] _voronoiColors;
        public string[] VoronoiColors
        {
            get
            {
                if (_voronoiColors == null) _voronoiColors = ParseColors(viridis);
                return _voronoiColors;
            }
        }
        private string InterpolateViridis(double t)
        {
            int index = (int)(Math.Max(0, Math.Min(VoronoiColors.Length - 1, Math.Floor(t * VoronoiColors.Length))));
            return VoronoiColors[index];
        }
        private string viridis = "44015444025645045745055946075a46085c460a5d460b5e470d60470e6147106347116447136548146748166848176948186a481a6c481b6d481c6e481d6f481f70482071482173482374482475482576482677482878482979472a7a472c7a472d7b472e7c472f7d46307e46327e46337f463480453581453781453882443983443a83443b84433d84433e85423f854240864241864142874144874045884046883f47883f48893e49893e4a893e4c8a3d4d8a3d4e8a3c4f8a3c508b3b518b3b528b3a538b3a548c39558c39568c38588c38598c375a8c375b8d365c8d365d8d355e8d355f8d34608d34618d33628d33638d32648e32658e31668e31678e31688e30698e306a8e2f6b8e2f6c8e2e6d8e2e6e8e2e6f8e2d708e2d718e2c718e2c728e2c738e2b748e2b758e2a768e2a778e2a788e29798e297a8e297b8e287c8e287d8e277e8e277f8e27808e26818e26828e26828e25838e25848e25858e24868e24878e23888e23898e238a8d228b8d228c8d228d8d218e8d218f8d21908d21918c20928c20928c20938c1f948c1f958b1f968b1f978b1f988b1f998a1f9a8a1e9b8a1e9c891e9d891f9e891f9f881fa0881fa1881fa1871fa28720a38620a48621a58521a68522a78522a88423a98324aa8325ab8225ac8226ad8127ad8128ae8029af7f2ab07f2cb17e2db27d2eb37c2fb47c31b57b32b67a34b67935b77937b87838b9773aba763bbb753dbc743fbc7340bd7242be7144bf7046c06f48c16e4ac16d4cc26c4ec36b50c46a52c56954c56856c66758c7655ac8645cc8635ec96260ca6063cb5f65cb5e67cc5c69cd5b6ccd5a6ece5870cf5773d05675d05477d1537ad1517cd2507fd34e81d34d84d44b86d54989d5488bd6468ed64590d74393d74195d84098d83e9bd93c9dd93ba0da39a2da37a5db36a8db34aadc32addc30b0dd2fb2dd2db5de2bb8de29bade28bddf26c0df25c2df23c5e021c8e020cae11fcde11dd0e11cd2e21bd5e21ad8e219dae319dde318dfe318e2e418e5e419e7e419eae51aece51befe51cf1e51df4e61ef6e620f8e621fbe723fde725";
        private string[] ParseColors(string colors)
        {
            var output = SpliceText(colors, 6);

            return output.Select(o => "#" + o).ToArray();
        }
        public static string[] SpliceText(string text, int lineLength)
        {
            return Regex.Matches(text, ".{1," + lineLength + "}").Cast<Match>().Select(m => m.Value).ToArray();
        }
        public DrawPanel()
        {
            this.DoubleBuffered = true;

        }

        public void Load()
        {
            byte[] myFont = Properties.Resources.RINGM;
            using (var ms = new MemoryStream(myFont))
            {
                fantasyFont = new PrivateFontCollection();
                var fontBytes = Properties.Resources.RINGM;
                var fontData = Marshal.AllocCoTaskMem(fontBytes.Length);
                Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length);
                fantasyFont.AddMemoryFont(fontData, fontBytes.Length);
                Marshal.FreeCoTaskMem(fontData);
            }
        }
        public void CreateTerrain(int seed)
        {
            _terrain = new Terrain(seed);
        }

        public enum Visualize
        {
            //Step 1
            LayerGridGenerateRandomPoints = 1,
            LayerGridImprovePoints = 2,
            LayerGridVoronoiCorners = 3,

            //Step 2
            LayerOutlineReset = 10,
            LayerOutlineSlope = 11,
            LayerOutlineCone = 12,
            LayerOutlineInvertedCone = 13,
            LayerOutlineFiveBlobs = 14,
            LayerOutlineNormalizeHeightmap = 15,
            LayerOutlineRoundHills = 16,
            LayerOutlineRelax = 17,
            LayerOutlineSetSeaLevelToMedian = 18,

            //Step 3
            LayerErosionGenerateRandomHeightmap = 20,
            LayerErosionErode = 21,
            LayerErosionSetSeaLevelToMedian = 22,
            LayerErosionCleanCoastlines = 23,
            LayerErosionShowErosionRate = 24,
            LayerErosionHideErosionRate = 25,

            //Step 4
            LayerRenderingGenerateRandomHeightmap = 30,
            LayerRenderingShowCoastline = 31,
            LayerRenderingShowRivers = 32,
            LayerRenderingShowSlopeShading = 33,

            //Step 5
            LayerCitiesViewCities = 40,
            LayerCitiesViewTerritories = 41,

            //Step 6
            LayerLabelsDoMap = 50
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            if (DrawQueue == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (Graphics g = Graphics.FromImage(_drawnBitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;




                foreach (var drawNum in DrawQueue)
                {
                    Mesh mesh;
                    double[] heights;
                    int[] downhill;
                    CityRender cityRender;
                    switch (drawNum)
                    {
                        //Step 1
                        case (int)Visualize.LayerGridGenerateRandomPoints:
                            VisualizePoints(g, LayerGrid.Instance.MeshPts);
                            break;
                        case (int)Visualize.LayerGridImprovePoints:
                            VisualizePoints(g, LayerGrid.Instance.MeshPts);
                            break;
                            break;
                        case (int)Visualize.LayerGridVoronoiCorners:
                            VisualizePoints(g, LayerGrid.Instance.MeshVxs);
                            break;
                        //Step 2
                        case (int)Visualize.LayerOutlineReset:
                            VisualizeVoronoi(g, LayerOutline.Instance.Mesh, LayerOutline.Instance.Heights, -1f, 1f);
                            break;
                        case (int)Visualize.LayerOutlineSlope:
                        case (int)Visualize.LayerOutlineCone:
                        case (int)Visualize.LayerOutlineInvertedCone:
                        case (int)Visualize.LayerOutlineFiveBlobs:
                        case (int)Visualize.LayerOutlineNormalizeHeightmap:
                        case (int)Visualize.LayerOutlineRoundHills:
                        case (int)Visualize.LayerOutlineRelax:
                        case (int)Visualize.LayerOutlineSetSeaLevelToMedian:
                            VisualizeVoronoi(g, LayerOutline.Instance.Mesh, LayerOutline.Instance.Heights, -1f, 1f);
                            mesh = LayerOutline.Instance.Mesh;
                            heights = LayerOutline.Instance.Heights;
                            VisualizePaths(g, Terrain.Contour(ref mesh, ref heights), _coastColor, _coastSize);
                            break;
                        //Step 3
                        case (int)Visualize.LayerErosionGenerateRandomHeightmap:
                        case (int)Visualize.LayerErosionErode:
                        case (int)Visualize.LayerErosionSetSeaLevelToMedian:
                        case (int)Visualize.LayerErosionCleanCoastlines:
                        case (int)Visualize.LayerErosionHideErosionRate:
                            VisualizeVoronoi(g, LayerErosion.Instance.Mesh, LayerErosion.Instance.Heights, 0f, 1f);
                            mesh = LayerErosion.Instance.Mesh;
                            heights = LayerErosion.Instance.Heights;
                            VisualizePaths(g, Terrain.Contour(ref mesh, ref heights), _coastColor, _coastSize);
                            break;
                        case (int)Visualize.LayerErosionShowErosionRate:
                            mesh = LayerErosion.Instance.Mesh;
                            heights = LayerErosion.Instance.Heights;
                            downhill = LayerErosion.Instance.Downhill;
                            VisualizeVoronoi(g, LayerErosion.Instance.Mesh, Terrain.ErosionRate(mesh, ref downhill, heights));
                            VisualizePaths(g, Terrain.Contour(ref mesh, ref heights), _coastColor, _coastSize);
                            break;
                        //Step 4
                        case (int)Visualize.LayerRenderingGenerateRandomHeightmap:
                            VisualizeVoronoi(g, LayerRendering.Instance.Mesh, LayerRendering.Instance.Heights, 0);
                            break;
                        case (int)Visualize.LayerRenderingShowCoastline:
                            mesh = LayerRendering.Instance.Mesh;
                            heights = LayerRendering.Instance.Heights;
                            VisualizePaths(g, Terrain.Contour(ref mesh, ref heights), _coastColor, _coastSize);
                            break;
                        case (int)Visualize.LayerRenderingShowRivers:
                            mesh = LayerRendering.Instance.Mesh;
                            heights = LayerRendering.Instance.Heights;
                            downhill = LayerRendering.Instance.Downhill;
                            VisualizePaths(g, Terrain.GetRivers(ref mesh, ref downhill, ref heights, 0.01d), _riverColor, _riverSize);
                            break;
                        case (int)Visualize.LayerRenderingShowSlopeShading:
                            VisualizeSlopes(g, LayerRendering.Instance.Mesh, LayerRendering.Instance.Heights);
                            break;
                        //Step 5
                        case (int)Visualize.LayerCitiesViewTerritories:
                        case (int)Visualize.LayerCitiesViewCities:
                            var instance = LayerCities.Instance;
                            if ((int)Visualize.LayerCitiesViewCities == drawNum)
                            {
                                var score = LayerCities.Instance.CityRender.Score;
                                VisualizeVoronoi(g, instance.Mesh, score, score.Max() - 0.5d);
                            }
                            else
                            {
                                VisualizeVoronoi(g, instance.Mesh, instance.CityRender.Territories);
                            }
                            mesh = instance.Mesh;
                            heights = instance.Heights;
                            downhill = instance.Downhill;
                            cityRender = instance.CityRender;

                            VisualizePaths(g, Terrain.Contour(ref mesh, ref heights), _coastColor, _coastSize);
                            VisualizePaths(g, Terrain.GetRivers(ref mesh, ref downhill, ref heights, 0.01d), _riverColor, _riverSize);
                            VisualizePaths(g, Terrain.GetBorders(ref mesh, ref cityRender, ref heights), _borderColor, _borderSize);
                            VisualizeSlopes(g, instance.Mesh, instance.Heights);
                            VisualizeCities(g, instance.CityRender, instance.Mesh);
                            break;
                        case (int)Visualize.LayerLabelsDoMap:
                            VisualizePaths(g, LayerLabels.Instance.CityRender.Rivers, Color.Black, _riverSize);
                            VisualizePaths(g, LayerLabels.Instance.CityRender.Coasts, _coastColor, _coastSize);
                            VisualizePaths(g, LayerLabels.Instance.CityRender.Borders, Color.Black, _borderSize, true);

                            VisualizeSlopes(g, LayerLabels.Instance.Mesh, LayerLabels.Instance.Heights);
                            VisualizeCities(g, LayerLabels.Instance.CityRender, LayerLabels.Instance.Mesh);

                            VisualizeLabels(g, LayerLabels.Instance);
                            break;
                    }
                }
                e.Graphics.DrawImage(_drawnBitmap, new Rectangle(0, 0, this.Width, this.Height), 0, 0, _drawnBitmap.Width, _drawnBitmap.Height, GraphicsUnit.Pixel);
            }
        }

        private void VisualizePoints(Graphics g, Point[] points)
        {
            var offsetHeight = (_drawnBitmap.Height) / 2;
            var offsetWidth = (_drawnBitmap.Width) / 2;
            var radius = (float)(100d / Math.Sqrt(points.Length));
            var halfradius = radius / 2f;
            foreach (var point in points)
            {
                g.FillEllipse(Brushes.Blue, (float)(point.X * _drawnBitmap.Width + offsetWidth - halfradius), (float)(point.Y * _drawnBitmap.Height + offsetHeight - halfradius), radius, radius);
            }
        }

        private void VisualizeCities(Graphics g, CityRender cityRender, Mesh mesh)
        {
            var cities = cityRender.Cities;
            var n = cityRender.AreaProperties.NumberOfTerritories;

            var multiplier = _drawnBitmap.Width;
            var offsetHeight = _drawnBitmap.Height / 2;
            var offsetWidth = _drawnBitmap.Width / 2;
            using (var brush = new SolidBrush(Color.White))
            {
                using (var pen = new Pen(Color.Black, 5))
                {
                    for (int i = 0; i < cities.Count; i++)
                    {
                        var city = cities[i];
                        var vxs = mesh.Vxs[city];
                        var r = i >= n ? 10 : 25;
                        g.FillEllipse(brush, (float)((vxs.X * multiplier) + offsetWidth), (float)((vxs.Y * multiplier) + offsetHeight), r, r);
                        g.DrawEllipse(pen, (float)((vxs.X * multiplier) + offsetWidth), (float)((vxs.Y * multiplier) + offsetHeight), r, r);
                    }
                }
            }
        }

        private void VisualizeSlopes(Graphics g, Mesh mesh, double[] h)
        {
            var strokes = new List<List<Point>>();
            var r = 0.25f / (double)Math.Sqrt(h.Length);
            for (var i = 0; i < h.Length; i++)
            {
                if (h[i] <= 0 || Terrain.IsNearEdge(mesh, i)) continue;
                var nbs = Terrain.Neighbours(mesh, i);
                nbs.Add(i);
                var s = 0d;
                var s2 = 0d;
                for (var j = 0; j < nbs.Count; j++)
                {
                    var slopes = Terrain.Trislope(mesh, h, nbs[j]);
                    s += slopes.X / 10d;
                    s2 += slopes.Y;
                }
                s /= nbs.Count;
                s2 /= nbs.Count;
                if (Math.Abs(s) < Terrain.Runif(0.1f, 0.4f)) continue;
                var l = r * Terrain.Runif(1f, 2f) * (1f - 0.2f * (double)Math.Pow(Math.Atan((double)s), 2.0f)) * (double)Math.Exp((double)(s2 / 100));
                var x = mesh.Vxs[i].X;
                var y = mesh.Vxs[i].Y;
                if (Math.Abs(l * s) > 2 * r)
                {
                    var n = (double)Math.Floor(Math.Abs(l * s / r));
                    l /= n;
                    if (n > 4) n = 4;
                    for (var j = 0; j < n; j++)
                    {
                        var rnorm1 = 0.0d;
                        var rnorm2 = 0.0d;
                        Terrain.RNorm(out rnorm1, out rnorm2);
                        var u = rnorm1 * r;
                        var v = rnorm2 * r;
                        strokes.Add(new List<Point>()
                    {
                        new Point(x + u - l, y + v + l*s),
                        new Point(x + u + l, y + v - l*s)
                    });

                    }

                }
                else
                {
                    strokes.Add(new List<Point>()
                    {
                        new Point(x - l, y + l * s),
                        new Point(x + l, y - l * s)
                    });
                }
            }

            var multiplier = _drawnBitmap.Width;
            var offsetHeight = _drawnBitmap.Height / 2;
            var offsetWidth = _drawnBitmap.Width / 2;
            using (var pen = new Pen(_slopeColor))
            {
                var graphicsPath = new GraphicsPath();
                foreach (var stroke in strokes)
                {
                    for (int index = 0; index < stroke.Count - 1; index++)
                    {
                        var lineGraphic = new GraphicsPath();
                        var end = index + 1 >= stroke.Count ? 0 : index + 1;
                        var startpoint = stroke[index];
                        var endpoint = stroke[end];
                        lineGraphic.AddLine((float)((startpoint.X * multiplier) + offsetWidth), (float)((startpoint.Y * multiplier) + offsetHeight), (float)((endpoint.X * multiplier) + offsetWidth), (float)((endpoint.Y * multiplier) + offsetHeight));
                        graphicsPath.AddPath(lineGraphic, false);
                    }
                }
                g.DrawPath(pen, graphicsPath);
                graphicsPath.Dispose();
            }

        }

        private void VisualizePaths(Graphics g, List<List<Point>> paths, Color color, int size, bool isBorder = false)
        {
            if (paths == null)
                return;

            var pen = new Pen(color, size);
            if (isBorder)
            {
                pen.DashCap = DashCap.Flat;
                pen.DashStyle = DashStyle.Dot;
                //pen.DashPattern = new[] { 10f, 10f };
            }
            var graphicsPath = new GraphicsPath();
            var multiplier = _drawnBitmap.Width;
            var offsetHeight = _drawnBitmap.Height / 2;
            var offsetWidth = _drawnBitmap.Width / 2;

            foreach (var path in paths)
            {

                for (int index = 0; index < path.Count - 1; index++)
                {
                    var lineGraphic = new GraphicsPath();
                    var end = index + 1 >= path.Count ? 0 : index + 1;
                    var startpoint = path[index];
                    var endpoint = path[end];
                    lineGraphic.AddLine((float)((startpoint.X * multiplier) + offsetWidth), (float)((startpoint.Y * multiplier) + offsetHeight), (float)((endpoint.X * multiplier) + offsetWidth), (float)((endpoint.Y * multiplier) + offsetHeight));
                    graphicsPath.AddPath(lineGraphic, false);
                }
            }

            g.DrawPath(pen, graphicsPath);
            pen.Dispose();
            graphicsPath.Dispose();
        }

        private void VisualizeVoronoi(Graphics g, Mesh mesh, double[] heights, double lo = double.MaxValue, double hi = double.MinValue)
        {
            if (hi == double.MinValue)
            {
                hi = heights.Max() + 1e-9f;
            }
            if (lo == double.MaxValue)
            {
                lo = heights.Min() - 1e-9f;
            }
            var graphicsPath = new GraphicsPath();
            double[] mappedVals = null;
            if (heights != null)
                mappedVals = heights.Select(x => x > hi ? 1 : x < lo ? 0 : (x - lo) / (hi - lo)).ToArray();

            var multiplier = _drawnBitmap.Width;
            var offsetHeight = _drawnBitmap.Height / 2;
            var offsetWidth = _drawnBitmap.Width / 2;
            for (int i = 0; i < mesh.Tris.Count; i++)
            {
                using (var pen = new Pen(Color.Aqua))
                {
                    using (var brush = new SolidBrush(Color.DarkBlue))
                    {
                        var a = mesh.Tris.ElementAt(i).Value.Select(s => new PointF((float)s.X, (float)s.Y)).ToList();
                        if (a.Count < 3)
                            continue;
                        var newPath = new GraphicsPath();

                        newPath.AddLine((a[0].X * multiplier) + offsetWidth, (a[0].Y * multiplier) + offsetHeight, (a[1].X * multiplier) + offsetWidth, (a[1].Y * multiplier) + offsetHeight);
                        newPath.AddLine((a[1].X * multiplier) + offsetWidth, (a[1].Y * multiplier) + offsetHeight, (a[2].X * multiplier) + offsetWidth, (a[2].Y * multiplier) + offsetHeight);
                        newPath.AddLine((a[2].X * multiplier) + offsetWidth, (a[2].Y * multiplier) + offsetHeight, (a[0].X * multiplier) + offsetWidth, (a[0].Y * multiplier) + offsetHeight);
                        newPath.CloseFigure();

                        graphicsPath.AddPath(newPath, true);
                        var heightVal = 0d;
                        if (mappedVals != null)
                            heightVal = mappedVals[i];

                        var hexColor = InterpolateViridis(heightVal);
                        brush.Color = ColorTranslator.FromHtml(hexColor);
                        pen.Color = ControlPaint.Light(ColorTranslator.FromHtml(hexColor), .3f);

                        g.FillPath(brush, newPath);
                        g.DrawPath(pen, newPath);
                    }
                }

            }

            graphicsPath.Dispose();
        }

        public void GenerateVoronoi()
        {
            LayerOutline.Instance.Heights = null;
            LayerOutline.Instance.Mesh = Terrain.GenerateGoodMesh(4096, Extent.DefaultExtent);
        }

        private void VisualizeLabels<T>(Graphics g, T instance) where T : IHasCityRender, IHasMesh, IHasHeights
        {
            var finalCityRender = instance.CityRender;
            var mesh = instance.Mesh;
            var h = instance.Heights;
            var terr = finalCityRender.Territories;
            var cities = finalCityRender.Cities;
            var nterrs = finalCityRender.AreaProperties.NumberOfTerritories;
            var avoids = new[] { finalCityRender.Rivers, finalCityRender.Coasts, finalCityRender.Borders };
            var lang = LanguageGenerator.MakeRandomLanguage();
            var citylabels = new List<PossibleLabelLocation>();


            for (var i = 0; i < cities.Count; i++)
            {
                var x = mesh.Vxs[cities[i]].X;
                var y = mesh.Vxs[cities[i]].Y;
                var text = LanguageGenerator.MakeName(lang, "city");
                var size = i < nterrs
                    ? finalCityRender.AreaProperties.FontSizeCity
                    : finalCityRender.AreaProperties.FontSizeTown;
                var sx = 0.65d * (float)size / _drawnBitmap.Width * (float)text.Length;
                var sy = size / (float)_drawnBitmap.Height;
                var possibleLabels = new[]
                {
                    new PossibleLabelLocation()
                    {
                        X = x + 1.1*sy,
                        Y = y - 0.23*sy,
                        Align = PossibleLabelLocation.AlignLeft,
                        X0 = x + 0.7d*sy,
                        Y0 = y - 0.3d*sy,
                        X1 = x + 0.7d*sy + sx,
                        Y1 = y + 0.6d*sy,
                        DebugIndex = 0
                    },
                    new PossibleLabelLocation()
                    {
                        X = x + 1.1d*sy,
                        Y = y - 0.23*sy,
                        Align = PossibleLabelLocation.AlignRight,
                        X0 = x - 0.9d*sy - sx,
                        Y0 = y - 0.7d*sy,
                        X1 = x - 0.9d*sy,
                        Y1 = y + 0.7d*sy,
                        DebugIndex = 1
                    },
                    new PossibleLabelLocation()
                    {
                        X = x,
                        Y = y - 0.8d *sy,
                        Align = PossibleLabelLocation.AlignCenter,
                        X0 = x - sx/2,
                        Y0 = y - 1.9d*sy,
                        X1 = x + sx/2,
                        Y1 = y - 0.7d*sy,
                        DebugIndex = 2
                    },
                    new PossibleLabelLocation()
                    {
                        X = x,
                        Y = y+ 1.2*sy,
                        Align = PossibleLabelLocation.AlignCenter,
                        X0 = x - sx/2,
                        Y0 = y + 0.1d*sy,
                        X1 = x + sx/2,
                        Y1 = y + 1.3d*sy,
                        DebugIndex = 3
                    }
                };

                var lowestPenalty = int.MaxValue;
                var lowestIndex = int.MaxValue;
                for (int j = 0; j < possibleLabels.Length; j++)
                {
                    var penalty = Terrain.Penalty(possibleLabels[j], mesh, citylabels, cities, avoids);
                    if (penalty < lowestPenalty)
                    {
                        lowestIndex = j;
                        lowestPenalty = penalty;
                    }
                }
                var label = possibleLabels[lowestIndex];
                label.Text = text;
                label.Size = (float)size / _drawnBitmap.Width;
                citylabels.Add(label);
            }
            DrawText(g, citylabels);

            var reglabels = new List<PossibleLabelLocation>();
            for (var i = 0; i < nterrs; i++)
            {
                var city = cities[i];
                var text = LanguageGenerator.MakeName(lang, "region");
                var sy = finalCityRender.AreaProperties.FontSizeRegion / 1000d;
                var sx = 0.6d * text.Length * sy;
                var lc = Terrain.TerrCenter(mesh, h, terr, city, true);
                var oc = Terrain.TerrCenter(mesh, h, terr, city, false);
                var best = 0;
                var bestscore = -double.MaxValue;
                for (var j = 0; j < h.Length; j++)
                {
                    var score = 0d;
                    var v = mesh.Vxs[j];
                    score -= 3000d * Math.Sqrt((v.X - lc.X) * (v.X - lc.X) + (v.Y - lc.Y) * (v.Y - lc.Y));
                    score -= 1000d * Math.Sqrt((v.X - oc.X) * (v.X - oc.X) + (v.Y - oc.Y) * (v.Y - oc.Y));
                    if (terr[j] != city) score -= 3000;
                    for (var k = 0; k < cities.Count; k++)
                    {
                        var u = mesh.Vxs[cities[k]];
                        if (Math.Abs(v.X - u.X) < sx &&
                            Math.Abs(v.X - sy / 2 - u.Y) < sy)
                        {
                            score -= k < nterrs ? 4000 : 500;
                        }
                        if (v.X - sx / 2 < citylabels[k].X1 &&
                            v.X + sx / 2 > citylabels[k].X0 &&
                            v.Y - sy < citylabels[k].Y1 &&
                            v.Y > citylabels[k].Y0)
                        {
                            score -= 5000;
                        }
                    }
                    for (var k = 0; k < reglabels.Count; k++)
                    {
                        var label = reglabels[k];
                        if (v.X - sx / 2 < label.X + label.Width / 2d &&
                            v.X + sx / 2 > label.X - label.Width / 2d &&
                            v.Y - sy < label.Y &&

                            v.Y > label.Y - label.Size)
                        {
                            score -= 20000;
                        }
                    }
                    if (h[j] <= 0) score -= 500;
                    if (v.X + sx / 2 > 0.5 * mesh.Extent.Width) score -= 50000;
                    if (v.X - sx / 2 < -0.5 * mesh.Extent.Width) score -= 50000;
                    if (v.Y > 0.5 * mesh.Extent.Height) score -= 50000;
                    if (v.Y - sy < -0.5 * mesh.Extent.Height) score -= 50000;
                    if (score > bestscore)
                    {
                        bestscore = score;
                        best = j;
                    }
                }
                reglabels.Add(new PossibleLabelLocation()
                {
                    Text = text,
                    X = mesh.Vxs[best].X,
                    Y = mesh.Vxs[best].Y,
                    Size = sy,
                    Width = (int)sx
                });
            }
            //DrawText(g, reglabels);
        }

        private void DrawText(Graphics g, List<PossibleLabelLocation> labels)
        {
            var multiplier = _drawnBitmap.Width;
            var offsetHeight = _drawnBitmap.Height / 2;
            var offsetWidth = _drawnBitmap.Width / 2;
            var textPath = new GraphicsPath();


            foreach (var label in labels)
            {
                var sf = new StringFormat();
                switch (label.Align)
                {
                    case PossibleLabelLocation.AlignRight:
                        sf.Alignment = StringAlignment.Far;
                        break;
                    case PossibleLabelLocation.AlignCenter:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case PossibleLabelLocation.AlignLeft:
                        sf.Alignment = StringAlignment.Near;
                        break;
                }
                var textPosition = new PointF((float)((label.X * multiplier) + offsetWidth), (float)((label.Y * multiplier) + offsetHeight));
                textPath.AddString(label.Text, fantasyFont.Families[0], (int)FontStyle.Regular, (int)(label.Size * multiplier), textPosition, sf);
            }

            var textBrush = new SolidBrush(Color.Black);

            var textOutline = new Pen(Color.White, 5);
            var textOutlineFill = new SolidBrush(Color.White);


            g.DrawPath(textOutline, textPath);
            g.FillPath(textOutlineFill, textPath);

            g.FillPath(textBrush, textPath);

            textBrush.Dispose();
            textOutline.Dispose();
            textOutlineFill.Dispose();
            textPath.Dispose();
        }
    }
}
