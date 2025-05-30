using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using D3Voronoi;
using TerrainGenerator;
using WorldMap.Layers;

namespace WorldMap
{
    /// <summary>
    /// Main form for the Fantasy World Generator application that manages the step-by-step terrain generation process.
    /// Provides UI controls for creating randomized terrain, applying erosion effects, placing cities, and generating final maps.
    /// </summary>
    public partial class FantasyWorldGeneratorForm : Form
    {
        /// <summary>
        /// Cancellation token source for managing long-running async operations and allowing user cancellation.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;
        
        /// <summary>
        /// Initializes a new instance of the FantasyWorldGeneratorForm and creates initial terrain with the default seed value.
        /// </summary>
        public FantasyWorldGeneratorForm()
        {
            InitializeComponent();
            paintPanel.CreateTerrain((int)seedStepper.Value);
        }
        
        /// <summary>
        /// Handles the form load event by initializing the paint panel for terrain visualization.
        /// </summary>
        /// <param name="sender">The form that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void FantasyWorldGeneratorFormLoad(object sender, EventArgs e)
        {
            paintPanel.Load();
        }

        /// <summary>
        /// Cancels any currently running generation operation by requesting cancellation through the cancellation token.
        /// Ensures clean shutdown of background tasks without blocking the UI thread.
        /// </summary>
        private void CancelCurrentOperation()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Generates a random set of 256 points for the initial mesh grid.
        /// Creates the foundation point distribution that will be improved and used for terrain generation.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void GenerateRandomPointsClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                Point[] points = await Task.Run(() => paintPanel.Terrain.GeneratePoints(256, Extent.DefaultExtent), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerGrid.Instance.MeshPts = points;
                    improvePoints.Enabled = showOriginalPoints.Enabled = true;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
                });
            });
        }

        /// <summary>
        /// Improves the current point distribution using Lloyd relaxation to create more evenly spaced points.
        /// Results in better quality Voronoi diagrams and more natural terrain generation.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ImprovePointsClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                Point[] improvedPoints = await Task.Run(() => 
                    paintPanel.Terrain.ImprovePoints(LayerGrid.Instance.MeshPts, Extent.DefaultExtent, 1), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerGrid.Instance.MeshPts = improvedPoints;
                    LayerGrid.Instance.MeshVxs = null;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
                });
            });
        }

        /// <summary>
        /// Toggles between showing the original random points and the computed Voronoi corner vertices.
        /// Allows visualization of the underlying mesh structure used for terrain generation.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ShowOriginalPointsClick(object sender, EventArgs e)
        {
            string originalText = "Show Original Points";
            Button button = (Button)sender;
            
            if (button.Text == originalText)
            {
                await ExecuteGenerationAsync(async (token) =>
                {
                    if (LayerGrid.Instance.MeshVxs == null)
                    {
                        Mesh mesh = await Task.Run(() => 
                            paintPanel.Terrain.MakeMesh(LayerGrid.Instance.MeshPts, Extent.DefaultExtent), token).ConfigureAwait(false);
                        
                        this.Invoke((MethodInvoker)delegate
                        {
                            LayerGrid.Instance.MeshVxs = mesh.Vxs;
                        });
                    }
                    
                    this.Invoke((MethodInvoker)delegate
                    {
                        button.Text = "Show Voronoi Corners";
                        paintPanel.DrawQueue = new int[] { (int)DrawPanel.Visualize.LayerGridVoronoiCorners };
                    });
                });
            }
            else
            {
                button.Text = originalText;
                paintPanel.DrawQueue = new int[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
            }
        }

        /// <summary>
        /// Resets the terrain generation process by creating a new optimized mesh with 4096 points.
        /// Prepares the foundation for outline generation and enables all height modification controls.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ResetClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                Mesh mesh = await Task.Run(() => paintPanel.Terrain.GenerateGoodMesh(4096, Extent.DefaultExtent), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = null;
                    LayerOutline.Instance.Mesh = mesh;
                    randomSlope.Enabled = cone.Enabled = invertedCone.Enabled = fiveBlobs.Enabled = 
                        normalizeHeightmap.Enabled = roundHills.Enabled = relax.Enabled = setSeaLevelToMedian.Enabled = true;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineReset };
                });
            });
        }

        /// <summary>
        /// Adds a random slope to the current heightmap based on a randomly generated direction vector.
        /// Creates directional height variation that can simulate tectonic tilting effects.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void RandomSlopeClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] slopeHeights = await Task.Run(() =>
                {
                    Mesh mesh = LayerOutline.Instance.Mesh;
                    double[] currentHeights = LayerOutline.Instance.Heights;
                    Point randomVector = paintPanel.Terrain.RandomVector(4);
                    double[] slope = paintPanel.Terrain.Slope(mesh, randomVector);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, slope);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = slopeHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSlope };
                });
            });
        }

        /// <summary>
        /// Adds a cone-shaped depression to the heightmap centered on the terrain.
        /// Creates a bowl-like lowland effect that can serve as lakes or valleys.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ConeClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] coneHeights = await Task.Run(() =>
                {
                    Mesh mesh = LayerOutline.Instance.Mesh;
                    double[] currentHeights = LayerOutline.Instance.Heights;
                    double[] cone = paintPanel.Terrain.Cone(mesh, -0.5f);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, cone);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = coneHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineCone };
                });
            });
        }

        /// <summary>
        /// Adds an inverted cone-shaped elevation to the heightmap centered on the terrain.
        /// Creates a mountain or hill-like protrusion rising from the center outward.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void InvertedConeClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] invertedConeHeights = await Task.Run(() =>
                {
                    Mesh mesh = LayerOutline.Instance.Mesh;
                    double[] currentHeights = LayerOutline.Instance.Heights;
                    double[] invertedCone = paintPanel.Terrain.Cone(mesh, 0.5f);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, invertedCone);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = invertedConeHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineInvertedCone };
                });
            });
        }

        /// <summary>
        /// Adds five blob-shaped mountain ranges randomly distributed across the terrain.
        /// Creates distinct elevated regions that form the basis for mountain systems.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void FiveBlobsClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] blobHeights = await Task.Run(() =>
                {
                    Mesh mesh = LayerOutline.Instance.Mesh;
                    double[] currentHeights = LayerOutline.Instance.Heights;
                    double[] mountains = paintPanel.Terrain.Mountains(mesh, 5);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, mountains);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = blobHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineFiveBlobs };
                });
            });
        }

        /// <summary>
        /// Normalizes the heightmap values to ensure they fall within the standard 0-1 range.
        /// Adjusts the terrain elevation scale for consistent processing by subsequent operations.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void NormalizeHeightmapClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] normalizedHeights = await Task.Run(() => 
                    paintPanel.Terrain.Normalize(LayerOutline.Instance.Heights), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = normalizedHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineNormalizeHeightmap };
                });
            });
        }

        /// <summary>
        /// Applies a peaky transformation to create more pronounced hills and mountain ridges.
        /// Enhances terrain features by sharpening elevation transitions and creating dramatic relief.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void RoundHillsClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] peakyHeights = await Task.Run(() => 
                    paintPanel.Terrain.Peaky(LayerOutline.Instance.Heights), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = peakyHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRoundHills };
                });
            });
        }

        /// <summary>
        /// Smooths the heightmap by averaging each point with its neighbors using mesh relaxation.
        /// Reduces sharp elevation changes and creates more natural, flowing terrain contours.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void RelaxClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] relaxedHeights = await Task.Run(() => 
                    paintPanel.Terrain.Relax(LayerOutline.Instance.Mesh, LayerOutline.Instance.Heights), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = relaxedHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRelax };
                });
            });
        }

        /// <summary>
        /// Sets the sea level to the median height value, determining which areas become water vs land.
        /// Establishes the fundamental division between ocean and terrestrial regions.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void SetSeaLevelToMedianClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] seaLevelHeights = await Task.Run(() => 
                    paintPanel.Terrain.SetSeaLevel(LayerOutline.Instance.Heights, 0.5f), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = seaLevelHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSetSeaLevelToMedian };
                });
            });
        }

        /// <summary>
        /// Generates a fresh random heightmap specifically for erosion simulation experiments.
        /// Creates uneroded terrain that serves as the starting point for erosion processing.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ErodeGenerateRandomHeightMapClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                (Mesh mesh, double[] heights) = await Task.Run(() =>
                {
                    Mesh meshVar = LayerErosion.Instance.Mesh;
                    double[] heightsVar = LayerErosion.Instance.Heights;
                    paintPanel.Terrain.GenerateUneroded(ref meshVar, ref heightsVar);
                    return (meshVar, heightsVar);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerErosion.Instance.Mesh = mesh;
                    LayerErosion.Instance.Heights = heights;
                    erode.Enabled = erodeSeaLeveltoMedian.Enabled = cleanCoastlines.Enabled = showErosionRate.Enabled = true;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionGenerateRandomHeightmap };
                });
            });
        }

        /// <summary>
        /// Applies hydraulic erosion simulation to carve realistic valleys and river channels.
        /// Simulates water flow and sediment transport to create natural-looking terrain features.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ErodeClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                (Mesh mesh, double[] heights, int[] downhill) = await Task.Run(() =>
                {
                    Mesh meshVar = LayerErosion.Instance.Mesh;
                    double[] heightsVar = LayerErosion.Instance.Heights;
                    int[] downhillVar = LayerErosion.Instance.Downhill;
                    double[] erodedHeights = paintPanel.Terrain.DoErosion(ref meshVar, ref downhillVar, heightsVar, 0.1f);
                    return (meshVar, erodedHeights, downhillVar);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerErosion.Instance.Mesh = mesh;
                    LayerErosion.Instance.Heights = heights;
                    LayerErosion.Instance.Downhill = downhill;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionErode };
                });
            });
        }

        /// <summary>
        /// Adjusts sea level to the median height after erosion to establish proper land-water boundaries.
        /// Ensures consistent water level definition after terrain modification by erosion.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ErodeSeaLeveltoMedianClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                double[] seaLevelHeights = await Task.Run(() => 
                    paintPanel.Terrain.SetSeaLevel(LayerErosion.Instance.Heights, 0.5f), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerErosion.Instance.Heights = seaLevelHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionSetSeaLevelToMedian };
                });
            });
        }

        /// <summary>
        /// Cleans up coastline artifacts and fills terrain sinks to prevent water from pooling inappropriately.
        /// Ensures realistic water flow by eliminating unrealistic inland seas and improving coastal definition.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void CleanCoastlinesClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                (Mesh mesh, double[] cleanedHeights) = await Task.Run(() =>
                {
                    Mesh meshVar = LayerErosion.Instance.Mesh;
                    double[] coastHeights = paintPanel.Terrain.CleanCoast(meshVar, LayerErosion.Instance.Heights, 1);
                    double[] filledHeights = paintPanel.Terrain.FillSinks(ref meshVar, coastHeights);
                    return (meshVar, filledHeights);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerErosion.Instance.Mesh = mesh;
                    LayerErosion.Instance.Heights = cleanedHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionCleanCoastlines };
                });
            });
        }

        /// <summary>
        /// Toggles between showing the erosion rate visualization and the standard heightmap display.
        /// Allows inspection of where erosion had the greatest impact on the terrain.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void ShowErosionRateClick(object sender, EventArgs e)
        {
            string originalText = "Show Erosion Rate";
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

        /// <summary>
        /// Generic method for toggling rendering layers on and off in the visualization pipeline.
        /// Manages the display state of visual elements like coastlines, rivers, and shading effects.
        /// </summary>
        /// <param name="sender">The button that controls the rendering toggle.</param>
        /// <param name="showText">Text to display when the layer is hidden.</param>
        /// <param name="hideText">Text to display when the layer is visible.</param>
        /// <param name="drawCall">The visualization layer identifier to toggle.</param>
        private void PhysRender(Button sender, string showText, string hideText, int drawCall)
        {
            List<int> alteredDrawQueue = paintPanel.DrawQueue.ToList();
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

        /// <summary>
        /// Generates a complete rendered terrain with coastlines, rivers, and shading for final visualization.
        /// Creates the definitive terrain suitable for final map rendering with all geographic features.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void RenderingGenerateRandomHeightmapClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                (int[] downhill, Mesh mesh, double[] heights) = await Task.Run(() =>
                {
                    LayerRendering instance = LayerRendering.Instance;
                    int[] downhillVar = instance.Downhill;
                    Mesh meshVar = instance.Mesh;
                    double[] generatedHeights = paintPanel.Terrain.GenerateCoast(ref downhillVar, ref meshVar, 4096, Extent.DefaultExtent);
                    return (downhillVar, meshVar, generatedHeights);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerRendering instance = LayerRendering.Instance;
                    instance.Downhill = downhill;
                    instance.Mesh = mesh;
                    instance.Heights = heights;
                    renderingShowCoastline.Enabled = renderingShowRivers.Enabled = renderingShowSlopeShading.Enabled = renderingHideHeightmap.Enabled = true;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerRenderingGenerateRandomHeightmap };
                });
            });
        }
        
        /// <summary>
        /// Toggles the visibility of coastline rendering in the terrain visualization.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void RenderingShowCoastlineClick(object sender, EventArgs e)
        {
            PhysRender((Button)sender, "Show Coastline", "Hide Coastline", (int)DrawPanel.Visualize.LayerRenderingShowCoastline);
        }
        
        /// <summary>
        /// Toggles the visibility of river system rendering in the terrain visualization.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void RenderingShowRiversClick(object sender, EventArgs e)
        {
            PhysRender((Button)sender, "Show Rivers", "Hide Rivers", (int)DrawPanel.Visualize.LayerRenderingShowRivers);
        }
        
        /// <summary>
        /// Toggles the visibility of slope-based shading effects in the terrain visualization.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void RenderingShowSlopeShadingClick(object sender, EventArgs e)
        {
            PhysRender((Button)sender, "Show Slope Shading", "Hide Slope Shading", (int)DrawPanel.Visualize.LayerRenderingShowSlopeShading);
        }
        
        /// <summary>
        /// Toggles the visibility of the base heightmap rendering in the terrain visualization.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private void RenderingHideHeightmapClick(object sender, EventArgs e)
        {
            PhysRender((Button)sender, "Show Heightmap", "Hide Heightmap", (int)DrawPanel.Visualize.LayerRenderingGenerateRandomHeightmap);
        }
        
        /// <summary>
        /// Generates a new terrain with cities for the cities layer.
        /// Creates height map, places initial cities, and enables city management controls.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void CitiesGenerateRandomHeightmapClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                (int[] downhill, Mesh mesh, double[] heights, CityRender cityRender) = await Task.Run(() =>
                {
                    LayerCities instance = LayerCities.Instance;
                    int[] downhillVar = instance.Downhill;
                    Mesh meshVar = instance.Mesh;
                    double[] heightsVar = instance.Heights;
                    CityRender cityRenderVar = paintPanel.Terrain.NewCityRender(ref downhillVar, ref meshVar, ref heightsVar, Extent.DefaultExtent);
                    return (downhillVar, meshVar, heightsVar, cityRenderVar);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerCities instance = LayerCities.Instance;
                    instance.Downhill = downhill;
                    instance.Mesh = mesh;
                    instance.Heights = heights;
                    instance.CityRender = cityRender;
                    cityAddNew.Enabled = showTerritories.Enabled = true;
                });
                
                await CityDrawAsync().ConfigureAwait(false);
            });
        }
        
        /// <summary>
        /// Adds a new city to the map at the location with the highest suitability score.
        /// Considers factors like water access, elevation, and distance from existing cities.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void CityAddNewClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                (int[] downhill, Mesh mesh, double[] heights, CityRender cityRender) = await Task.Run(() =>
                {
                    LayerCities instance = LayerCities.Instance;
                    CityRender cityRenderVar = instance.CityRender;
                    int[] downhillVar = instance.Downhill;
                    Mesh meshVar = instance.Mesh;
                    double[] heightsVar = instance.Heights;
                    List<int> cities = cityRenderVar.Cities;
                    
                    paintPanel.Terrain.PlaceCity(ref cities, ref downhillVar, ref meshVar, ref heightsVar);
                    cityRenderVar.Cities = cities;
                    
                    return (downhillVar, meshVar, heightsVar, cityRenderVar);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerCities instance = LayerCities.Instance;
                    instance.Downhill = downhill;
                    instance.Mesh = mesh;
                    instance.Heights = heights;
                    instance.CityRender = cityRender;
                });
                
                await CityDrawAsync().ConfigureAwait(false);
            });
        }
        
        /// <summary>
        /// Performs city drawing operations including territory assignment and scoring.
        /// Calculates territories for each city and optionally computes city placement scores.
        /// </summary>
        /// <param name="cityViewScore">Whether to display city placement scores (true) or territories (false).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CityDrawAsync(bool cityViewScore = true)
        {
            (int[] downhill, Mesh mesh, double[] heights, CityRender cityRender) = await Task.Run(() =>
            {
                LayerCities instance = LayerCities.Instance;
                CityRender cityRenderVar = instance.CityRender;
                int[] downhillVar = instance.Downhill;
                Mesh meshVar = instance.Mesh;
                double[] heightsVar = instance.Heights;
                
                cityRenderVar.Territories = paintPanel.Terrain.GetTerritories(ref cityRenderVar, ref downhillVar, ref meshVar, ref heightsVar);
                
                List<int> cities = cityRenderVar.Cities;
                if (cityViewScore)
                    cityRenderVar.Score = paintPanel.Terrain.CityScore(ref cities, ref downhillVar, ref meshVar, ref heightsVar);
                
                cityRenderVar.Cities = cities;
                return (downhillVar, meshVar, heightsVar, cityRenderVar);
            }).ConfigureAwait(false);
            
            this.Invoke((MethodInvoker)delegate
            {
                LayerCities instance = LayerCities.Instance;
                instance.CityRender = cityRender;
                instance.Downhill = downhill;
                instance.Mesh = mesh;
                instance.Heights = heights;
                
                paintPanel.DrawQueue = cityViewScore ? new[] { (int)DrawPanel.Visualize.LayerCitiesViewCities } : new[] { (int)DrawPanel.Visualize.LayerCitiesViewTerritories };
            });
        }
        
        /// <summary>
        /// Toggles between showing territory boundaries and city placement scores.
        /// Updates the visualization to highlight either territorial control or optimal city locations.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void ShowTerritoriesClick(object sender, EventArgs e)
        {
            string originalText = "Show Territories";
            Button button = (Button)sender;
            
            if (button.Text == originalText)
            {
                button.Text = "Hide Territories";
                await CityDrawAsync(false).ConfigureAwait(false);
            }
            else
            {
                button.Text = originalText;
                await CityDrawAsync().ConfigureAwait(false);
            }
        }
        
        /// <summary>
        /// Generates a complete high-resolution map with all features including labels.
        /// Creates the final map output with terrain, cities, rivers, borders, and procedurally generated names.
        /// </summary>
        /// <param name="sender">The button that triggered this event.</param>
        /// <param name="e">Event arguments containing event data.</param>
        private async void GenHighResolutionMapClick(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                await DoMapAsync(token).ConfigureAwait(false);
            });
        }
        
        /// <summary>
        /// Performs the complete map generation process asynchronously.
        /// Generates terrain, places cities, creates rivers and borders, assigns territories, and prepares for labeling.
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation if needed.</param>
        /// <returns>A task representing the asynchronous map generation operation.</returns>
        private async Task DoMapAsync(CancellationToken cancellationToken = default)
        {
            (int[] downhill, Mesh mesh, double[] heights, CityRender cityRender) = await Task.Run(() =>
            {
                CityRender cityRenderVar = new CityRender();
                int[] downhillVar = new int[0];
                Mesh meshVar = new Mesh();
                double[] generatedHeights = paintPanel.Terrain.GenerateCoast(ref downhillVar, ref meshVar, cityRenderVar.AreaProperties.NumberOfPoints, Extent.DefaultExtent);
                
                paintPanel.Terrain.PlaceCities(ref cityRenderVar, ref downhillVar, ref meshVar, ref generatedHeights);
                
                return (downhillVar, meshVar, generatedHeights, cityRenderVar);
            }, cancellationToken).ConfigureAwait(false);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            (List<List<Point>> rivers, List<List<Point>> coasts, double[] territories, List<List<Point>> borders) = await Task.Run(() =>
            {
                List<List<Point>> riversVar = paintPanel.Terrain.GetRivers(ref mesh, ref downhill, ref heights, 0.01d);
                List<List<Point>> coastsVar = paintPanel.Terrain.Contour(ref mesh, ref heights, 0);
                double[] territoriesVar = paintPanel.Terrain.GetTerritories(ref cityRender, ref downhill, ref mesh, ref heights);
                
                cityRender.Territories = territoriesVar;
                List<List<Point>> bordersVar = paintPanel.Terrain.GetBorders(ref mesh, ref cityRender, ref heights);
                
                return (riversVar, coastsVar, territoriesVar, bordersVar);
            }, cancellationToken).ConfigureAwait(false);
            
            this.Invoke((MethodInvoker)delegate
            {
                LayerLabels instance = LayerLabels.Instance;
                instance.Reset();
                
                cityRender.Rivers = rivers;
                cityRender.Coasts = coasts;
                cityRender.Territories = territories;
                cityRender.Borders = borders;
                
                instance.CityRender = cityRender;
                instance.Downhill = downhill;
                instance.Mesh = mesh;
                instance.Heights = heights;
                
                paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerLabelsDoMap };
            });
        }

        /// <summary>
        /// Executes a generation operation asynchronously with proper cancellation handling and error management.
        /// Manages rendering state, cancellation tokens, and provides user feedback during long operations.
        /// </summary>
        /// <param name="operation">The async operation to execute, which takes a cancellation token.</param>
        /// <returns>A task representing the asynchronous execution operation.</returns>
        private async Task ExecuteGenerationAsync(Func<CancellationToken, Task> operation)
        {
            CancelCurrentOperation();
            _cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                this.Invoke((MethodInvoker)delegate { paintPanel.IsRendering = true; });
                await operation(_cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"An error occurred during generation: {ex.Message}", "Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            finally
            {
                this.Invoke((MethodInvoker)delegate { paintPanel.IsRendering = false; });
            }
        }

        /// <summary>
        /// Handles key down events on the seed numeric control to recreate terrain with new seed values.
        /// </summary>
        /// <param name="sender">The numeric up/down control that triggered this event.</param>
        /// <param name="e">Key event arguments containing information about the key pressed.</param>
        private void NumericUpDown1KeyDown(object sender, KeyEventArgs e)
        {
            int seed = (int)seedStepper.Value;
            seed = seed < 0 ? -seed : seed;
            paintPanel.CreateTerrain(seed);
        }

        /// <summary>
        /// Handles form closing by cancelling any running operations to ensure clean shutdown.
        /// </summary>
        /// <param name="e">Form closed event arguments.</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CancelCurrentOperation();
            base.OnFormClosed(e);
        }
    }
}
