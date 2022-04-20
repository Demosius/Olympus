using Pantheon.Annotations;
using Pantheon.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class DailyRosterVM : INotifyPropertyChanged
{
    public DailyRoster DailyRoster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    public Dictionary<string, ShiftCounter> CounterAccessDict { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<ShiftCounter> shiftCounter;
    public ObservableCollection<ShiftCounter> ShiftCounter
    {
        get => shiftCounter;
        set
        {
            shiftCounter = value;
            OnPropertyChanged(nameof(ShiftCounter));
        }
    }

    #endregion

    public DailyRosterVM(DailyRoster roster, DepartmentRosterVM departmentRosterVM)
    {
        DailyRoster = roster;
        DepartmentRosterVM = departmentRosterVM;
        shiftCounter = new ObservableCollection<ShiftCounter>();
        CounterAccessDict = new Dictionary<string, ShiftCounter>();

        foreach (var counter in DepartmentRosterVM.ShiftTargets)
        {
            var dailyCounter = new ShiftCounter(counter.Shift, counter.Target);
            ShiftCounter.Add(dailyCounter);
            CounterAccessDict.Add(dailyCounter.Shift.ID, dailyCounter);
        }
    }

    public void AddCount(Shift shift)
    {
        CounterAccessDict[shift.ID].Count++;
    }

    public void SubCount(Shift shift)
    {
        CounterAccessDict[shift.ID].Count--;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DailyRoster.ToString();
}