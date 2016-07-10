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
    public class WordKmeans
    {
        private Dictionary<string, float[]> wordMap = null;

        private int iter;

        private Classes[] cArray = null;

        public WordKmeans(Dictionary<string, float[]> wordMap, int clcn, int iter)
        {
            this.wordMap = wordMap;
            this.iter = iter;
            cArray = new Classes[clcn];
        }

        public Classes[] explain()
        {
            //取前clcn个点
            int n = 0;
            foreach (KeyValuePair<string, float[]> item in wordMap)
            {
                if (n < cArray.Length)
                    cArray[n++] = new Classes(n, item.Value);
                else
                    break;
            }

            for (int i = 0; i < iter; i++)
            {
                foreach (Classes classes in cArray)
                {
                    classes.clean();
                }
                foreach (KeyValuePair<string, float[]> item in wordMap)
                {
                    double miniScore = Double.MaxValue;
                    double tempScore;
                    int classesId = 0;
                    foreach (Classes classes in cArray)
                    {
                        tempScore = classes.distance(item.Value);
                        if (miniScore > tempScore)
                        {
                            miniScore = tempScore;
                            classesId = classes.id;
                        }
                    }
                    cArray[classesId].putValue(item.Key, miniScore);
                }

                foreach (Classes classes in cArray)
                {
                    classes.updateCenter(wordMap);
                }
                Console.WriteLine("iter " + i + " ok!");
            }

            return cArray;
        }

        public class Classes
        {
            internal int id;

            internal float[] center;

            public Classes(int id, float[] center)
            {
                this.id = id;
                this.center = (float[])center.Clone();
            }

            Dictionary<String, Double> values = new Dictionary<string, double>();

            public double distance(float[] value)
            {
                double sum = 0;
                for (int i = 0; i < value.Length; i++)
                {
                    sum += (center[i] - value[i]) * (center[i] - value[i]);
                }
                return sum;
            }

            public void putValue(String word, double score)
            {
                values.Add(word, score);
            }

            /// <summary>
            /// 重新计算中心点
            /// </summary>
            /// <param name="wordMap"></param>
            public void updateCenter(Dictionary<String, float[]> wordMap)
            {
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = 0;
                }
                float[] value = null;
                foreach (String keyWord in values.Keys)
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
            public void clean()
            {
                values.Clear();
            }

            /// <summary>
            /// 取得每个类别的前n个结果
            /// </summary>
            /// <param name="n"></param>
            /// <returns></returns>
            public List<KeyValuePair<String, Double>> getTop(int n)
            {
                List<KeyValuePair<String, Double>> arrayList = values.ToList();
                arrayList.Sort(
                    delegate (KeyValuePair<String, Double> x, KeyValuePair<String, Double> y) 
                    { return x.Value > y.Value ? 1 : -1; }
                );
                int min = Math.Min(n, arrayList.Count - 1);
                if (min <= 1) return new List<KeyValuePair<string, double>>();
                //return arrayList.subList(0, min);
                return arrayList;

            }

        }

    }
}
