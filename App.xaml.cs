using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace WinLossCounter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainWindow _MainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _MainWindow = new MainWindow();
            _MainWindow.Show();
        }
    }
}
