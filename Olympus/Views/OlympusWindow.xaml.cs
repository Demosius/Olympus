using System;
using System.ComponentModel;
using Olympus.Properties;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Olympus.ViewModels;

namespace Olympus.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public OlympusVM? VM { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-AU");
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // Saves the settings when closing the App.
        Settings.Default.Save();
    }

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Forward or NavigationMode.Back) e.Cancel = true;
    }

    private async void MainWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await OlympusVM.CreateAsync();
        DataContext = VM;
        VM.UserHandlerVM.LogIn();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            VM?.TestPb();
        });
        /*VM.ProgressBarVM.Activate("Testing", newMax:100, showPercent:true, determinative:false);
        BackgroundWorker worker = new BackgroundWorker();
        worker.WorkerReportsProgress = true;
        worker.DoWork += worker_DoWork;
        worker.ProgressChanged += worker_ProgressChanged;

        worker.RunWorkerAsync();*/
    }

    /*void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        for (int i = 0; i < 101; i++)
        {
            (sender as BackgroundWorker).ReportProgress(i);
            Thread.Sleep(100);
        }
        VM.ProgressBarVM.Deactivate();
    }

    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        VM.ProgressBarVM.Inc();
    }*/
}