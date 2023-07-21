using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Staff.Models;
using Uranus.Users.Models;
using Role = Uranus.Users.Models.Role;

namespace Prometheus.ViewModels.Controls;

public class UserVM : INotifyPropertyChanged
{
    public User User { get; }

    #region User Access

    public Employee? Employee => User.Employee;

    public Role? Role
    {
        get => User.Role;
        set
        {
            User.Role = value;
            OnPropertyChanged();
        }
    }

    public int ID
    {
        get => User.ID;
        set
        {
            User.ID = value;
            OnPropertyChanged();
        }
    }

    public string RoleName
    {
        get => User.RoleName;
        set
        {
            User.RoleName = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public EmployeeIcon? Icon
    {
        get => Employee?.Icon;
        set
        {
            if (Employee is null) return;
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

    public UserVM(User user)
    {
        User = user;
        IconUri = Icon is null ? null : new Uri(Icon.FullPath, UriKind.RelativeOrAbsolute);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}