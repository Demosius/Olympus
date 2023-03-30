using System;
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

    public bool InUse => ShiftCounters.Any(c => c.Target > 0);

    public bool PublicHoliday
    {
        get => DailyRoster.IsPublicHoliday;
        set
        {
            DailyRoster.IsPublicHoliday = value;
            if (!value) return;
            foreach (var counter in ShiftCounters) counter.Target = 0;
        }
    }

    public DayOfWeek Day => DailyRoster.Day;

    #region INotifyPropertyChanged Members

    private ObservableCollection<DailyCounterVM> shiftCounters;
    public ObservableCollection<DailyCounterVM> ShiftCounters
    {
        get => shiftCounters;
        set
        {
            shiftCounters = value;
            OnPropertyChanged(nameof(ShiftCounters));
        }
    }

    #endregion

    public DailyRosterVM(DailyRoster roster, DepartmentRosterVM departmentRosterVM)
    {
        DailyRoster = roster;
        shiftCounters = new ObservableCollection<DailyCounterVM>();
        CounterAccessDict = new Dictionary<string, DailyCounterVM>();
        Rosters = new Dictionary<int, RosterVM>();

        DepartmentRosterVM = departmentRosterVM;

        foreach (var counter in DailyRoster.ShiftCounters.Select(c => new DailyCounterVM(c, this)))
        {
            ShiftCounters.Add(counter);
            CounterAccessDict.Add(counter.ShiftID, counter);
        }
    }

    public void AddShifts(IEnumerable<Shift> shifts)
    {
        foreach (var shift in shifts)
        {
            if (CounterAccessDict.ContainsKey(shift.ID)) continue;
            var dailyShiftCounter = new DailyCounterVM(new DailyShiftCounter(DailyRoster, shift, shift.DailyTarget), this);
            CounterAccessDict.Add(shift.ID, dailyShiftCounter);
            ShiftCounters.Add(dailyShiftCounter);
        }
    }

    public void SetTarget(Shift shift, int target) => ShiftCounter(shift).Target = target;

    public void SubCount(Shift rosterShift)
    {
        ShiftCounter(rosterShift).Count--;
    }

    public void AddCount(Shift rosterShift)
    {
        ShiftCounter(rosterShift).Count++;
    }

    public DailyCounterVM ShiftCounter(Shift shift)
    {
        if (CounterAccessDict.TryGetValue(shift.ID, out var counterVM)) return counterVM;

        var counter = DailyRoster.ShiftCounter(shift);
        counterVM = new DailyCounterVM(counter, this);
        ShiftCounters.Add(counterVM);
        CounterAccessDict.Add(shift.ID, counterVM);

        return counterVM;
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

    public void SetPublicHoliday(bool isPublicHoliday, bool preserveTargets = false)
    {
        if (!preserveTargets) PublicHoliday = isPublicHoliday;
        else DailyRoster.IsPublicHoliday = isPublicHoliday;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DailyRoster.ToString();
}