using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace ExcelToXmlConverter
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<List<string>> data;
        public List<List<string>> Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".csv";
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                Data = LoadCsvData(openFileDialog.FileName);
                UpdateDataGridColumns();
            }
        }

        private List<List<string>> LoadCsvData(string filePath)
        {
            List<List<string>> csvData = new List<List<string>>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> rowData = line.Split(';').ToList(); //anhand von ; trennen
                    csvData.Add(rowData);
                }
            }

            return csvData;
        }


        private void SaveAsXml_Click(object sender, RoutedEventArgs e)
        {
            if (Data != null && Data.Any())
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.DefaultExt = ".xml";
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml";

                if (saveFileDialog.ShowDialog() == true)
                {
                    SaveAsXml(Data, saveFileDialog.FileName);
                    MessageBox.Show("Data saved as XML successfully!");
                }
            }
            else
            {
                MessageBox.Show("No data to save!");
            }
        }

        private void SaveAsXml(List<List<string>> data, string filePath)
        {
            XElement xmlData = new XElement("Data");

            foreach (var row in data)
            {
                XElement xmlRow = new XElement("Row");

                foreach (var cell in row)
                {
                    XElement xmlCell = new XElement("Cell", cell);
                    xmlRow.Add(xmlCell);
                }

                xmlData.Add(xmlRow);
            }

            xmlData.Save(filePath);
        }

        private void UpdateDataGridColumns()
        {
            if (Data != null && Data.Any())
            {
                dataGrid.Columns.Clear();

                // Create columns based on the number of columns in the data
                int numColumns = Data[0].Count;
                for (int i = 0; i < numColumns; i++)
                {
                    DataGridTextColumn column = new DataGridTextColumn();
                    column.Header = Data[0][i]; // Verwende den ersten Eintrag als Spaltenüberschrift
                    column.Binding = new Binding($"[{i}]");
                    dataGrid.Columns.Add(column);
                }

                // Entferne die erste Zeile (Spaltenüberschriften) aus den Daten
                Data.RemoveAt(0);
            }
        }

    }
}
