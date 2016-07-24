using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class WordEntry : IComparable<WordEntry>,IComparable
    {
        public string name;
        public double score;

        public WordEntry(string name, double score)
        {
            this.name = name;
            this.score = score;
        }


        public override string ToString()
        {
            return name + "\t" + score;
        }


        public int CompareTo(WordEntry o)
        {
            if (score < o.score)
                return 1;
            else
                return -1;
        }

        public int CompareTo(object obj)
        {
            WordEntry o = (WordEntry)obj;
            if (score < o.score)
                return 1;
            else
                return -1;
        }
    }
}
