using Olympus.Model;
using Styx;
using Uranus;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Olympus.Properties;

namespace Olympus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        public static Charon Charon { get; set; } = new Charon(Settings.Default.SolLocation);
        public static Helios Helios { get; set; } = new Helios(Settings.Default.SolLocation);

        public static string BaseDirectory()
        {
            return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
