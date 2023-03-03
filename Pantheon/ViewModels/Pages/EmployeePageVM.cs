using Pantheon.ViewModels.Commands.Employees;
using Pantheon.Views.PopUp.Employees;
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
using Pantheon.Annotations;
using Pantheon.ViewModels.Controls.Employees;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Pages;

public enum ESortMethod
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

public class EmployeePageVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public EmployeeAvatar Avatar => (SelectedEmployeeVM ?? new EmployeeVM(new Employee(), Charon, Helios)).Avatar ?? new EmployeeAvatar();

    public EmployeeDataSet? EmployeeDataSet;

    public List<EmployeeVM> ReportingEmployees { get; set; }

    public Employee? SelectedEmployee => SelectedEmployeeVM?.Employee;

    #region OnPropertyChanged_Properties

    private ObservableCollection<EmployeeVM> employees;
    public ObservableCollection<EmployeeVM> Employees
    {
        get => employees;
        set
        {
            employees = value;
            OnPropertyChanged(nameof(Employees));
        }
    }

    private EmployeeVM? selectedEmployeeVM;
    public EmployeeVM? SelectedEmployeeVM
    {
        get => selectedEmployeeVM;
        set
        {
            selectedEmployeeVM = value;
            OnPropertyChanged(nameof(Avatar));
            OnPropertyChanged(nameof(SensitiveVisibility));
            OnPropertyChanged(nameof(VerySensitiveVisibility));
            OnPropertyChanged();
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

    private ObservableCollection<string> roleNames;
    public ObservableCollection<string> RoleNames
    {
        get => roleNames;
        set
        {
            roleNames = value;
            OnPropertyChanged(nameof(RoleNames));
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
            if (EmployeeDataSet is null) return;
            Managers = useAllAsManagers ?
                new ObservableCollection<Employee>(EmployeeDataSet.Employees.Values) :
                new ObservableCollection<Employee>(EmployeeDataSet.Managers);
        }
    }

    // As determined by employee.
    public bool SensitiveVisibility => SelectedEmployeeVM is not null && (Charon.CanReadEmployeeSensitive(SelectedEmployeeVM.Employee));
    public bool VerySensitiveVisibility => SelectedEmployeeVM is not null && (Charon.CanReadEmployeeVerySensitive(SelectedEmployeeVM.Employee));

    #endregion

    #region Commands
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public CreateNewEmployeeCommand CreateNewEmployeeCommand { get; set; }
    public DeleteEmployeeCommand DeleteEmployeeCommand { get; set; }
    public ActivateUserCommand ActivateUserCommand { get; set; }
    public LaunchAvatarSelectorCommand LaunchAvatarSelectorCommand { get; set; }
    public FillFullTimeRostersCommand FillFullTimeRostersCommand { get; set; }
    #endregion

    public EmployeePageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        CreateNewEmployeeCommand = new CreateNewEmployeeCommand(this);
        DeleteEmployeeCommand = new DeleteEmployeeCommand(this);
        ActivateUserCommand = new ActivateUserCommand(this);
        LaunchAvatarSelectorCommand = new LaunchAvatarSelectorCommand(this);
        FillFullTimeRostersCommand = new FillFullTimeRostersCommand(this);

        ReportingEmployees = new List<EmployeeVM>();
        employees = new ObservableCollection<EmployeeVM>();

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
        roleNames = new ObservableCollection<string>();
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshData();
    }

    public void RefreshData()
    {
        Task.Run(() =>
        {
            EmployeeDataSet = Helios.StaffReader.EmployeeDataSet();

            // Make sure that the user has an assigned role.
            if (Charon.Employee is not null && Charon.Employee.Role is null)
                if (EmployeeDataSet.Roles.TryGetValue(Charon.Employee.RoleName, out var role))
                    Charon.Employee.Role = role;

            // Reporting employees (and other collections for filtering that list) is base purely on the employees that report to the current user.
            ReportingEmployees = EmployeeDataSet.GetReportsByRole(Charon.Employee?.ID ?? 0)
                .Select(e => new EmployeeVM(e, Charon, Helios)).ToList();

            Departments = new ObservableCollection<Department?>(ReportingEmployees
                .Select(employee => employee.Department).Distinct().OrderBy(department => department?.Name));
            selectedDepartment = null;

            Roles = new ObservableCollection<Role?>(ReportingEmployees.Select(employee => employee.Role).Distinct()
                .OrderBy(role => role?.Name));
            selectedRole = null;

            EmploymentTypes =
                new ObservableCollection<EEmploymentType?>(Enum.GetValues(typeof(EEmploymentType))
                    .Cast<EEmploymentType?>());
            selectedEmploymentType = null;

            // Here we want the full potential lists of data.
            Locations = new ObservableCollection<string>(EmployeeDataSet.Locations);
            PayPoints = new ObservableCollection<string>(EmployeeDataSet.PayPoints);
            FullDepartments =
                new ObservableCollection<Department>(EmployeeDataSet.Departments.Values.OrderBy(d => d.Name));
            AllRoles = new ObservableCollection<Role>(EmployeeDataSet.Roles.Values.OrderBy(r => r.DepartmentName)
                .ThenBy(r => r.Level));
            RoleNames = new ObservableCollection<string>(AllRoles.Select(r => r.Name));
            UseAllAsManagers = false;

            employeeSearchString = "";

            ApplyFilters();
        });
    }

    public void RepairData()
    {
        throw new NotImplementedException();
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
        IEnumerable<EmployeeVM> employeeList = ReportingEmployees;

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
        ApplySorting(ReportingEmployees);
    }

    public void ApplySorting(IEnumerable<EmployeeVM> employeeList)
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
        Employees = new ObservableCollection<EmployeeVM>(employeeList);
    }

    public void CreateNewEmployee()
    {
        var employeeCreationWindow = new EmployeeCreationWindow(this);
        if (employeeCreationWindow.ShowDialog() != true) return;

        var newEmployee = employeeCreationWindow.VM.Employee;

        if (newEmployee is null) return;

        ReportingEmployees.Add(new EmployeeVM(newEmployee, Charon, Helios));
        EmployeeDataSet?.AddEmployee(newEmployee);
        if (newEmployee.Reports.Any()) Managers.Add(newEmployee);

        ApplyFilters();

        SelectedEmployeeVM = new EmployeeVM(newEmployee, Charon, Helios);
    }

    public void DeleteEmployee()
    {
        if (SelectedEmployeeVM is null) return;

        // Confirm with user.
        if (MessageBox.Show(
                $"Are you sure you want to delete {SelectedEmployeeVM}?\n\n(They will be recoverable in the database, but inaccessible until they are.)",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        // 'Delete' in database.
        Helios.StaffDeleter.Employee(SelectedEmployeeVM.Employee);

        // Remove current active references to the employee.
        SelectedEmployeeVM.Delete();
        EmployeeDataSet?.Employees.Remove(SelectedEmployeeVM.ID);
        ReportingEmployees.Remove(SelectedEmployeeVM);
        Employees.Remove(SelectedEmployeeVM);
        SelectedEmployeeVM = null;
    }

    public void ActivateUser()
    {
        if (SelectedEmployeeVM is null) return;

        if (MessageBox.Show($"Are you sure you would like to activate the employee: {SelectedEmployeeVM} as a default user?",
                "Activate User", MessageBoxButton.YesNoCancel, MessageBoxImage.Information) !=
            MessageBoxResult.Yes) return;

        if (Charon.CreateNewUser(SelectedEmployeeVM.Employee))
            MessageBox.Show("Successfully Activated new user!", "Success", MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
    }

    public void LaunchAvatarSelector()
    {
        if (SelectedEmployeeVM is null) return;

        var avatarSelector = new AvatarSelectionWindow(this);
        if (avatarSelector.ShowDialog() != true) return;
        SelectedEmployeeVM.Avatar = avatarSelector.VM.SelectedAvatar;
    }

    /// <summary>
    /// Creates a roster shift rule for every full time (non-salary) employee that doesn't already have one.
    /// Standard M-F roster, unspecified shift.
    /// </summary>
    public void FillFullTimeRosters()
    {
        // Get list of employees.
        var targets =
            ReportingEmployees.Where(e => e.EmploymentType is EEmploymentType.FP && e.RosterRules.Count == 0).ToList();

        foreach (var employee in targets)
        {
            var rosterRule = new ShiftRuleRoster(employee.Employee)
            {
                Description = "Standard Roster",
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                MinDays = 5,
                MaxDays = 5
            };
            employee.RosterRules.Add(rosterRule);
        }

        // Update database with full group of new rules.
        var lines = Helios.StaffCreator.ShiftRuleRosters(targets.SelectMany(e => e.RosterRules));

        MessageBox.Show($"Created {lines} new roster rules for current Full-Time Permanent employees.", "Success",
            MessageBoxButton.OK);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}