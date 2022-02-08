using System;
using System.Windows.Threading;
using Uranus.Staff.Model;

namespace AionClock.View
{
    /// <summary>
    /// Interaction logic for ClockSuccessView.xaml
    /// </summary>
    public partial class ClockSuccessView
    {
        public ClockSuccessView(ClockEvent clock)
        {
            InitializeComponent();
            VM.Clock = clock;

            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1.2)
            };
            timer.Tick += CloseUp;
            timer.Start();
        }

        void CloseUp(object sender, EventArgs e)
        {
            Close();
        }
    }
}
