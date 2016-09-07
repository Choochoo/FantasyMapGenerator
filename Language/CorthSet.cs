using System.Collections.Generic;

namespace Language
{
    public class CorthSet : Set
    {
        public Dictionary<char, string> Ortho;
        public CorthSet(string name, Dictionary<char, string> ortho)
        {
            Name = name;
            Ortho = ortho;
        }
    }
}
