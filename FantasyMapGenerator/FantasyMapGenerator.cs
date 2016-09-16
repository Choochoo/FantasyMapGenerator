﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using D3Voronoi;
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

            //LayerGrid.Instance.MeshPts = Terrain.GeneratePoints(10, Extent.DefaultExtent);

            var l = new List<Point>()
            {
                new Point(-.33f,-.33f),
                new Point(.33f,-.33f),
                new Point(0,0),
                new Point(-.33f,.33f),
                new Point(.33f,.33f)
            };
            LayerGrid.Instance.MeshPts = l;
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
        }

        private void improvePoints_Click(object sender, EventArgs e)
        {
            var w = Extent.DefaultExtent.Width / 2d;
            var h = Extent.DefaultExtent.Height / 2d;

            //LayerGrid.Instance.MeshPts = Terrain.GeneratePoints(335, Extent.DefaultExtent);
            //LayerGrid.Instance.Voronoi.Construct(LayerGrid.Instance.MeshPts, new Extent(-w, -h, w, h));
            // LayerGrid.Instance.MeshPts = Terrain.ImprovePoints(LayerGrid.Instance, LayerGrid.Instance.MeshPts, Extent.DefaultExtent, 1);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridTesting };

            /*
            var w = Extent.DefaultExtent.width / 2d;
            var h = Extent.DefaultExtent.height / 2d;

            LayerGrid.Instance.MeshPts = Terrain.GeneratePoints(335, Extent.DefaultExtent);
            LayerGrid.Instance.Voronoi.Construct(LayerGrid.Instance.MeshPts, new Extent(-w, -h, w, h));
            //LayerGrid.Instance.MeshPts = Terrain.ImprovePoints(LayerGrid.Instance, LayerGrid.Instance.MeshPts, Extent.DefaultExtent, 1);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridTesting };
            */
        }

        private void showOriginalPoints_Click(object sender, EventArgs e)
        {

            var originalText = "Show Original Points";
            if (((Button)sender).Text == originalText)
            {
                ((Button)sender).Text = "Show Voronoi Corners";

                paintPanel.DrawQueue = new int[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
            }
            else
            {
                ((Button)sender).Text = originalText;
                if (LayerGrid.Instance.MeshVxs == null)
                {
                    LayerGrid.Instance.MeshVxs = Terrain.MakeMesh(LayerGrid.Instance.MeshPts, Extent.DefaultExtent).Vxs;
                }
                paintPanel.DrawQueue = new int[] { (int)DrawPanel.Visualize.LayerGridVoronoiCorners };
            }

        }

        private void reset_Click(object sender, EventArgs e)
        {
            paintPanel.GenerateVoronoi();
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineReset };
        }

        private void randomSlope_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, Terrain.Slope(LayerOutline.Instance.Mesh, Terrain.RandomVector(4)));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSlope };
        }

        private void cone_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, Terrain.Cone(LayerOutline.Instance.Mesh, -0.5f));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineCone };
        }

        private void invertedCone_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, Terrain.Cone(LayerOutline.Instance.Mesh, 0.5f));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineInvertedCone };
        }

        private void fiveBlobs_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Add(LayerOutline.Instance.Heights.Length, LayerOutline.Instance.Heights, Terrain.Mountains(LayerOutline.Instance.Mesh, 5));
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineFiveBlobs };
        }

        private void normalizeHeightmap_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Normalize(LayerOutline.Instance.Heights);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineNormalizeHeightmap };
        }

        private void roundHills_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Peaky(LayerOutline.Instance.Heights);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRoundHills };
        }

        private void relax_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.Relax(LayerOutline.Instance.Mesh, LayerOutline.Instance.Heights);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRelax };
        }

        private void setSeaLevelToMedian_Click(object sender, EventArgs e)
        {
            LayerOutline.Instance.Heights = Terrain.SetSeaLevel(LayerOutline.Instance.Heights, 0.5f);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSetSeaLevelToMedian };
        }

        private void erodeGenerateRandomHeightMap_Click(object sender, EventArgs e)
        {
            LayerErosion.GenerateUneroded();
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionGenerateRandomHeightmap };
        }

        private void erode_Click(object sender, EventArgs e)
        {
            LayerErosion.Instance.Heights = Terrain.DoErosion(LayerErosion.Instance.Mesh, LayerErosion.Instance, LayerErosion.Instance.Heights, 0.1f);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionErode };
        }

        private void erodeSeaLeveltoMedian_Click(object sender, EventArgs e)
        {
            LayerErosion.Instance.Heights = Terrain.SetSeaLevel(LayerErosion.Instance.Heights, 0.5f);
            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionSetSeaLevelToMedian };
        }

        private void cleanCoastlines_Click(object sender, EventArgs e)
        {
            LayerErosion.Instance.Heights = Terrain.CleanCoast(LayerErosion.Instance.Mesh, LayerErosion.Instance.Heights, 1);
            LayerErosion.Instance.Heights = Terrain.FillSinks(LayerErosion.Instance.Mesh, LayerErosion.Instance.Heights);
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
            LayerRendering.Instance.Heights = Terrain.GenerateCoast(LayerRendering.Instance, 4096, Extent.DefaultExtent);
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
            LayerCities.Instance.CityRender = Terrain.NewCityRender(LayerCities.Instance, Extent.DefaultExtent);
            CityDraw();
        }
        private void cityAddNew_Click(object sender, EventArgs e)
        {
            Terrain.PlaceCity(LayerCities.Instance);
            CityDraw();
        }
        public void CityDraw(bool cityViewScore = true)
        {
            LayerCities.Instance.CityRender.Territories = Terrain.GetTerritories(LayerCities.Instance);
            if (cityViewScore)
            {
                LayerCities.Instance.CityRender.Score = Terrain.CityScore(LayerCities.Instance);
                paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerCitiesViewCities };
            }
            else
            {
                paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerCitiesViewTerritories };
            }

        }
        private void showTerritories_Click(object sender, EventArgs e)
        {
            var originalText = "Show Territories";
            if (((Button)sender).Text == originalText)
            {
                ((Button)sender).Text = "Hide Territories";
                CityDraw();
            }
            else
            {
                ((Button)sender).Text = originalText;
                CityDraw(false);
            }
        }
        private void genHighResolutionMap_Click(object sender, EventArgs e)
        {
            DoMap();
        }
        private void DoMap()
        {
            var instance = LayerLabels.Instance;
            var cityRender = instance.CityRender = new CityRender();
            instance.Heights = Terrain.GenerateCoast(instance, 4096, Extent.DefaultExtent);

            Terrain.PlaceCities(instance);

            cityRender.Rivers = Terrain.GetRivers(instance, 0.01d);
            cityRender.Coasts = Terrain.Contour(instance, 0);
            cityRender.Territories = Terrain.GetTerritories(instance);
            cityRender.Borders = Terrain.GetBorders(instance);

            paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerLabelsDoMap };
        }
    }
}