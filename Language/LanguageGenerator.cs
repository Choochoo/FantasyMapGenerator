using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Language
{
    public class LanguageGenerator
    {
        //private static readonly Random Random = new Random(200);
        private static double[] arrayDoubles =
        {
            0.11861134297243359,
            0.0859232856314478,
            0.8094547136993859,
            0.20318095652985946,
            0.7320816582294085,
            0.4013082538290318,
            0.20462041703923228,
            0.26343005048228707,
            0.8764383401252938,
            0.5412671933969304,
            0.8176661397435248,
            0.5422033394469694,
            0.034406518292599886,
            0.16306678055973545,
            0.6417936710495316,
            0.372624977397215,
            0.21169715930666522,
            0.7807465848009727,
            0.7444490369049139,
            0.6036506043084864,
            0.37217122357856036,
            0.5352198755469855,
            0.7934759111819809,
            0.1298794313991336,
            0.34990951463795605,
            0.32103956237686826,
            0.12097217246704339,
            0.1828231619998899,
            0.5179004098222801,
            0.4502286747288684,
            0.15213855621002992,
            0.8967170023333719,
            0.1622162537882832,
            0.8651235811213998,
            0.12557963384387327,
            0.8490577512267654,
            0.3498855992110552,
            0.8033731379200002,
            0.11533535809059359,
            0.19453285610997906,
            0.6040492751387312,
            0.2432111151579921,
            0.9012631467212224,
            0.6489022925808758,
            0.7697426034340753,
            0.2645778503799925,
            0.726137312507805,
            0.6358185276825583,
            0.10408723925429486,
            0.9367558303728774,
            0.9273030777522913,
            0.252799062608255,
            0.6435061924761258,
            0.7894589127352141,
            0.19758980355938527,
            0.5696250603727753,
            0.10381920838245207,
            0.6659772555490706,
            0.9416469807177859,
            0.7575793766696495,
            0.3731613681261987,
            0.3356780743725407,
            0.6137625759282734,
            0.006604106770820906,
            0.5886809590960003,
            0.5080286189084364,
            0.28077872583748964,
            0.6923784217568683,
            0.23260841676887112,
            0.6731685576095483,
            0.7649232397745154,
            0.16819703102841688,
            0.09457483123205246,
            0.2939209721926943,
            0.3263268588957442,
            0.9056166234077996,
            0.6330871764157535,
            0.8204645095780112,
            0.3965546046497255,
            0.7898963946463318,
            0.21975226212463284,
            0.6181660667835187,
            0.9109867093421691,
            0.04707476416957457,
            0.6356164806759224,
            0.14296954067673262,
            0.13699305254353256,
            0.2993384729731574,
            0.4244163335274449,
            0.7592057737127567,
            0.6066791145178825,
            0.42114322898542045,
            0.7695262398087097,
            0.05529522186415603,
            0.5039077752681973,
            0.29357840292058013,
            0.42726655459312957,
            0.1599778933398699,
            0.9873687332833208,
            0.6134987111205943,
            0.5843091051582299,
            0.47299915594725084,
            0.12414118371888372,
            0.3350836103046688,
            0.4963266899437013,
            0.4882286377816125,
            0.5458446055444321,
            0.7834110546195094,
            0.3194268964901632,
            0.026145397925010583,
            0.8197057103689069,
            0.6535712911592761,
            0.7955007509047824,
            0.13996326880247012,
            0.4782178536700843,
            0.48283280804781503,
            0.3810205905393673,
            0.975935572062204,
            0.6326158092756653,
            0.7836558963460432,
            0.8728297621801526,
            0.723680213125456,
            0.6532611453690065,
            0.5810966054193523,
            0.05343641252788167,
            0.6635245324563304,
            0.2027116554100068,
            0.045425833571128615,
            0.2717254946873289,
            0.38138763126597364,
            0.6251507390164612,
            0.813954436721614,
            0.5271190901505285,
            0.6161292691112472,
            0.5199554665219357,
            0.19700630552887421,
            0.7975284300018683,
            0.548483125054045,
            0.9720877672786599,
            0.6607124490542842,
            0.5027175929672472,
            0.6935029178783414,
            0.14935263275697608,
            0.5095452884447442,
            0.7838012630783366,
            0.31608343240264847,
            0.8502493284757522,
            0.9616042924894503,
            0.8164877880227555,
            0.72377523141346,
            0.4403752736810713,
            0.29250843149536787,
            0.8922757412153386,
            0.4960775824936947,
            0.05376548419465199,
            0.06990677139527124,
            0.9974576275735352,
            0.5378973997988981,
            0.6074394076495442,
            0.7215827431512618,
            0.9378732637426956,
            0.23163079959770294,
            0.7074640275704915,
            0.9178098966232087,
            0.5519791356269237,
            0.955385006482234,
            0.30392645500314863,
            0.8827548587683482,
            0.9094219713906588,
            0.26769980972629925,
            0.09040544737339351,
            0.16373879924293555,
            0.9848376415469802,
            0.4120127664306914,
            0.8032502347525676,
            0.9845644549133588,
            0.48912187722848,
            0.911999417679247,
            0.7226776879594867,
            0.576413829994604,
            0.12896033129401907,
            0.7015263636861548,
            0.3168334660971106,
            0.6184681757972834,
            0.5578623122189414,
            0.6613525363479171,
            0.24550372912956475,
            0.8721853755084348,
            0.18890806291012363,
            0.4101809754741681,
            0.7822889443289358,
            0.43354976653005806,
            0.41197254991984167,
            0.3504242133020623,
            0.030872922787522183,
            0.15154821789959794,
            0.08972797916640296,
            0.4019318196971804,
            0.6456199561968148,
            0.5220955967642182
        };
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
            ChooseCount = ChooseCount + 1 == arrayDoubles.Length ? 0 : ChooseCount + 1;

            return (int)Math.Floor(Math.Pow(arrayDoubles[ChooseCount], exponent) * listLength);
        }
        private static int ChooseCount = 0;
        private static int RandRange(double lo, double? hi = null)
        {
            if (!hi.HasValue)
            {
                hi = lo;
                lo = 0f;
            }
            ChooseCount = ChooseCount + 1 == arrayDoubles.Length ? 0 : ChooseCount + 1;
            return (int)(Math.Floor(arrayDoubles[ChooseCount] * (hi.Value - lo)) + lo);
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
                        ChooseCount = ChooseCount + 1 == arrayDoubles.Length ? 0 : ChooseCount + 1;
                        if (arrayDoubles[ChooseCount] < 0.5)
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
                counter = counter + 1 == arrayDoubles.Length ? 0 : counter + 1;
                var name = string.Empty;
                if (arrayDoubles[counter] < 0.5)
                {
                    name = Capitalize(GetWord(lang, key));
                }
                else
                {
                    var w1 = Capitalize(GetWord(lang, arrayDoubles[counter] < 0.6 ? key : string.Empty));
                    var w2 = Capitalize(GetWord(lang, arrayDoubles[arrayDoubles.Length - counter] < 0.6 ? key : string.Empty));
                    if (w1 == w2) continue;

                    if (arrayDoubles[counter] > 0.5)
                    {
                        name = Join(new[] { w1, w2 }, lang.Joiner.ToString());
                    }
                    else
                    {
                        name = Join(new[] { w1, lang.Genitive, w2 }, lang.Joiner.ToString());
                    }
                }

                if (arrayDoubles[counter] < 0.1)
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
