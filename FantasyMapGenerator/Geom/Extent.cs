namespace WorldMap.Geom
{
    public struct Extent
    {
        public static Extent DefaultExtent { get; } = new Extent(0, 0, 1, 1);

        public static readonly Extent zero = new Extent(0, 0, 0, 0);
        public static readonly Extent one = new Extent(1, 1, 1, 1);

        public double x, y, width, height;

        public Extent(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public double left
        {
            get
            {
                return x;
            }
        }

        public double right
        {
            get
            {
                return x + width;
            }
        }

        public double top
        {
            get
            {
                return y;
            }
        }

        public double bottom
        {
            get
            {
                return y + height;
            }
        }

        public Vector2f topLeft
        {
            get
            {
                return new Vector2f(left, top);
            }
        }

        public Vector2f bottomRight
        {
            get
            {
                return new Vector2f(right, bottom);
            }
        }
    }
}
