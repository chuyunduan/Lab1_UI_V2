using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
using System.IO;

[assembly: InternalsVisibleToAttribute("Lab1_UI_V2")]

namespace ClassLibrary
{
    [Serializable]
    class V2MainCollection : IEnumerable<V2Data>, System.Collections.Specialized.INotifyCollectionChanged, INotifyPropertyChanged
    {
        private List<V2Data> v2Datas;

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnCollectionChanged(NotifyCollectionChangedAction ev)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public bool CollectionChangedAfterSave{ get; set;}

        public string ErrorMessage
        {
            get; set;
        }

        public void Save(string filename)
        {
            Directory.SetCurrentDirectory("..\\..\\..\\");
            FileStream FS = null;

            try
            {
                if (File.Exists(filename))
                    FS = File.OpenWrite(filename);
                else
                    FS = File.Create(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(FS, v2Datas);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "Save failed:" + ex.Message;
            }
            finally
            {
                if (FS != null)
                    FS.Close();
                CollectionChangedAfterSave = false;
                OnPropertyChanged("CollectionChangedAfterSave");
            }
        }

        public void Load(string filename)
        {
            Directory.SetCurrentDirectory("..\\..\\..\\");
            FileStream FS = null;

            try
            {
                FS = File.OpenRead(filename);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                v2Datas = (List<V2Data>)binaryFormatter.Deserialize(FS);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "Load: " + ex.Message;
            }
            finally
            {
                FS.Close();
                CollectionChangedAfterSave = true;
                OnPropertyChanged("CollectionChangedAfterSave");
            }
        }

        public int Count
        {
            get { return v2Datas.Count; }
        }

        public void Add(V2Data item)
        {
            try
            {
                v2Datas.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
                OnPropertyChanged("Average");
                CollectionChangedAfterSave = true;
                OnPropertyChanged("CollectionChangedAfterSave");
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "Add failed: " + ex.Message;
            }
        }

        public bool Remove(string id, double w)
        {
            bool flag = false;
            for (int i = 0; i < v2Datas.Count; i++)
            {
                if (v2Datas[i].Freq == w && v2Datas[i].Info == id)
                {
                    v2Datas.RemoveAt(i);
                    flag = true;
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove);
                    OnPropertyChanged("Average");
                    CollectionChangedAfterSave = true;
                    OnPropertyChanged("CollectionChangedAfterSave");
                    break;
                }
                else
                {
                    i++;
                }
            }
            return flag;
        }

        public void AddDefaults()
        {
            Grid1D Ox = new Grid1D(1, 3);
            Grid1D Oy = new Grid1D(1, 3);
            v2Datas = new List<V2Data>();
            V2DataOnGrid[] mag = new V2DataOnGrid[4];
            V2DataCollection[] collections = new V2DataCollection[4];
            for (int i = 0; i < 4; i++)
            {
                //mag[i] = new V2DataOnGrid("Data", 123, Ox, Oy);
                //collections[i] = new V2DataCollection("Data", 123);
                mag[i] = new V2DataOnGrid("Data " + i.ToString(), i, Ox, Oy);
                collections[i] = new V2DataCollection("Collection number: " + i.ToString(), i);
            }
            for (int i = 0; i < 4; i++)
            {
                mag[i].initRandom(0, 10);
                collections[i].initRandom(4, 10, 10, 0, 100);
                this.Add(mag[i]);
                this.Add(collections[i]);
            }

            Grid1D NULLOX = new Grid1D(0, 0);
            Grid1D NULLOY = new Grid1D(0, 0);
            mag[3] = new V2DataOnGrid("NULL", 10, NULLOX, NULLOY);
            collections[3] = new V2DataCollection("NULL", 10);

            mag[3].initRandom(0, 10);
            collections[3].initRandom(0, 10, 10, 0, 10);
            this.Add(mag[3]);
            this.Add(collections[3]);

        }

        public V2MainCollection()
        {
            this.v2Datas = new List<V2Data>();
            CollectionChangedAfterSave = false;
        }

        public void AddDefaultDataCollection()
        {
            V2DataCollection collection = new V2DataCollection("Default", 1);
            collection.initRandom(4, 10, 10, 0, 100);
            this.Add(collection);
        }

        public void AddDefaultDataOnGrid()
        {
            Grid1D Ox = new Grid1D(1, 3);
            Grid1D Oy = new Grid1D(1, 3);
            V2DataOnGrid grid = new V2DataOnGrid("data info ", 2, Ox, Oy);
            grid.initRandom(0, 10);
            this.Add(grid);
        }

        public void AddElementFromFile(string filename)
        {
            try
            {
                V2DataOnGrid datas = new V2DataOnGrid(filename);
                this.Add(datas);
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "Add failed:  " + ex.Message;
            }
        }

        public override string ToString()
        {
            string ret = String.Empty;
            try
            {
                foreach (V2Data data in v2Datas)
                    ret += (data.ToString() + '\n');
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "ERROR To String: " + ex.Message;
            }
            return ret;
        }

        public string ToLongString(string format)
        {
            string ret = String.Empty;
            try
            {
                foreach (V2Data data in v2Datas)
                    ret += (data.ToLongString(format) + '\n');
            }
            catch (Exception ex)
            {
                this.ErrorMessage = "ERROR ToLongString: " + ex.Message;
            }
            return ret;
        }

        public IEnumerator<V2Data> GetEnumerator()
        {
            return ((IEnumerable<V2Data>)v2Datas).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)v2Datas).GetEnumerator();
        }

        public double Average
        {
            get
            {
                IEnumerable<DataItem> collection = from elem in (from data in v2Datas
                                                                 where data is V2DataCollection
                                                                 select (V2DataCollection)data)
                                                   from item in elem
                                                   select item;

                IEnumerable<DataItem> mag = from elem in (from data in v2Datas
                                                           where data is V2DataOnGrid
                                                           select (V2DataOnGrid)data)
                                             from item in elem
                                             select item;

                IEnumerable<DataItem> items = collection.Union(mag);

                if (items.Count() != 0)
                    return items.Average(n => n.Complex.Magnitude);
                else return 0;
            }
        }

        public DataItem NearAverage
        {
            get
            {
                double a = this.Average;

                IEnumerable<DataItem> collection = from elem in (from data in v2Datas
                                                                 where data is V2DataCollection
                                                                 select (V2DataCollection)data)
                                                   from item in elem
                                                   select item;

                IEnumerable<DataItem> mag = from elem in (from data in v2Datas
                                                           where data is V2DataOnGrid
                                                           select (V2DataOnGrid)data)
                                             from item in elem
                                             select item;

                IEnumerable<DataItem> items = collection.Union(mag);

                var dif = from item in items
                          select Math.Abs(item.Complex.Magnitude - a);

                double min = dif.Min();

                var ret = from item in items
                          where Math.Abs(item.Complex.Magnitude - a) <= min
                          select item;

                return ret.First();
            }
        }

        public IEnumerable<Vector2> Vectors
        {
            get
            {
                return from elem in (from data in v2Datas
                                     where data is V2DataCollection
                                     select (V2DataCollection)data)
                       from item in elem
                       select item.Vector;
            }
        }
    }
}
