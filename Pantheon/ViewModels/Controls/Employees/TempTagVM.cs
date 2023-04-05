using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.Employees;

public class TempTagVM :INotifyPropertyChanged 
{
    public TempTag TempTag { get; }

    #region TempTag Access

    public bool IsAssigned => TempTag.Employee is not null;

    public string RF_ID => TempTag.RF_ID;

    public Employee? Employee => TempTag.Employee;

    public int EmployeeID => TempTag.EmployeeID;

    public string EmployeeString => Employee is null ? EmployeeID == 0 ? "" : EmployeeID.ToString() : $"{Employee.ID} - {Employee.FullName}";

    #endregion

    public TempTagVM(TempTag tempTag)
    {
        TempTag = tempTag;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}