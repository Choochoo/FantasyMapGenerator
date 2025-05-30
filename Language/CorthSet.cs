using System.Collections.Generic;

namespace Language
{
    /// <summary>
    /// Represents a set of consonant orthographic mappings for language generation.
    /// Inherits from Set and contains mappings from consonant sounds to their written representations.
    /// </summary>
    public class CorthSet : Set
    {
        /// <summary>
        /// Dictionary mapping consonant characters to their orthographic string representations.
        /// Defines how consonant sounds are written in the language's script system.
        /// </summary>
        public Dictionary<char, string> Ortho;
        
        /// <summary>
        /// Initializes a new instance of the CorthSet class with a name and orthographic dictionary.
        /// </summary>
        /// <param name="name">The name identifier for this consonant orthographic set.</param>
        /// <param name="ortho">The dictionary mapping consonant characters to their written forms.</param>
        public CorthSet(string name, Dictionary<char, string> ortho)
        {
            Name = name;
            Ortho = ortho;
        }
    }
}
