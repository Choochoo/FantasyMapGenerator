namespace Language
{
    /// <summary>
    /// Represents a set of sibilant sounds for language generation.
    /// Inherits from Set and contains sibilant phonemes (like 's', 'sh', 'z') used in syllable construction.
    /// </summary>
    public class SSet : Set
    {
        /// <summary>
        /// String containing the sibilant characters available in this set.
        /// Each character represents a distinct sibilant sound.
        /// </summary>
        public string S;
        
        /// <summary>
        /// Initializes a new instance of the SSet class with a name and sibilant string.
        /// </summary>
        /// <param name="name">The name identifier for this sibilant set.</param>
        /// <param name="s">The string of sibilant characters in this set.</param>
        public SSet(string name, string s)
        {
            Name = name;
            S = s;
        }
    }
}
