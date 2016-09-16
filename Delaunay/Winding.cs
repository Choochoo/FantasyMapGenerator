using System;

namespace Delaunay
{
    public class Winding
    {
        public static readonly Winding CLOCKWISE = new Winding(typeof(PrivateConstructorEnforcer), "clockwise");
        public static readonly Winding COUNTERCLOCKWISE = new Winding(typeof(PrivateConstructorEnforcer), "counterclockwise");
        public static readonly Winding NONE = new Winding(typeof(PrivateConstructorEnforcer), "none");

        private string _name;

        public Winding(Type pce, string name)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Invalid static readonlyructor access");
            }
            _name = name;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}