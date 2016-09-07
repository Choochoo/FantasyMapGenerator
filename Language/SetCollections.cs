using System.Collections.Generic;

namespace Language
{
    public static class SetCollections
    {
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
            {'ʔ', "‘"},
            {'A', "á"},
            {'E', "é"},
            {'I', "í"},
            {'O', "ó"},
            {'U', "ú"}
        };

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

        public static SSet[] Ssets =
        {
            new SSet("Just s", "s"),
            new SSet("s ʃ", "sʃ"),
            new SSet("s ʃ f", "sʃf")
        };

        public static LSet[] Lsets =
        {
            new LSet("r l", "rl"),
            new LSet("Just r", "r"),
            new LSet("Just l", "l"),
            new LSet("w j", "wj"),
            new LSet("r l w j", "rlwj")
        };

        public static FSet[] Fsets =
        {
            new FSet("m n", "mn"),
            new FSet("s k", "sk"),
            new FSet("m n ŋ", "mnŋ"),
            new FSet("s ʃ z ʒ", "sʃzʒ")
        };

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

        public static string[] Syllstructs = new[]
        {
            "CVC", "CVV?C", "CVVC?", "CVC?", "CV", "VC", "CVF", "C?VC", "CVF?", "CL?VC", "CL?VF", "S?CVC", "S?CVF",
            "S?CVC?", "C?VF", "C?VC?", "C?VF?", "C?L?VC", "VC", "CVL?C?", "C?VL?C", "C?VLC?"
        };

        public static ResSet[] Ressets =
        {
            new ResSet("None", new string[] {}),
            new ResSet("Double sounds", new string[] {"/(.)\\1/"}),
            new ResSet("Doubles and hard clusters", new string[] {"[sʃf][sʃ]", "(.)\\1", "[rl][rl]"}),
        };

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
