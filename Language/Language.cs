using System.Collections.Generic;

namespace Language
{
    /// <summary>
    /// Represents a constructed language with phonemes, structure, orthography, and morphological rules.
    /// Used for generating fantasy language names and words for map features and locations.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Dictionary mapping phoneme categories to their possible sounds.
        /// Default categories include consonants (C), vowels (V), sibilants (S), fricatives (F), and liquids (L).
        /// </summary>
        public Dictionary<string, string> Phonemes = new()
        {
            {"C", "ptkmnls"},
            {"V", "aeiou"},
            {"S", "s"},
            {"F", "mn"},
            {"L", "rl"}
        };

        /// <summary>
        /// The syllable structure pattern for word generation.
        /// Uses phoneme category letters to define syllable patterns (e.g., "CVC" for consonant-vowel-consonant).
        /// Default value is "CVC".
        /// </summary>
        public string Structure = "CVC";

        /// <summary>
        /// Exponent value used in language generation algorithms.
        /// Affects the probability distribution of phoneme selection.
        /// Default value is 2.
        /// </summary>
        public int Exponent = 2;

        /// <summary>
        /// Array of phoneme restrictions or rules that limit certain combinations.
        /// Used to prevent unrealistic or unwanted sound combinations in generated words.
        /// </summary>
        public string[] Restricts;

        /// <summary>
        /// Dictionary mapping consonant characters to their orthographic representations.
        /// Defines how consonant sounds are written in the language's script.
        /// </summary>
        public Dictionary<char, string> Cortho = [];

        /// <summary>
        /// Dictionary mapping vowel characters to their orthographic representations.
        /// Defines how vowel sounds are written in the language's script.
        /// </summary>
        public Dictionary<char, string> Vortho = [];

        /// <summary>
        /// Flag indicating whether orthographic rules should be ignored.
        /// When true, phonetic representation is used instead of orthographic.
        /// Default value is true.
        /// </summary>
        public bool NoOrtho = true;

        /// <summary>
        /// Flag indicating whether morphological rules should be ignored.
        /// When true, simple word generation is used without complex morphology.
        /// Default value is true.
        /// </summary>
        public bool NoMorph = true;

        /// <summary>
        /// Flag indicating whether to use a predefined word pool.
        /// When true, words are generated algorithmically rather than from a pool.
        /// Default value is true.
        /// </summary>
        public bool NoWordPool = true;

        /// <summary>
        /// Minimum number of syllables allowed in generated words.
        /// Default value is 1.
        /// </summary>
        public int MinSyll = 1;

        /// <summary>
        /// Maximum number of syllables allowed in generated words.
        /// Default value is 1.
        /// </summary>
        public int MaxSyll = 1;

        /// <summary>
        /// Dictionary containing morpheme categories and their possible forms.
        /// Used for complex word formation when morphological rules are enabled.
        /// </summary>
        public Dictionary<string, List<string>> Morphemes = [];

        /// <summary>
        /// Dictionary containing word categories and their possible forms.
        /// Used when working with predefined word pools instead of algorithmic generation.
        /// </summary>
        public Dictionary<string, List<string>> Words = [];

        /// <summary>
        /// List of generated names for places, people, or features.
        /// Stores the output of the language generation process.
        /// </summary>
        public List<string> Names = [];

        /// <summary>
        /// Character used to join multiple words or morphemes together.
        /// Default value is a space character.
        /// </summary>
        public char Joiner = ' ';

        /// <summary>
        /// Maximum number of characters allowed in generated words.
        /// Default value is 12.
        /// </summary>
        public int MaxChar = 12;

        /// <summary>
        /// Minimum number of characters required in generated words.
        /// Default value is 5.
        /// </summary>
        public int MinChar = 5;

        /// <summary>
        /// The definite article form in the language (e.g., "the" in English).
        /// Used for grammatical construction of place names and phrases.
        /// </summary>
        public string Definite;

        /// <summary>
        /// The genitive case marker or possessive form in the language.
        /// Used for creating possessive constructions and compound names.
        /// </summary>
        public string Genitive;
    }
}
