﻿using Styx;
using Styx.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Prometheus.ViewModel.Commands;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;
using UserRole = Uranus.Users.Model.Role;

namespace Prometheus.ViewModel.Pages.Users;

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

internal class UserActivateVM : INotifyPropertyChanged, IDataSource, IFilters, IDBInteraction
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    private IEnumerable<Employee> fullEmployees;

    #region INotifyPropertyChanged Members

    private ObservableCollection<Employee> employees;
    public ObservableCollection<Employee> Employees
    {
        get => employees;
        set
        {
            employees = value;
            OnPropertyChanged();
        }
    }

    private Employee? selectedEmployee;
    public Employee? SelectedEmployee
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
            ApplySorting();
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
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public UserActivateVM()
    {
        employees = new ObservableCollection<Employee>();
        fullEmployees = new List<Employee>();
        filterString = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        ActivateManagersCommand = new ActivateManagersCommand(this);
        ActivateUserCommand = new ActivateUserCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshData();
    }
    
    public void RefreshData()
    {
        CanMassCreate = Charon?.User?.Role?.CreateUser >= 10;

        GatherEmployees();

        ApplyFilters();
    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Fill the employees list based on User Role permissions and Employee reports (as required).
    /// </summary>
    private void GatherEmployees()
    {
        if (Charon?.User is null || Helios is null) return;

        // Get the full data set.
        var dataSet = Helios.StaffReader.EmployeeDataSet();

        // Make sure that the user is assigned a relevant employee.role.
        if (dataSet.Employees.TryGetValue(Charon.User.ID, out var userEmployee))
        {
            if (Charon.User.Employee is null)
                Charon.User.Employee = userEmployee;
            else
                Charon.User.Employee.Role = userEmployee.Role;
        }

        fullEmployees = dataSet.Employees.Values
            .Where(employee => employee.IsActive && !employee.IsUser && Charon.CanCreateUser(employee));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public void ClearFilters()
    {
        FilterString = "";
        ApplySorting();
    }

    public void ApplyFilters()
    {
        IEnumerable<Employee> employeeList = fullEmployees;

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

    public void ApplySorting(IEnumerable<Employee> employeeList)
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
        Employees = new ObservableCollection<Employee>(employeeList);
    }

    public void ActivateManagers()
    {
        if (Helios is null || Charon is null) return;

        if (MessageBox.Show("Do you want to activate all managers (any employee with direct reports) as users?",
                "Activate Managers", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        
        var role = Helios.UserReader.Role("Manager");
        role ??= CreateManagerRole();

        if (role is null) return;

        var managers = Helios.StaffReader.GetManagers();

        var i = 0;

        foreach (var manager in managers)
        {
            if (manager.IsUser) continue;
            Charon.CreateNewUser(manager, "Manager");
            ++i;
        }
        
        RefreshData();

        if (i == 0)
            MessageBox.Show(
                "No managers found to activate.\n\n(This could be because all managers are already activated as users.)",
                "No New Users", MessageBoxButton.OK);
        else
            MessageBox.Show($"Successfully activated {i} managers as users.", "Success", MessageBoxButton.OK);
    }

    private UserRole? CreateManagerRole()
    {
        if (Helios is null || Charon is null) return null;

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

        return role;
    }

    public void ActivateUser()
    {
        if (Charon is null || Helios is null || SelectedEmployee is null) return;

        if (Charon.CreateNewUser(SelectedEmployee, "Default"))
            MessageBox.Show($"{SelectedEmployee} has successfully been activated as a user.", "Success",
                MessageBoxButton.OK);

        fullEmployees = fullEmployees.Where(e => !e.IsUser);

        SelectedEmployee = null;

        ApplyFilters();
    }
}