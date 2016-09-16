using System.Drawing;

namespace Delaunay
{
    public class Circle
    {
        public PointF center;
        public float radius;

        public Circle(float centerX, float centerY, float radius)
        {
            this.center = new PointF(centerX, centerY);
            this.radius = radius;
        }

        public override string ToString()
        {
            return "Circle (center: " + center + "; radius: " + radius + ")";
        }
    }
}