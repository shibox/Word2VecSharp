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
        public bool isUsed;

        public WordEntry(string name, float score)
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
            if (o.isUsed == true)
                return 0;
            if (score < o.score)
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
