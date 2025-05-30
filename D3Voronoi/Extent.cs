namespace D3Voronoi
{
    /// <summary>
    /// Represents a rectangular extent or bounding box with position and dimensions.
    /// Used to define the boundaries for Voronoi diagram calculations and clipping.
    /// </summary>
    public class Extent
    {
        /// <summary>
        /// Gets a default extent instance representing a unit square from (0,0) to (1,1).
        /// </summary>
        public static Extent DefaultExtent => new Extent(0, 0, 1, 1);

        /// <summary>
        /// The X coordinate of the extent's origin (left edge).
        /// </summary>
        public double X;
        
        /// <summary>
        /// The Y coordinate of the extent's origin (top edge).
        /// </summary>
        public double Y;
        
        /// <summary>
        /// The width of the extent (horizontal dimension).
        /// </summary>
        public double Width;
        
        /// <summary>
        /// The height of the extent (vertical dimension).
        /// </summary>
        public double Height;
        
        /// <summary>
        /// Initializes a new instance of the Extent class with specified position and dimensions.
        /// </summary>
        /// <param name="x">The X coordinate of the extent's origin.</param>
        /// <param name="y">The Y coordinate of the extent's origin.</param>
        /// <param name="width">The width of the extent.</param>
        /// <param name="height">The height of the extent.</param>
        public Extent(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
