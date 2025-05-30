using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Language
{
    /// <summary>
    /// Static class responsible for generating fantasy languages, words, and names.
    /// Provides methods for creating syllables, morphemes, words, and complete language systems with phonetic rules.
    /// </summary>
    public class LanguageGenerator
    {
        /// <summary>
        /// Static random number generator with a fixed seed for consistent language generation.
        /// </summary>
        private static readonly Random Random = new(200);

        /// <summary>
        /// Shuffles the characters in a phoneme string to create variation in sound selection.
        /// </summary>
        /// <param name="phoneme">The phoneme string to shuffle.</param>
        /// <returns>A new string with the characters randomly rearranged.</returns>
        private static string Shuffled(string phoneme)
        {
            var list = phoneme.ToCharArray();
            var newlist = new List<char>();
            for (var i = 0; i < list.Length; i++)
            {
                newlist.Add(list[i]);
            }
            for (var i = list.Length - 1; i > 0; i--)
            {
                var tmp = newlist[i];
                var j = RandRange(i, null);
                newlist[i] = newlist[j];
                newlist[j] = tmp;
            }
            return new string([.. newlist]);
        }

        /// <summary>
        /// Chooses a random index from a list with weighted probability distribution.
        /// Higher exponent values favor earlier indices in the list.
        /// </summary>
        /// <param name="listLength">The length of the list to choose from.</param>
        /// <param name="exponent">The exponent for probability weighting (default is 1).</param>
        /// <returns>A randomly selected index within the list bounds.</returns>
        private static int Choose(int listLength, int exponent = 1)
        {
            return (int)Math.Floor(Math.Pow(Random.NextDouble(), exponent) * listLength);
        }

        /// <summary>
        /// Counter for tracking the number of choose operations performed.
        /// </summary>
        private static int ChooseCount = 0;

        /// <summary>
        /// Generates a random integer within the specified range.
        /// </summary>
        /// <param name="lo">The lower bound (inclusive) or upper bound if hi is null.</param>
        /// <param name="hi">The upper bound (exclusive), or null to use lo as upper bound with 0 as lower bound.</param>
        /// <returns>A random integer within the specified range.</returns>
        private static int RandRange(double lo, double? hi = null)
        {
            if (!hi.HasValue)
            {
                hi = lo;
                lo = 0f;
            }
            return (int)(Math.Floor(Random.NextDouble() * (hi.Value - lo)) + lo);
        }

        /// <summary>
        /// Joins an array of strings with the specified separator.
        /// </summary>
        /// <param name="list">The array of strings to join.</param>
        /// <param name="sep">The separator string to use between elements (default is empty string).</param>
        /// <returns>A single string with all elements joined by the separator.</returns>
        private static string Join(string[] list, string sep = "")
        {
            if (list.Length == 0) return string.Empty;
            var s = list[0];
            for (var i = 1; i < list.Length; i++)
            {
                s += sep;
                s += list[i];
            }

            return s;
        }

        /// <summary>
        /// Capitalizes the first character of a word.
        /// </summary>
        /// <param name="word">The word to capitalize.</param>
        /// <returns>The word with its first character converted to uppercase.</returns>
        private static string Capitalize(string word)
        {
            return char.ToUpper(word[0]) + word[1..];
        }

        /// <summary>
        /// Converts a phonetic syllable to its orthographic representation using the language's spelling rules.
        /// </summary>
        /// <param name="lang">The language containing orthographic mappings.</param>
        /// <param name="syll">The phonetic syllable to convert.</param>
        /// <returns>The orthographically spelled version of the syllable.</returns>
        private static string Spell(Language lang, string syll)
        {
            if (lang.NoOrtho) return syll;
            var s = string.Empty;
            for (var i = 0; i < syll.Length; i++)
            {
                var c = syll[i];
                if (lang.Cortho.ContainsKey(c))
                    s += lang.Cortho[c];
                else if (lang.Vortho.ContainsKey(c))
                    s += lang.Vortho[c];
                else if (SetCollections.DefaultOrtho.ContainsKey(c))
                    s += SetCollections.DefaultOrtho[c];
                else
                    s += c;
            }
            return s;
        }

        /// <summary>
        /// Generates a single syllable according to the language's phonetic structure and restrictions.
        /// </summary>
        /// <param name="lang">The language containing phoneme sets, structure, and restrictions.</param>
        /// <returns>A randomly generated syllable that conforms to the language rules.</returns>
        private static string MakeSyllable(Language lang)
        {
            while (true)
            {
                var syll = "";
                for (var i = 0; i < lang.Structure.Length; i++)
                {
                    var ptype = lang.Structure[i].ToString();
                    var structCharArray = lang.Structure.ToCharArray();
                    if (structCharArray.Length > i + 1 && structCharArray[i + 1] == '?')
                    {
                        i++;
                        if (Random.NextDouble() < 0.5)
                        {
                            continue;
                        }
                    }
                    syll += lang.Phonemes[ptype][Choose(lang.Phonemes[ptype].Length, lang.Exponent)];
                }
                var bad = false;
                for (var i = 0; i < lang.Restricts.Length; i++)
                {
                    if (Regex.Match(syll, lang.Restricts[i], RegexOptions.IgnoreCase).Success)
                    {
                        bad = true;
                        break;
                    }
                }
                if (bad) continue;
                return Spell(lang, syll);
            }
        }

        /// <summary>
        /// Retrieves or generates a morpheme for the specified semantic key.
        /// </summary>
        /// <param name="lang">The language containing morpheme collections.</param>
        /// <param name="key">The semantic key for the morpheme (empty string for generic morphemes).</param>
        /// <returns>A morpheme string associated with the given key.</returns>
        private static string GetMorpheme(Language lang, string key = "")
        {
            if (lang.NoMorph)
                return MakeSyllable(lang);

            var list = lang.Morphemes.ContainsKey(key) ? lang.Morphemes[key] : [];
            var extras = 10;
            if (!string.IsNullOrEmpty(key)) extras = 1;
            while (true)
            {
                var n = RandRange((double)(list.Count + extras), null);
                if (list.ElementAtOrDefault(n) != null) return list[n];
                var morph = MakeSyllable(lang);
                var bad = false;
                foreach (var k in lang.Morphemes)
                {
                    if (lang.Morphemes[k.Key].Contains(morph))
                    {
                        bad = true;
                        break;
                    }
                }
                if (bad) continue;
                list.Add(morph);
                lang.Morphemes[key] = list;
                return morph;
            }
        }

        /// <summary>
        /// Creates a multi-syllable word by combining morphemes according to the language's syllable constraints.
        /// </summary>
        /// <param name="lang">The language containing syllable and morpheme rules.</param>
        /// <param name="key">The semantic key for the word's primary morpheme.</param>
        /// <returns>A complete word composed of one or more syllables.</returns>
        private static string MakeWord(Language lang, string key)
        {
            var nsylls = RandRange(lang.MinSyll, lang.MaxSyll + 1);
            var w = string.Empty;
            var keys = new Dictionary<int, string>();
            keys[RandRange(nsylls)] = key;
            for (var i = 0; i < nsylls; i++)
            {
                var inKey = keys.ContainsKey(i) ? keys[i] : string.Empty;
                w += GetMorpheme(lang, inKey);
            }
            return w;
        }

        /// <summary>
        /// Retrieves or generates a word for the specified semantic key from the language's vocabulary.
        /// </summary>
        /// <param name="lang">The language containing word collections.</param>
        /// <param name="key">The semantic key for the word (empty string for generic words).</param>
        /// <returns>A word string associated with the given key.</returns>
        private static string GetWord(Language lang, string key = "")
        {
            List<string> ws = null;
            if (lang.Words.ContainsKey(key))
                ws = lang.Words[key];
            else
                ws = new List<string>();
            var extras = 3;
            if (!string.IsNullOrEmpty(key)) extras = 2;
            while (true)
            {
                var n = RandRange(ws.Count + extras);
                var w = ws.Count > n ? ws[n] : null;
                if (w != null)
                {
                    return w;
                }
                w = MakeWord(lang, key);
                var bad = false;
                foreach (var k in lang.Words)
                {
                    if (lang.Words[k.Key].Contains(w))
                    {
                        bad = true;
                        break;
                    }
                }
                if (bad) continue;
                ws.Add(w);
                lang.Words[key] = ws;
                return w;
            }
        }

        /// <summary>
        /// Generates a complete name using the language's vocabulary and grammatical rules.
        /// Names can be simple words or compound constructions with articles and genitive markers.
        /// </summary>
        /// <param name="lang">The language to use for name generation.</param>
        /// <param name="key">The semantic key for the name's primary component (empty string for generic names).</param>
        /// <returns>A unique name that follows the language's naming conventions.</returns>
        public static string MakeName(Language lang, string key = "")
        {
            if (lang.Genitive == null)
                lang.Genitive = GetMorpheme(lang, "of");
            if (lang.Definite == null)
                lang.Definite = GetMorpheme(lang, "the");

            while (true)
            {
                var name = string.Empty;
                if (Random.NextDouble() < 0.5)
                {
                    name = Capitalize(GetWord(lang, key));
                }
                else
                {
                    var w1 = Capitalize(GetWord(lang, Random.NextDouble() < 0.6 ? key : string.Empty));
                    var w2 = Capitalize(GetWord(lang, Random.NextDouble() < 0.6 ? key : string.Empty));
                    if (w1 == w2) continue;

                    if (Random.NextDouble() > 0.5)
                    {
                        name = Join(new[] { w1, w2 }, lang.Joiner.ToString());
                    }
                    else
                    {
                        name = Join(new[] { w1, lang.Genitive, w2 }, lang.Joiner.ToString());
                    }
                }

                if (Random.NextDouble() < 0.1)
                {
                    name = Join(new[] { lang.Definite, name }, lang.Joiner.ToString());
                }

                if ((name.Length < lang.MinChar) || (name.Length > lang.MaxChar)) continue;
                var used = false;
                for (var i = 0; i < lang.Names.Count; i++)
                {
                    var name2 = lang.Names[i];
                    if ((name.IndexOf(name2, StringComparison.Ordinal) != -1) || (name2.IndexOf(name, StringComparison.Ordinal) != -1))
                    {
                        used = true;
                        break;
                    }
                }
                if (used) continue;
                lang.Names.Add(name);
                return name;
            }
        }

        /// <summary>
        /// Creates a new language with orthographic spelling enabled.
        /// </summary>
        /// <returns>A Language instance configured to use orthographic representations.</returns>
        public static Language MakeOrthoLanguage()
        {
            var lang = new Language();
            lang.NoOrtho = false;
            return lang;
        }

        /// <summary>
        /// Generates a completely random language with randomized phoneme sets, structure, and orthographic rules.
        /// </summary>
        /// <returns>A fully configured Language instance with randomly selected linguistic features.</returns>
        public static Language MakeRandomLanguage()
        {
            ChooseCount = 0;
            var lang = new Language();
            lang.NoOrtho = false;
            lang.NoMorph = false;
            lang.NoWordPool = false;
            lang.Phonemes["C"] = Shuffled(SetCollections.Consets[Choose(SetCollections.Consets.Length, 2)].C);
            lang.Phonemes["V"] = Shuffled(SetCollections.Vowsets[Choose(SetCollections.Vowsets.Length, 2)].V);
            lang.Phonemes["L"] = Shuffled(SetCollections.Lsets[Choose(SetCollections.Lsets.Length, 2)].L);
            lang.Phonemes["S"] = Shuffled(SetCollections.Ssets[Choose(SetCollections.Ssets.Length, 2)].S);
            lang.Phonemes["F"] = Shuffled(SetCollections.Fsets[Choose(SetCollections.Fsets.Length, 2)].F);
            lang.Structure = SetCollections.Syllstructs[Choose(SetCollections.Syllstructs.Length)];
            lang.Restricts = SetCollections.Ressets[2].Res;
            lang.Cortho = SetCollections.CorthSets[Choose(SetCollections.CorthSets.Length, 2)].Ortho;
            lang.Vortho = SetCollections.VorthSets[Choose(SetCollections.VorthSets.Length, 2)].Ortho;
            lang.MinSyll = RandRange(1, 3);
            if (lang.Structure.Length < 3) lang.MinSyll++;
            lang.MaxSyll = RandRange(lang.MinSyll + 1, 7);
            var joinerArray = "   -".ToCharArray();
            lang.Joiner = joinerArray[Choose(joinerArray.Length)];
            return lang;
        }
    }
}
