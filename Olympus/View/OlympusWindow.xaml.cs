using System.Globalization;
using System.Threading;
using Olympus.Properties;

namespace Olympus.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-AU");
    }

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // Saves the settings when closing the App.
        Settings.Default.Save();
    }
}