using System.Windows;
using System.Threading;
using System.Globalization;
using Olympus.Properties;

namespace Olympus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new("en-AU");
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Saves the settings when closing the App.
            Settings.Default.Save();
        }
    }
}
