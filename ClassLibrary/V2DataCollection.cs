using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;

namespace ClassLibrary
{
    [Serializable]
    class V2DataCollection : V2Data, IEnumerable<DataItem>
    {
        public List<DataItem> dataItems { get; set; }

        public V2DataCollection(string info, double freq) : base(info, freq)
        {
            dataItems = new List<DataItem>();
            Info = info;
            Freq = freq;
        }

        public void initRandom(int nItems, float xmax, float ymax, double minValue, double maxValue)
        {
            dataItems = new List<DataItem>();
            Random next = new Random();
            for (int i = 0; i < nItems; i++)
            {
                DataItem pre = new DataItem(new Vector2((float)next.NextDouble() * xmax, (float)next.NextDouble() * ymax), new Complex(next.NextDouble() * (maxValue - minValue), next.NextDouble() * (maxValue - minValue)));
                dataItems.Add(pre);
            }
        }

        public override Complex[] NearAverage(float eps)
        {
            int count = 0;
            double sum = 0;
            foreach (DataItem item in dataItems)
                sum += item.Complex.Real;
            double average = sum / dataItems.Count;
            foreach (DataItem item in dataItems)
            {
                if (Math.Abs(item.Complex.Real - average) < eps)
                    count++;
            }
            Complex[] ret = new Complex[count];
            count = 0;
            foreach (DataItem item in dataItems)
            {
                if (Math.Abs(item.Complex.Real - average) < eps)
                    ret[count++] = item.Complex;
            }
            return ret;
        }

        public override string ToString()
        {
            return $"Type: V2DataCollection Info: { Info } Freq: { Freq.ToString() } Count: { dataItems.Count.ToString() }";
        }

        public override string ToLongString()
        {
            string ret = String.Empty;
            foreach (DataItem item in dataItems)
                ret += item.ToString();
            return $"Type: V2DataCollection Info: { Info } Freq: { Freq.ToString() } Count: { dataItems.Count.ToString() } \n{ ret }";
        }

        public override string ToLongString(string format)
        {
            string ret = String.Empty;
            foreach (DataItem item in dataItems)
                ret += item.ToString(format);
            return $"Type: V2DataCollection Info: { Info } Freq: { Freq.ToString(format) } Count: { dataItems.Count.ToString(format) } \n{ ret }";
        }

        public IEnumerator<DataItem> GetEnumerator()
        {
            return ((IEnumerable<DataItem>)dataItems).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dataItems).GetEnumerator();
        }
    }
}
