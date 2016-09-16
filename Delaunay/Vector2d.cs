using System;

// Recreation of the UnityEngine.Vector3, so it can be used in other thread
namespace Delaunay
{
    public struct Vector2d
    {

        public double X, Y;

        public static readonly Vector2d zero = new Vector2d(0, 0);
        public static readonly Vector2d one = new Vector2d(1, 1);

        public static readonly Vector2d right = new Vector2d(1, 0);
        public static readonly Vector2d left = new Vector2d(-1, 0);

        public static readonly Vector2d up = new Vector2d(0, 1);
        public static readonly Vector2d down = new Vector2d(0, -1);

        public Vector2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }



        public override bool Equals(object other)
        {
            if (!(other is Vector2d))
            {
                return false;
            }
            Vector2d v = (Vector2d)other;
            return X == v.X &&
                   Y == v.Y;
        }

        public override string ToString()
        {
            return string.Format("[Vector2d]" + X + "," + Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() << 2;
        }

        public double DistanceSquare(Vector2d v)
        {
            return Vector2d.DistanceSquare(this, v);
        }
        public static double DistanceSquare(Vector2d a, Vector2d b)
        {
            double cx = b.X - a.X;
            double cy = b.Y - a.Y;
            return cx * cx + cy * cy;
        }

        public static bool operator ==(Vector2d a, Vector2d b)
        {
            return a.X == b.X &&
                   a.Y == b.Y;
        }

        public static bool operator !=(Vector2d a, Vector2d b)
        {
            return a.X != b.X ||
                   a.Y != b.Y;
        }

        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2d operator *(Vector2d a, int i)
        {
            return new Vector2d(a.X * i, a.Y * i);
        }

        public static Vector2d Min(Vector2d a, Vector2d b)
        {
            return new Vector2d(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }
        public static Vector2d Max(Vector2d a, Vector2d b)
        {
            return new Vector2d(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }
    }
}
