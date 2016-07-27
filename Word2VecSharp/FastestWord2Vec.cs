using java.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    /// <summary>
    /// 最快的计算方式，使用向量计算及将数据整合到一个数组并使用单精度浮点数
    /// 由于训练数据存储在单一数组中，因而训练数据不能太大，如超过2G
    /// </summary>
    public class FastestWord2Vec:IWord2Vec
    {
        #region 字段

        private static int MAX_SIZE = 40;
        /// <summary>
        /// 词向量
        /// </summary>
        private Dictionary<string, int> wordMap = null;
        private string[] wordList = null;
        private float[] scoreList = null;
        public int words;
        public int size;
        public int topN = 40;

        #endregion

        #region 公共

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="path">模型的路径</param>
        public void LoadModel(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs, Encoding.UTF8);
                words = reader.ReadInt32();
                size = reader.ReadInt32();
                wordMap = new Dictionary<string, int>(words);
                wordList = new string[words];
                scoreList = new float[words * size];
                double vector = 0;
                string key = null;
                float[] value = new float[size];
                for (int i = 0; i < words; i++)
                {
                    double len = 0;
                    key = reader.ReadString();
                    for (int j = 0; j < size; j++)
                    {
                        vector = reader.ReadDouble();
                        len += vector * vector;
                        value[j] = (float)vector;
                    }
                    len = Math.Sqrt(len);
                    for (int j = 0; j < size; j++)
                    {
                        value[j] = (float)(value[j] / len);

                    }
                    wordMap.Add(key, i);
                    wordList[i] = key;
                    int of = i * size;
                    for (int n = 0; n < size; n++)
                        scoreList[of + n] = value[n];
                }
                reader.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 近义词
        /// </summary>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public HashSet<WordEntry> Analogy(string w0, string w1, string w2)
        {
            //float[] wv0 = wordMap[w0];
            //float[] wv1 = wordMap[w1];
            //float[] wv2 = wordMap[w2];

            //if (wv1 == null || wv2 == null || wv0 == null)
            //    return null;

            //float[] wordVector = new float[size];
            //for (int i = 0; i < size; i++)
            //{
            //    wordVector[i] = wv1[i] - wv0[i] + wv2[i];
            //}
            //float[] tempVector;
            //string name;
            //List<WordEntry> wordEntrys = new List<WordEntry>(topN);
            //foreach (KeyValuePair<string, float[]> entry in wordMap)
            //{
            //    name = entry.Key;
            //    if (name.Equals(w0) || name.Equals(w1) || name.Equals(w2))
            //        continue;

            //    float dist = 0;
            //    tempVector = entry.Value;
            //    for (int i = 0; i < wordVector.Length; i++)
            //    {
            //        dist += wordVector[i] * tempVector[i];
            //    }
            //    InsertTopN(name, dist, wordEntrys);
            //}
            //return new HashSet<WordEntry>(wordEntrys);

            return null;
        }

        public TreeSet Distance(string word)
        {
            return Distance(word, topN);
        }

        public TreeSet Distance(string word, int count = 20)
        {
            int idx = 0;
            if (wordMap.TryGetValue(word, out idx) == false)
                return null;

            float[] center = new float[size];
            Buffer.BlockCopy(scoreList, idx * size * 4, center, 0, center.Length * 4);
            int resultSize = wordMap.Count < count ? wordMap.Count : count;
            TreeSet result = new TreeSet();
            float min = float.MinValue;
            for (int n = 0; n < words; n++)
            {
                float dist = 0;
                int of = n * size;

                //乘法运算这里将使用simd的向量运算，提高性能
                //Vector4 v = new Vector4();
                //System.Numerics.Vector v = new System.Numerics.Vector();
                //for (int i = 0; i < size; i += 4)
                //{
                //    dist += center[i] * scoreList[of + i];
                //    dist += center[i + 1] * scoreList[of + i + 1];
                //    dist += center[i + 2] * scoreList[of + i + 2];
                //    dist += center[i + 3] * scoreList[of + i + 3];
                //}

                dist = Utils.MultiplySimd(center, scoreList, of);

                //if (dist > min)
                //{
                //    result.Add(new WordEntry(wordList[n], dist));
                //    if (resultSize < result.size())
                //    {
                //        result.pollLast();
                //    }
                //    min = (float)((WordEntry)result.last()).score;
                //}
            }
            result.pollFirst();
            return result;
        }

        public HashSet<WordEntry> Distance(List<string> words)
        {
            //float[] center = null;
            //foreach (String word in words)
            //{
            //    center = Sum(center, wordMap[word]);
            //}

            //if (center == null)
            //{
            //    return new HashSet<WordEntry>();
            //}

            //int resultSize = wordMap.Count < topN ? wordMap.Count : topN;
            //HashSet<WordEntry> result = new HashSet<WordEntry>();

            //double min = float.MinValue;
            //foreach (KeyValuePair<String, float[]> entry in wordMap)
            //{
            //    float[] vector = entry.Value;
            //    float dist = 0;
            //    for (int i = 0; i < vector.Length; i++)
            //    {
            //        dist += center[i] * vector[i];
            //    }

            //    if (dist > min)
            //    {
            //        //result.AddAfter(new WordEntry(entry.Key, dist));
            //        //if (resultSize < result.Count)
            //        //{
            //        //    result.pollLast();
            //        //}
            //        //min = result.last().score;
            //    }
            //}
            ////result.pollFirst();

            //return result;

            return null;
        }

        #endregion
    }
}
