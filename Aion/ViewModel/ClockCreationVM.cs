using Aion.ViewModel.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public class ClockCreationVM : INotifyPropertyChanged
    {
        public Helios Helios { get; set; }

        private ObservableCollection<Employee> employees;
        public ObservableCollection<Employee> Employees
        {
            get => employees;
            set
            {
                employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        public Employee SelectedEmployee { get; set; }

        public DateTime Date { get; set; }

        public ClockEvent NewClock { get; set; }

        public CreateClockCommand CreateClockCommand { get; set; }

        public ClockCreationVM()
        {
            CreateClockCommand = new(this);
        }

        public void SetDataSource(Helios helios)
        {
            Helios = helios;
            Employees = new(Helios.StaffReader.Employees());
            Date = DateTime.Now.Date;
        }

        public void CreateClock()
        {
            if (SelectedEmployee is null) return;

            NewClock = new() { EmployeeID = SelectedEmployee.ID, Status = EClockStatus.Pending, Timestamp = Date.ToString("yyyy-MM-dd HH:mm:ss") };
            NewClock.Employee = Helios.StaffReader.Employee(NewClock.EmployeeID);
            Helios.StaffCreator.ClockEvent(NewClock);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

    }
}
