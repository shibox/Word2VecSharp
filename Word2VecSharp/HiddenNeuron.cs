using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{

    public class HiddenNeuron : Neuron
    {
        public double[] syn1;

        public HiddenNeuron(int layerSize)
        {
            syn1 = new double[layerSize];
        }

    }
}
