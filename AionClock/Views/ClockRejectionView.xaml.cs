using System;
using System.Windows.Threading;

namespace AionClock.Views;

/// <summary>
/// Interaction logic for ClockRejectionView.xaml
/// </summary>
public partial class ClockRejectionView
{
    public ClockRejectionView(string rejectReason = "Too soon between clock events.")
    {
        InitializeComponent();
        VM.SetReason(rejectReason);

        DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromSeconds(1.2)
        };
        timer.Tick += CloseUp;
        timer.Start();
    }

    private void CloseUp(object sender, EventArgs e)
    {
        Close();
    }
}