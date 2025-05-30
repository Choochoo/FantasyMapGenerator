﻿using System.Collections.Generic;

namespace Language
{
    /// <summary>
    /// Static class containing predefined collections of phoneme sets, orthographic mappings, and linguistic rules.
    /// Provides various language templates and configurations for fantasy language generation.
    /// </summary>
    public static class SetCollections
    {
        /// <summary>
        /// Default orthographic mappings for common phonetic symbols to their written representations.
        /// Maps special phonetic characters to their standard orthographic equivalents.
        /// </summary>
        public static Dictionary<char, string> DefaultOrtho = new Dictionary<char, string>
        {
            {'ʃ', "sh"},
            {'ʒ', "zh"},
            {'ʧ', "ch"},
            {'ʤ', "j"},
            {'ŋ', "ng"},
            {'j', "y"},
            {'x', "kh"},
            {'ɣ', "gh"},
            {'ʔ', "'"},
            {'A', "á"},
            {'E', "é"},
            {'I', "í"},
            {'O', "ó"},
            {'U', "ú"}
        };

        /// <summary>
        /// Array of predefined consonant sets representing different language families and styles.
        /// Each set contains consonants typical of specific linguistic traditions.
        /// </summary>
        public static Conset[] Consets =
        {
            new Conset("Minimal", "ptkmnls"),
            new Conset("English-ish", "ptkbdgmnlrsʃzʒʧ"),
            new Conset("Pirahã (very simple)", "ptkmnh"),
            new Conset("Hawaiian-ish", "hklmnpwʔ"),
            new Conset("Greenlandic-ish", "ptkqvsgrmnŋlj"),
            new Conset("Arabic-ish", "tksʃdbqɣxmnlrwj"),
            new Conset("Arabic-lite", "tkdgmnsʃ"),
            new Conset("English-lite", "ptkbdgmnszʒʧhjw")
        };

        /// <summary>
        /// Array of predefined sibilant sets containing fricative and sibilant sounds.
        /// Used for languages that distinguish between different types of sibilant consonants.
        /// </summary>
        public static SSet[] Ssets =
        {
            new SSet("Just s", "s"),
            new SSet("s ʃ", "sʃ"),
            new SSet("s ʃ f", "sʃf")
        };

        /// <summary>
        /// Array of predefined liquid consonant sets containing sounds like 'r', 'l', 'w', and 'j'.
        /// Liquids are consonants that can function similarly to vowels in syllable structure.
        /// </summary>
        public static LSet[] Lsets =
        {
            new LSet("r l", "rl"),
            new LSet("Just r", "r"),
            new LSet("Just l", "l"),
            new LSet("w j", "wj"),
            new LSet("r l w j", "rlwj")
        };

        /// <summary>
        /// Array of predefined fricative consonant sets containing various fricative sounds.
        /// Fricatives are consonants produced by forcing air through a narrow channel.
        /// </summary>
        public static FSet[] Fsets =
        {
            new FSet("m n", "mn"),
            new FSet("s k", "sk"),
            new FSet("m n ŋ", "mnŋ"),
            new FSet("s ʃ z ʒ", "sʃzʒ")
        };

        /// <summary>
        /// Array of predefined vowel sets representing different vowel systems.
        /// Includes both simple and complex vowel inventories with various accent marks.
        /// </summary>
        public static VowSet[] Vowsets =
        {
            new VowSet("Standard 5-vowel", "aeiou"),
            new VowSet("3-vowel a i u", "aiu"),
            new VowSet("Extra A E I", "aeiouAEI"),
            new VowSet("Extra U", "aeiouU"),
            new VowSet("5-vowel a i u A I", "aiuAI"),
            new VowSet("3-vowel e o u", "eou"),
            new VowSet("Extra A O U", "aeiouAOU")
        };

        /// <summary>
        /// Array of predefined syllable structure patterns for word generation.
        /// Each pattern defines how consonants (C), vowels (V), liquids (L), fricatives (F), and sibilants (S) combine.
        /// Optional elements are marked with '?' for probabilistic inclusion.
        /// </summary>
        public static string[] Syllstructs = new[]
        {
            "CVC", "CVV?C", "CVVC?", "CVC?", "CV", "VC", "CVF", "C?VC", "CVF?", "CL?VC", "CL?VF", "S?CVC", "S?CVF",
            "S?CVC?", "C?VF", "C?VC?", "C?VF?", "C?L?VC", "VC", "CVL?C?", "C?VL?C", "C?VLC?"
        };

        /// <summary>
        /// Array of predefined phoneme restriction sets that prevent certain sound combinations.
        /// Used to avoid unrealistic or difficult-to-pronounce phoneme sequences in generated words.
        /// </summary>
        public static ResSet[] Ressets =
        {
            new ResSet("None", new string[] {}),
            new ResSet("Double sounds", new string[] {"/(.)\\1/"}),
            new ResSet("Doubles and hard clusters", new string[] {"[sʃf][sʃ]", "(.)\\1", "[rl][rl]"}),
        };

        /// <summary>
        /// Array of predefined consonant orthographic mapping sets for different writing systems.
        /// Each set defines how consonant sounds are represented in specific linguistic traditions.
        /// </summary>
        public static CorthSet[] CorthSets =
        {
            new CorthSet("Default", new Dictionary<char, string>()),
            new CorthSet("Slavic", new Dictionary<char, string>
            {
                {'ʃ', "š"},
                {'ʒ', "ž"},
                {'ʧ', "č"},
                {'ʤ', "ǧ"},
                {'j', "j"}
            }),
            new CorthSet("German", new Dictionary<char, string>
            {
                {'ʃ', "sch"},
                {'ʒ', "zh"},
                {'ʧ', "tsch"},
                {'ʤ', "dz"},
                {'j', "j"},
                {'x', "ch"}
            }),
            new CorthSet("French", new Dictionary<char, string>
            {
                {'ʃ', "ch"},
                {'ʒ', "j"},
                {'ʧ', "tch"},
                {'ʤ', "dj"},
                {'x', "kh"}
            }),
            new CorthSet("Chinese (pinyin)", new Dictionary<char, string>
            {
                {'ʃ', "x"},
                {'ʧ', "q"},
                {'ʤ', "j"}
            })
        };

        /// <summary>
        /// Array of predefined vowel orthographic mapping sets for different writing systems.
        /// Each set defines how vowel sounds are represented with diacritics and special characters.
        /// </summary>
        public static VorthSet[] VorthSets =
        {
            new VorthSet("Ácutes", new Dictionary<char, string>()),
            new VorthSet("Ümlauts", new Dictionary<char, string>
            {
                {'A', "ä"},
                {'E', "ë"},
                {'I', "ï"},
                {'O', "ö"},
                {'U', "ü"}
            }),
            new VorthSet("Welsh", new Dictionary<char, string>
            {
                {'A', "â"},
                {'E', "ê"},
                {'I', "y"},
                {'O', "ô"},
                {'U', "w"}
            }),
            new VorthSet("Diphthongs", new Dictionary<char, string>
            {
                {'A', "au"},
                {'E', "ei"},
                {'I', "ie"},
                {'O', "ou"},
                {'U', "oo"}
            }),
            new VorthSet("Doubles", new Dictionary<char, string>
            {
                {'A', "aa"},
                {'E', "ee"},
                {'I', "ii"},
                {'O', "oo"},
                {'U', "uu"}
            })
        };
    }
}
