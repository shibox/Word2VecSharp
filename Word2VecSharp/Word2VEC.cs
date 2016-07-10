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
        private Dictionary<String, float[]> wordMap = new Dictionary<string, float[]>();

        private int words;
        private int size;
        private int topNSize = 40;

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="path">模型的路径</param>
        public void loadGoogleModel(String path)
        {
            //  DataInputStream dis = null;
            //  BufferedInputStream bis = null;
            //double len = 0;
            //float vector = 0;
            //try {
            //      bis = new BufferedInputStream(new FileInputStream(path));
            //      dis = new DataInputStream(bis);
            //      // //读取词数
            //      words = Integer.parseInt(readString(dis));
            //      // //大小
            //      size = Integer.parseInt(readString(dis));
            //      String word;
            //      float[] vectors = null;
            //      for (int i = 0; i < words; i++)
            //      {
            //          word = readString(dis);
            //          vectors = new float[size];
            //          len = 0;
            //          for (int j = 0; j < size; j++)
            //          {
            //              vector = readFloat(dis);
            //              len += vector * vector;
            //              vectors[j] = (float)vector;
            //          }
            //          len = Math.sqrt(len);

            //          for (int j = 0; j < size; j++)
            //          {
            //              vectors[j] /= len;
            //          }

            //          wordMap.put(word, vectors);
            //          dis.read();
            //      }
            //  } finally {
            //      bis.close();
            //      dis.close();
            //  }

            BinaryReader dis = new BinaryReader(new FileStream(path, FileMode.Open));
            double len = 0;
            float vector = 0;
            try
            {
                //读取词数
                words = int.Parse(readString(dis));
                //大小
                size = int.Parse(readString(dis));
                String word;
                float[] vectors = null;
                for (int i = 0; i < words; i++)
                {
                    word = readString(dis);
                    vectors = new float[size];
                    len = 0;
                    for (int j = 0; j < size; j++)
                    {
                        vector = readFloat(dis);
                        len += vector * vector;
                        vectors[j] = (float)vector;
                    }
                    len = Math.Sqrt(len);

                    for (int j = 0; j < size; j++)
                    {
                        vectors[j] = (float)(vectors[j] / len);
                    }

                    wordMap.Add(word, vectors);
                    dis.Read();
                }
            }
            finally
            {
                dis.Close();
            }
        }

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="path">模型的路径</param>
        public void loadJavaModel(String path)
        {
            BinaryReader dis = new BinaryReader(new FileStream(path, FileMode.Open), Encoding.UTF8);
            words = dis.ReadInt32();
            size = dis.ReadInt32();

            float vector = 0;

            String key = null;
            float[] value = null;
            for (int i = 0; i < words; i++)
            {
                double len = 0;
                key = dis.ReadString();
                value = new float[size];
                for (int j = 0; j < size; j++)
                {
                    vector = dis.ReadSingle();
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


        }

        private static int MAX_SIZE = 50;

        /// <summary>
        /// 近义词
        /// </summary>
        /// <param name="word0"></param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <returns></returns>
        public HashSet<WordEntry> analogy(String word0, String word1, String word2)
        {
            float[] wv0 = getWordVector(word0);
            float[] wv1 = getWordVector(word1);
            float[] wv2 = getWordVector(word2);

            if (wv1 == null || wv2 == null || wv0 == null)
            {
                return null;
            }
            float[] wordVector = new float[size];
            for (int i = 0; i < size; i++)
            {
                wordVector[i] = wv1[i] - wv0[i] + wv2[i];
            }
            float[] tempVector;
            String name;
            List<WordEntry> wordEntrys = new List<WordEntry>(topNSize);
            foreach (KeyValuePair<String, float[]> entry in wordMap)
            {
                name = entry.Key;
                if (name.Equals(word0) || name.Equals(word1) || name.Equals(word2))
                {
                    continue;
                }
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

        private void insertTopN(String name, float score, List<WordEntry> wordsEntrys)
        {
            if (wordsEntrys.Count < topNSize)
            {
                wordsEntrys.Add(new WordEntry(name, score));
                return;
            }
            float min = float.MaxValue;
            int minOffe = 0;
            for (int i = 0; i < topNSize; i++)
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

        public HashSet<WordEntry> distance(String queryWord)
        {
            float[] center = wordMap[queryWord];
            if (center == null)
            {
                return null;
            }

            int resultSize = wordMap.Count < topNSize ? wordMap.Count : topNSize;
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
                    result.Add(new WordEntry(entry.Key, dist));
                    if (resultSize < result.Count)
                    {
                        //result.pollLast();
                    }
                    min = result.Last().score;
                }
            }
            //result.pollFirst();

            return result;
        }

        public HashSet<WordEntry> distance(List<String> words)
        {
            float[] center = null;
            foreach (String word in words)
            {
                center = sum(center, wordMap[word]);
            }

            if (center == null)
            {
                return new HashSet<WordEntry>();
            }

            int resultSize = wordMap.Count < topNSize ? wordMap.Count : topNSize;
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

        private float[] sum(float[] center, float[] fs)
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
        /// 得到词向量
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public float[] getWordVector(String word)
        {
            return wordMap[word];
        }

        public static float readFloat(BinaryReader fs)
        {
            byte[] bytes = new byte[4];
            fs.Read(bytes, 0, bytes.Length);
            return getFloat(bytes);
        }

        /// <summary>
        /// 读取一个float
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float getFloat(byte[] b)
        {
            //int accum = 0;
            //accum = accum | (b[0] & 0xff) << 0;
            //accum = accum | (b[1] & 0xff) << 8;
            //accum = accum | (b[2] & 0xff) << 16;
            //accum = accum | (b[3] & 0xff) << 24;
            //return Float.intBitsToFloat(accum);
            return 0;
        }

        /// <summary>
        /// 读取一个字符串
        /// </summary>
        /// <param name="dis"></param>
        /// <returns></returns>
        private static String readString(BinaryReader dis)
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

        public int getTopNSize()
        {
            return topNSize;
        }

        public void setTopNSize(int topNSize)
        {
            this.topNSize = topNSize;
        }

        public Dictionary<String, float[]> getWordMap()
        {
            return wordMap;
        }

        public int getWords()
        {
            return words;
        }

        public int getSize()
        {
            return size;
        }

    }
}
