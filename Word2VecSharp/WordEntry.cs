using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class WordEntry : IComparable<WordEntry>
    {
        public string name;
        public float score;

        public WordEntry(string name, float score)
        {
            this.name = name;
            this.score = score;
        }


        public override string ToString()
        {
            return this.name + "\t" + score;
        }


        public int CompareTo(WordEntry o)
        {
            if (this.score < o.score)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

    }
}
