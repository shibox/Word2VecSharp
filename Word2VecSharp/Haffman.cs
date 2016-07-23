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

        int idx = 0;
        int startCount = 0;
        private SortedSet<Neuron> set = new SortedSet<Neuron>();
        //RedBlackTree<Neuron, bool> set = new RedBlackTree<Neuron, bool>(null);

        public void make(IEnumerable<Neuron> neurons)
        {
            startCount = neurons.Count();
            foreach (Neuron item in neurons)
                set.Add(item);


            //while (set.Count < startCount * 2 - 1)
            //{
            //    merger();
            //}

            while (set.Count > 1)
            {
                merger();
            }

        }

        private void merger()
        {
            HiddenNeuron hn = new HiddenNeuron(layerSize);
            //Neuron min1 = set.First();
            //set.Remove(min1);
            //Neuron min2 = set.First();
            //set.Remove(min2);

            //Neuron min1 = set.ElementAt(idx++);
            //Neuron min2 = set.ElementAt(idx++);

            Neuron min1 = set.ElementAt(0);
            Neuron min2 = set.ElementAt(1);

            set.ElementAt(0).isUsed = true;
            set.ElementAt(1).isUsed = true;
            set.RemoveWhere(item => item.isUsed == true);

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
