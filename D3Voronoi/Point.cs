namespace D3Voronoi
{
    public class Point
    {
        public double X, Y;
        public int Index;
        public Point Data;
        public static Point Zero => new Point(0, 0);

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point()
        {
        }

        public override string ToString()
        {
            return $"X:{X},Y:{Y}";
        }
    }
}
