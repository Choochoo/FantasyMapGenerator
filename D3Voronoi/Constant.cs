namespace D3Voronoi
{
    /// <summary>
    /// Static class containing constant utility functions for Voronoi diagram calculations.
    /// Provides basic mathematical functions used throughout the Voronoi generation process.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// Identity function that returns the input value unchanged.
        /// Used as a default accessor function for X coordinates.
        /// </summary>
        /// <param name="x">The input value to return.</param>
        /// <returns>The same value that was passed as input.</returns>
        public static double XFunc(double x)
        {
            return x;
        }
    }
}
