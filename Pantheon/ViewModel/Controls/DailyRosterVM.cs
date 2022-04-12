using Pantheon.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class DailyRosterVM : INotifyPropertyChanged
{
    public DailyRoster DailyRoster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }

    public DailyRosterVM(DailyRoster roster, DepartmentRosterVM departmentRosterVM)
    {
        DailyRoster = roster;
        DepartmentRosterVM = departmentRosterVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}