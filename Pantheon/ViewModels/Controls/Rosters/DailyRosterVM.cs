using Pantheon.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class DailyRosterVM : INotifyPropertyChanged
{
    public DailyRoster DailyRoster { get; set; }
    
    public Dictionary<string, DailyShiftCounter> CounterAccessDict { get; set; }

    public Dictionary<int, RosterVM> Rosters { get; set; }

    public bool PublicHoliday
    {
        get => DailyRoster.IsPublicHoliday;
        set => DailyRoster.IsPublicHoliday = value;
    }

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

    public DailyRosterVM(DailyRoster roster)
    {
        DailyRoster = roster;
        shiftCounter = new ObservableCollection<ShiftCounter>();
        CounterAccessDict = new Dictionary<string, DailyShiftCounter>();
        Rosters = new Dictionary<int, RosterVM>();

        foreach (var counter in DailyRoster.ShiftCounters)
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
            var dailyShiftCounter = new DailyShiftCounter(DailyRoster, shift, shift.DailyTarget);
            CounterAccessDict.Add(shift.ID, dailyShiftCounter);
            ShiftCounter.Add(dailyShiftCounter);
        }
    } 

    /*
    public void AddCount(Shift shift)
    {
        CounterAccessDict[shift.ID].Count++;
    }

    public void SubCount(Shift shift)
    {
        CounterAccessDict[shift.ID].Count--;
    }
    */

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