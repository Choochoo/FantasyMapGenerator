namespace Language
{
    /// <summary>
    /// Represents a set of fricative sounds for language generation.
    /// Inherits from Set and contains fricative phonemes used in syllable construction.
    /// </summary>
    public class FSet : Set
    {
        /// <summary>
        /// String containing the fricative characters available in this set.
        /// Each character represents a distinct fricative sound.
        /// </summary>
        public string F;
        
        /// <summary>
        /// Initializes a new instance of the FSet class with a name and fricative string.
        /// </summary>
        /// <param name="name">The name identifier for this fricative set.</param>
        /// <param name="f">The string of fricative characters in this set.</param>
        public FSet(string name, string f)
        {
            Name = name;
            F = f;
        }
    }
}
