using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Pages;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

internal class EmployeeCreationVM : INotifyPropertyChanged
{
    public EmployeePageVM? ParentVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    private Dictionary<int, Employee> employees;

    public Employee? Employee { get; set; }

    #region Notifiable Properties

    private string idText;
    public string IDText
    {
        get => idText;
        set
        {
            idText = value;
            OnPropertyChanged(nameof(IDText));
            _ = CheckIDValidity();
        }
    }

    private string confirmToolTip;
    public string ConfirmToolTip
    {
        get => confirmToolTip;
        set
        {
            confirmToolTip = value;
            OnPropertyChanged(nameof(ConfirmToolTip));
        }
    }

    private bool validID;
    public bool ValidID
    {
        get => validID;
        set
        {
            validID = value;
            OnPropertyChanged(nameof(ValidID));
        }
    }

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

    private Department? department;
    public Department? Department
    {
        get => department;
        set
        {
            department = value;
            OnPropertyChanged(nameof(Department));
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

    private Role? role;
    public Role? Role
    {
        get => role;
        set
        {
            role = value;
            OnPropertyChanged(nameof(Role));
        }
    }

    private ObservableCollection<Employee> managers;
    public ObservableCollection<Employee> Managers
    {
        get => managers;
        set
        {
            managers = value;
            OnPropertyChanged(nameof(Managers));
        }
    }

    private Employee? manager;
    public Employee? Manager
    {
        get => manager;
        set
        {
            manager = value;
            OnPropertyChanged(nameof(Manager));
        }
    }
    #endregion

    public ConfirmEmployeeCreationCommand ConfirmEmployeeCreationCommand { get; set; }

    public EmployeeCreationVM()
    {
        confirmToolTip = string.Empty;
        idText = string.Empty;
        employees = new Dictionary<int, Employee>();
        departments = new ObservableCollection<Department>();
        roles = new ObservableCollection<Role>();
        managers = new ObservableCollection<Employee>();

        ConfirmEmployeeCreationCommand = new ConfirmEmployeeCreationCommand(this);
    }

    private int CheckIDValidity()
    {
        ValidID = true;
        ConfirmToolTip = string.Empty;

        if (!int.TryParse(IDText, out var id))
        {
            ValidID = false;
            ConfirmToolTip = string.Join("\n", ConfirmToolTip, "Must be numeric.");
        }

        if (IDText.Length != 5)
        {
            ValidID = false;
            ConfirmToolTip = string.Join("\n", ConfirmToolTip, "Must be 5 digits.");
        }

        if (employees.ContainsKey(id))
        {
            ValidID = false;
            ConfirmToolTip = string.Join("\n", ConfirmToolTip, "Must Be Unique.");
        }

        return id;
    }

    public void SetDataSources(EmployeePageVM pageVM)
    {
        ParentVM = pageVM;
        Helios = pageVM.Helios;
        Charon = pageVM.Charon;

        employees = pageVM.EmployeeDataSet?.Employees ?? new Dictionary<int, Employee>();

        foreach (var parentVMDepartment in ParentVM.Departments)
            if (parentVMDepartment is not null) Departments.Add(parentVMDepartment);

        foreach (var parentVMRole in ParentVM.Roles)
            if (parentVMRole is not null) Roles.Add(parentVMRole);

        Managers = ParentVM.Managers;
    }

    public bool ConfirmEmployeeCreation()
    {
        if (Helios is null) return false;

        var id = CheckIDValidity();

        if (!ValidID) return false;

        Employee = new Employee(id)
        {
            Role = Role,
            RoleName = Role?.Name ?? string.Empty,
            Department = Department,
            DepartmentName = Department?.Name ?? string.Empty,
            ReportsTo = Manager,
            ReportsToID = Manager?.ID ?? 0
        };

        Helios.StaffCreator.Employee(Employee);

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}