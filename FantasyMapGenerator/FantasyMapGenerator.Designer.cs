namespace WorldMap
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.erodeGenerateRandomHeightMap = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Grids = new System.Windows.Forms.TabPage();
            this.showOriginalPoints = new System.Windows.Forms.Button();
            this.improvePoints = new System.Windows.Forms.Button();
            this.RoughOutlines = new System.Windows.Forms.TabPage();
            this.setSeaLevelToMedian = new System.Windows.Forms.Button();
            this.invertedCone = new System.Windows.Forms.Button();
            this.cone = new System.Windows.Forms.Button();
            this.normalizeHeightmap = new System.Windows.Forms.Button();
            this.relax = new System.Windows.Forms.Button();
            this.fiveBlobs = new System.Windows.Forms.Button();
            this.roundHills = new System.Windows.Forms.Button();
            this.randomSlope = new System.Windows.Forms.Button();
            this.reset = new System.Windows.Forms.Button();
            this.Erosion = new System.Windows.Forms.TabPage();
            this.showErosionRate = new System.Windows.Forms.Button();
            this.cleanCoastlines = new System.Windows.Forms.Button();
            this.erodeSeaLeveltoMedian = new System.Windows.Forms.Button();
            this.erode = new System.Windows.Forms.Button();
            this.erosionGenerateRandomHeightMap = new System.Windows.Forms.Button();
            this.renderingTerrain = new System.Windows.Forms.TabPage();
            this.renderingHideHeightmap = new System.Windows.Forms.Button();
            this.renderingShowSlopeShading = new System.Windows.Forms.Button();
            this.renderingShowRivers = new System.Windows.Forms.Button();
            this.renderingShowCoastline = new System.Windows.Forms.Button();
            this.renderingGenerateRandomHeightmap = new System.Windows.Forms.Button();
            this.CitiesBorders = new System.Windows.Forms.TabPage();
            this.showTerritories = new System.Windows.Forms.Button();
            this.cityAddNew = new System.Windows.Forms.Button();
            this.citiesGenerateRandomHeightmap = new System.Windows.Forms.Button();
            this.PlacingLabels = new System.Windows.Forms.TabPage();
            this.paintPanel = new WorldMap.DrawPanel();
            this.genHighResolutionMap = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.Grids.SuspendLayout();
            this.RoughOutlines.SuspendLayout();
            this.Erosion.SuspendLayout();
            this.renderingTerrain.SuspendLayout();
            this.CitiesBorders.SuspendLayout();
            this.PlacingLabels.SuspendLayout();
            this.SuspendLayout();
            // 
            // erodeGenerateRandomHeightMap
            // 
            this.erodeGenerateRandomHeightMap.BackColor = System.Drawing.Color.White;
            this.erodeGenerateRandomHeightMap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.erodeGenerateRandomHeightMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.erodeGenerateRandomHeightMap.ForeColor = System.Drawing.Color.Black;
            this.erodeGenerateRandomHeightMap.Location = new System.Drawing.Point(6, 33);
            this.erodeGenerateRandomHeightMap.Name = "erodeGenerateRandomHeightMap";
            this.erodeGenerateRandomHeightMap.Size = new System.Drawing.Size(226, 43);
            this.erodeGenerateRandomHeightMap.TabIndex = 1;
            this.erodeGenerateRandomHeightMap.Text = "Generate Random Points";
            this.erodeGenerateRandomHeightMap.UseVisualStyleBackColor = false;
            this.erodeGenerateRandomHeightMap.Click += new System.EventHandler(this.generateRandomPoints_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Grids);
            this.tabControl1.Controls.Add(this.RoughOutlines);
            this.tabControl1.Controls.Add(this.Erosion);
            this.tabControl1.Controls.Add(this.renderingTerrain);
            this.tabControl1.Controls.Add(this.CitiesBorders);
            this.tabControl1.Controls.Add(this.PlacingLabels);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(630, 131);
            this.tabControl1.TabIndex = 2;
            // 
            // Grids
            // 
            this.Grids.Controls.Add(this.showOriginalPoints);
            this.Grids.Controls.Add(this.improvePoints);
            this.Grids.Controls.Add(this.erodeGenerateRandomHeightMap);
            this.Grids.Location = new System.Drawing.Point(4, 22);
            this.Grids.Name = "Grids";
            this.Grids.Padding = new System.Windows.Forms.Padding(3);
            this.Grids.Size = new System.Drawing.Size(622, 105);
            this.Grids.TabIndex = 0;
            this.Grids.Text = "Grids";
            this.Grids.UseVisualStyleBackColor = true;
            // 
            // showOriginalPoints
            // 
            this.showOriginalPoints.BackColor = System.Drawing.Color.White;
            this.showOriginalPoints.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.showOriginalPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showOriginalPoints.ForeColor = System.Drawing.Color.Black;
            this.showOriginalPoints.Location = new System.Drawing.Point(407, 33);
            this.showOriginalPoints.Name = "showOriginalPoints";
            this.showOriginalPoints.Size = new System.Drawing.Size(199, 43);
            this.showOriginalPoints.TabIndex = 3;
            this.showOriginalPoints.Text = "Show Original Points";
            this.showOriginalPoints.UseVisualStyleBackColor = false;
            this.showOriginalPoints.Click += new System.EventHandler(this.showOriginalPoints_Click);
            // 
            // improvePoints
            // 
            this.improvePoints.BackColor = System.Drawing.Color.White;
            this.improvePoints.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.improvePoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.improvePoints.ForeColor = System.Drawing.Color.Black;
            this.improvePoints.Location = new System.Drawing.Point(247, 33);
            this.improvePoints.Name = "improvePoints";
            this.improvePoints.Size = new System.Drawing.Size(143, 43);
            this.improvePoints.TabIndex = 2;
            this.improvePoints.Text = "Improve Points";
            this.improvePoints.UseVisualStyleBackColor = false;
            this.improvePoints.Click += new System.EventHandler(this.improvePoints_Click);
            // 
            // RoughOutlines
            // 
            this.RoughOutlines.Controls.Add(this.setSeaLevelToMedian);
            this.RoughOutlines.Controls.Add(this.invertedCone);
            this.RoughOutlines.Controls.Add(this.cone);
            this.RoughOutlines.Controls.Add(this.normalizeHeightmap);
            this.RoughOutlines.Controls.Add(this.relax);
            this.RoughOutlines.Controls.Add(this.fiveBlobs);
            this.RoughOutlines.Controls.Add(this.roundHills);
            this.RoughOutlines.Controls.Add(this.randomSlope);
            this.RoughOutlines.Controls.Add(this.reset);
            this.RoughOutlines.Location = new System.Drawing.Point(4, 22);
            this.RoughOutlines.Name = "RoughOutlines";
            this.RoughOutlines.Padding = new System.Windows.Forms.Padding(3);
            this.RoughOutlines.Size = new System.Drawing.Size(622, 105);
            this.RoughOutlines.TabIndex = 1;
            this.RoughOutlines.Text = "Rough Outlines";
            this.RoughOutlines.UseVisualStyleBackColor = true;
            // 
            // setSeaLevelToMedian
            // 
            this.setSeaLevelToMedian.BackColor = System.Drawing.Color.White;
            this.setSeaLevelToMedian.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.setSeaLevelToMedian.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setSeaLevelToMedian.ForeColor = System.Drawing.Color.Black;
            this.setSeaLevelToMedian.Location = new System.Drawing.Point(398, 55);
            this.setSeaLevelToMedian.Name = "setSeaLevelToMedian";
            this.setSeaLevelToMedian.Size = new System.Drawing.Size(219, 43);
            this.setSeaLevelToMedian.TabIndex = 12;
            this.setSeaLevelToMedian.Text = "Set Sea Level to Median";
            this.setSeaLevelToMedian.UseVisualStyleBackColor = false;
            this.setSeaLevelToMedian.Click += new System.EventHandler(this.setSeaLevelToMedian_Click);
            // 
            // invertedCone
            // 
            this.invertedCone.BackColor = System.Drawing.Color.White;
            this.invertedCone.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.invertedCone.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invertedCone.ForeColor = System.Drawing.Color.Black;
            this.invertedCone.Location = new System.Drawing.Point(325, 6);
            this.invertedCone.Name = "invertedCone";
            this.invertedCone.Size = new System.Drawing.Size(148, 43);
            this.invertedCone.TabIndex = 11;
            this.invertedCone.Text = "+ Inverted Cone";
            this.invertedCone.UseVisualStyleBackColor = false;
            this.invertedCone.Click += new System.EventHandler(this.invertedCone_Click);
            // 
            // cone
            // 
            this.cone.BackColor = System.Drawing.Color.White;
            this.cone.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.cone.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cone.ForeColor = System.Drawing.Color.Black;
            this.cone.Location = new System.Drawing.Point(240, 6);
            this.cone.Name = "cone";
            this.cone.Size = new System.Drawing.Size(79, 43);
            this.cone.TabIndex = 10;
            this.cone.Text = "+ Cone";
            this.cone.UseVisualStyleBackColor = false;
            this.cone.Click += new System.EventHandler(this.cone_Click);
            // 
            // normalizeHeightmap
            // 
            this.normalizeHeightmap.BackColor = System.Drawing.Color.White;
            this.normalizeHeightmap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.normalizeHeightmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.normalizeHeightmap.ForeColor = System.Drawing.Color.Black;
            this.normalizeHeightmap.Location = new System.Drawing.Point(6, 55);
            this.normalizeHeightmap.Name = "normalizeHeightmap";
            this.normalizeHeightmap.Size = new System.Drawing.Size(188, 43);
            this.normalizeHeightmap.TabIndex = 9;
            this.normalizeHeightmap.Text = "Normalize Heightmap";
            this.normalizeHeightmap.UseVisualStyleBackColor = false;
            this.normalizeHeightmap.Click += new System.EventHandler(this.normalizeHeightmap_Click);
            // 
            // relax
            // 
            this.relax.BackColor = System.Drawing.Color.White;
            this.relax.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.relax.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.relax.ForeColor = System.Drawing.Color.Black;
            this.relax.Location = new System.Drawing.Point(325, 55);
            this.relax.Name = "relax";
            this.relax.Size = new System.Drawing.Size(67, 43);
            this.relax.TabIndex = 8;
            this.relax.Text = "Relax";
            this.relax.UseVisualStyleBackColor = false;
            this.relax.Click += new System.EventHandler(this.relax_Click);
            // 
            // fiveBlobs
            // 
            this.fiveBlobs.BackColor = System.Drawing.Color.White;
            this.fiveBlobs.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.fiveBlobs.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fiveBlobs.ForeColor = System.Drawing.Color.Black;
            this.fiveBlobs.Location = new System.Drawing.Point(479, 6);
            this.fiveBlobs.Name = "fiveBlobs";
            this.fiveBlobs.Size = new System.Drawing.Size(137, 43);
            this.fiveBlobs.TabIndex = 7;
            this.fiveBlobs.Text = "+ Five Blobs";
            this.fiveBlobs.UseVisualStyleBackColor = false;
            this.fiveBlobs.Click += new System.EventHandler(this.fiveBlobs_Click);
            // 
            // roundHills
            // 
            this.roundHills.BackColor = System.Drawing.Color.White;
            this.roundHills.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.roundHills.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundHills.ForeColor = System.Drawing.Color.Black;
            this.roundHills.Location = new System.Drawing.Point(200, 55);
            this.roundHills.Name = "roundHills";
            this.roundHills.Size = new System.Drawing.Size(119, 43);
            this.roundHills.TabIndex = 6;
            this.roundHills.Text = "Round Hills";
            this.roundHills.UseVisualStyleBackColor = false;
            this.roundHills.Click += new System.EventHandler(this.roundHills_Click);
            // 
            // randomSlope
            // 
            this.randomSlope.BackColor = System.Drawing.Color.White;
            this.randomSlope.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.randomSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.randomSlope.ForeColor = System.Drawing.Color.Black;
            this.randomSlope.Location = new System.Drawing.Point(79, 6);
            this.randomSlope.Name = "randomSlope";
            this.randomSlope.Size = new System.Drawing.Size(155, 43);
            this.randomSlope.TabIndex = 5;
            this.randomSlope.Text = "+ Random Slope";
            this.randomSlope.UseVisualStyleBackColor = false;
            this.randomSlope.Click += new System.EventHandler(this.randomSlope_Click);
            // 
            // reset
            // 
            this.reset.BackColor = System.Drawing.Color.White;
            this.reset.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reset.ForeColor = System.Drawing.Color.Black;
            this.reset.Location = new System.Drawing.Point(6, 6);
            this.reset.Name = "reset";
            this.reset.Size = new System.Drawing.Size(67, 43);
            this.reset.TabIndex = 4;
            this.reset.Text = "Reset";
            this.reset.UseVisualStyleBackColor = false;
            this.reset.Click += new System.EventHandler(this.reset_Click);
            // 
            // Erosion
            // 
            this.Erosion.Controls.Add(this.showErosionRate);
            this.Erosion.Controls.Add(this.cleanCoastlines);
            this.Erosion.Controls.Add(this.erodeSeaLeveltoMedian);
            this.Erosion.Controls.Add(this.erode);
            this.Erosion.Controls.Add(this.erosionGenerateRandomHeightMap);
            this.Erosion.Location = new System.Drawing.Point(4, 22);
            this.Erosion.Name = "Erosion";
            this.Erosion.Size = new System.Drawing.Size(622, 105);
            this.Erosion.TabIndex = 2;
            this.Erosion.Text = "Erosion";
            this.Erosion.UseVisualStyleBackColor = true;
            // 
            // showErosionRate
            // 
            this.showErosionRate.BackColor = System.Drawing.Color.White;
            this.showErosionRate.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.showErosionRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showErosionRate.ForeColor = System.Drawing.Color.Black;
            this.showErosionRate.Location = new System.Drawing.Point(219, 52);
            this.showErosionRate.Name = "showErosionRate";
            this.showErosionRate.Size = new System.Drawing.Size(210, 43);
            this.showErosionRate.TabIndex = 9;
            this.showErosionRate.Text = "Show Erosion Rate";
            this.showErosionRate.UseVisualStyleBackColor = false;
            this.showErosionRate.Click += new System.EventHandler(this.showErosionRate_Click);
            // 
            // cleanCoastlines
            // 
            this.cleanCoastlines.BackColor = System.Drawing.Color.White;
            this.cleanCoastlines.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.cleanCoastlines.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cleanCoastlines.ForeColor = System.Drawing.Color.Black;
            this.cleanCoastlines.Location = new System.Drawing.Point(3, 52);
            this.cleanCoastlines.Name = "cleanCoastlines";
            this.cleanCoastlines.Size = new System.Drawing.Size(210, 43);
            this.cleanCoastlines.TabIndex = 8;
            this.cleanCoastlines.Text = "Clean Coastlines";
            this.cleanCoastlines.UseVisualStyleBackColor = false;
            this.cleanCoastlines.Click += new System.EventHandler(this.cleanCoastlines_Click);
            // 
            // erodeSeaLeveltoMedian
            // 
            this.erodeSeaLeveltoMedian.BackColor = System.Drawing.Color.White;
            this.erodeSeaLeveltoMedian.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.erodeSeaLeveltoMedian.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.erodeSeaLeveltoMedian.ForeColor = System.Drawing.Color.Black;
            this.erodeSeaLeveltoMedian.Location = new System.Drawing.Point(395, 3);
            this.erodeSeaLeveltoMedian.Name = "erodeSeaLeveltoMedian";
            this.erodeSeaLeveltoMedian.Size = new System.Drawing.Size(224, 43);
            this.erodeSeaLeveltoMedian.TabIndex = 7;
            this.erodeSeaLeveltoMedian.Text = "Set Sea Level to Median";
            this.erodeSeaLeveltoMedian.UseVisualStyleBackColor = false;
            this.erodeSeaLeveltoMedian.Click += new System.EventHandler(this.erodeSeaLeveltoMedian_Click);
            // 
            // erode
            // 
            this.erode.BackColor = System.Drawing.Color.White;
            this.erode.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.erode.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.erode.ForeColor = System.Drawing.Color.Black;
            this.erode.Location = new System.Drawing.Point(290, 3);
            this.erode.Name = "erode";
            this.erode.Size = new System.Drawing.Size(99, 43);
            this.erode.TabIndex = 6;
            this.erode.Text = "Erode";
            this.erode.UseVisualStyleBackColor = false;
            this.erode.Click += new System.EventHandler(this.erode_Click);
            // 
            // erosionGenerateRandomHeightMap
            // 
            this.erosionGenerateRandomHeightMap.BackColor = System.Drawing.Color.White;
            this.erosionGenerateRandomHeightMap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.erosionGenerateRandomHeightMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.erosionGenerateRandomHeightMap.ForeColor = System.Drawing.Color.Black;
            this.erosionGenerateRandomHeightMap.Location = new System.Drawing.Point(3, 3);
            this.erosionGenerateRandomHeightMap.Name = "erosionGenerateRandomHeightMap";
            this.erosionGenerateRandomHeightMap.Size = new System.Drawing.Size(281, 43);
            this.erosionGenerateRandomHeightMap.TabIndex = 5;
            this.erosionGenerateRandomHeightMap.Text = "Generate Random Heightmap";
            this.erosionGenerateRandomHeightMap.UseVisualStyleBackColor = false;
            this.erosionGenerateRandomHeightMap.Click += new System.EventHandler(this.erodeGenerateRandomHeightMap_Click);
            // 
            // renderingTerrain
            // 
            this.renderingTerrain.Controls.Add(this.renderingHideHeightmap);
            this.renderingTerrain.Controls.Add(this.renderingShowSlopeShading);
            this.renderingTerrain.Controls.Add(this.renderingShowRivers);
            this.renderingTerrain.Controls.Add(this.renderingShowCoastline);
            this.renderingTerrain.Controls.Add(this.renderingGenerateRandomHeightmap);
            this.renderingTerrain.Location = new System.Drawing.Point(4, 22);
            this.renderingTerrain.Name = "renderingTerrain";
            this.renderingTerrain.Size = new System.Drawing.Size(622, 105);
            this.renderingTerrain.TabIndex = 5;
            this.renderingTerrain.Text = "Rendering Terrain";
            this.renderingTerrain.UseVisualStyleBackColor = true;
            // 
            // renderingHideHeightmap
            // 
            this.renderingHideHeightmap.BackColor = System.Drawing.Color.White;
            this.renderingHideHeightmap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.renderingHideHeightmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderingHideHeightmap.ForeColor = System.Drawing.Color.Black;
            this.renderingHideHeightmap.Location = new System.Drawing.Point(237, 55);
            this.renderingHideHeightmap.Name = "renderingHideHeightmap";
            this.renderingHideHeightmap.Size = new System.Drawing.Size(225, 43);
            this.renderingHideHeightmap.TabIndex = 15;
            this.renderingHideHeightmap.Text = "Hide Heightmap";
            this.renderingHideHeightmap.UseVisualStyleBackColor = false;
            this.renderingHideHeightmap.Click += new System.EventHandler(this.renderingHideHeightmap_Click);
            // 
            // renderingShowSlopeShading
            // 
            this.renderingShowSlopeShading.BackColor = System.Drawing.Color.White;
            this.renderingShowSlopeShading.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.renderingShowSlopeShading.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderingShowSlopeShading.ForeColor = System.Drawing.Color.Black;
            this.renderingShowSlopeShading.Location = new System.Drawing.Point(3, 55);
            this.renderingShowSlopeShading.Name = "renderingShowSlopeShading";
            this.renderingShowSlopeShading.Size = new System.Drawing.Size(228, 43);
            this.renderingShowSlopeShading.TabIndex = 14;
            this.renderingShowSlopeShading.Text = "Show Slope Shading";
            this.renderingShowSlopeShading.UseVisualStyleBackColor = false;
            this.renderingShowSlopeShading.Click += new System.EventHandler(this.renderingShowSlopeShading_Click);
            // 
            // renderingShowRivers
            // 
            this.renderingShowRivers.BackColor = System.Drawing.Color.White;
            this.renderingShowRivers.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.renderingShowRivers.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderingShowRivers.ForeColor = System.Drawing.Color.Black;
            this.renderingShowRivers.Location = new System.Drawing.Point(468, 6);
            this.renderingShowRivers.Name = "renderingShowRivers";
            this.renderingShowRivers.Size = new System.Drawing.Size(151, 43);
            this.renderingShowRivers.TabIndex = 13;
            this.renderingShowRivers.Text = "Show Rivers";
            this.renderingShowRivers.UseVisualStyleBackColor = false;
            this.renderingShowRivers.Click += new System.EventHandler(this.renderingShowRivers_Click);
            // 
            // renderingShowCoastline
            // 
            this.renderingShowCoastline.BackColor = System.Drawing.Color.White;
            this.renderingShowCoastline.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.renderingShowCoastline.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderingShowCoastline.ForeColor = System.Drawing.Color.Black;
            this.renderingShowCoastline.Location = new System.Drawing.Point(290, 6);
            this.renderingShowCoastline.Name = "renderingShowCoastline";
            this.renderingShowCoastline.Size = new System.Drawing.Size(172, 43);
            this.renderingShowCoastline.TabIndex = 12;
            this.renderingShowCoastline.Text = "Show Coastline";
            this.renderingShowCoastline.UseVisualStyleBackColor = false;
            this.renderingShowCoastline.Click += new System.EventHandler(this.renderingShowCoastline_Click);
            // 
            // renderingGenerateRandomHeightmap
            // 
            this.renderingGenerateRandomHeightmap.BackColor = System.Drawing.Color.White;
            this.renderingGenerateRandomHeightmap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.renderingGenerateRandomHeightmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renderingGenerateRandomHeightmap.ForeColor = System.Drawing.Color.Black;
            this.renderingGenerateRandomHeightmap.Location = new System.Drawing.Point(3, 6);
            this.renderingGenerateRandomHeightmap.Name = "renderingGenerateRandomHeightmap";
            this.renderingGenerateRandomHeightmap.Size = new System.Drawing.Size(281, 43);
            this.renderingGenerateRandomHeightmap.TabIndex = 11;
            this.renderingGenerateRandomHeightmap.Text = "Generate Random Heightmap";
            this.renderingGenerateRandomHeightmap.UseVisualStyleBackColor = false;
            this.renderingGenerateRandomHeightmap.Click += new System.EventHandler(this.renderingGenerateRandomHeightmap_Click);
            // 
            // CitiesBorders
            // 
            this.CitiesBorders.Controls.Add(this.showTerritories);
            this.CitiesBorders.Controls.Add(this.cityAddNew);
            this.CitiesBorders.Controls.Add(this.citiesGenerateRandomHeightmap);
            this.CitiesBorders.Location = new System.Drawing.Point(4, 22);
            this.CitiesBorders.Name = "CitiesBorders";
            this.CitiesBorders.Size = new System.Drawing.Size(622, 105);
            this.CitiesBorders.TabIndex = 3;
            this.CitiesBorders.Text = "Cities, Borders";
            this.CitiesBorders.UseVisualStyleBackColor = true;
            // 
            // showTerritories
            // 
            this.showTerritories.BackColor = System.Drawing.Color.White;
            this.showTerritories.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.showTerritories.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showTerritories.ForeColor = System.Drawing.Color.Black;
            this.showTerritories.Location = new System.Drawing.Point(422, 28);
            this.showTerritories.Name = "showTerritories";
            this.showTerritories.Size = new System.Drawing.Size(197, 43);
            this.showTerritories.TabIndex = 14;
            this.showTerritories.Text = "Show Territories";
            this.showTerritories.UseVisualStyleBackColor = false;
            this.showTerritories.Click += new System.EventHandler(this.showTerritories_Click);
            // 
            // cityAddNew
            // 
            this.cityAddNew.BackColor = System.Drawing.Color.White;
            this.cityAddNew.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.cityAddNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cityAddNew.ForeColor = System.Drawing.Color.Black;
            this.cityAddNew.Location = new System.Drawing.Point(278, 28);
            this.cityAddNew.Name = "cityAddNew";
            this.cityAddNew.Size = new System.Drawing.Size(138, 43);
            this.cityAddNew.TabIndex = 13;
            this.cityAddNew.Text = "Add New City";
            this.cityAddNew.UseVisualStyleBackColor = false;
            this.cityAddNew.Click += new System.EventHandler(this.cityAddNew_Click);
            // 
            // citiesGenerateRandomHeightmap
            // 
            this.citiesGenerateRandomHeightmap.BackColor = System.Drawing.Color.White;
            this.citiesGenerateRandomHeightmap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.citiesGenerateRandomHeightmap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.citiesGenerateRandomHeightmap.ForeColor = System.Drawing.Color.Black;
            this.citiesGenerateRandomHeightmap.Location = new System.Drawing.Point(3, 28);
            this.citiesGenerateRandomHeightmap.Name = "citiesGenerateRandomHeightmap";
            this.citiesGenerateRandomHeightmap.Size = new System.Drawing.Size(269, 43);
            this.citiesGenerateRandomHeightmap.TabIndex = 12;
            this.citiesGenerateRandomHeightmap.Text = "Generate Random Heightmap";
            this.citiesGenerateRandomHeightmap.UseVisualStyleBackColor = false;
            this.citiesGenerateRandomHeightmap.Click += new System.EventHandler(this.citiesGenerateRandomHeightmap_Click);
            // 
            // PlacingLabels
            // 
            this.PlacingLabels.Controls.Add(this.genHighResolutionMap);
            this.PlacingLabels.Location = new System.Drawing.Point(4, 22);
            this.PlacingLabels.Name = "PlacingLabels";
            this.PlacingLabels.Size = new System.Drawing.Size(622, 105);
            this.PlacingLabels.TabIndex = 4;
            this.PlacingLabels.Text = "Placing Labels";
            this.PlacingLabels.UseVisualStyleBackColor = true;
            // 
            // paintPanel
            // 
            this.paintPanel.BackColor = System.Drawing.Color.White;
            this.paintPanel.Location = new System.Drawing.Point(12, 149);
            this.paintPanel.Name = "paintPanel";
            this.paintPanel.Size = new System.Drawing.Size(626, 626);
            this.paintPanel.TabIndex = 0;
            // 
            // genHighResolutionMap
            // 
            this.genHighResolutionMap.BackColor = System.Drawing.Color.White;
            this.genHighResolutionMap.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.genHighResolutionMap.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.genHighResolutionMap.ForeColor = System.Drawing.Color.Black;
            this.genHighResolutionMap.Location = new System.Drawing.Point(185, 32);
            this.genHighResolutionMap.Name = "genHighResolutionMap";
            this.genHighResolutionMap.Size = new System.Drawing.Size(269, 43);
            this.genHighResolutionMap.TabIndex = 13;
            this.genHighResolutionMap.Text = "Generate High Resolution Map";
            this.genHighResolutionMap.UseVisualStyleBackColor = false;
            this.genHighResolutionMap.Click += new System.EventHandler(this.genHighResolutionMap_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 820);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.paintPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Fantasy World Generator";
            this.tabControl1.ResumeLayout(false);
            this.Grids.ResumeLayout(false);
            this.RoughOutlines.ResumeLayout(false);
            this.Erosion.ResumeLayout(false);
            this.renderingTerrain.ResumeLayout(false);
            this.CitiesBorders.ResumeLayout(false);
            this.PlacingLabels.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DrawPanel paintPanel;
        private System.Windows.Forms.Button erodeGenerateRandomHeightMap;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Grids;
        private System.Windows.Forms.Button showOriginalPoints;
        private System.Windows.Forms.Button improvePoints;
        private System.Windows.Forms.TabPage RoughOutlines;
        private System.Windows.Forms.TabPage Erosion;
        private System.Windows.Forms.TabPage CitiesBorders;
        private System.Windows.Forms.TabPage PlacingLabels;
        private System.Windows.Forms.Button setSeaLevelToMedian;
        private System.Windows.Forms.Button invertedCone;
        private System.Windows.Forms.Button cone;
        private System.Windows.Forms.Button normalizeHeightmap;
        private System.Windows.Forms.Button relax;
        private System.Windows.Forms.Button fiveBlobs;
        private System.Windows.Forms.Button roundHills;
        private System.Windows.Forms.Button randomSlope;
        private System.Windows.Forms.Button reset;
        private System.Windows.Forms.Button showErosionRate;
        private System.Windows.Forms.Button cleanCoastlines;
        private System.Windows.Forms.Button erodeSeaLeveltoMedian;
        private System.Windows.Forms.Button erode;
        private System.Windows.Forms.Button erosionGenerateRandomHeightMap;
        private System.Windows.Forms.TabPage renderingTerrain;
        private System.Windows.Forms.Button renderingHideHeightmap;
        private System.Windows.Forms.Button renderingShowSlopeShading;
        private System.Windows.Forms.Button renderingShowRivers;
        private System.Windows.Forms.Button renderingShowCoastline;
        private System.Windows.Forms.Button renderingGenerateRandomHeightmap;
        private System.Windows.Forms.Button showTerritories;
        private System.Windows.Forms.Button cityAddNew;
        private System.Windows.Forms.Button citiesGenerateRandomHeightmap;
        private System.Windows.Forms.Button genHighResolutionMap;
    }
}

