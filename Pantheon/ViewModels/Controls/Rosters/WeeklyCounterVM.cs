using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class WeeklyCounterVM : INotifyPropertyChanged
{
    public WeeklyShiftCounter WeeklyShiftCounter { get; set; }
    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    #region Direct Counter Access
    
    public Shift? Shift => WeeklyShiftCounter.Shift;
    
    // Handle the targets of daily shifts of the roster as well.
    public int Target
    {
        get => WeeklyShiftCounter.Target;
        set
        {
            WeeklyShiftCounter.Target = value;

            if (DepartmentRosterVM.LinkTargets) DepartmentRosterVM.MatchWeeklyTargets();

            OnPropertyChanged();
            DepartmentRosterVM.RefreshTargets();
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

    public bool OverTarget => Count >= Target;

    public float Priority => OverTarget ? 0 : Target / (float) (Count == 0 ? 0.5 : Count);

    #endregion

    #region INotifyPropertyChanged Members


    #endregion

    public WeeklyCounterVM(WeeklyShiftCounter weeklyShiftCounter, DepartmentRosterVM departmentRosterVM)
    {
        WeeklyShiftCounter = weeklyShiftCounter;
        DepartmentRosterVM = departmentRosterVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}