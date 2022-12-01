using Olympus.Properties;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;

namespace Olympus.Views;

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

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Forward or NavigationMode.Back) e.Cancel = true;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        VM.UserHandlerVM.LogIn();
    }
}