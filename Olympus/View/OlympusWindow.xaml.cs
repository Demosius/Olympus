using System;
using System.Windows;
using System.Threading;
using System.Globalization;

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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-AU");
        }

        void MainWindow_Closing(object sender, ConsoleCancelEventArgs e)
        {
            // TODO: Figure out settings saving on app close.
            MessageBox.Show("Closing called");
        }

    }
}
