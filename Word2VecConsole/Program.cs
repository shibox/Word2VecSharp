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

            // Learn learn = new Learn();
            // learn.learnFile(new File("library/xh.txt"));
            // learn.saveModel(new File("library/javaSkip1"));

            Word2Vec vec = new Word2Vec();
            vec.loadJavaModel("library/javaSkip1");

            // System.out.println("中国" + "\t" +
            // Arrays.toString(vec.getWordVector("中国")));
            // ;
            // System.out.println("毛泽东" + "\t" +
            // Arrays.toString(vec.getWordVector("毛泽东")));
            // ;
            // System.out.println("足球" + "\t" +
            // Arrays.toString(vec.getWordVector("足球")));

            // Word2VEC vec2 = new Word2VEC();
            // vec2.loadGoogleModel("library/vectors.bin") ;
            //
            //


            //String str = "毛泽东";
            //      long start = System.currentTimeMillis();
            //for (int i = 0; i< 100; i++) {
            //	System.out.println(vec.distance(str));
            //	;
            //}
            //  System.out.println(System.currentTimeMillis() - start);

            //System.out.println(System.currentTimeMillis() - start);


            // System.out.println(vec2.distance(str));
            //
            //
            // //男人 国王 女人
            // System.out.println(vec.analogy("邓小平", "毛泽东思想", "毛泽东"));
            // System.out.println(vec2.analogy("毛泽东", "毛泽东思想", "邓小平"));
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
