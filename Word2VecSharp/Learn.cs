using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class Learn
    {
        #region 私有

        public const int EXP_TABLE_SIZE = 1000;
        private Dictionary<String, WordNeuron> wordMap = new Dictionary<string, WordNeuron>();
        private double[] expTable = new double[EXP_TABLE_SIZE];
        private double[] neu1e = null;
        //private int trainWordsCount = 0;
        //private int MAX_EXP = 6;
        private double trainWordsCount = 0;
        private double MAX_EXP = 6;

        #endregion

        #region 公共

        /// <summary>
        /// 训练多少个特征
        /// </summary>
        public int layerSize = 200;
        /// <summary>
        /// 上下文窗口大小
        /// </summary>
        public int window = 5;
        public double sample = 1e-3;
        public double alpha = 0.025;
        public double startingAlpha = 0.025;
        public bool isCbow = false;
        double[] buffer = null;
        double[] swap = null;
        int layerBlockCount = 100;
        int curLayerBlockCount = 0;

        public long newArrayCount = 0;
        public long cCount = 0;
        public long jCount = 0;

        #endregion

        #region 构造函数

        public Learn()
        {
            CreateExpTable();
            neu1e = new double[layerSize];// 误差项

            buffer = new double[layerSize * layerBlockCount];
            swap = new double[layerSize];
        }

        public Learn(bool isCbow, int layerSize, int window, double alpha, double sample)
        {
            CreateExpTable();
            this.isCbow = isCbow;
            this.layerSize = layerSize;
            this.window = window;
            this.alpha = alpha;
            this.sample = sample;
            //this.neu1e = new double[layerSize];// 误差项

            buffer = new double[layerSize * layerBlockCount];
            swap = new double[layerSize];
        }

        #endregion

        #region 训练

        private void Reset()
        {
            for (int i = 0; i < layerBlockCount; i++)
                Buffer.BlockCopy(swap, 0, buffer, i * layerSize * 8, layerSize * 8);
        }

        /// <summary>
        /// trainModel
        /// </summary>
        /// <param name="file"></param>
        private void TrainModel(string file)
        {
            #region old
            //    try (BufferedReader br = new BufferedReader(new InputStreamReader(
            //        new FileInputStream(file)))) {
            //      String temp = null;
            //        long nextRandom = 5;
            //        int wordCount = 0;
            //        int lastWordCount = 0;
            //        int wordCountActual = 0;
            //while ((temp = br.readLine()) != null) {
            //  if (wordCount - lastWordCount > 10000) {
            //    System.out.println("alpha:" + alpha + "\tProgress: "
            //        + (int) (wordCountActual / (double) (trainWordsCount + 1) * 100)
            //        + "%");
            //    wordCountActual += wordCount - lastWordCount;
            //    lastWordCount = wordCount;
            //    alpha = startingAlpha
            //        * (1 - wordCountActual / (double) (trainWordsCount + 1));
            //    if (alpha<startingAlpha* 0.0001) {
            //      alpha = startingAlpha* 0.0001;
            //    }
            //}
            //String[] strs = temp.split(" ");
            //wordCount += strs.length;
            //        List<WordNeuron> sentence = new ArrayList<WordNeuron>();
            //        for (int i = 0; i<strs.length; i++) {
            //          Neuron entry = wordMap.get(strs[i]);
            //          if (entry == null) {
            //            continue;
            //          }
            //          // The subsampling randomly discards frequent words while keeping the
            //          // ranking same
            //          if (sample > 0) {
            //            double ran = (Math.sqrt(entry.freq / (sample * trainWordsCount)) + 1)
            //                * (sample * trainWordsCount) / entry.freq;
            //nextRandom = nextRandom* 25214903917L + 11;
            //            if (ran< (nextRandom & 0xFFFF) / (double) 65536) {
            //              continue;
            //            }
            //          }
            //          sentence.add((WordNeuron) entry);
            //        }

            //        for (int index = 0; index<sentence.size(); index++) {
            //          nextRandom = nextRandom* 25214903917L + 11;
            //          if (isCbow) {
            //            cbowGram(index, sentence, (int) nextRandom % window);
            //          } else {
            //            skipGram(index, sentence, (int) nextRandom % window);
            //          }
            //        }

            //      }
            //      System.out.println("Vocab size: " + wordMap.size());
            //      System.out.println("Words in train file: " + trainWordsCount);
            //System.out.println("sucess train over!");
            //    }
            #endregion

            StreamReader br = new StreamReader(file);
            string temp = null;
            long rd = 5;
            int wordCount = 0;
            int lastWordCount = 0;
            int wordCountActual = 0;
            while ((temp = br.ReadLine()) != null)
            {
                if (wordCount - lastWordCount > 10000)
                {
                    Console.WriteLine(string.Format("alpha:{0}%", (wordCountActual / (trainWordsCount + 1) * 100).ToString("f2")));
                    Console.WriteLine("new:" + newArrayCount + ",cCount:" + cCount + ",jCount:" + jCount);
                    wordCountActual += wordCount - lastWordCount;
                    lastWordCount = wordCount;
                    alpha = startingAlpha * (1 - wordCountActual / (trainWordsCount + 1));
                    if (alpha < startingAlpha * 0.0001)
                    {
                        alpha = startingAlpha * 0.0001;
                    }
                }
                string[] strs = temp.Split(' ');
                wordCount += strs.Length;
                List<WordNeuron> sentence = new List<WordNeuron>();
                for (int i = 0; i < strs.Length; i++)
                {
                    //Neuron entry = wordMap[strs[i]];
                    WordNeuron entry = null;
                    wordMap.TryGetValue(strs[i], out entry);
                    if (entry == null)
                    {
                        continue;
                    }
                    // The subsampling randomly discards frequent words while keeping the
                    // ranking same
                    if (sample > 0)
                    {
                        double ran = (Math.Sqrt(entry.freq / (sample * trainWordsCount)) + 1) * (sample * trainWordsCount) / entry.freq;
                        rd = rd * 25214903917L + 11;
                        if (ran < (rd & 0xFFFF) / (double)65536)
                        {
                            continue;
                        }
                    }
                    sentence.Add((WordNeuron)entry);
                }

                for (int index = 0; index < sentence.Count; index++)
                {
                    rd = rd * 25214903917L + 11;
                    if (isCbow)
                    {
                        CbowGram(index, sentence, (int)rd % window);
                    }
                    else
                    {
                        SkipGramSimd(index, sentence, (int)rd % window);
                        //SkipGramFast(index, sentence, (int)rd % window);
                    }
                }

            }
            Console.WriteLine("Vocab size: " + wordMap.Count);
            Console.WriteLine("Words in train file: " + trainWordsCount);
            Console.WriteLine("sucess train over!");
            Console.WriteLine("new:" + newArrayCount + ",cCount:" + cCount+",jCount:" + jCount);
        }

        private void SkipGram(int index, List<WordNeuron> sentence, int b)
        {
            WordNeuron word = sentence[index];
            int a, c = 0;
            for (a = b; a < window * 2 + 1 - b; a++)
            {
                if (a == window)
                    continue;
                c = index - window + a;
                if (c < 0 || c >= sentence.Count)
                    continue;

                double[] neu1e = new double[layerSize];// 误差项
                // HIERARCHICAL SOFTMAX
                List<Neuron> neurons = word.neurons;
                WordNeuron we = sentence[c];
                for (int i = 0; i < neurons.Count; i++)
                {
                    HiddenNeuron ou = (HiddenNeuron)neurons[i];
                    double f = 0;
                    // Propagate hidden -> output
                    for (int j = 0; j < layerSize; j++)
                    {
                        f += we.syn0[j] * ou.syn1[j];
                    }
                    if (f <= -MAX_EXP || f >= MAX_EXP)
                    {
                        continue;
                    }
                    else
                    {
                        //f = (f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2);
                        f = (f + MAX_EXP) * ((double)EXP_TABLE_SIZE / MAX_EXP / (double)2);
                        f = expTable[(int)f];
                    }
                    // 'g' is the gradient multiplied by the learning rate
                    //double g = (1 - word.codeArr[i] - f) * alpha;
                    double g = ((double)1 - (double)word.codeArr[i] - f) * alpha;
                    // Propagate errors output -> hidden
                    for (c = 0; c < layerSize; c++)
                    {
                        neu1e[c] += g * ou.syn1[c];
                    }
                    // Learn weights hidden -> output
                    for (c = 0; c < layerSize; c++)
                    {
                        ou.syn1[c] += g * we.syn0[c];
                    }
                }

                // Learn weights input -> hidden
                for (int j = 0; j < layerSize; j++)
                {
                    we.syn0[j] += neu1e[j];
                }
            }

        }

        /// <summary>
        /// 模型训练
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sentence"></param>
        /// <param name="b"></param>
        private void SkipGramSimd(int index, List<WordNeuron> sentence, int b)
        {
            WordNeuron word = sentence[index];
            int a, c = 0;
            for (a = b; a < window * 2 + 1 - b; a++)
            {
                if (a == window)
                    continue;
                c = index - window + a;
                if (c < 0 || c >= sentence.Count)
                    continue;

                newArrayCount++;

                Buffer.BlockCopy(swap, 0, neu1e, 0, layerSize * 8);
                // HIERARCHICAL SOFTMAX
                List<Neuron> neurons = word.neurons;
                WordNeuron we = sentence[c];
                for (int i = 0; i < neurons.Count; i++)
                {
                    HiddenNeuron ou = (HiddenNeuron)neurons[i];
                    double f = 0;
                    // Propagate hidden -> output
                    f = Utils.MultiplySimd(we.syn0, ou.syn1);
                    cCount++;
                    //continue;
                    if (f <= -MAX_EXP || f >= MAX_EXP)
                    {
                        continue;
                    }
                    else
                    {
                        //f = (f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2);
                        f = (f + MAX_EXP) * ((double)EXP_TABLE_SIZE / MAX_EXP / (double)2);
                        f = expTable[(int)f];
                    }
                    // 'g' is the gradient multiplied by the learning rate
                    //double g = (1 - word.codeArr[i] - f) * alpha;
                    double g = ((double)1 - (double)word.codeArr[i] - f) * alpha;
                    // Propagate errors output -> hidden
                    //for (c = 0; c < layerSize; c++)
                    //{
                    //    neu1e[c] += g * ou.syn1[c];
                    //}
                    //// Learn weights hidden -> output
                    //for (c = 0; c < layerSize; c++)
                    //{
                    //    ou.syn1[c] += g * we.syn0[c];
                    //}

                    for (c = 0; c < layerSize; c+=4)
                    {
                        neu1e[c] += g * ou.syn1[c];
                        ou.syn1[c] += g * we.syn0[c];

                        neu1e[c+1] += g * ou.syn1[c + 1];
                        ou.syn1[c + 1] += g * we.syn0[c + 1];

                        neu1e[c + 2] += g * ou.syn1[c + 2];
                        ou.syn1[c + 2] += g * we.syn0[c + 2];

                        neu1e[c + 3] += g * ou.syn1[c + 3];
                        ou.syn1[c + 3] += g * we.syn0[c + 3];
                    }
                    jCount++;
                }

                //Learn weights input->hidden
                double[] syn0 = we.syn0;
                for (int j = 0; j < layerSize; j += 4)
                {
                    syn0[j] += neu1e[j];
                    syn0[j + 1] += neu1e[j + 1];
                    syn0[j + 2] += neu1e[j + 2];
                    syn0[j + 3] += neu1e[j + 3];
                }
            }

        }

        private void SkipGramFast(int index, List<WordNeuron> sentence, int b)
        {
            WordNeuron word = sentence[index];
            int a, c = 0;
            for (a = b; a < window * 2 + 1 - b; a++)
            {
                if (a == window)
                    continue;
                c = index - window + a;
                if (c < 0 || c >= sentence.Count)
                    continue;

                int offset = curLayerBlockCount * layerSize;
                //double[] neu1e = new double[layerSize];// 误差项
                // HIERARCHICAL SOFTMAX
                List<Neuron> neurons = word.neurons;
                WordNeuron we = sentence[c];
                for (int i = 0; i < neurons.Count; i++)
                {
                    HiddenNeuron ou = (HiddenNeuron)neurons[i];
                    double f = 0;
                    // Propagate hidden -> output
                    for (int j = 0; j < layerSize; j++)
                    {
                        f += we.syn0[j] * ou.syn1[j];
                    }
                    if (f <= -MAX_EXP || f >= MAX_EXP)
                    {
                        continue;
                    }
                    else
                    {
                        //f = (f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2);
                        f = (f + MAX_EXP) * ((double)EXP_TABLE_SIZE / MAX_EXP / (double)2);
                        f = expTable[(int)f];
                    }
                    // 'g' is the gradient multiplied by the learning rate
                    //double g = (1 - word.codeArr[i] - f) * alpha;
                    double g = ((double)1 - (double)word.codeArr[i] - f) * alpha;
                    // Propagate errors output -> hidden
                    for (c = 0; c < layerSize; c++)
                    {
                        buffer[c+ offset] += g * ou.syn1[c];
                    }
                    // Learn weights hidden -> output
                    for (c = 0; c < layerSize; c++)
                    {
                        ou.syn1[c] += g * we.syn0[c];
                    }
                }

                // Learn weights input -> hidden
                for (int j = 0; j < layerSize; j++)
                {
                    we.syn0[j] += buffer[j+ offset];
                }

                curLayerBlockCount++;
                if (curLayerBlockCount == layerBlockCount)
                {
                    Reset();
                    curLayerBlockCount = 0;
                }
            }
            
        }

        /// <summary>
        /// 词袋模型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sentence"></param>
        /// <param name="b"></param>
        private void CbowGram(int index, List<WordNeuron> sentence, int b)
        {
            WordNeuron word = sentence[index];
            int a, c = 0;
            List<Neuron> neurons = word.neurons;
            double[] neu1e = new double[layerSize];// 误差项
            double[] neu1 = new double[layerSize];// 误差项
            WordNeuron last_word;

            for (a = b; a < window * 2 + 1 - b; a++)
            {
                if (a != window)
                {
                    c = index - window + a;
                    if (c < 0)
                        continue;
                    if (c >= sentence.Count)
                        continue;
                    last_word = sentence[c];
                    if (last_word == null)
                        continue;
                    for (c = 0; c < layerSize; c++)
                        neu1[c] += last_word.syn0[c];
                }
            }

            // HIERARCHICAL SOFTMAX
            for (int d = 0; d < neurons.Count; d++)
            {
                HiddenNeuron ou = (HiddenNeuron)neurons[d];
                double f = 0;
                // Propagate hidden -> output
                for (c = 0; c < layerSize; c++)
                    f += neu1[c] * ou.syn1[c];
                if (f <= -MAX_EXP)
                    continue;
                else if (f >= MAX_EXP)
                    continue;
                else
                    f = expTable[(int)((f + MAX_EXP) * (EXP_TABLE_SIZE / MAX_EXP / 2))];
                // 'g' is the gradient multiplied by the learning rate
                // double g = (1 - word.codeArr[d] - f) * alpha;
                // double g = f*(1-f)*( word.codeArr[i] - f) * alpha;
                double g = f * (1 - f) * (word.codeArr[d] - f) * alpha;
                //
                for (c = 0; c < layerSize; c++)
                {
                    neu1e[c] += g * ou.syn1[c];
                }
                // Learn weights hidden -> output
                for (c = 0; c < layerSize; c++)
                {
                    ou.syn1[c] += g * neu1[c];
                }
            }
            for (a = b; a < window * 2 + 1 - b; a++)
            {
                if (a != window)
                {
                    c = index - window + a;
                    if (c < 0)
                        continue;
                    if (c >= sentence.Count)
                        continue;
                    last_word = sentence[c];
                    if (last_word == null)
                        continue;
                    for (c = 0; c < layerSize; c++)
                        last_word.syn0[c] += neu1e[c];
                }

            }
        }

        /// <summary>
        /// 统计词频
        /// </summary>
        /// <param name="file"></param>
        private void ReadVocab(string file)
        {
            Dictionary<string, int> wc = new Dictionary<string, int>();
            StreamReader br = new StreamReader(file);
            string temp = null;
            while ((temp = br.ReadLine()) != null)
            {
                string[] split = temp.Split(' ');
                trainWordsCount += split.Length;
                foreach (string s in split)
                {
                    if (wc.ContainsKey(s))
                        wc[s]++;
                    else
                        wc.Add(s, 1);
                }
            }
            if (wc.ContainsKey(""))
                wc.Remove("");
            foreach (KeyValuePair<string, int> item in wc)
            {
                wordMap.Add(item.Key, new WordNeuron(item.Key, (double)item.Value / wc.Count, layerSize));
            }
        }

        /// <summary>
        /// 对文本进行预分类
        /// </summary>
        /// <param name="files"></param>
        private void ReadVocabWithSupervised(string[] files)
        {
            for (int category = 0; category < files.Length; category++)
            {
                // 对多个文件学习
                Dictionary<string, int> wc = new Dictionary<string, int>();
                StreamReader reader = new StreamReader(files[category]);
                string temp = null;
                while ((temp = reader.ReadLine()) != null)
                {
                    string[] split = temp.Split(' ');
                    trainWordsCount += split.Length;
                    foreach (string s in split)
                    {
                        if (wc.ContainsKey(s))
                            wc[s]++;
                        else
                            wc.Add(s, 1);
                    }
                }

                foreach (KeyValuePair<string, int> item in wc)
                {
                    double tarFreq = (double)item.Value / wc.Count;
                    if (wordMap[item.Key] != null)
                    {
                        double srcFreq = wordMap[item.Key].freq;
                        if (srcFreq >= tarFreq)
                        {
                            continue;
                        }
                        else
                        {
                            Neuron wordNeuron = wordMap[item.Key];
                            wordNeuron.category = category;
                            wordNeuron.freq = tarFreq;
                        }
                    }
                    else
                    {
                        wordMap.Add(item.Key, new WordNeuron(item.Key,
                            tarFreq, category, layerSize));
                    }
                }
            }
        }

        /// <summary>
        /// Precompute the exp() table f(x) = x / (x + 1)
        /// </summary>
        private void CreateExpTable()
        {
            for (int i = 0; i < EXP_TABLE_SIZE; i++)
            {
                expTable[i] = Math.Exp(((i / (double)EXP_TABLE_SIZE * 2 - 1) * MAX_EXP));
                expTable[i] = expTable[i] / (expTable[i] + 1);
            }
        }

        /// <summary>
        /// 根据文件学习
        /// </summary>
        /// <param name="file"></param>
        public void LearnFile(string file)
        {
            Stopwatch w = Stopwatch.StartNew();
            ReadVocab(file);
            Console.WriteLine("读取耗时：" + w.ElapsedMilliseconds);

            w = Stopwatch.StartNew();
            new Haffman(layerSize).Make(wordMap.Values);
            Console.WriteLine("make 耗时：" + w.ElapsedMilliseconds);

            w = Stopwatch.StartNew();
            // 查找每个神经元
            foreach (Neuron item in wordMap.Values)
            {
                ((WordNeuron)item).MakeNeurons();
            }
            Console.WriteLine("查找每个神经元耗时:" + w.ElapsedMilliseconds);

            w = Stopwatch.StartNew();
            TrainModel(file);
            Console.WriteLine("训练耗时:" + w.ElapsedMilliseconds);
        }

        /// <summary>
        /// 根据预分类的文件学习
        /// </summary>
        /// <param name="summaryFile">合并文件</param>
        /// <param name="classifiedFiles">分类文件</param>
        public void LearnFile(string summaryFile, string[] classifiedFiles)
        {
            ReadVocabWithSupervised(classifiedFiles);
            new Haffman(layerSize).Make(wordMap.Values);
            // 查找每个神经元
            foreach (Neuron neuron in wordMap.Values)
            {
                ((WordNeuron)neuron).MakeNeurons();
            }
            TrainModel(summaryFile);
        }

        /// <summary>
        /// 保存模型
        /// </summary>
        /// <param name="file"></param>
        public void SaveModel(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8);
                writer.Write(wordMap.Count);
                writer.Write(layerSize);
                double[] syn0 = null;
                foreach (KeyValuePair<String, WordNeuron> item in wordMap)
                {
                    writer.Write(item.Key);
                    syn0 = ((WordNeuron)item.Value).syn0;
                    foreach (double d in syn0)
                    {
                        //writer.Write((float)d);
                        writer.Write(d);
                    }
                }
                writer.Close();
                fs.Close();
            }
        }

        #endregion



    }
}
