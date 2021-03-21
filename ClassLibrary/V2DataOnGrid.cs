using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Lab1_UI_V2")]

namespace ClassLibrary
{
    [Serializable]
    class V2DataOnGrid : V2Data, IEnumerable<DataItem>
    {
        public Grid1D[] Grids { get; set; }
        public Complex[,] Node { get; set; }

        public V2DataOnGrid(string info, double freq, Grid1D ox, Grid1D oy) : base(info, freq)
        {
            Info = info;
            Freq = freq;
            Grids = new Grid1D[2] { ox, oy };
        }

        public V2DataOnGrid(string filename) : base()
        {

            CultureInfo CI = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            CultureInfo CIen = new CultureInfo("en-US");
            CultureInfo CIru = new CultureInfo("ru-RU");
            CultureInfo CIzh = new CultureInfo("zh-CN");

            if ((CI == CIru) || (CI == CIzh))
                if (CI == CIru)
                    CI = CIru;
                else
                    CI = CIzh;
            else
                CI = CIen;

            FileStream f = null;
            try
            {
                Directory.SetCurrentDirectory("..\\..\\..\\");
                f = new FileStream(filename, FileMode.Open);
                StreamReader reader = new StreamReader(f);

                Info = reader.ReadLine();//Info
                Freq = double.Parse(reader.ReadLine(),CI);//Freq
                string[] str;
                Grids = new Grid1D[2];

                for (int i = 0; i < 2; i++)
                {
                    str = reader.ReadLine().Split(' ');//Count and Step
                    Grids[i].Count = int.Parse(str[0],CI);
                    Grids[i].Step = float.Parse(str[1],CI);
                }

                Node = new Complex[Grids[0].Count, Grids[1].Count];
                string[] strnode;
                string[] strcompl;

                for (int i = 0; i < Grids[0].Count; i++)//Matrix
                {
                    strnode = reader.ReadLine().Split(' ');
                    for (int j = 0; j < Grids[1].Count; j++)
                    {
                        strcompl = strnode[j].Split('|');
                        Node[i, j] = new Complex(double.Parse(strcompl[0],CI), double.Parse(strcompl[1],CI));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (f != null) f.Close();
            }
        }

        public void initRandom(double minValue, double maxValue)
        {
            Node = new Complex[Grids[0].Count, Grids[1].Count];
            Random rnd = new Random();

            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                    Node[i, j] = new Complex(rnd.NextDouble() * (maxValue - minValue), rnd.NextDouble() * (maxValue - minValue));
            }
        }

        public static explicit operator V2DataCollection(V2DataOnGrid val)
        {
            V2DataCollection ret = new V2DataCollection(val.Info, val.Freq);

            for (int i = 0; i < val.Grids[0].Count; i++)
            {
                for (int j = 0; j < val.Grids[1].Count; j++)
                    ret.dataItems.Add(new DataItem(new Vector2((i + 1) * val.Grids[0].Step, (j + 1) * val.Grids[1].Step), val.Node[i, j]));
            }
            return ret;
        }

        public override Complex[] NearAverage(float eps)
        {
            int num = Grids[0].Count * Grids[1].Count;
            double sum = 0;

            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                    sum += Node[i, j].Real;
            }
            double average = sum / num;
            int count = 0;
            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                {
                    if (Math.Abs(Node[i, j].Real - average) < eps)
                        count++;
                }
            }
            Complex[] ret = new Complex[count];
            count = 0;
            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                {
                    if (Math.Abs(Node[i, j].Real - average) < eps)
                        ret[count++] = Node[i, j];
                }
            }
            return ret;
        }

        public override string ToString()
        {
            return $"Type: 2DataOnGrid Info: { Info.ToString() } Freq: { Freq.ToString() } Ox: { Grids[0].ToString() } Oy: { Grids[1].ToString() } ";
        }

        public override string ToLongString()
        {
            string matrix = String.Empty;
            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                {
                    matrix += ("|(" + (Grids[0].Step * i).ToString() + "," + (Grids[1].Step * j).ToString() + ")| Value: " + Node[i, j].ToString());
                }
                matrix += "\n";
            }  
            return $"Type: V2DataOnGrid Info: { Info } \nFreq: { Freq.ToString() } \nOx: { Grids[0].ToString() } \nOy: { Grids[1].ToString() } \n{ matrix }";
        }
        public override string ToLongString(string format)
        {
            string matrix = String.Empty;
            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                {
                    matrix += ("|(" + (Grids[0].Step * i).ToString(format) + "," + (Grids[1].Step * j).ToString(format) + ")| Value: " + Node[i, j].ToString(format) + "\n");
                }
                matrix += "\n";
            }
            return $"Type: V2DataOnGrid Info: { Info } \nFreq: { Freq.ToString(format) } \nOx: { Grids[0].ToString(format) } \nOy: { Grids[1].ToString(format) } \n{ matrix }";
        }

        public IEnumerator<DataItem> GetEnumerator()
        {
            V2DataCollection ret = new V2DataCollection(Info, Freq);
            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                    yield return new DataItem(new Vector2((i + 1) * Grids[0].Step, (j + 1) * Grids[1].Step), Node[i, j]);
            }
        }


    IEnumerator IEnumerable.GetEnumerator()
        {
            V2DataCollection ret = new V2DataCollection(Info, Freq);

            for (int i = 0; i < Grids[0].Count; i++)
            {
                for (int j = 0; j < Grids[1].Count; j++)
                    ret.dataItems.Add(new DataItem(new Vector2((i + 1) * Grids[0].Step,(j + 1) * Grids[1].Step), Node[i, j]));
            }
            return ((IEnumerable)ret.dataItems).GetEnumerator();
        }
    }
}
