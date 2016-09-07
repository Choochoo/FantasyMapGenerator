using System.Collections.Generic;

namespace Language
{
    public class VorthSet : Set
    {
        public Dictionary<char, string> Ortho;
        public VorthSet(string name, Dictionary<char, string> ortho)
        {
            Name = name;
            Ortho = ortho;
        }
    }
}
