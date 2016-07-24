using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Word2VecSharp.Collections;
using java.util;

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

        private java.util.TreeSet set = new TreeSet();

        public void Make(IEnumerable<Neuron> neurons)
        {
            #region test
            //startCount = neurons.Count();
            //foreach (Neuron item in neurons)
            //    set.Add(item);
            ////while (set.Count < startCount * 2 - 1)
            ////{
            ////    merger();
            ////}
            //while (set.Count > 1)
            //{
            //    Merge();
            //}
            #endregion

            foreach (Neuron item in neurons)
                set.add(item);
            while (set.size() > 1)
            {
                Merge();
            }
        }

        private void Merge()
        {
            HiddenNeuron hn = new HiddenNeuron(layerSize);
            Neuron min1 = (Neuron)set.pollFirst();
            Neuron min2 = (Neuron)set.pollFirst();
            hn.category = min2.category;
            hn.freq = min1.freq + min2.freq;
            min1.parent = hn;
            min2.parent = hn;
            min1.code = 0;
            min2.code = 1;
            set.Add(hn);

            #region test
            //HiddenNeuron hn = new HiddenNeuron(layerSize);
            ////Neuron min1 = set.First();
            ////set.Remove(min1);
            ////Neuron min2 = set.First();
            ////set.Remove(min2);

            ////Neuron min1 = set.ElementAt(idx++);
            ////Neuron min2 = set.ElementAt(idx++);

            //Neuron min1 = set.ElementAt(0);
            //Neuron min2 = set.ElementAt(1);

            //set.ElementAt(0).isUsed = true;
            //set.ElementAt(1).isUsed = true;
            //set.RemoveWhere(item => item.isUsed == true);

            ////Neuron min1 = set.pollFirst();
            ////Neuron min2 = set.pollFirst();
            //hn.category = min2.category;
            //hn.freq = min1.freq + min2.freq;
            //min1.parent = hn;
            //min2.parent = hn;
            //min1.code = 0;
            //min2.code = 1;
            //set.Add(hn);
            #endregion
        }

    }
}
