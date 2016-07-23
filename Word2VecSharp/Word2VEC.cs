using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class Word2Vec
    {
        #region 字段

        private static int MAX_SIZE = 50;
        /// <summary>
        /// 词向量
        /// </summary>
        public Dictionary<string, float[]> wordMap = new Dictionary<string, float[]>();
        public int words;
        public int size;
        public int topN = 10;

        #endregion

        #region 公共

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="path">模型的路径</param>
        public void LoadGoogleModel(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);
                double len = 0;
                float vector = 0;
                //读取词数及大小
                words = int.Parse(readString(reader));
                size = int.Parse(readString(reader));
                string word;
                float[] vectors = null;
                for (int i = 0; i < words; i++)
                {
                    word = readString(reader);
                    vectors = new float[size];
                    len = 0;
                    for (int j = 0; j < size; j++)
                    {
                        vector = reader.ReadSingle();
                        len += vector * vector;
                        vectors[j] = vector;
                    }
                    len = Math.Sqrt(len);

                    for (int j = 0; j < size; j++)
                    {
                        vectors[j] = (float)(vectors[j] / len);
                    }

                    wordMap.Add(word, vectors);
                    reader.Read();
                }
                reader.Close();
                fs.Close();
            }
                
        }

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
                float vector = 0;

                string key = null;
                float[] value = null;
                for (int i = 0; i < words; i++)
                {
                    double len = 0;
                    key = reader.ReadString();
                    value = new float[size];
                    for (int j = 0; j < size; j++)
                    {
                        vector = reader.ReadSingle();
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

        /// <summary>
        /// 近义词
        /// </summary>
        /// <param name="w0"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        public HashSet<WordEntry> Analogy(string w0, string w1, string w2)
        {
            float[] wv0 = wordMap[w0];
            float[] wv1 = wordMap[w1];
            float[] wv2 = wordMap[w2];

            if (wv1 == null || wv2 == null || wv0 == null)
                return null;

            float[] wordVector = new float[size];
            for (int i = 0; i < size; i++)
            {
                wordVector[i] = wv1[i] - wv0[i] + wv2[i];
            }
            float[] tempVector;
            string name;
            List<WordEntry> wordEntrys = new List<WordEntry>(topN);
            foreach (KeyValuePair<string, float[]> entry in wordMap)
            {
                name = entry.Key;
                if (name.Equals(w0) || name.Equals(w1) || name.Equals(w2))
                    continue;

                float dist = 0;
                tempVector = entry.Value;
                for (int i = 0; i < wordVector.Length; i++)
                {
                    dist += wordVector[i] * tempVector[i];
                }
                insertTopN(name, dist, wordEntrys);
            }
            return new HashSet<WordEntry>(wordEntrys);
        }

        public SortedSet<WordEntry> Distance(string word)
        {
            float[] center = wordMap[word];
            if (center == null)
                return null;

            int resultSize = wordMap.Count < topN ? wordMap.Count : topN;
            SortedSet<WordEntry> result = new SortedSet<WordEntry>();

            double min = float.MinValue;
            foreach (KeyValuePair<string, float[]> entry in wordMap)
            {
                float[] vector = entry.Value;
                float dist = 0;
                for (int i = 0; i < vector.Length; i++)
                {
                    dist += center[i] * vector[i];
                }

                if (dist > min)
                {
                    result.Add(new WordEntry(entry.Key, dist));
                    if (resultSize < result.Count)
                    {
                        //result.pollLast();

                        result.ElementAt(result.Count - 1).isUsed = true;
                        result.RemoveWhere(item => item.isUsed == true);
                    }
                    min = result.Last().score;
                }
            }
            //result.pollFirst();
            //result.ElementAt(0).isUsed = true;
            //result.RemoveWhere(item => item.isUsed == true);

            return result;
        }

        public HashSet<WordEntry> Distance(List<string> words)
        {
            float[] center = null;
            foreach (String word in words)
            {
                center = Sum(center, wordMap[word]);
            }

            if (center == null)
            {
                return new HashSet<WordEntry>();
            }

            int resultSize = wordMap.Count < topN ? wordMap.Count : topN;
            HashSet<WordEntry> result = new HashSet<WordEntry>();

            double min = float.MinValue;
            foreach (KeyValuePair<String, float[]> entry in wordMap)
            {
                float[] vector = entry.Value;
                float dist = 0;
                for (int i = 0; i < vector.Length; i++)
                {
                    dist += center[i] * vector[i];
                }

                if (dist > min)
                {
                    //result.AddAfter(new WordEntry(entry.Key, dist));
                    //if (resultSize < result.Count)
                    //{
                    //    result.pollLast();
                    //}
                    //min = result.last().score;
                }
            }
            //result.pollFirst();

            return result;
        }

        #endregion

        #region 私有

        private void insertTopN(string name, float score, List<WordEntry> wordsEntrys)
        {
            if (wordsEntrys.Count < topN)
            {
                wordsEntrys.Add(new WordEntry(name, score));
                return;
            }
            float min = float.MaxValue;
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

        private float[] Sum(float[] center, float[] fs)
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

        /// <summary>
        /// 读取一个字符串
        /// </summary>
        /// <param name="dis"></param>
        /// <returns></returns>
        private static string readString(BinaryReader dis)
        {
            byte[] bytes = new byte[MAX_SIZE];
            int b = dis.ReadByte();
            int i = -1;
            StringBuilder sb = new StringBuilder();
            while (b != 32 && b != 10)
            {
                i++;
                bytes[i] = (byte)b;
                b = dis.ReadByte();
                if (i == 49)
                {
                    sb.Append(Encoding.UTF8.GetString(bytes));
                    i = -1;
                    bytes = new byte[MAX_SIZE];
                }
            }
            //sb.Append(new String(bytes, 0, i + 1));
            sb.Append(Encoding.UTF8.GetString(bytes, 0, i + 1));
            return sb.ToString();

        }

        #endregion

    }
}
