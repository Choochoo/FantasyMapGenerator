namespace Language
{
    /// <summary>
    /// Represents a set of consonant sounds for language generation.
    /// Inherits from Set and contains consonant phonemes used in syllable construction.
    /// </summary>
    public class Conset : Set
    {
        /// <summary>
        /// String containing the consonant characters available in this set.
        /// Each character represents a distinct consonant sound.
        /// </summary>
        public string C;

        /// <summary>
        /// Initializes a new instance of the Conset class with a name and consonant string.
        /// </summary>
        /// <param name="name">The name identifier for this consonant set.</param>
        /// <param name="c">The string of consonant characters in this set.</param>
        public Conset(string name, string c)
        {
            Name = name;
            C = c;
        }
    }
}
