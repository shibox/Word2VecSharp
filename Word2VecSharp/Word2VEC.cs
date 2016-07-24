using java.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class Word2Vec : IWord2Vec
    {
        #region 字段

        private static int MAX_SIZE = 50;
        /// <summary>
        /// 词向量
        /// </summary>
        public Dictionary<string, double[]> wordMap = new Dictionary<string, double[]>();
        public int words;
        public int size;
        public int topN = 40;

        #endregion

        #region 公共

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="path">模型的路径</param>
        public void LoadGoogleModel(string path)
        {
            //using (FileStream fs = new FileStream(path, FileMode.Open))
            //{
            //    BinaryReader reader = new BinaryReader(fs);
            //    double len = 0;
            //    float vector = 0;
            //    //读取词数及大小
            //    words = int.Parse(readString(reader));
            //    size = int.Parse(readString(reader));
            //    string word;
            //    float[] vectors = null;
            //    for (int i = 0; i < words; i++)
            //    {
            //        word = readString(reader);
            //        vectors = new float[size];
            //        len = 0;
            //        for (int j = 0; j < size; j++)
            //        {
            //            vector = reader.ReadSingle();
            //            len += vector * vector;
            //            vectors[j] = vector;
            //        }
            //        len = Math.Sqrt(len);

            //        for (int j = 0; j < size; j++)
            //        {
            //            vectors[j] = (float)(vectors[j] / len);
            //        }

            //        wordMap.Add(word, vectors);
            //        reader.Read();
            //    }
            //    reader.Close();
            //    fs.Close();
            //}
                
        }

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
                    wordMap.Add(key, value);
                }
                reader.Close();
                fs.Close();
            }
        }

        public void LoadModelFromTxt(string path)
        {
            StreamReader reader = new StreamReader(path);
            words = int.Parse(reader.ReadLine());
            size = int.Parse(reader.ReadLine());
            float vector = 0;
            string key = null;
            double[] value = null;
            for (int i = 0; i < words; i++)
            {
                double len = 0;
                key = reader.ReadLine();
                value = new double[size];
                string[] s = reader.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < size; j++)
                {
                    vector = float.Parse(s[j]);
                    len += vector * vector;
                    value[j] = vector;
                }
                len = Math.Sqrt(len);
                for (int j = 0; j < size; j++)
                {
                    value[j] = (float)(value[j] / len);
                }
                wordMap.Add(key, value);
            }
            reader.Close();
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
            double[] wv0 = wordMap[w0];
            double[] wv1 = wordMap[w1];
            double[] wv2 = wordMap[w2];

            if (wv1 == null || wv2 == null || wv0 == null)
                return null;

            double[] wordVector = new double[size];
            for (int i = 0; i < size; i++)
            {
                wordVector[i] = wv1[i] - wv0[i] + wv2[i];
            }
            double[] tempVector;
            string name;
            List<WordEntry> wordEntrys = new List<WordEntry>(topN);
            foreach (KeyValuePair<string, double[]> entry in wordMap)
            {
                name = entry.Key;
                if (name.Equals(w0) || name.Equals(w1) || name.Equals(w2))
                    continue;

                double dist = 0;
                tempVector = entry.Value;
                for (int i = 0; i < wordVector.Length; i++)
                {
                    dist += wordVector[i] * tempVector[i];
                }
                InsertTopN(name, dist, wordEntrys);
            }
            return new HashSet<WordEntry>(wordEntrys);
        }

        public TreeSet Distance(string word)
        {
            double[] center = null;
            if (wordMap.TryGetValue(word, out center) == false)
                return null;
            int resultSize = wordMap.Count < topN ? wordMap.Count : topN;
            TreeSet result = new TreeSet();
            double min = double.MinValue;
            foreach (KeyValuePair<string, double[]> entry in wordMap)
            {
                double[] vector = entry.Value;
                double dist = 0;
                for (int i = 0; i < vector.Length; i+=4)
                {
                    dist += center[i] * vector[i];
                    dist += center[i + 1] * vector[i + 1];
                    dist += center[i + 2] * vector[i + 2];
                    dist += center[i + 3] * vector[i + 3];
                }

                if (dist > min)
                {
                    result.Add(new WordEntry(entry.Key, dist));
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

        public TreeSet Distance(List<string> words)
        {
            double[] center = null;
            foreach (string word in words)
            {
                center = Sum(center, wordMap[word]);
            }

            if (center == null)
            {
                return new TreeSet();
            }

            int resultSize = wordMap.Count < topN ? wordMap.Count : topN;
            TreeSet result = new TreeSet();

            double min = float.MinValue;
            foreach (KeyValuePair<string, double[]> entry in wordMap)
            {
                double[] vector = entry.Value;
                double dist = 0;
                for (int i = 0; i < vector.Length; i++)
                {
                    dist += center[i] * vector[i];
                }

                if (dist > min)
                {
                    result.add(new WordEntry(entry.Key, dist));
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

        #region 私有

        private void InsertTopN(string name, double score, List<WordEntry> wordsEntrys)
        {
            if (wordsEntrys.Count < topN)
            {
                wordsEntrys.Add(new WordEntry(name, score));
                return;
            }
            double min = double.MaxValue;
            int minOffe = 0;
            for (int i = 0; i < topN; i++)
            {
                WordEntry wordEntry = wordsEntrys[i];
                if (min > wordEntry.score)
                {
                    min = wordEntry.score;
                    minOffe = i;
                }
            }
            if (score > min)
            {
                wordsEntrys[minOffe] = new WordEntry(name, score);
            }
        }

        private double[] Sum(double[] center, double[] fs)
        {
            if (center == null && fs == null)
            {
                return null;
            }

            if (fs == null)
            {
                return center;
            }

            if (center == null)
            {
                return fs;
            }

            for (int i = 0; i < fs.Length; i++)
            {
                center[i] += fs[i];
            }

            return center;
        }

        public TreeSet Distance(string word, int count = 20)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
