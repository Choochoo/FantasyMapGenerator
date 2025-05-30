namespace D3Voronoi
{
    /// <summary>
    /// Represents a 2D point with X and Y coordinates used in Voronoi diagram calculations.
    /// Contains additional properties for indexing and data association.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// The X coordinate of the point.
        /// </summary>
        public double X;
        
        /// <summary>
        /// The Y coordinate of the point.
        /// </summary>
        public double Y;
        
        /// <summary>
        /// The index identifier for this point in collections or arrays.
        /// </summary>
        public int Index;
        
        /// <summary>
        /// Additional data point associated with this point for extended functionality.
        /// </summary>
        public Point Data;
        
        /// <summary>
        /// Gets a static point instance representing the origin (0, 0).
        /// </summary>
        public static Point Zero => new Point(0, 0);

        /// <summary>
        /// Initializes a new instance of the Point class with specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the point.</param>
        /// <param name="y">The Y coordinate of the point.</param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the Point class with default coordinates (0, 0).
        /// </summary>
        public Point()
        {
        }

        /// <summary>
        /// Returns a string representation of the point showing its X and Y coordinates.
        /// </summary>
        /// <returns>A string in the format "X:{X},Y:{Y}".</returns>
        public override string ToString()
        {
            return $"X:{X},Y:{Y}";
        }
    }
}
