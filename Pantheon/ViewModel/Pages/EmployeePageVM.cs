using Pantheon.Properties;
using Pantheon.View;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

    private List<Employee> fullEmployees;

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

    #endregion

    #region Commands
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    #endregion

    public EmployeePageVM()
    {
        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);

        fullEmployees = new List<Employee>();
        employees = new ObservableCollection<Employee>();

        employeeSearchString = string.Empty;
        employmentTypes = new ObservableCollection<EEmploymentType?> { null };
        roles = new ObservableCollection<Role?>();
        departments = new ObservableCollection<Department?>();
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

        // Make sure that the user has an assigned role.
        Charon.UserEmployee.Role ??= Helios.StaffReader.Role(Charon.UserEmployee.RoleName);

        fullEmployees = Helios.StaffReader.GetReportsByRole(Charon.UserEmployee.Role).ToList();
        Employees = new ObservableCollection<Employee>(fullEmployees);

        Departments = new ObservableCollection<Department?>(fullEmployees.Select(employee => employee.Department).Distinct().OrderBy(department => department?.Name));
        selectedDepartment = null;

        Roles = new ObservableCollection<Role?>(fullEmployees.Select(employee => employee.Role).Distinct().OrderBy(role => role?.Name));
        selectedRole = null;

        EmploymentTypes = new ObservableCollection<EEmploymentType?>(Enum.GetValues(typeof(EEmploymentType)).Cast<EEmploymentType?>());
        selectedEmploymentType = null;

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
        IEnumerable<Employee> employeeList = fullEmployees;

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
        ApplySorting(fullEmployees);
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

        fullEmployees.Add(newEmployee);

        SelectedEmployee = newEmployee;

        ApplyFilters();

    }
}