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
    public partial class FantasyWorldGeneratorForm : Form
    {
        private CancellationTokenSource _cancellationTokenSource;
        
        public FantasyWorldGeneratorForm()
        {
            InitializeComponent();
            paintPanel.CreateTerrain((int)seedStepper.Value);
        }
        
        private void FantasyWorldGeneratorForm_Load(object sender, EventArgs e)
        {
            paintPanel.Load();
        }

        private void CancelCurrentOperation()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private async void generateRandomPoints_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var points = await Task.Run(() => paintPanel.Terrain.GeneratePoints(256, Extent.DefaultExtent), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerGrid.Instance.MeshPts = points;
                    improvePoints.Enabled = showOriginalPoints.Enabled = true;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
                });
            });
        }

        private async void improvePoints_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var improvedPoints = await Task.Run(() => 
                    paintPanel.Terrain.ImprovePoints(LayerGrid.Instance.MeshPts, Extent.DefaultExtent, 1), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerGrid.Instance.MeshPts = improvedPoints;
                    LayerGrid.Instance.MeshVxs = null;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerGridGenerateRandomPoints };
                });
            });
        }

        private async void showOriginalPoints_Click(object sender, EventArgs e)
        {
            var originalText = "Show Original Points";
            var button = (Button)sender;
            
            if (button.Text == originalText)
            {
                await ExecuteGenerationAsync(async (token) =>
                {
                    if (LayerGrid.Instance.MeshVxs == null)
                    {
                        var mesh = await Task.Run(() => 
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

        private async void reset_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var mesh = await Task.Run(() => paintPanel.Terrain.GenerateGoodMesh(4096, Extent.DefaultExtent), token).ConfigureAwait(false);
                
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

        private async void randomSlope_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var slopeHeights = await Task.Run(() =>
                {
                    var mesh = LayerOutline.Instance.Mesh;
                    var currentHeights = LayerOutline.Instance.Heights;
                    var randomVector = paintPanel.Terrain.RandomVector(4);
                    var slope = paintPanel.Terrain.Slope(mesh, randomVector);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, slope);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = slopeHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSlope };
                });
            });
        }

        private async void cone_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var coneHeights = await Task.Run(() =>
                {
                    var mesh = LayerOutline.Instance.Mesh;
                    var currentHeights = LayerOutline.Instance.Heights;
                    var cone = paintPanel.Terrain.Cone(mesh, -0.5f);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, cone);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = coneHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineCone };
                });
            });
        }

        private async void invertedCone_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var invertedConeHeights = await Task.Run(() =>
                {
                    var mesh = LayerOutline.Instance.Mesh;
                    var currentHeights = LayerOutline.Instance.Heights;
                    var invertedCone = paintPanel.Terrain.Cone(mesh, 0.5f);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, invertedCone);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = invertedConeHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineInvertedCone };
                });
            });
        }

        private async void fiveBlobs_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var blobHeights = await Task.Run(() =>
                {
                    var mesh = LayerOutline.Instance.Mesh;
                    var currentHeights = LayerOutline.Instance.Heights;
                    var mountains = paintPanel.Terrain.Mountains(mesh, 5);
                    return paintPanel.Terrain.Add(currentHeights.Length, currentHeights, mountains);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = blobHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineFiveBlobs };
                });
            });
        }

        private async void normalizeHeightmap_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var normalizedHeights = await Task.Run(() => 
                    paintPanel.Terrain.Normalize(LayerOutline.Instance.Heights), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = normalizedHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineNormalizeHeightmap };
                });
            });
        }

        private async void roundHills_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var peakyHeights = await Task.Run(() => 
                    paintPanel.Terrain.Peaky(LayerOutline.Instance.Heights), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = peakyHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRoundHills };
                });
            });
        }

        private async void relax_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var relaxedHeights = await Task.Run(() => 
                    paintPanel.Terrain.Relax(LayerOutline.Instance.Mesh, LayerOutline.Instance.Heights), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = relaxedHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineRelax };
                });
            });
        }

        private async void setSeaLevelToMedian_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var seaLevelHeights = await Task.Run(() => 
                    paintPanel.Terrain.SetSeaLevel(LayerOutline.Instance.Heights, 0.5f), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerOutline.Instance.Heights = seaLevelHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerOutlineSetSeaLevelToMedian };
                });
            });
        }

        private async void erodeGenerateRandomHeightMap_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var (mesh, heights) = await Task.Run(() =>
                {
                    var meshVar = LayerErosion.Instance.Mesh;
                    var heightsVar = LayerErosion.Instance.Heights;
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

        private async void erode_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var (mesh, heights, downhill) = await Task.Run(() =>
                {
                    var meshVar = LayerErosion.Instance.Mesh;
                    var heightsVar = LayerErosion.Instance.Heights;
                    var downhillVar = LayerErosion.Instance.Downhill;
                    var erodedHeights = paintPanel.Terrain.DoErosion(ref meshVar, ref downhillVar, heightsVar, 0.1f);
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

        private async void erodeSeaLeveltoMedian_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var seaLevelHeights = await Task.Run(() => 
                    paintPanel.Terrain.SetSeaLevel(LayerErosion.Instance.Heights, 0.5f), token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    LayerErosion.Instance.Heights = seaLevelHeights;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerErosionSetSeaLevelToMedian };
                });
            });
        }

        private async void cleanCoastlines_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var (mesh, cleanedHeights) = await Task.Run(() =>
                {
                    var meshVar = LayerErosion.Instance.Mesh;
                    var coastHeights = paintPanel.Terrain.CleanCoast(meshVar, LayerErosion.Instance.Heights, 1);
                    var filledHeights = paintPanel.Terrain.FillSinks(ref meshVar, coastHeights);
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

        private async void renderingGenerateRandomHeightmap_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var (downhill, mesh, heights) = await Task.Run(() =>
                {
                    var instance = LayerRendering.Instance;
                    var downhillVar = instance.Downhill;
                    var meshVar = instance.Mesh;
                    var generatedHeights = paintPanel.Terrain.GenerateCoast(ref downhillVar, ref meshVar, 4096, Extent.DefaultExtent);
                    return (downhillVar, meshVar, generatedHeights);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    var instance = LayerRendering.Instance;
                    instance.Downhill = downhill;
                    instance.Mesh = mesh;
                    instance.Heights = heights;
                    renderingShowCoastline.Enabled = renderingShowRivers.Enabled = renderingShowSlopeShading.Enabled = renderingHideHeightmap.Enabled = true;
                    paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerRenderingGenerateRandomHeightmap };
                });
            });
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
        
        private async void citiesGenerateRandomHeightmap_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var (downhill, mesh, heights, cityRender) = await Task.Run(() =>
                {
                    var instance = LayerCities.Instance;
                    var downhillVar = instance.Downhill;
                    var meshVar = instance.Mesh;
                    var heightsVar = instance.Heights;
                    var cityRenderVar = paintPanel.Terrain.NewCityRender(ref downhillVar, ref meshVar, ref heightsVar, Extent.DefaultExtent);
                    return (downhillVar, meshVar, heightsVar, cityRenderVar);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    var instance = LayerCities.Instance;
                    instance.Downhill = downhill;
                    instance.Mesh = mesh;
                    instance.Heights = heights;
                    instance.CityRender = cityRender;
                    cityAddNew.Enabled = showTerritories.Enabled = true;
                });
                
                await CityDrawAsync().ConfigureAwait(false);
            });
        }
        
        private async void cityAddNew_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                var (downhill, mesh, heights, cityRender) = await Task.Run(() =>
                {
                    var instance = LayerCities.Instance;
                    var cityRenderVar = instance.CityRender;
                    var downhillVar = instance.Downhill;
                    var meshVar = instance.Mesh;
                    var heightsVar = instance.Heights;
                    var cities = cityRenderVar.Cities;
                    
                    paintPanel.Terrain.PlaceCity(ref cities, ref downhillVar, ref meshVar, ref heightsVar);
                    cityRenderVar.Cities = cities;
                    
                    return (downhillVar, meshVar, heightsVar, cityRenderVar);
                }, token).ConfigureAwait(false);
                
                this.Invoke((MethodInvoker)delegate
                {
                    var instance = LayerCities.Instance;
                    instance.Downhill = downhill;
                    instance.Mesh = mesh;
                    instance.Heights = heights;
                    instance.CityRender = cityRender;
                });
                
                await CityDrawAsync().ConfigureAwait(false);
            });
        }
        
        public async Task CityDrawAsync(bool cityViewScore = true)
        {
            var (downhill, mesh, heights, cityRender) = await Task.Run(() =>
            {
                var instance = LayerCities.Instance;
                var cityRenderVar = instance.CityRender;
                var downhillVar = instance.Downhill;
                var meshVar = instance.Mesh;
                var heightsVar = instance.Heights;
                
                cityRenderVar.Territories = paintPanel.Terrain.GetTerritories(ref cityRenderVar, ref downhillVar, ref meshVar, ref heightsVar);
                
                var cities = cityRenderVar.Cities;
                if (cityViewScore)
                    cityRenderVar.Score = paintPanel.Terrain.CityScore(ref cities, ref downhillVar, ref meshVar, ref heightsVar);
                
                cityRenderVar.Cities = cities;
                return (downhillVar, meshVar, heightsVar, cityRenderVar);
            }).ConfigureAwait(false);
            
            this.Invoke((MethodInvoker)delegate
            {
                var instance = LayerCities.Instance;
                instance.CityRender = cityRender;
                instance.Downhill = downhill;
                instance.Mesh = mesh;
                instance.Heights = heights;
                
                // Set DrawQueue last to ensure all data is ready before triggering repaint
                paintPanel.DrawQueue = cityViewScore ? new[] { (int)DrawPanel.Visualize.LayerCitiesViewCities } : new[] { (int)DrawPanel.Visualize.LayerCitiesViewTerritories };
            });
        }
        
        private async void showTerritories_Click(object sender, EventArgs e)
        {
            var originalText = "Show Territories";
            var button = (Button)sender;
            
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
        
        private async void genHighResolutionMap_Click(object sender, EventArgs e)
        {
            await ExecuteGenerationAsync(async (token) =>
            {
                await DoMapAsync(token).ConfigureAwait(false);
            });
        }
        
        private async Task DoMapAsync(CancellationToken cancellationToken = default)
        {
            var (downhill, mesh, heights, cityRender) = await Task.Run(() =>
            {
                // Don't reset the instance here - do it on UI thread
                var cityRenderVar = new CityRender();
                var downhillVar = new int[0]; // Initialize with empty arrays
                var meshVar = new Mesh();
                var generatedHeights = paintPanel.Terrain.GenerateCoast(ref downhillVar, ref meshVar, cityRenderVar.AreaProperties.NumberOfPoints, Extent.DefaultExtent);
                
                paintPanel.Terrain.PlaceCities(ref cityRenderVar, ref downhillVar, ref meshVar, ref generatedHeights);
                
                return (downhillVar, meshVar, generatedHeights, cityRenderVar);
            }, cancellationToken).ConfigureAwait(false);
            
            cancellationToken.ThrowIfCancellationRequested();
            
            var (rivers, coasts, territories, borders) = await Task.Run(() =>
            {
                var riversVar = paintPanel.Terrain.GetRivers(ref mesh, ref downhill, ref heights, 0.01d);
                var coastsVar = paintPanel.Terrain.Contour(ref mesh, ref heights, 0);
                var territoriesVar = paintPanel.Terrain.GetTerritories(ref cityRender, ref downhill, ref mesh, ref heights);
                
                cityRender.Territories = territoriesVar;
                var bordersVar = paintPanel.Terrain.GetBorders(ref mesh, ref cityRender, ref heights);
                
                return (riversVar, coastsVar, territoriesVar, bordersVar);
            }, cancellationToken).ConfigureAwait(false);
            
            this.Invoke((MethodInvoker)delegate
            {
                var instance = LayerLabels.Instance;
                // Reset on UI thread to avoid race condition with rendering
                instance.Reset();
                
                cityRender.Rivers = rivers;
                cityRender.Coasts = coasts;
                cityRender.Territories = territories;
                cityRender.Borders = borders;
                
                instance.CityRender = cityRender;
                instance.Downhill = downhill;
                instance.Mesh = mesh;
                instance.Heights = heights;
                
                // Set DrawQueue last to ensure all data is ready before triggering repaint
                paintPanel.DrawQueue = new[] { (int)DrawPanel.Visualize.LayerLabelsDoMap };
            });
        }

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
                // Operation was cancelled, this is expected
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

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            var seed = (int)seedStepper.Value;
            seed = seed < 0 ? -seed : seed;
            paintPanel.CreateTerrain(seed);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            CancelCurrentOperation();
            base.OnFormClosed(e);
        }
    }
}
