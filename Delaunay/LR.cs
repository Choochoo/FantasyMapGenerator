using System;

namespace Delaunay
{
    /// <summary>
    /// Represents a left/right orientation enumeration for Delaunay triangulation operations.
    /// Provides static instances for LEFT and RIGHT orientations with utility methods.
    /// </summary>
    public class LR
    {
        /// <summary>
        /// Static instance representing the LEFT orientation.
        /// </summary>
        public static readonly LR LEFT = new LR(typeof(PrivateConstructorEnforcer), "left");
        
        /// <summary>
        /// Static instance representing the RIGHT orientation.
        /// </summary>
        public static readonly LR RIGHT = new LR(typeof(PrivateConstructorEnforcer), "right");

        /// <summary>
        /// Private field storing the name of this orientation.
        /// </summary>
        private string _name;

        /// <summary>
        /// Initializes a new instance of the LR class with enforced private construction.
        /// </summary>
        /// <param name="pce">Private constructor enforcer type to prevent external instantiation.</param>
        /// <param name="name">The name of this orientation.</param>
        /// <exception cref="Exception">Thrown when attempting to construct without proper enforcer.</exception>
        public LR(Type pce, string name)
        {
            if (pce != typeof(PrivateConstructorEnforcer))
            {
                throw new Exception("Illegal static readonlyructor access");
            }
            _name = name;
        }

        /// <summary>
        /// Returns the opposite orientation of the given LR value.
        /// </summary>
        /// <param name="leftRight">The LR orientation to get the opposite of.</param>
        /// <returns>RIGHT if input is LEFT, LEFT if input is RIGHT.</returns>
        public static LR Other(LR leftRight)
        {
            return leftRight == LEFT ? RIGHT : LEFT;
        }

        /// <summary>
        /// Returns the string representation of this orientation.
        /// </summary>
        /// <returns>The name of this orientation ("left" or "right").</returns>
        public override string ToString()
        {
            return _name;
        }
    }
}

/// <summary>
/// Private class used to enforce controlled instantiation of LR objects.
/// Prevents external code from creating LR instances directly.
/// </summary>
class PrivateConstructorEnforcer {}