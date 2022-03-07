using AionClock.ViewModel.Commands;
using System.ComponentModel;
using Uranus.Staff.Model;

namespace AionClock.ViewModel;

public class ClockConfirmationVM : INotifyPropertyChanged
{
    public Employee Employee { get; set; }
    private string status;
    public string Status 
    {
        get => status; 
        set
        {
            status = value;
            OnPropertyChanged(nameof(Status));
        }
    }
    private string name;
    public string Name 
    {
        get => name; 
        set
        {
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public ClockCommand ClockCommand { get; set; }

    public ClockConfirmationVM()
    {
        ClockCommand = new ClockCommand(this);
    }

    public void SetDisplay()
    {
        Status = App.Helios.StaffReader.GetClockCount(Employee.ID) % 2 == 0 ? "IN" : "OUT";
        Name = $"{Employee.FirstName} {Employee.LastName}";
    }

    public ClockEvent ClockIn()
    {
        ClockEvent clock = new() { EmployeeID = Employee.ID, Employee = Employee, Status = EClockStatus.Pending };
        clock.StampTime();
        App.Helios.StaffCreator.ClockEvent(clock);
        App.Helios.StaffCreator.SetShiftEntry(clock.DtDate, Employee);
        return clock;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}