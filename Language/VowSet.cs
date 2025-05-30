namespace Language
{
    /// <summary>
    /// Represents a set of vowel sounds for language generation.
    /// Inherits from Set and contains vowel phonemes used in syllable construction.
    /// </summary>
    public class VowSet : Set
    {
        /// <summary>
        /// String containing the vowel characters available in this set.
        /// Each character represents a distinct vowel sound.
        /// </summary>
        public string V;
        
        /// <summary>
        /// Initializes a new instance of the VowSet class with a name and vowel string.
        /// </summary>
        /// <param name="name">The name identifier for this vowel set.</param>
        /// <param name="v">The string of vowel characters in this set.</param>
        public VowSet(string name, string v)
        {
            Name = name;
            V = v;
        }
    }
}
