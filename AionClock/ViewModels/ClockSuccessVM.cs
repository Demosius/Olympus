using System;
using System.ComponentModel;
using Uranus.Staff.Models;

namespace AionClock.ViewModels;

public class ClockSuccessVM : INotifyPropertyChanged
{
    private ClockEvent clock;
    public ClockEvent Clock
    {
        get => clock;
        set
        {
            clock = value;
            EmployeeName = clock.Employee?.ToString();
            TimeStamp = DateTime.Parse(clock.Timestamp).ToString("dddd, d-MMM-yyyy, HH:mmtt");
            Action = App.Helios.StaffReader.GetClockCount(clock.EmployeeID) % 2 == 0 ? "OUT" : "IN";
        }
    }

    private string employeeName;
    public string EmployeeName
    {
        get => employeeName;
        set
        {
            employeeName = value;
            OnPropertyChanged(nameof(EmployeeName));
        }
    }

    private string timeStamp;
    public string TimeStamp
    {
        get => timeStamp;
        set
        {
            timeStamp = value;
            OnPropertyChanged(nameof(TimeStamp));
        }
    }

    private string action = "IN"; // In/Out
    public string Action
    {
        get => action;
        set
        {
            action = value;
            OnPropertyChanged(nameof(Action));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}