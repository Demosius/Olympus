﻿using Pantheon.Properties;
using Pantheon.View;
using Pantheon.ViewModel.Commands;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Pages;

internal enum ESortMethod
{
    [Description("Full Name")]
    Name,
    [Description("Role => Name")]
    RoleName,
    [Description("Dept. => Role => Name")]
    DepartmentRoleName,
    [Description("EmpType => Role => Name")]
    EmploymentTypeRoleName,
    [Description("Role => EmpType => Name")]
    RoleEmploymentTypeName,
    [Description("ID")]
    ID
}

internal class EmployeePageVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public EmployeeAvatar Avatar => (SelectedEmployee ?? new Employee()).Avatar ?? new EmployeeAvatar();

    private EmployeeDataSet? employeeDataSet;

    private List<Employee> reportingEmployees;

    #region OnPropertyChanged_Properties

    private ObservableCollection<Employee> employees;
    public ObservableCollection<Employee> Employees
    {
        get => employees;
        set
        {
            employees = value;
            OnPropertyChanged(nameof(Employees));
        }
    }

    private Employee? selectedEmployee;
    public Employee? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged(nameof(SelectedEmployee));
            OnPropertyChanged(nameof(Avatar));
        }
    }

    private string employeeSearchString;
    public string EmployeeSearchString
    {
        get => employeeSearchString;
        set
        {
            employeeSearchString = value;
            OnPropertyChanged(nameof(EmployeeSearchString));
            ApplyFilters();
        }
    }

    private ObservableCollection<string> locations;
    public ObservableCollection<string> Locations
    {
        get => locations;
        set
        {
            locations = value;
            OnPropertyChanged(nameof(Locations));
        }
    }

    private ObservableCollection<string> payPoints;
    public ObservableCollection<string> PayPoints
    {
        get => payPoints;
        set
        {
            payPoints = value;
            OnPropertyChanged(nameof(PayPoints));
        }
    }

    private EEmploymentType? selectedEmploymentType;
    public EEmploymentType? SelectedEmploymentType
    {
        get => selectedEmploymentType;
        set
        {
            selectedEmploymentType = value;
            OnPropertyChanged(nameof(SelectedEmploymentType));
            ApplyFilters();
        }
    }

    private ESortMethod selectedESortMethod;
    public ESortMethod SelectedESortMethod
    {
        get => selectedESortMethod;
        set
        {
            selectedESortMethod = value;
            OnPropertyChanged(nameof(SelectedESortMethod));
            ApplyFilters();
        }
    }

    private Role? selectedRole;
    public Role? SelectedRole
    {
        get => selectedRole;
        set
        {
            selectedRole = value;
            OnPropertyChanged(nameof(SelectedRole));
            ApplyFilters();
        }
    }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged(nameof(SelectedDepartment));
            ApplyFilters();
        }
    }

    private ObservableCollection<EEmploymentType?> employmentTypes;
    public ObservableCollection<EEmploymentType?> EmploymentTypes
    {
        get => employmentTypes;
        set
        {
            employmentTypes = value;
            OnPropertyChanged(nameof(EmploymentTypes));
        }
    }

    private ObservableCollection<Role?> roles;
    public ObservableCollection<Role?> Roles
    {
        get => roles;
        set
        {
            roles = value;
            OnPropertyChanged(nameof(Roles));
        }
    }

    private ObservableCollection<Department?> departments;
    public ObservableCollection<Department?> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged(nameof(Departments));
        }
    }

    private ObservableCollection<Department> fullDepartments;
    public ObservableCollection<Department> FullDepartments
    {
        get => fullDepartments;
        set
        {
            fullDepartments = value;
            OnPropertyChanged(nameof(FullDepartments));
        }
    }

    private ObservableCollection<Role> allRoles;
    public ObservableCollection<Role> AllRoles
    {
        get => allRoles;
        set
        {
            allRoles = value;
            OnPropertyChanged(nameof(AllRoles));
        }
    }

    private ObservableCollection<Clan> clans;

    public ObservableCollection<Clan> Clans
    {
        get => clans;
        set
        {
            clans = value;
            OnPropertyChanged(nameof(Clans));
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

    private bool useAllAsManagers;
    public bool UseAllAsManagers
    {
        get => useAllAsManagers;
        set
        {
            useAllAsManagers = value;
            OnPropertyChanged(nameof(UseAllAsManagers));
            if (employeeDataSet is null) return;
            Managers = useAllAsManagers ?
                new ObservableCollection<Employee>(employeeDataSet.Employees.Values) :
                new ObservableCollection<Employee>(employeeDataSet.Managers);
        }
    }

    #endregion

    #region Commands
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public AddLocationCommand AddLocationCommand { get; set; }
    public AddDepartmentCommand AddDepartmentCommand { get; set; }
    public AddRoleCommand AddRoleCommand { get; set; }
    public AddClanCommand AddClanCommand { get; set; }
    public AddPayPointCommand AddPayPointCommand { get; set; }
    public CreateNewEmployeeCommand CreateNewEmployeeCommand { get; set; }
    public SaveEmployeeCommand SaveEmployeeCommand { get; set; }
    #endregion

    public EmployeePageVM()
    {
        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        AddLocationCommand = new AddLocationCommand(this);
        AddDepartmentCommand = new AddDepartmentCommand(this);
        AddRoleCommand = new AddRoleCommand(this);
        AddClanCommand = new AddClanCommand(this);
        AddPayPointCommand = new AddPayPointCommand(this);
        CreateNewEmployeeCommand = new CreateNewEmployeeCommand(this);
        SaveEmployeeCommand = new SaveEmployeeCommand(this);

        reportingEmployees = new List<Employee>();
        employees = new ObservableCollection<Employee>();

        employeeSearchString = string.Empty;
        employmentTypes = new ObservableCollection<EEmploymentType?> { null };
        roles = new ObservableCollection<Role?>();
        departments = new ObservableCollection<Department?>();
        locations = new ObservableCollection<string>();
        fullDepartments = new ObservableCollection<Department>();
        allRoles = new ObservableCollection<Role>();
        managers = new ObservableCollection<Employee>();
        clans = new ObservableCollection<Clan>();
        payPoints = new ObservableCollection<string>();
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        Task.Run(RefreshData);
    }

    public void RefreshData()
    {
        if (Charon is null || Helios is null) return;

        employeeDataSet = Helios.StaffReader.EmployeeDataSet();

        // Make sure that the user has an assigned role.
        if (Charon.UserEmployee is not null && Charon.UserEmployee.Role is null)
            if (employeeDataSet.Roles.TryGetValue(Charon.UserEmployee.RoleName, out var role))
                Charon.UserEmployee.Role = role;

        // Reporting employees (and other collections for filtering that list) is base purely on the employees that report to the current user.
        reportingEmployees = employeeDataSet.GetReportsByRole(Charon.UserEmployee?.ID ?? 0).ToList();

        Departments = new ObservableCollection<Department?>(reportingEmployees.Select(employee => employee.Department).Distinct().OrderBy(department => department?.Name));
        selectedDepartment = null;

        Roles = new ObservableCollection<Role?>(reportingEmployees.Select(employee => employee.Role).Distinct().OrderBy(role => role?.Name));
        selectedRole = null;

        EmploymentTypes = new ObservableCollection<EEmploymentType?>(Enum.GetValues(typeof(EEmploymentType)).Cast<EEmploymentType?>());
        selectedEmploymentType = null;

        // Here we want the full potential lists of data.
        Locations = new ObservableCollection<string>(employeeDataSet.Locations);
        PayPoints = new ObservableCollection<string>(employeeDataSet.PayPoints);
        FullDepartments = new ObservableCollection<Department>(employeeDataSet.Departments.Values.OrderBy(d => d.Name));
        AllRoles = new ObservableCollection<Role>(employeeDataSet.Roles.Values.OrderBy(r => r.DepartmentName).ThenBy(r => r.Level));
        UseAllAsManagers = false;

        employeeSearchString = "";

        ApplyFilters();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ClearFilters()
    {
        SelectedDepartment = null;
        SelectedEmploymentType = null;
        SelectedRole = null;
        EmployeeSearchString = "";
        ApplySorting();
    }

    public void ApplyFilters()
    {
        IEnumerable<Employee> employeeList = reportingEmployees;

        if (SelectedDepartment is not null)
            employeeList = employeeList.Where(e => e.Department == SelectedDepartment);
        if (SelectedEmploymentType is not null)
            employeeList = employeeList.Where(e => e.EmploymentType == SelectedEmploymentType);
        if (SelectedRole is not null)
            employeeList = employeeList.Where(e => e.Role == SelectedRole);
        if (EmployeeSearchString != "")
        {
            var regex = new Regex(EmployeeSearchString, RegexOptions.IgnoreCase);
            employeeList = employeeList.Where(e => regex.IsMatch(e.FullName));
        }

        ApplySorting(employeeList);
    }

    public void ApplySorting()
    {
        ApplySorting(reportingEmployees);
    }

    public void ApplySorting(IEnumerable<Employee> employeeList)
    {
        employeeList = SelectedESortMethod switch
        {
            ESortMethod.Name => employeeList.OrderBy(e => e.FullName),
            ESortMethod.RoleName => employeeList.OrderBy(e => e.Role).ThenBy(e => e.FullName),
            ESortMethod.DepartmentRoleName => employeeList.OrderBy(e => e.Department)
                .ThenBy(e => e.Role)
                .ThenBy(e => e.FullName),
            ESortMethod.EmploymentTypeRoleName => employeeList.OrderBy(e => e.EmploymentType)
                .ThenBy(e => e.Role)
                .ThenBy(e => e.FullName),
            ESortMethod.RoleEmploymentTypeName => employeeList.OrderBy(e => e.Role)
                .ThenBy(e => e.EmploymentType)
                .ThenBy(e => e.FullName),
            ESortMethod.ID => employeeList.OrderBy(e => e.ID),
            _ => employeeList.OrderBy(e => e.EmploymentType).ThenBy(e => e.FullName)
        };
        Employees = new ObservableCollection<Employee>(employeeList);
    }

    public void CreateNewEmployee()
    {
        if (Helios is null) return;

        var employeeCreationWindow = new EmployeeCreationWindow();
        if (employeeCreationWindow.ShowDialog() != true) return;

        var newEmployee = employeeCreationWindow.VM.Employee;

        if (newEmployee == null) return;

        if (Helios.StaffReader.EmployeeExists(newEmployee.ID)) return;

        reportingEmployees.Add(newEmployee);

        SelectedEmployee = newEmployee;

        ApplyFilters();

    }

    public void SaveEmployee()
    {
        if (Helios is null || SelectedEmployee is null) return;

        if (Helios.StaffUpdater.Employee(SelectedEmployee) > 0)
            MessageBox.Show($"Successfully saved changes to {SelectedEmployee.FullName}.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);

    }

    public void AddLocation()
    {
        var input = new InputWindow("Enter new location:", "New Location");
        if (input.ShowDialog() != true) return;

        Locations.Add(input.VM.Input);
        if (SelectedEmployee is not null) SelectedEmployee.Location = input.VM.Input;
    }

    public void AddDepartment()
    {

    }

    public void AddRole()
    {

    }

    public void AddClan()
    {

    }

    public void AddPayPoint()
    {
        var input = new InputWindow("Enter new Pay Point:", "New PayPoint");
        if (input.ShowDialog() != true) return;

        PayPoints.Add(input.VM.Input);
        if (SelectedEmployee is not null) SelectedEmployee.PayPoint = input.VM.Input;
    }
}