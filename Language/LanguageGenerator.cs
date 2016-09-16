using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Language
{
    public class LanguageGenerator
    {
        private static readonly Random Random = new Random(200);
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
            return new string(newlist.ToArray());
        }
        private static int Choose(int listLength, int exponent = 1)
        {
            return (int)Math.Floor(Math.Pow(Random.NextDouble(), exponent) * listLength);
        }
        private static int ChooseCount = 0;
        private static int RandRange(double lo, double? hi = null)
        {
            if (!hi.HasValue)
            {
                hi = lo;
                lo = 0f;
            }
            return (int)(Math.Floor(Random.NextDouble() * (hi.Value - lo)) + lo);
        }
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
        private static string Capitalize(string word)
        {
            return char.ToUpper(word[0]) + word.Substring(1);
        }
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
        private static string GetMorpheme(Language lang, string key = "")
        {
            if (lang.NoMorph)
                return MakeSyllable(lang);

            var list = lang.Morphemes.ContainsKey(key) ? lang.Morphemes[key] : new List<string>();
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
        public static string MakeName(Language lang, string key = "")
        {
            if (lang.Genitive == null)
                lang.Genitive = GetMorpheme(lang, "of");
            if (lang.Definite == null)
                lang.Definite = GetMorpheme(lang, "the");

            var counter = 0;

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
        public static Language MakeOrthoLanguage()
        {
            var lang = new Language();
            lang.NoOrtho = false;
            return lang;
        }
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
