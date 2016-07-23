using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    /// <summary>
    /// keanmeans聚类
    /// </summary>
    public class WordKMeans
    {
        private Dictionary<string, float[]> wordMap = null;

        private int iter;

        private Classes[] array = null;

        public WordKMeans(Dictionary<string, float[]> wordMap, int clcn, int iter)
        {
            this.wordMap = wordMap;
            this.iter = iter;
            array = new Classes[clcn];
        }

        public Classes[] Explain()
        {
            //取前clcn个点
            int n = 0;
            foreach (KeyValuePair<string, float[]> item in wordMap)
            {
                if (n < array.Length)
                    array[n++] = new Classes(n, item.Value);
                else
                    break;
            }

            for (int i = 0; i < iter; i++)
            {
                foreach (Classes item in array)
                {
                    item.Clean();
                }
                foreach (KeyValuePair<string, float[]> item in wordMap)
                {
                    double miniScore = Double.MaxValue;
                    double tempScore;
                    int classesId = 0;
                    foreach (Classes classes in array)
                    {
                        tempScore = classes.Distance(item.Value);
                        if (miniScore > tempScore)
                        {
                            miniScore = tempScore;
                            classesId = classes.id;
                        }
                    }
                    array[classesId].PutValue(item.Key, miniScore);
                }

                foreach (Classes classes in array)
                {
                    classes.UpdateCenter(wordMap);
                }
                Console.WriteLine("iter " + i + " ok!");
            }

            return array;
        }

        public class Classes
        {
            Dictionary<string, double> values = new Dictionary<string, double>();
            internal float[] center;
            internal int id;

            public Classes(int id, float[] center)
            {
                this.id = id;
                this.center = (float[])center.Clone();
            }

            public double Distance(float[] value)
            {
                double sum = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    sum += (center[i] - value[i]) * (center[i] - value[i]);
                }
                return sum;
            }

            public void PutValue(string word, double score)
            {
                values.Add(word, score);
            }

            /// <summary>
            /// 重新计算中心点
            /// </summary>
            /// <param name="wordMap"></param>
            public void UpdateCenter(Dictionary<string, float[]> wordMap)
            {
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = 0;
                }
                float[] value = null;
                foreach (string keyWord in values.Keys)
                {
                    value = wordMap[keyWord];
                    for (int i = 0; i < value.Length; i++)
                    {
                        center[i] += value[i];
                    }
                }
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = center[i] / values.Count;
                }
            }

            /// <summary>
            /// 清空历史结果
            /// </summary>
            public void Clean()
            {
                values.Clear();
            }

            /// <summary>
            /// 取得每个类别的前n个结果
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public List<KeyValuePair<string, double>> GetTop(int n)
            {
                List<KeyValuePair<string, double>> arrayList = values.ToList();
                arrayList.Sort(
                    delegate (KeyValuePair<string, double> x, KeyValuePair<string, double> y) 
                    { return x.Value > y.Value ? 1 : -1; }
                );
                int min = Math.Min(n, arrayList.Count - 1);
                if (min <= 1) return new List<KeyValuePair<string, double>>();
                return arrayList.GetRange(0,min);

            }

        }

    }
}
