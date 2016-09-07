using System.Collections.Generic;

namespace Language
{
    public class Language
    {
        public Dictionary<string, string> Phonemes = new Dictionary<string, string>()
        {
            {"C", "ptkmnls"},
            {"V", "aeiou"},
            {"S", "s"},
            {"F", "mn"},
            {"L", "rl"}
        };

        public string Structure = "CVC";
        public int Exponent = 2;
        public string[] Restricts;
        public Dictionary<char, string> Cortho = new Dictionary<char, string>();
        public Dictionary<char, string> Vortho = new Dictionary<char, string>();
        public bool NoOrtho = true;
        public bool NoMorph = true;
        public bool NoWordPool = true;
        public int MinSyll = 1;
        public int MaxSyll = 1;
        public Dictionary<string, List<string>> Morphemes = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Words = new Dictionary<string, List<string>>();
        public List<string> Names = new List<string>();
        public char Joiner = ' ';
        public int MaxChar = 12;
        public int MinChar = 5;
        public string Definite;
        public string Genitive;
    }
}
