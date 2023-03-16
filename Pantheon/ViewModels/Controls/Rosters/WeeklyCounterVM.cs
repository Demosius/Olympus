using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class WeeklyCounterVM : INotifyPropertyChanged
{
    public WeeklyShiftCounter WeeklyShiftCounter { get; set; }

    #region Direct Counter Access

    public DepartmentRoster? Roster => WeeklyShiftCounter.Roster;

    public Shift? Shift => WeeklyShiftCounter.Shift;
    
    // Handle the targets of daily shifts of the roster as well.
    public int Target
    {
        get => WeeklyShiftCounter.Target;
        set
        {
            if (Roster is null) throw new ArgumentNullException(nameof(Roster));

            foreach (var dailyCounter in Roster.DailyShiftCounters(ShiftID))
            {
                dailyCounter.Target -= Target;
                dailyCounter.Target += value;
                if (dailyCounter.Target < 0) dailyCounter.Target = 0;
            }

            WeeklyShiftCounter.Target = value;
            OnPropertyChanged();
        }
    }

    public int Count
    {
        get => WeeklyShiftCounter.Count;
        set
        {
            WeeklyShiftCounter.Count = value;
            OnPropertyChanged();
        }
    }

    public string ShiftID => WeeklyShiftCounter.ShiftID;

    public int Discrepancy => WeeklyShiftCounter.Discrepancy;

    public bool Lacking => WeeklyShiftCounter.Lacking;

    #endregion

    #region INotifyPropertyChanged Members


    #endregion

    public WeeklyCounterVM(WeeklyShiftCounter weeklyShiftCounter)
    {
        WeeklyShiftCounter = weeklyShiftCounter;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}