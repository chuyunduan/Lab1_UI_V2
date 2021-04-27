using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ClassLibrary;

namespace Lab1_UI_V2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        V2MainCollection mainCollection = new V2MainCollection();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainCollection;
        }

        private void DataCollection(object sender, FilterEventArgs args)
        {
            var item = args.Item;
            if (item != null)
            {
                if (item.GetType() == typeof(V2DataCollection)) 
                    args.Accepted = true;
                else args.Accepted = false;
            }
        }

        private void DataOnGrid(object sender, FilterEventArgs args)
        {
            var item = args.Item;
            if (item != null)
            {
                if (item.GetType() == typeof(V2DataOnGrid)) 
                    args.Accepted = true;
                else args.Accepted = false;
            }
        }

        private void AddDef_btn_Click(object sender, RoutedEventArgs e)
        {
            mainCollection.AddDefaults();
        }

        private void AddDefDC_btn_Click(object sender, RoutedEventArgs e)
        {
            mainCollection.AddDefaultDataCollection();
        }

        private void AddDefDOG_btn_Click(object sender, RoutedEventArgs e)
        {
            mainCollection.AddDefaultDataOnGrid();
        }

        private void AddElemFile_btn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "TXTFiles|*.txt";
            if ((bool)dlg.ShowDialog())
                mainCollection.AddElementFromFile(dlg.FileName);
            MessageError();
        }

        private void Remove_btn_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.listBox_Main.SelectedItems;
            if (this.listBox_Main.SelectedItems.Count != 0)
            {
                List<V2Data> selectedItems = new List<V2Data>();
                selectedItems.AddRange(selected.Cast<V2Data>());
                foreach (V2Data item in selectedItems)
                    mainCollection.Remove(item.Info, item.Freq);
            }
            else
            {
                mainCollection = new V2MainCollection();
                DataContext = mainCollection;
            }
        }

        private bool UnsavedChanges()
        {
            MessageBoxResult mes = MessageBox.Show("Do you want save records?", "Save", MessageBoxButton.YesNoCancel);
            if (mes == MessageBoxResult.Yes)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Inf";
                dlg.DefaultExt = ".txt";
                dlg.Filter = "TXTFiles(.txt)|*.txt";
                if ((bool)dlg.ShowDialog())
                    mainCollection.Save(dlg.FileName);
            }
            else if (mes == MessageBoxResult.Cancel)
                return true;
            return false;
        }

        private void New_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainCollection.CollectionChangedAfterSave)
                UnsavedChanges();
            mainCollection = new V2MainCollection();
            DataContext = mainCollection;
            MessageError();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Inf";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "TXTFiles(.txt)|*.txt";
            if ((bool)dlg.ShowDialog())
                mainCollection.Save(dlg.FileName);
            MessageError();
        }

        private void Open_btn_Click(object sender, RoutedEventArgs e)
        {
            if (mainCollection.CollectionChangedAfterSave)
                UnsavedChanges();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if ((bool)dlg.ShowDialog())
            {
                mainCollection = new V2MainCollection();
                mainCollection.Load(dlg.FileName);
                DataContext = mainCollection;
            }
            MessageError();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs cl)
        {
            if (mainCollection.CollectionChangedAfterSave)
                cl.Cancel = UnsavedChanges();
            MessageError();
        }

        public void MessageError()
        {
            if (mainCollection.ErrorMessage != null)
            {
                MessageBox.Show(mainCollection.ErrorMessage, "Error");
                mainCollection.ErrorMessage = null;
            }
        }
    }
}
