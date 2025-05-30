namespace Language
{
    /// <summary>
    /// Represents a set of phoneme restrictions for language generation.
    /// Inherits from Set and contains restriction rules that prevent certain phoneme combinations.
    /// </summary>
    public class ResSet : Set
    {
        /// <summary>
        /// Array of restriction strings that define forbidden phoneme combinations.
        /// Each string represents a pattern that should be avoided in word generation.
        /// </summary>
        public string[] Res;
        
        /// <summary>
        /// Initializes a new instance of the ResSet class with a name and restriction array.
        /// </summary>
        /// <param name="name">The name identifier for this restriction set.</param>
        /// <param name="res">The array of restriction patterns for this set.</param>
        public ResSet(string name, string[] res)
        {
            Name = name;
            Res = res;
        }
    }
}
