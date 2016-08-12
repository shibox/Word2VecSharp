using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class Utils
    {
        public static float MultiplySimd(float[] a, float[] b)
        {
            float result = 0;
            int simdLength = Vector<float>.Count;
            Vector<float> swapResult = new Vector<float>();
            for (int i = 0; i < a.Length; i += simdLength)
            {
                Vector<float> left = new Vector<float>(a, i);
                Vector<float> right = new Vector<float>(b, i);
                Vector<float> swap = Vector.Multiply(left, right);
                swapResult = Vector.Add(swap, swapResult);
            }
            for (int i = 0; i < simdLength; ++i)
            {
                result += swapResult[i];
            }
            return result;
        }

        public static double MultiplySimd(double[] a, double[] b)
        {
            double result = 0;
            int simdLength = Vector<double>.Count;
            Vector<double> swapResult = new Vector<double>();
            for (int i = 0; i < a.Length; i += simdLength)
            {
                Vector<double> left = new Vector<double>(a, i);
                Vector<double> right = new Vector<double>(b, i);
                Vector<double> swap = Vector.Multiply(left, right);
                swapResult = Vector.Add(swap, swapResult);
            }
            for (int i = 0; i < simdLength; ++i)
            {
                result += swapResult[i];
            }
            return result;
        }

        public static float MultiplySimd(float[] a, float[] b, int bof)
        {
            float result = 0;
            int simdLength = Vector<int>.Count;
            Vector<float> swapResult = new Vector<float>();
            for (int i = 0; i < a.Length; i += simdLength)
            {
                //Vector<float> left = new Vector<float>(a, i);
                //Vector<float> right = new Vector<float>(b, i + bof);
                //Vector<float> swap = Vector.Multiply(left, right);
                //swapResult = Vector.Add(swap, swapResult);

                swapResult += new Vector<float>(a, i) * new Vector<float>(b, i + bof);
            }
            for (int i = 0; i < simdLength; ++i)
            {
                result += swapResult[i];
            }
            return result;
        }

        //public static float MultiplySimd(float[] a,int aof, float[] b,int bof)
        //{
        //    float result = 0;
        //    int simdLength = Vector<int>.Count;
        //    Vector<float> swapResult = new Vector<float>();
        //    for (int i = 0; i < a.Length; i += simdLength)
        //    {
        //        Vector<float> left = new Vector<float>(a, i);
        //        Vector<float> right = new Vector<float>(b, i);
        //        Vector<float> swap = Vector.Multiply(left, right);
        //        swapResult = Vector.Add(swap, swapResult);
        //    }
        //    for (int i = 0; i < simdLength; ++i)
        //    {
        //        result += swapResult[i];
        //    }
        //    return result;
        //}

        public static void Add(double[] a,double[] b,double v)
        {
            int simdLength = Vector<int>.Count;
            Vector<double> swap = new Vector<double>(v);
            for (int i = 0; i < a.Length; i += simdLength)
            {
                Vector<double> ar = new Vector<double>(a, i);
                ar = new Vector<double>(b, i)+swap;
                ar.CopyTo(a, i);
            }
        }

        public static void Abs(double[] a, double[] b, double v)
        {
            int simdLength = Vector<int>.Count;
            Vector<double> swap = new Vector<double>(v);
            for (int i = 0; i < a.Length; i += simdLength)
            {
                Vector<double> ar = new Vector<double>(a, i);
                ar = new Vector<double>(b, i) + swap;
                ar.CopyTo(a, i);
            }
        }

        static readonly Vector<double> zero = new Vector<double>();

        public static void Wipe(double[] value)
        {
            int simdLength = Vector<double>.Count;
            for (int i = 0; i < value.Length; i += simdLength)
            {
                zero.CopyTo(value, i);
            }
        }

    }
}
