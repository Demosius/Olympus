using Prometheus.ViewModels.Commands.Users;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Prometheus.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using UserRole = Uranus.Users.Models.Role;

namespace Prometheus.ViewModels.Pages.Users;

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

public class UserActivateVM : INotifyPropertyChanged, IFilters, IDBInteraction, ISorting
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private IEnumerable<EmployeeVM> fullEmployees;

    #region INotifyPropertyChanged Members

    private ObservableCollection<EmployeeVM> employees;
    public ObservableCollection<EmployeeVM> Employees
    {
        get => employees;
        set
        {
            employees = value;
            OnPropertyChanged();
        }
    }

    private EmployeeVM? selectedEmployee;
    public EmployeeVM? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged();
        }
    }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private ESortMethod selectedSortMethod;
    public ESortMethod SelectedSortMethod
    {
        get => selectedSortMethod;
        set
        {
            selectedSortMethod = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private bool canMassCreate;
    public bool CanMassCreate
    {
        get => canMassCreate;
        set
        {
            canMassCreate = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public ActivateManagersCommand ActivateManagersCommand { get; set; }
    public ActivateUserCommand ActivateUserCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public UserActivateVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanMassCreate = Charon.User?.Role?.CreateUser >= 10;

        employees = new ObservableCollection<EmployeeVM>();
        fullEmployees = new List<EmployeeVM>();
        filterString = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        ActivateManagersCommand = new ActivateManagersCommand(this);
        ActivateUserCommand = new ActivateUserCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);

        Task.Run(RefreshDataAsync);
    }
    
    public async Task RefreshDataAsync()
    {
        await GatherEmployees();
        ApplyFilters();
    }
    
    /// <summary>
    /// Fill the employees list based on User Role permissions and Employee reports (as required).
    /// </summary>
    private async Task GatherEmployees()
    {
        if (Charon.User is null) return;

        // Get the full data set.
        var dataSet = await Helios.StaffReader.EmployeeDataSetAsync();

        // Make sure that the user is assigned a relevant employee.role.
        if (dataSet.Employees.TryGetValue(Charon.User.ID, out var userEmployee))
        {
            if (Charon.User.Employee is null)
                Charon.User.Employee = userEmployee;
            else
                Charon.User.Employee.Role = userEmployee.Role;
        }

        fullEmployees = dataSet.Employees.Values
            .Where(employee => employee.IsActive && !employee.IsUser && Charon.CanCreateUser(employee)).Select(e => new EmployeeVM(e));
    }

    public void ClearFilters()
    {
        FilterString = "";
        ApplySorting();
    }

    public void ApplyFilters()
    {
        IEnumerable<EmployeeVM> employeeList = fullEmployees;

        if (FilterString != "")
        {
            var regex = new Regex(filterString, RegexOptions.IgnoreCase);
            employeeList = employeeList.Where(e => regex.IsMatch(e.FullName));
        }

        ApplySorting(employeeList);
    }

    public void ApplySorting()
    {
        ApplySorting(fullEmployees);
    }

    public void ApplySorting(IEnumerable<EmployeeVM> employeeList)
    {
        employeeList = SelectedSortMethod switch
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
        Employees.Clear();
        foreach (var employeeVM in employeeList) Employees.Add(employeeVM);
    }

    public async Task ActivateManagers()
    {
        if (MessageBox.Show("Do you want to activate all managers (any employee with direct reports) as users?",
                "Activate Managers", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        if (Helios.UserReader.Role("Manager") is null) CreateManagerRole();
        
        var managers = Helios.StaffReader.GetManagersAsync();

        var i = 0;

        foreach (var manager in await managers)
        {
            if (manager.IsUser) continue;
            Charon.CreateNewUser(manager, "Manager");
            ++i;
        }

        var refresh = RefreshDataAsync();

        if (i == 0)
            MessageBox.Show(
                "No managers found to activate.\n\n(This could be because all managers are already activated as users.)",
                "No New Users", MessageBoxButton.OK);
        else
            MessageBox.Show($"Successfully activated {i} managers as users.", "Success", MessageBoxButton.OK);

        await refresh;
    }

    private void CreateManagerRole()
    {
        var role = new UserRole()
        {
            Name = "Manager",
            CreateUser = 0,
            ReadUser = 0,
            UpdateUser = 0,
            DeleteUser = -1,
            CreateEmployee = true,
            ReadEmployee = 1,
            ReadEmployeeSensitive = 0,
            ReadEmployeeVerySensitive = -1,
            UpdateEmployee = 1,
            DeleteEmployee = 1,
            CreateDepartment = true,
            UpdateDepartment = true,
            DeleteDepartment = true,
            AssignRole = true,
            EditRoles = true,
            CreateClan = true,
            UpdateClan = true,
            DeleteClan = true,
            CreateShift = 1,
            UpdateShift = 1,
            DeleteShift = 1,
            CreateLicence = true,
            ReadLicence = true,
            UpdateLicence = true,
            DeleteLicence = true,
            CreateVehicle = true,
            ReadVehicle = true,
            UpdateVehicle = true,
            DeleteVehicle = true,
            CopyDatabase = false,
            MoveDatabase = false,
            ManageLockers = false
        };

        Helios.UserCreator.Role(role);
    }

    public void ActivateUser()
    {
        if (SelectedEmployee is null) return;

        if (Charon.CreateNewUser(SelectedEmployee.Employee, "Default"))
            MessageBox.Show($"{SelectedEmployee} has successfully been activated as a user.", "Success",
                MessageBoxButton.OK);

        fullEmployees = fullEmployees.Where(e => !e.IsUser);

        SelectedEmployee = null;

        ApplyFilters();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}