using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Prometheus.ViewModels.Controls;

public class EmployeeVM : INotifyPropertyChanged
{
    public Employee Employee { get; }

    #region Employee Access

    public string FullName => Employee.FullName;

    public Role? Role => Employee.Role;

    public Department? Department => Employee.Department;

    public EEmploymentType EmploymentType => Employee.EmploymentType;

    public int ID => Employee.ID;

    public bool IsUser => Employee.IsUser;

    #endregion

    #region INotifyPropertyChanged Members


    public EmployeeIcon? Icon
    {
        get => Employee.Icon;
        set
        {
            Employee.Icon = value;
            OnPropertyChanged();
            IconUri = new Uri(value?.FullPath ?? "", UriKind.RelativeOrAbsolute);
        }
    }

    private Uri? iconUri;
    public Uri? IconUri
    {
        get => iconUri;
        set
        {
            iconUri = value;
            OnPropertyChanged();
        }
    }


    #endregion

    public EmployeeVM(Employee employee)
    {
        Employee = employee;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}