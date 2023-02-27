using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Interface;
using Pantheon.Views;
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

public class EmployeePageVM : INotifyPropertyChanged, IDBInteraction, IFilters, IPayPoints
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public EmployeeAvatar Avatar => (SelectedEmployee ?? new Employee()).Avatar ?? new EmployeeAvatar();

    public EmployeeDataSet? EmployeeDataSet;

    public List<Employee> ReportingEmployees { get; set; }

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
            OnPropertyChanged(nameof(SensitiveVisibility));
            OnPropertyChanged(nameof(VerySensitiveVisibility));
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
    public bool SensitiveVisibility => SelectedEmployee is not null && (Charon?.CanReadEmployeeSensitive(SelectedEmployee) ?? false);
    public bool VerySensitiveVisibility => SelectedEmployee is not null && (Charon?.CanReadEmployeeVerySensitive(SelectedEmployee) ?? false);

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
    public DeleteEmployeeCommand DeleteEmployeeCommand { get; set; }
    public ActivateUserCommand ActivateUserCommand { get; set; }
    public LaunchIconiferCommand LaunchIconiferCommand { get; set; }
    public LaunchAvatarSelectorCommand LaunchAvatarSelectorCommand { get; set; }
    public LaunchEmployeeShiftWindowCommand LaunchEmployeeShiftWindowCommand { get; set; }
    public FillFullTimeRostersCommand FillFullTimeRostersCommand { get; set; }
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
        DeleteEmployeeCommand = new DeleteEmployeeCommand(this);
        ActivateUserCommand = new ActivateUserCommand(this);
        LaunchIconiferCommand = new LaunchIconiferCommand(this);
        LaunchAvatarSelectorCommand = new LaunchAvatarSelectorCommand(this);
        LaunchEmployeeShiftWindowCommand = new LaunchEmployeeShiftWindowCommand(this);
        FillFullTimeRostersCommand = new FillFullTimeRostersCommand(this);

        ReportingEmployees = new List<Employee>();
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
        roleNames = new ObservableCollection<string>();
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

        EmployeeDataSet = Helios.StaffReader.EmployeeDataSet();

        // Make sure that the user has an assigned role.
        if (Charon.Employee is not null && Charon.Employee.Role is null)
            if (EmployeeDataSet.Roles.TryGetValue(Charon.Employee.RoleName, out var role))
                Charon.Employee.Role = role;

        // Reporting employees (and other collections for filtering that list) is base purely on the employees that report to the current user.
        ReportingEmployees = EmployeeDataSet.GetReportsByRole(Charon.Employee?.ID ?? 0).ToList();

        Departments = new ObservableCollection<Department?>(ReportingEmployees.Select(employee => employee.Department).Distinct().OrderBy(department => department?.Name));
        selectedDepartment = null;

        Roles = new ObservableCollection<Role?>(ReportingEmployees.Select(employee => employee.Role).Distinct().OrderBy(role => role?.Name));
        selectedRole = null;

        EmploymentTypes = new ObservableCollection<EEmploymentType?>(Enum.GetValues(typeof(EEmploymentType)).Cast<EEmploymentType?>());
        selectedEmploymentType = null;

        // Here we want the full potential lists of data.
        Locations = new ObservableCollection<string>(EmployeeDataSet.Locations);
        PayPoints = new ObservableCollection<string>(EmployeeDataSet.PayPoints);
        FullDepartments = new ObservableCollection<Department>(EmployeeDataSet.Departments.Values.OrderBy(d => d.Name));
        AllRoles = new ObservableCollection<Role>(EmployeeDataSet.Roles.Values.OrderBy(r => r.DepartmentName).ThenBy(r => r.Level));
        RoleNames = new ObservableCollection<string>(AllRoles.Select(r => r.Name));
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
        IEnumerable<Employee> employeeList = ReportingEmployees;

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
        if (Helios is null || Charon is null) return;

        var employeeCreationWindow = new EmployeeCreationWindow(this);
        if (employeeCreationWindow.ShowDialog() != true) return;

        var newEmployee = employeeCreationWindow.VM.Employee;

        if (newEmployee is null) return;

        ReportingEmployees.Add(newEmployee);
        EmployeeDataSet?.AddEmployee(newEmployee);
        if (newEmployee.Reports.Any()) Managers.Add(newEmployee);

        ApplyFilters();

        SelectedEmployee = newEmployee;
    }

    public void SaveEmployee()
    {
        if (Helios is null || Charon is null || SelectedEmployee is null || EmployeeDataSet is null) return;

        if ((SelectedEmployee.Role is null || SelectedEmployee.Role.Name != SelectedEmployee.RoleName) && !ConfirmUnEditableChange()) return;

        SelectedEmployee.SetDataFromObjects();

        if (Helios.StaffUpdater.Employee(SelectedEmployee) > 0)
            MessageBox.Show($"Successfully saved changes to {SelectedEmployee.FullName}.", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);

    }

    /// <summary>
    /// Assuming the user is about to adjust the employee in such a way that removes that employee from the user's permissions to edit further, make sure confirmation is attained.
    /// </summary>
    /// <returns></returns>
    private bool ConfirmUnEditableChange()
    {
        if (Helios is null || Charon is null || SelectedEmployee is null || EmployeeDataSet is null) return false;

        if (!EmployeeDataSet.Roles.TryGetValue(SelectedEmployee.RoleName, out var newRole)) return false;

        if (Charon.CanUpdateEmployee(newRole)) return true;

        if (MessageBox.Show(
                "Changing this employee's Role will mean you will not be able to edit them in the future.\n\nDo you want to continue?",
                "Confirm New Role", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) !=
            MessageBoxResult.Yes) return false;

        SelectedEmployee.Role = newRole;
        return true;
    }

    public void DeleteEmployee()
    {
        if (SelectedEmployee is null || Helios is null || Charon is null) return;

        // Confirm with user.
        if (MessageBox.Show(
                $"Are you sure you want to delete {SelectedEmployee}?\n\n(They will be recoverable in the database, but inaccessible until they are.)",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        // 'Delete' in database.
        Helios.StaffDeleter.Employee(SelectedEmployee);

        // Remove current active references to the employee.
        SelectedEmployee.Delete();
        EmployeeDataSet?.Employees.Remove(SelectedEmployee.ID);
        ReportingEmployees.Remove(SelectedEmployee);
        Employees.Remove(SelectedEmployee);
        SelectedEmployee = null;
    }

    public void ActivateUser()
    {
        if (SelectedEmployee is null || Charon is null) return;

        if (MessageBox.Show($"Are you sure you would like to activate the employee: {SelectedEmployee} as a default user?",
                "Activate User", MessageBoxButton.YesNoCancel, MessageBoxImage.Information) !=
            MessageBoxResult.Yes) return;

        if (Charon.CreateNewUser(SelectedEmployee))
            MessageBox.Show("Successfully Activated new user!", "Success", MessageBoxButton.OK,
                MessageBoxImage.Asterisk);
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
        if (Helios is null || Charon is null) return;

        var departmentCreator = new DepartmentCreationWindow(this);
        if (departmentCreator.ShowDialog() != true) return;

        var newDepartment = departmentCreator.VM.Department;

        EmployeeDataSet?.AddDepartment(ref newDepartment);
        Departments.Add(newDepartment);
        FullDepartments.Add(newDepartment);

        if (SelectedEmployee is not null) SelectedEmployee.Department = newDepartment;
    }

    public void AddRole()
    {
        if (Helios is null || Charon is null) return;

        var roleCreator = new RoleCreationWindow(this);
        if (roleCreator.ShowDialog() != true) return;

        var newRole = roleCreator.VM.Role;

        EmployeeDataSet?.AddRole(ref newRole);
        Roles.Add(newRole);
        AllRoles.Add(newRole);
    }

    public void AddClan()
    {
        if (Helios is null || Charon is null) return;

        var clanCreator = new ClanCreationWindow(this);
        if (clanCreator.ShowDialog() != true) return;

        var newClan = clanCreator.VM.Clan;

        EmployeeDataSet?.AddClan(ref newClan);
        Clans.Add(newClan);

        if (SelectedEmployee is not null) SelectedEmployee.Clan = newClan;
    }

    public void AddPayPoint()
    {
        var input = new InputWindow("Enter new Pay Point:", "New PayPoint");
        if (input.ShowDialog() != true) return;

        PayPoints.Add(input.VM.Input);
        if (SelectedEmployee is not null) SelectedEmployee.PayPoint = input.VM.Input;
    }

    public void LaunchIconifer()
    {
        if (SelectedEmployee is null) return;

        var iconifer = new IconSelectionWindow(this);
        if (iconifer.ShowDialog() != true) return;
        SelectedEmployee.Icon = iconifer.VM.SelectedIcon;
    }

    public void LaunchAvatarSelector()
    {
        if (SelectedEmployee is null) return;

        var avatarSelector = new AvatarSelectionWindow(this);
        if (avatarSelector.ShowDialog() != true) return;
        SelectedEmployee.Avatar = avatarSelector.VM.SelectedAvatar;
    }

    public void LaunchEmployeeShiftWindow()
    {
        if (Helios is null || Charon is null || selectedEmployee is null) return;
        var shiftWindow = new EmployeeShiftWindow(this, selectedEmployee);

        shiftWindow.ShowDialog();
    }

    /// <summary>
    /// Creates a roster shift rule for every full time (non-salary) employee that doesn't already have one.
    /// Standard M-F roster, unspecified shift.
    /// </summary>
    public void FillFullTimeRosters()
    {
        if (Helios is null) return;

        // Get list of employees.
        var targets =
            ReportingEmployees.Where(e => e.EmploymentType is EEmploymentType.FP && e.RosterRules.Count == 0).ToList();

        foreach (var employee in targets)
        {
            var rosterRule = new ShiftRuleRoster(employee)
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
}