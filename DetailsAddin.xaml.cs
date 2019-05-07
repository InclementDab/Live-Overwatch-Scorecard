using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;
using WinLossCounter.Properties;

namespace WinLossCounter
{
    /// <summary>
    /// Interaction logic for DetailsAddin.xaml
    /// </summary>
    public partial class DetailsAddin : UserControl
    {
        public DetailsAddin()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Select Output File",
                Filter = "Text Files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if ((bool)dialog.ShowDialog())
                Settings.Default.OutputFile = dialog.FileName;

        }

        private void FileBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsInitialized) return;
            if (!File.Exists(fileBox.Text))
            {
                InfoBox.Text = "File Not Found!";
                InfoBox.Foreground = Brushes.Red;
            }
            else
            {
                InfoBox.Text = "";
                InfoBox.Foreground = Brushes.Black;
            }


        }
    }
}
