using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class DailyCounterVM : INotifyPropertyChanged
{
    public DailyShiftCounter DailyShiftCounter { get; set; }
    public DailyRosterVM DailyRosterVM { get; set; }
    public DepartmentRosterVM DepartmentRosterVM => DailyRosterVM.DepartmentRosterVM;

    #region Direct Counter Access

    public string ShiftID => DailyShiftCounter.ShiftID;
    public string ShiftName => DailyShiftCounter.Shift?.Name ?? DailyShiftCounter.ShiftID.Split('|')[1];

    public int Target
    {
        get => DailyShiftCounter.Target;
        set
        {
            DailyShiftCounter.Target = value;
            OnPropertyChanged();
            // TODO: Refresh targets??
        }
    }

    public int Count
    {
        get => DailyShiftCounter.Count;
        set
        {
            DailyShiftCounter.Count = value;
            OnPropertyChanged();
        }
    }

    public bool OnTarget => Count == Target;
    public bool UnderTarget => Count < Target;
    public bool OverTarget => Count > Target;

    #endregion

    public DailyCounterVM(DailyShiftCounter dailyShiftCounter, DailyRosterVM dailyRosterVM)
    {
        DailyShiftCounter = dailyShiftCounter;
        DailyRosterVM = dailyRosterVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}