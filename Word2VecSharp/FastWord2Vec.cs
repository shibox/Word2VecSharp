using java.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class FastWord2Vec:IWord2Vec
    {
        #region 字段

        private static int MAX_SIZE = 50;
        /// <summary>
        /// 词向量
        /// </summary>
        Dictionary<string, int> wordMap = null;
        private string[] wordList = null;
        private double[][] scoreList = null;
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
            //using (FileStream fs = new FileStream(path, FileMode.Open))
            //{
            //    BinaryReader reader = new BinaryReader(fs, Encoding.UTF8);
            //    words = reader.ReadInt32();
            //    size = reader.ReadInt32();
            //    float vector = 0;

            //    string key = null;
            //    float[] value = null;
            //    for (int i = 0; i < words; i++)
            //    {
            //        double len = 0;
            //        key = reader.ReadString();
            //        value = new float[size];
            //        for (int j = 0; j < size; j++)
            //        {
            //            vector = reader.ReadSingle();
            //            len += vector * vector;
            //            value[j] = vector;
            //        }
            //        len = Math.Sqrt(len);
            //        for (int j = 0; j < size; j++)
            //        {
            //            value[j] = (float)(value[j] / len);
            //        }
            //        wordMap.Add(key, value);
            //    }
            //    reader.Close();
            //    fs.Close();
            //}

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs, Encoding.UTF8);
                words = reader.ReadInt32();
                size = reader.ReadInt32();
                wordMap = new Dictionary<string, int>(words);
                wordList = new string[words];
                scoreList = new double[words][];
                //scoreList = new double[words*256];
                double vector = 0;
                string key = null;
                double[] value = null;
                for (int i = 0; i < words; i++)
                {
                    double len = 0;
                    key = reader.ReadString();
                    value = new double[size];
                    for (int j = 0; j < size; j++)
                    {
                        vector = reader.ReadDouble();
                        len += vector * vector;
                        value[j] = vector;
                    }
                    len = Math.Sqrt(len);
                    for (int j = 0; j < size; j++)
                    {
                        value[j] = (float)(value[j] / len);
                    }
                    wordMap.Add(key, i);
                    wordList[i] = key;
                    scoreList[i] = value;

                    //int of = i * 256;
                    //for (int n = 0; n < size; n++)
                    //    scoreList[of + n] = value[n];
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
            //    insertTopN(name, dist, wordEntrys);
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

            double[] center = scoreList[idx];
            int resultSize = wordMap.Count < topN ? wordMap.Count : topN;
            TreeSet result = new TreeSet();
            double min = double.MinValue;
            for (int n = 0; n < words; n++)
            {
                double[] vector = scoreList[n];
                double dist = 0;
                for (int i = 0; i < vector.Length; i += 4)
                {
                    dist += center[i] * vector[i];
                    dist += center[i + 1] * vector[i + 1];
                    dist += center[i + 2] * vector[i + 2];
                    dist += center[i + 3] * vector[i + 3];
                }
                if (dist > min)
                {
                    result.Add(new WordEntry(wordList[n], dist));
                    if (resultSize < result.size())
                    {
                        result.pollLast();
                    }
                    min = ((WordEntry)result.last()).score;
                }
            }
            result.pollFirst();
            return result;
        }

        #endregion


    }
}
