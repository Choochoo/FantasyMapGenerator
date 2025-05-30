namespace Language
{
    /// <summary>
    /// Represents a set of liquid sounds for language generation.
    /// Inherits from Set and contains liquid phonemes (like 'l', 'r') used in syllable construction.
    /// </summary>
    public class LSet : Set
    {
        /// <summary>
        /// String containing the liquid characters available in this set.
        /// Each character represents a distinct liquid sound.
        /// </summary>
        public string L;
        
        /// <summary>
        /// Initializes a new instance of the LSet class with a name and liquid string.
        /// </summary>
        /// <param name="name">The name identifier for this liquid set.</param>
        /// <param name="l">The string of liquid characters in this set.</param>
        public LSet(string name, string l)
        {
            Name = name;
            L = l;
        }
    }
}
