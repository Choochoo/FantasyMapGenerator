namespace TerrainGenerator
{
    /// <summary>
    /// Represents a potential location for placing a label on the map.
    /// Contains positioning, alignment, and text properties for label placement optimization.
    /// </summary>
    public class PossibleLabelLocation
    {
        /// <summary>
        /// Constant for left-aligned text positioning.
        /// </summary>
        public const string AlignLeft = "start";
        
        /// <summary>
        /// Constant for right-aligned text positioning.
        /// </summary>
        public const string AlignRight = "end";
        
        /// <summary>
        /// Constant for center-aligned text positioned at the top.
        /// </summary>
        public const string AlignCenterTop = "middletop";
        
        /// <summary>
        /// Constant for center-aligned text positioned at the bottom.
        /// </summary>
        public const string AlignCenterBottom = "middlebottom";
        
        /// <summary>
        /// The X coordinate for the label position.
        /// </summary>
        public double X;
        
        /// <summary>
        /// The Y coordinate for the label position.
        /// </summary>
        public double Y;
        
        /// <summary>
        /// The text alignment setting for the label.
        /// Should be one of the alignment constants defined in this class.
        /// </summary>
        public string Align;
        
        /// <summary>
        /// The starting X coordinate of the bounding area for label placement.
        /// </summary>
        public double X0;
        
        /// <summary>
        /// The starting Y coordinate of the bounding area for label placement.
        /// </summary>
        public double Y0;
        
        /// <summary>
        /// The ending X coordinate of the bounding area for label placement.
        /// </summary>
        public double X1;
        
        /// <summary>
        /// The ending Y coordinate of the bounding area for label placement.
        /// </summary>
        public double Y1;
        
        /// <summary>
        /// The text content to be displayed in the label.
        /// </summary>
        public string Text;
        
        /// <summary>
        /// The font size for the label text.
        /// </summary>
        public double Size;
        
        /// <summary>
        /// The width of the label in pixels or units.
        /// </summary>
        public int Width;
        
        /// <summary>
        /// Debug index for tracking and identifying label locations during development.
        /// </summary>
        public int DebugIndex;
    }
}
