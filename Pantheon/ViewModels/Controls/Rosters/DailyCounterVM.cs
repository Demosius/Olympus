using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Rosters;

public class DailyCounterVM : INotifyPropertyChanged
{
    public DailyShiftCounter DailyShiftCounter { get; set; }

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

    #endregion

    public DailyCounterVM(DailyShiftCounter dailyShiftCounter)
    {
        DailyShiftCounter = dailyShiftCounter;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}