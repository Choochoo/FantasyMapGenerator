using System;

namespace Delaunay
{
    public class LR
    {
        public static readonly LR LEFT = new LR(typeof(PrivateConstructorEnforcer), "left");
        public static readonly LR RIGHT = new LR(typeof(PrivateConstructorEnforcer), "right");

        private string _name;

        public LR(Type pce, string name)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Illegal static readonlyructor access");
            }
            _name = name;
        }

        public static LR Other(LR leftRight)
        {
            return leftRight == LEFT ? RIGHT : LEFT;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}

class PrivateConstructorEnforcer {}