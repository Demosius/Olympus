using AionClock.View;
using AionClock.ViewModel.Commands;
using System;
using System.ComponentModel;
using System.Linq;
using AionClock.Properties;
using AionClock.ViewModel.Utility;
using Uranus.Staff.Model;

namespace AionClock.ViewModel
{
    public class ClockInVM : INotifyPropertyChanged
    {
        private string input;
        public string Input
        {
            get => input;
            set
            {
                input = value;
                OnPropertyChanged(nameof(Input));
                switch (input.Length)
                {
                    case 5:
                        CheckEmployeeCode();
                        break;
                    case > 5:
                        input = input[..5];
                        OnPropertyChanged(nameof(Input));
                        break;
                }
            }
        }

        public DBManager DBManager { get; set; }

        public InputKeyCommand InputKeyCommand { get; set; }
        public ClearInputCommand ClearInputCommand { get; set; }
        public BackspaceCommand BackspaceCommand { get; set; }
        public LaunchClockConfirmCommand LaunchClockConfirmCommand { get; set; }

        public ClockInVM()
        {
            Input = "";
            InputKeyCommand = new(this);
            ClearInputCommand = new(this);
            BackspaceCommand = new(this);
            LaunchClockConfirmCommand = new();
            DBManager = new();
        }

        public static void ResetDB()
        {
            App.Helios.ResetChariots(Settings.Default.SolLocation);
        }

        public void CheckEmployeeCode()
        {
            if (int.TryParse(Input, out var num))
            {
                try
                {
                    var e = App.Helios.StaffReader.Employee(num);
                    if (e is null) throw new();
                    CheckClock(e);
                    Input = "";
                }
                catch
                {
                    // TODO: bump/flash error
                }
            }
            else
            {
                Input = "";
            }
        }

        /// <summary>
        /// Checks for possible reasons to reject the attempted clock. 
        /// Will either launch confirmation or rejection from this function.
        /// </summary>
        public static void CheckClock(Employee employee)
        {
            // Get a list of today's clocks for this employee.
            var todayClocks = App.Helios.StaffReader.ClocksForToday(employee.ID);
                
            var closeClocks = todayClocks.Where(c => DateTime.Now.Subtract(DateTime.Parse($"{c.Date} {c.Time}")).Duration().TotalMinutes < 5).ToArray();

            if (closeClocks.Any())
            {
                LaunchClockRejection($"Time too close to existing clock event: {closeClocks.First()}");
            }
            else
            {
                LaunchClockConfirm(employee);
            }
        }

        public static void LaunchClockConfirm(Employee employee)
        {
            ClockConfirmationView confirmationView = new(employee);
            confirmationView.ShowDialog();
        }

        public static void LaunchClockRejection(string rejectReason)
        {
            ClockRejectionView rejectionView = new(rejectReason);
            rejectionView.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
