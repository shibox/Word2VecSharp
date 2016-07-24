using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public interface IWord2Vec
    {
        void LoadModel(string path);
        TreeSet Distance(string word);
        TreeSet Distance(string word,int count=20);
    }
}
