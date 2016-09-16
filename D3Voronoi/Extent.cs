namespace D3Voronoi
{
    public class Extent
    {
        public static Extent DefaultExtent => new Extent(0, 0, 1, 1);

        public double X, Y, Width, Height;
        public Extent(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
