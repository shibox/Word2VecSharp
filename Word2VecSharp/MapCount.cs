using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word2VecSharp
{
    public class MapCount<T>
    {
        internal Dictionary<T, int> hm = null;

        public MapCount()
        {
            this.hm = new Dictionary<T, int>();
        }

        public MapCount(int initialCapacity)
        {
            this.hm = new Dictionary<T, int>(initialCapacity);
        }

        public void add(T t, int n)
        {
            if (hm.ContainsKey(t))
            {
                this.hm[t] += n;
            }
            else
            {
                this.hm.Add(t, n);
            }
        }

        public void add(T t)
        {
            this.add(t, 1);
        }

        public int size()
        {
            return this.hm.Count;
        }

        public void remove(T t)
        {
            this.hm.Remove(t);
        }

        public Dictionary<T, int> get()
        {
            return this.hm;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<T, int> item in hm)
            {
                sb.Append(item.Key);
                sb.Append("\t");
                sb.Append(item.Value);
                sb.Append("\n");
            }
            return sb.ToString();
        }

        
    }
}
