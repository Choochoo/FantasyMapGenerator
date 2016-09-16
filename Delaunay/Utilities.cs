using System;
using System.Drawing;

namespace Delaunay
{
    class Utilities
    {
        public static float Distance(PointF one, PointF two)
        {
            float x = (two.X - one.X) * (two.X - one.X);
            float y = (two.Y - one.Y) * (two.Y - one.Y);
            return (float)Math.Sqrt(x + y);
        }
    }
}