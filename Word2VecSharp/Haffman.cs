using DataStructures.RedBlackTreeSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    /// <summary>
    /// 构建Haffman编码树
    /// </summary>
    public class Haffman
    {
        private int layerSize;

        public Haffman(int layerSize)
        {
            this.layerSize = layerSize;
        }

        //private SortedSet<Neuron> set = new SortedSet<Neuron>();
        RedBlackTree<Neuron, bool> set = new RedBlackTree<Neuron, bool>(null);

        public void make(IEnumerable<Neuron> neurons)
        {
            foreach (Neuron item in neurons)
                set.Add(item);
            while (set.Count > 1)
            {
                merger();
            }
        }

        private void merger()
        {
            HiddenNeuron hn = new HiddenNeuron(layerSize);
            Neuron min1 = set.First();
            set.Remove(min1);
            Neuron min2 = set.First();
            set.Remove(min2);

            //Neuron min1 = set.pollFirst();
            //Neuron min2 = set.pollFirst();
            hn.category = min2.category;
            hn.freq = min1.freq + min2.freq;
            min1.parent = hn;
            min2.parent = hn;
            min1.code = 0;
            min2.code = 1;
            set.Add(hn);
        }

    }
}
