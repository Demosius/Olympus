using Pantheon.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Controls;

internal class RosterVM : INotifyPropertyChanged
{
    public Roster Roster { get; set; }

    public DepartmentRosterVM DepartmentRosterVM { get; set; }
    public DailyRosterVM DailyRosterVM { get; set; }
    public EmployeeRosterVM EmployeeRosterVM { get; set; }

    public RosterVM(Roster roster, DepartmentRosterVM departmentRosterVM, DailyRosterVM dailyRosterVM, EmployeeRosterVM employeeRosterVM)
    {
        Roster = roster;
        DepartmentRosterVM = departmentRosterVM;
        DailyRosterVM = dailyRosterVM;
        EmployeeRosterVM = employeeRosterVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}