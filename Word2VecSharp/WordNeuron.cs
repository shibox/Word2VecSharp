using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class WordNeuron : Neuron
    {
        public String name;
        public double[] syn0 = null; // input->hidden
        /// <summary>
        /// 路径神经元
        /// </summary>
        public List<Neuron> neurons = null;
        public int[] codeArr = null;

        public WordNeuron()
        { }

        public List<Neuron> makeNeurons()
        {
            if (neurons != null)
            {
                return neurons;
            }
            Neuron neuron = this;
            neurons = new List<Neuron>();
            while ((neuron = neuron.parent) != null)
            {
                neurons.Add(neuron);
            }
            neurons.Reverse();
            codeArr = new int[neurons.Count];

            for (int i = 1; i < neurons.Count; i++)
            {
                codeArr[i - 1] = neurons[i].code;
            }
            codeArr[codeArr.Length - 1] = this.code;

            return neurons;
        }

        public WordNeuron(String name, double freq, int layerSize)
        {
            this.name = name;
            this.freq = freq;
            this.syn0 = new double[layerSize];
            Random random = new Random();
            for (int i = 0; i < syn0.Length; i++)
            {
                syn0[i] = (random.NextDouble() - 0.5) / layerSize;
            }
        }

        /// <summary>
        /// 用于有监督的创造hoffman tree
        /// </summary>
        /// <param name="name"></param>
        /// <param name="freq"></param>
        /// <param name="category"></param>
        /// <param name="layerSize"></param>
        public WordNeuron(string name, double freq, int category, int layerSize)
        {
            this.name = name;
            this.freq = freq;
            this.syn0 = new double[layerSize];
            this.category = category;
            Random random = new Random();
            for (int i = 0; i < syn0.Length; i++)
            {
                syn0[i] = (random.NextDouble() - 0.5) / layerSize;
            }
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

    }
}
