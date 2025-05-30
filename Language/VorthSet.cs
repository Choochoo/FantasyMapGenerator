using System.Collections.Generic;

namespace Language
{
    /// <summary>
    /// Represents a set of vowel orthographic mappings for language generation.
    /// Inherits from Set and contains mappings from vowel sounds to their written representations.
    /// </summary>
    public class VorthSet : Set
    {
        /// <summary>
        /// Dictionary mapping vowel characters to their orthographic string representations.
        /// Defines how vowel sounds are written in the language's script system.
        /// </summary>
        public Dictionary<char, string> Ortho;
        
        /// <summary>
        /// Initializes a new instance of the VorthSet class with a name and orthographic dictionary.
        /// </summary>
        /// <param name="name">The name identifier for this vowel orthographic set.</param>
        /// <param name="ortho">The dictionary mapping vowel characters to their written forms.</param>
        public VorthSet(string name, Dictionary<char, string> ortho)
        {
            Name = name;
            Ortho = ortho;
        }
    }
}
