using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word2VecSharp;
using static Word2VecSharp.WordKMeans;

namespace Word2VecConsole
{
    class Program
    {
        //private static string file = "D:\\weiwei\\OSS\\result.txt";
        //private static string file = "D:\\weiwei\\Java\\CorpuAll.txt";
        private static string file = "D:\\weiwei\\Java\\corpus_big.txt";
        

        static void Main(string[] args)
        {
            string output = "vector.mod";
            if (File.Exists(output) == false)
            {
                //进行分词训练
                Learn lean = new Learn();
                lean.LearnFile(file);
                lean.SaveModel(output);
            }

            //加载测试
            //Word2Vec w2v = new Word2Vec();
            //FastWord2Vec w2v = new FastWord2Vec();
            FastestWord2Vec w2v = new FastestWord2Vec();
            w2v.LoadModel(output);

            //Console.WriteLine(JsonConvert.SerializeObject(w2v.distance("算法"), Formatting.Indented));
            //Console.WriteLine(JsonConvert.SerializeObject(w2v.distance("群众/n")));

            Stopwatch w = Stopwatch.StartNew();
            object o = w2v.Distance("群众/n");
            //List<WordEntry> o = w2v.DistanceAll("群众/n");
            //List<WordEntry> o = w2v.DistanceAll("中国/ns");
            w.Stop();
            File.WriteAllText("result.txt", JsonConvert.SerializeObject(o, Formatting.Indented));
            Console.WriteLine(w.ElapsedMilliseconds);
            //Console.WriteLine(o);
            Console.ReadLine();
        }

        public static void Word2VecTest(String[] args)
        {

            //Learn learn = new Learn();
            //learn.learnFile("library/xh.txt");
            //learn.saveModel("library/javaSkip1");

            //Word2Vec vec = new Word2Vec();
            //vec.loadJavaModel("library/javaSkip1");
            //Console.WriteLine("中国" + "\t" + JsonConvert.SerializeObject(vec.getWordVector("中国")));
            //Console.WriteLine("毛泽东" + "\t" +JsonConvert.SerializeObject(vec.getWordVector("毛泽东")));
            //Console.WriteLine("足球" + "\t" +JsonConvert.SerializeObject(vec.getWordVector("足球")));
            //Word2Vec vec2 = new Word2Vec();
            //vec2.loadGoogleModel("library/vectors.bin");

            //String str = "毛泽东";
            //for (int i = 0; i < 100; i++)
            //{
            //    Console.WriteLine(vec.distance(str));
            //}
            //Console.WriteLine(vec2.distance(str));


            //男人 国王 女人
            //Console.WriteLine(vec.analogy("邓小平", "毛泽东思想", "毛泽东"));
            //Console.WriteLine(vec2.analogy("毛泽东", "毛泽东思想", "邓小平"));
        }

        public static void WordKmeansTest(String[] args)
        {
            //Word2Vec vec = new Word2Vec();
            //vec.LoadGoogleModel("vectors.bin");
            //Console.WriteLine("load model ok!");
            //WordKMeans wordKmeans = new WordKMeans(vec.wordMap, 50, 50);
            //Classes[] explain = wordKmeans.Explain();

            //for (int i = 0; i < explain.Length; i++)
            //{
            //    Console.WriteLine("--------" + i + "---------");
            //    Console.WriteLine(explain[i].GetTop(10));
            //}
        }


    }
}
