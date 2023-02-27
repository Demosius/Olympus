using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Pages;
using Styx;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

internal class RoleCreationVM : INotifyPropertyChanged
{
    public EmployeePageVM? ParentVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public Role Role { get; set; }

    #region Notifiable Properties
    private ObservableCollection<Department> departments;
    public ObservableCollection<Department> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged(nameof(Departments));
        }
    }

    private ObservableCollection<Role> roles;
    public ObservableCollection<Role> Roles
    {
        get => roles;
        set
        {
            roles = value;
            OnPropertyChanged(nameof(Roles));
        }
    }

    #endregion

    public ConfirmRoleCreationCommand ConfirmRoleCreationCommand { get; set; }

    public RoleCreationVM()
    {
        Role = new Role
        {
            Name = string.Empty
        };
        roles = new ObservableCollection<Role>();
        departments = new ObservableCollection<Department>();

        ConfirmRoleCreationCommand = new ConfirmRoleCreationCommand(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetDataSources(EmployeePageVM employeePage)
    {
        ParentVM = employeePage;
        Helios = employeePage.Helios;
        Charon = employeePage.Charon;

        foreach (var parentVMDepartment in ParentVM.Departments)
            if (parentVMDepartment is not null) Departments.Add(parentVMDepartment);

        Roles = new ObservableCollection<Role>(ParentVM.AllRoles);

    }

    public bool ConfirmRoleCreation()
    {
        if (Role.Name is null or "" || Helios is null || Role.Department is null) return false;

        if (Role.ReportsToRole is not null)
        {
            Role.ReportsToRoleName = Role.ReportsToRole.Name;
            Role.Level = Role.ReportsToRole.Level - 1;
        }

        Role.DepartmentName = Role.Department.Name;

        return Helios.StaffCreator.Role(Role);
    }
}