using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class DailyRosterVM : INotifyPropertyChanged
{
    public DailyRoster DailyRoster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    public Dictionary<string, DailyCounterVM> CounterAccessDict { get; set; }

    public Dictionary<int, RosterVM> Rosters { get; set; }

    public bool PublicHoliday
    {
        get => DailyRoster.IsPublicHoliday;
        set => DailyRoster.IsPublicHoliday = value;
    }

    #region INotifyPropertyChanged Members

    private ObservableCollection<DailyCounterVM> shiftCounter;
    public ObservableCollection<DailyCounterVM> ShiftCounter
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
        shiftCounter = new ObservableCollection<DailyCounterVM>();
        CounterAccessDict = new Dictionary<string, DailyCounterVM>();
        Rosters = new Dictionary<int, RosterVM>();

        DepartmentRosterVM = departmentRosterVM;

        foreach (var counter in DailyRoster.ShiftCounters.Select(c => new DailyCounterVM(c)))
        {
            ShiftCounter.Add(counter);
            CounterAccessDict.Add(counter.ShiftID, counter);
        }
    }

    public void AddShifts(IEnumerable<Shift> shifts)
    {
        foreach (var shift in shifts)
        {
            if (CounterAccessDict.ContainsKey(shift.ID)) continue;
            var dailyShiftCounter = new DailyCounterVM(new DailyShiftCounter(DailyRoster, shift, shift.DailyTarget));
            CounterAccessDict.Add(shift.ID, dailyShiftCounter);
            ShiftCounter.Add(dailyShiftCounter);
        }
    }

    public void SubShift(Shift rosterShift)
    {
        CounterAccessDict[rosterShift.ID].Count--;
    }

    public void AddShift(Shift rosterShift)
    {
        CounterAccessDict[rosterShift.ID].Count++;
    }
    
    /// <summary>
    /// Sets all rosters as public holiday.
    /// </summary>
    public void SetPublicHoliday(bool isPublicHoliday = true) => DailyRoster.SetPublicHoliday(isPublicHoliday);
    /*{
        // Do not set roster type directly, as that will result in recursive prompting.
        // Use SetPublicHoliday method.
        PublicHoliday = isPublicHoliday;
        foreach (var (_, rosterVM) in Rosters)
            rosterVM.SetPublicHoliday(isPublicHoliday);
    }*/

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