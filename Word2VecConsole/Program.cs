using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word2VecSharp;
using static Word2VecSharp.WordKmeans;

namespace Word2VecConsole
{
    class Program
    {
        private static string sportCorpusFile = "D:\\weiwei\\OSS\\result.txt";

        static void Main(string[] args)
        {
            //进行分词训练
            Learn lean = new Learn();
            lean.learnFile(sportCorpusFile);
            lean.saveModel("D:\\weiwei\\OSS\\vector.mod");

            //加载测试
            Word2Vec w2v = new Word2Vec();
            w2v.loadJavaModel("D:\\weiwei\\OSS\\vector.mod");
            Console.WriteLine(w2v.distance("执行"));
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
            Word2Vec vec = new Word2Vec();
            vec.loadGoogleModel("vectors.bin");
            Console.WriteLine("load model ok!");
            WordKmeans wordKmeans = new WordKmeans(vec.getWordMap(), 50, 50);
            Classes[] explain = wordKmeans.explain();

            for (int i = 0; i < explain.Length; i++)
            {
                Console.WriteLine("--------" + i + "---------");
                Console.WriteLine(explain[i].getTop(10));
            }

        }
    }
}
