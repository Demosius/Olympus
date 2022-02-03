using Styx;
using Uranus;
using System;
using System.IO;
using System.Windows;
using Olympus.Properties;

namespace Olympus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /*private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _ = MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }*/

        public static Charon Charon { get; set; } = new(Settings.Default.SolLocation);
        public static Helios Helios { get; set; } = new(Settings.Default.SolLocation);

        public static string BaseDirectory()
        {
            return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
