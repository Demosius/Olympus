using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pantheon.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class DailyRosterVM : INotifyPropertyChanged
{
    public DailyRoster DailyRoster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<KeyValuePair<Shift, int>> shiftCounter;
    public ObservableCollection<KeyValuePair<Shift, int>> ShiftCounter
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
        shiftCounter = new ObservableCollection<KeyValuePair<Shift, int>>();

        foreach (var (shift, _) in DepartmentRosterVM.ShiftTargets)
            shiftCounter.Add(new KeyValuePair<Shift, int>(shift, 0));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString() => DailyRoster.ToString();
}