using Pantheon.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

public class DailyRosterVM : INotifyPropertyChanged
{
    public DailyRoster DailyRoster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    public Dictionary<string, ShiftCounter> CounterAccessDict { get; set; }

    public Dictionary<int, RosterVM> Rosters { get; set; }

    public bool PublicHoliday { get; set; }

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
        Rosters = new Dictionary<int, RosterVM>();

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

    /// <summary>
    /// Sets all rosters as public holiday.
    /// </summary>
    public void SetPublicHoliday(bool isPublicHoliday = true)
    {
        // Do not set roster type directly, as that will result in recursive prompting.
        // Use SetPublicHoliday method.
        PublicHoliday = isPublicHoliday;
        foreach (var (_, rosterVM) in Rosters)
            rosterVM.SetPublicHoliday(isPublicHoliday);
    }

    /// <summary>
    /// Sets associated rosters to standard, typically used when switching from public holiday.
    /// </summary>
    public void SetStandard()
    {
        PublicHoliday = false;
        foreach (var (_, rosterVM) in Rosters)
            rosterVM.Type = ERosterType.Standard;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DailyRoster.ToString();
}