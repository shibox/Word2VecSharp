using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public abstract class Neuron : IComparable<Neuron>,IEquatable<Neuron>
    {
        public double freq;
        public Neuron parent;
        public int code;
        public bool isUsed = false;
        /// <summary>
        /// 语料预分类
        /// </summary>
        public int category = -1;

        public int CompareTo(Neuron other)
        {
            if (this.category == other.category)
            {
                if (other.isUsed == true)
                    return 0;
                if (this.freq > other.freq)
                {
                    return 1;
                }
                //else if (this.freq == other.freq)
                //{
                //    return 0;
                //}
                else
                {
                    return -1;
                }
            }
            else if (this.category > other.category)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool Equals(Neuron other)
        {
            return this.isUsed == true;
        }
    }
}
