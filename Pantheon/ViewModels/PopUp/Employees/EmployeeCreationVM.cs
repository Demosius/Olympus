using System;
using Pantheon.ViewModels.Commands.Employees;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class EmployeeCreationVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private Dictionary<int, Employee> employees;

    private List<Role> allRoles;

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

    private bool? validID;
    public bool? ValidID
    {
        get => validID;
        set
        {
            validID = value;
            OnPropertyChanged(nameof(ValidID));
        }
    }
    
    public ObservableCollection<Department> Departments { get; set; }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged(nameof(SelectedDepartment));
            SetRoles();
            DetectManager();
        }
    }
    
    public ObservableCollection<Role> Roles { get; set; }

    private Role? selectedRole;
    public Role? SelectedRole
    {
        get => selectedRole;
        set
        {
            selectedRole = value;
            OnPropertyChanged(nameof(SelectedRole));
        }
    }
    
    public ObservableCollection<Employee> Managers { get; set; }

    private Employee? selectedManager;
    public Employee? SelectedManager
    {
        get => selectedManager;
        set
        {
            selectedManager = value;
            OnPropertyChanged(nameof(SelectedManager));
        }
    }
    #endregion

    public ConfirmEmployeeCreationCommand ConfirmEmployeeCreationCommand { get; set; }

    private EmployeeCreationVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        confirmToolTip = string.Empty;
        idText = string.Empty;

        employees = new Dictionary<int, Employee>();
        allRoles = new List<Role>();
        Departments = new ObservableCollection<Department>();
        Roles = new ObservableCollection<Role>();
        Managers = new ObservableCollection<Employee>();

        ConfirmEmployeeCreationCommand = new ConfirmEmployeeCreationCommand(this);
    }

    private async Task<EmployeeCreationVM> InitializeAsync()
    {
        await SetData();
        return this;
    }

    public static Task<EmployeeCreationVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new EmployeeCreationVM(helios, charon);
        return ret.InitializeAsync();
    }

    private async Task SetData()
    {
        var employeeTask = Helios.StaffReader.EmployeesAsync(e => true);
        var managerTask = Helios.StaffReader.GetManagersAsync();
        var departmentTask = Helios.StaffReader.DepartmentsAsync();
        var roleTask = Helios.StaffReader.RolesAsync();

        await Task.WhenAll(employeeTask, managerTask, departmentTask, roleTask);

        employees = (await employeeTask).ToDictionary(e => e.ID, e => e);
        allRoles = (await roleTask).OrderBy(r => r.DepartmentName).ThenBy(r => r.Name).ToList();

        var roleDict = allRoles.ToDictionary(r => r.Name, r => r);
        
        foreach (var department in (await departmentTask).OrderBy(d => d.Name))
            Departments.Add(department);

        foreach (var manager in (await managerTask).OrderBy(m => m.FullName))
        {
            Managers.Add(manager);
            if (roleDict.TryGetValue(manager.RoleName, out var role))
                manager.Role = role;
        }

        SetRoles();
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

        if (!employees.ContainsKey(id)) return id;

        if (employees[id].IsActive)
            ValidID = false;
        else
            ValidID = null;
        ConfirmToolTip = string.Join("\n", ConfirmToolTip, "Must Be Unique.");

        return id;
    }

    private void SetRoles()
    {
        Roles.Clear();

        foreach (var role in allRoles.Where(role => SelectedDepartment is null || role.DepartmentName == SelectedDepartment.Name))
            Roles.Add(role);

        SelectedRole = null;
    }

    private void DetectManager()
    {
        if (SelectedDepartment is null) return;
        var shortList = Managers.Where(m => m.DepartmentName == SelectedDepartment.Name).ToList();
        if (!shortList.Any()) return;

        if (shortList.Any(m => m.RoleName.Contains("manager", StringComparison.OrdinalIgnoreCase)))
            shortList = shortList.Where(m => m.RoleName.Contains("manager", StringComparison.OrdinalIgnoreCase)).ToList();

        shortList = shortList.OrderByDescending(m => m.Role?.Level ?? 0).ToList();

        SelectedManager = shortList.FirstOrDefault();
    }

    public bool ConfirmEmployeeCreation()
    {
        var id = CheckIDValidity();

        switch (ValidID)
        {
            case false:
            case null when MessageBox.Show(
                $"Employee with ID ({IDText}) already exists, but is inactive. Would you like to reactive this employee?",
                "Reactivate?", MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
                           MessageBoxResult.Yes:
                return false;
            case null:
                Employee = employees[id];

                var role = allRoles.FirstOrDefault(r => r.Name == Employee.RoleName);
                Employee.Role = role ?? SelectedRole;
                Employee.RoleName = Employee.Role?.Name ?? string.Empty;
                var department = Departments.FirstOrDefault(d => Employee.DepartmentName == d.Name);
                Employee.Department = department ?? SelectedDepartment;
                Employee.DepartmentName = Employee.Department?.Name ?? string.Empty;
                var manager = employees.Values.FirstOrDefault(e => e.ID == Employee.ReportsToID);
                Employee.ReportsTo = manager ?? SelectedManager;
                Employee.ReportsToID = Employee.ReportsTo?.ID ?? 0;
                Employee.IsActive = true;
                try
                {
                    Helios.StaffUpdater.Employee(Employee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected Error:\n\n{ex}", "Unexpected Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Log.Error(ex, "Update reactivated employee.");
                }
                break;
            default:
                Employee = new Employee(id)
                {
                    Role = SelectedRole,
                    RoleName = SelectedRole?.Name ?? string.Empty,
                    Department = SelectedDepartment,
                    DepartmentName = SelectedDepartment?.Name ?? string.Empty,
                    ReportsTo = SelectedManager,
                    ReportsToID = SelectedManager?.ID ?? 0
                };
                try
                {
                    Helios.StaffCreator.Employee(Employee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected Error:\n\n{ex}", "Unexpected Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Log.Error(ex, "Create new employee.");
                }
                break;
        }

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}