using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using D3Voronoi;
using TerrainGenerator;
using WorldMap.Layers;

namespace WorldMap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void generateRandomPoints_Click(object sender, EventArgs e)
        {
            LayerGrid.Instance.MeshPts = paintPanel.Terrain.GeneratePoints(10, Extent.DefaultExtent);
            improvePoints.Enabled = showOriginalPoints.Enabled = true;
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
        }

        private void improvePoints_Click(object sender, EventArgs e)
        {
            LayerGrid.Instance.MeshPts = paintPanel.Terrain.ImprovePoints(LayerGrid.Instance.MeshPts, Extent.DefaultExtent, 1);
            LayerGrid.Instance.MeshVxs = null;
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
        }

        private void showOriginalPoints_Click(object sender, EventArgs e)
        {

            var originalText = "Show Original Points";
            if (((Button)sender).Text == originalText)
            {
                ((Button)sender).Text = "Show Voronoi Corners";

                if (LayerGrid.Instance.MeshVxs == null)
                {
                    LayerGrid.Instance.MeshVxs = paintPanel.Terrain.MakeMesh(LayerGrid.Instance.MeshPts, Extent.DefaultExtent).Vxs;
                }
                paintPanel.DrawQueue = new int[] { (int)DrawPanel.Visualize.LayerGridVoronoiCorners };
            }
            else
            {
                ((Button)sender).Text = originalText;
                paintPanel.DrawQueue = new int[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
            }

        }

        private void reset_Click(object sender, EventArgs e)
        {
            paintPanel.GenerateVoronoi();
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineReset };
        }

        private void randomSlope_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, paintPanel.Terrain.Slope(LayerOutline.Instance.Mesh, paintPanel.Terrain.RandomVector(4)));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSlope };
        }

        private void cone_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, paintPanel.Terrain.Cone(LayerOutline.Instance.Mesh, -0.5f));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineCone };
        }

        private void invertedCone_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, paintPanel.Terrain.Cone(LayerOutline.Instance.Mesh, 0.5f));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineInvertedCone };
        }

        private void fiveBlobs_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, paintPanel.Terrain.Mountains(LayerOutline.Instance.Mesh, 5));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineFiveBlobs };
        }

        private void normalizeHeightmap_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.Normalize(LayerOutline.Instance.Heights);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineNormalizeHeightmap };
        }

        private void roundHills_Click(object sender, EventArgs e)
        {
            var heights = LayerOutline.Instance.Heights;
            LayerOutline.Instance.Heights = paintPanel.Terrain.Peaky(heights);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRoundHills };
        }

        private void relax_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.Relax(LayerOutline.Instance.Mesh, LayerOutline.Instance.Heights);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRelax };
        }

        private void setSeaLevelToMedian_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = paintPanel.Terrain.SetSeaLevel(LayerOutline.Instance.Heights, 0.5f);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSetSeaLevelToMedian };
        }

        private void erodeGenerateRandomHeightMap_Click(object sender, EventArgs e)
        {
            var mesh = LayerErosion.Instance.Mesh;
            var heights = LayerErosion.Instance.Heights;

            paintPanel.Terrain.GenerateUneroded(ref mesh, ref heights);

            LayerErosion.Instance.Mesh = mesh;
            LayerErosion.Instance.Heights = heights;

            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionGenerateRandomHeightmap };
        }

        private void erode_Click(object sender, EventArgs e)
        {
            var mesh = LayerErosion.Instance.Mesh;
            var heights = LayerErosion.Instance.Heights;
            var downhill = LayerErosion.Instance.Downhill;

            heights = paintPanel.Terrain.DoErosion(ref mesh, ref downhill, heights, 0.1f);

            LayerErosion.Instance.Mesh = mesh;
            LayerErosion.Instance.Heights = heights;
            LayerErosion.Instance.Downhill = downhill;

            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionErode };
        }

        private void erodeSeaLeveltoMedian_Click(object sender, EventArgs e)
        {
            LayerErosion.Instance.Heights = paintPanel.Terrain.SetSeaLevel(LayerErosion.Instance.Heights, 0.5f);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionSetSeaLevelToMedian };
        }

        private void cleanCoastlines_Click(object sender, EventArgs e)
        {
            var mesh = LayerErosion.Instance.Mesh;
            LayerErosion.Instance.Heights = paintPanel.Terrain.CleanCoast(mesh, LayerErosion.Instance.Heights, 1);
            LayerErosion.Instance.Heights = paintPanel.Terrain.FillSinks(ref mesh, LayerErosion.Instance.Heights);
            LayerErosion.Instance.Mesh = mesh;

            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionCleanCoastlines };
        }

        private void showErosionRate_Click(object sender, EventArgs e)
        {
            var originalText = "Show Erosion Rate";
            if (((Button)sender).Text == originalText)
            {
                ((Button)sender).Text = "Show Heightmap";
                paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionShowErosionRate };
            }
            else
            {
                ((Button)sender).Text = originalText;
                paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionHideErosionRate };
            }
        }

        private void physRender(Button sender, string showText, string hideText, int drawCall)
        {

            var alteredDrawQueue = paintPanel.DrawQueue.ToList();
            alteredDrawQueue.Remove(drawCall);
            if (sender.Text == showText)
            {
                sender.Text = hideText;
                alteredDrawQueue.Add(drawCall);
            }
            else
            {
                sender.Text = showText;
            }
            paintPanel.DrawQueue = alteredDrawQueue.OrderBy(o => o).ToArray();

        }

        private void renderingGenerateRandomHeightmap_Click(object sender, EventArgs e)
        {
            var instance = LayerRendering.Instance;
            var downhill = instance.Downhill;
            var mesh = instance.Mesh;

            LayerRendering.Instance.Heights = paintPanel.Terrain.GenerateCoast(ref downhill, ref mesh, 4096, Extent.DefaultExtent);

            instance.Downhill = downhill;
            instance.Mesh = mesh;

            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerRenderingGenerateRandomHeightmap };
        }
        private void renderingShowCoastline_Click(object sender, EventArgs e)
        {
            physRender((Button)sender, "Show Coastline", "Hide Coastline", (int)DrawPanel.Visualize.LayerRenderingShowCoastline);
        }
        private void renderingShowRivers_Click(object sender, EventArgs e)
        {
            physRender((Button)sender, "Show Rivers", "Hide Rivers", (int)DrawPanel.Visualize.LayerRenderingShowRivers);
        }
        private void renderingShowSlopeShading_Click(object sender, EventArgs e)
        {
            physRender((Button)sender, "Show Slope Shading", "Hide Slope Shading", (int)DrawPanel.Visualize.LayerRenderingShowSlopeShading);
        }
        private void renderingHideHeightmap_Click(object sender, EventArgs e)
        {
            physRender((Button)sender, "Show Heightmap", "Hide Heightmap", (int)DrawPanel.Visualize.LayerRenderingGenerateRandomHeightmap);
        }
        private void citiesGenerateRandomHeightmap_Click(object sender, EventArgs e)
        {
            var instance = LayerCities.Instance;
            var downhill = instance.Downhill;
            var mesh = instance.Mesh;
            var heights = instance.Heights;

            instance.CityRender = paintPanel.Terrain.NewCityRender(ref downhill, ref mesh, ref heights, Extent.DefaultExtent);

            instance.Downhill = downhill;
            instance.Mesh = mesh;
            instance.Heights = heights;

            CityDraw();
        }
        private void cityAddNew_Click(object sender, EventArgs e)
        {
            var instance = LayerCities.Instance;
            var cityRender = instance.CityRender;
            var downhill = instance.Downhill;
            var mesh = instance.Mesh;
            var heights = instance.Heights;
            var cities = cityRender.Cities;

            paintPanel.Terrain.PlaceCity(ref cities, ref downhill, ref mesh, ref heights);
            cityRender.Cities = cities;
            instance.CityRender = cityRender;
            instance.Downhill = downhill;
            instance.Mesh = mesh;
            instance.Heights = heights;

            CityDraw();
        }
        public void CityDraw(bool cityViewScore = true)
        {
            var instance = LayerCities.Instance;
            var cityRender = instance.CityRender;
            var downhill = instance.Downhill;
            var mesh = instance.Mesh;
            var heights = instance.Heights;

            cityRender.Territories = paintPanel.Terrain.GetTerritories(ref cityRender, ref downhill, ref mesh, ref heights);

            instance.CityRender = cityRender;

            var cities = cityRender.Cities;
            if (cityViewScore)
                LayerCities.Instance.CityRender.Score = paintPanel.Terrain.CityScore(ref cities, ref downhill, ref mesh, ref heights);

            instance.CityRender.Cities = cities;
            instance.Downhill = downhill;
            instance.Mesh = mesh;
            instance.Heights = heights;

            paintPanel.DrawQueue = cityViewScore ? new[] { (int)DrawPanel.Visualize.LayerCitiesViewCities } : new[] { (int)DrawPanel.Visualize.LayerCitiesViewTerritories };

        }
        private void showTerritories_Click(object sender, EventArgs e)
        {
            var originalText = "Show Territories";
            if (((Button)sender).Text == originalText)
            {
                ((Button)sender).Text = "Hide Territories";
                CityDraw(false);
            }
            else
            {
                ((Button)sender).Text = originalText;
                CityDraw();
            }
        }
        private void genHighResolutionMap_Click(object sender, EventArgs e)
        {
            DoMap();
        }
        private void DoMap()
        {
            var instance = LayerLabels.Instance;
            instance.Reset();
            var cityRender = instance.CityRender = new CityRender();
            var downhill = instance.Downhill;
            var mesh = instance.Mesh;
            var heights = paintPanel.Terrain.GenerateCoast(ref downhill, ref mesh, cityRender.AreaProperties.NumberOfPoints, Extent.DefaultExtent);

            paintPanel.Terrain.PlaceCities(ref cityRender, ref downhill, ref mesh, ref heights);

            cityRender.Rivers = paintPanel.Terrain.GetRivers(ref mesh, ref downhill, ref heights, 0.01d);
            cityRender.Coasts = paintPanel.Terrain.Contour(ref mesh, ref heights, 0);
            cityRender.Territories = paintPanel.Terrain.GetTerritories(ref cityRender, ref downhill, ref mesh, ref heights);
            cityRender.Borders = paintPanel.Terrain.GetBorders(ref mesh, ref cityRender, ref heights);

            instance.CityRender = cityRender;
            instance.Downhill = downhill;
            instance.Mesh = mesh;
            instance.Heights = heights;

            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerLabelsDoMap };
        }
    }
}
