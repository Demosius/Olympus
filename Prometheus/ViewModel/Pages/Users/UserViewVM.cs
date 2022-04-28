using Prometheus.ViewModel.Commands.Users;
using Styx;
using Styx.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Prometheus.View.PopUp.Users;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;
using Uranus.Users.Model;

namespace Prometheus.ViewModel.Pages.Users;

internal class UserViewVM : INotifyPropertyChanged, IDataSource, IDBInteraction, IFilters
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    private readonly List<User> fullUserList;
    private EmployeeDataSet? employeeDataSet;

    #region INotifyPropertyChanged Members

    private ObservableCollection<User> users;
    public ObservableCollection<User> Users
    {
        get => users;
        set
        {
            users = value;
            OnPropertyChanged();
        }
    }

    private User? selectedUser;
    public User? SelectedUser
    {
        get => selectedUser;
        set
        {
            selectedUser = value;
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
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public DeactivateUserCommand DeactivateUserCommand { get; set; }
    public ChangeUserRoleCommand ChangeUserRoleCommand { get; set; }

    #endregion

    public UserViewVM()
    {
        fullUserList = new List<User>();
        users = new ObservableCollection<User>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        DeactivateUserCommand = new DeactivateUserCommand(this);
        ChangeUserRoleCommand = new ChangeUserRoleCommand(this);
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshData();
    }

    public void RefreshData()
    {
        if (Helios is null || Charon is null) return;

        GatherUsers();

        ApplyFilters();
    }

    private void GatherUsers()
    {
        if (Helios is null || Charon is null) return;

        employeeDataSet = Helios.StaffReader.EmployeeDataSet();

        var userList = Helios.UserReader.Users();
        var roles = Helios.UserReader.Roles().ToDictionary(r => r.Name, r => r);

        fullUserList.Clear();
        foreach (var user in userList)
        {
            if (roles.TryGetValue(user.RoleName, out var role)) user.Role = role;
            if (!employeeDataSet.Employees.TryGetValue(user.ID, out var employee)) continue;
            user.Employee = employee;
            fullUserList.Add(user);
        }
    }

    public void ClearFilters()
    {
        FilterString = "";
        ApplySorting();
    }

    public void ApplyFilters()
    {
        IEnumerable<User> userList = fullUserList;

        if (FilterString != "")
        {
            var regex = new Regex(filterString, RegexOptions.IgnoreCase);
            userList = userList.Where(e => regex.IsMatch(e.Employee?.FullName ?? "") || regex.IsMatch(e.ID.ToString()));
        }

        ApplySorting(userList);
    }

    public void ApplySorting()
    {
        ApplySorting(fullUserList);
    }

    public void ApplySorting(IEnumerable<User> userList)
    {
        userList = SelectedSortMethod switch
        {
            ESortMethod.Name => userList.OrderBy(e => e.Employee?.FullName ?? ""),
            ESortMethod.RoleName => userList.OrderBy(e => e.Role).ThenBy(e => e.Employee?.FullName ?? ""),
            ESortMethod.DepartmentRoleName => userList.OrderBy(e => e.Employee?.Department ?? new Department())
                .ThenBy(e => e.Role)
                .ThenBy(e => e.Employee),
            ESortMethod.EmploymentTypeRoleName => userList.OrderBy(e => e.Employee?.EmploymentType)
                .ThenBy(e => e.Role)
                .ThenBy(e => e.Employee),
            ESortMethod.RoleEmploymentTypeName => userList.OrderBy(e => e.Role)
                .ThenBy(e => e.Employee?.EmploymentType)
                .ThenBy(e => e.Employee?.FullName ?? ""),
            ESortMethod.ID => userList.OrderBy(e => e.ID),
            _ => userList.OrderBy(e => e.Employee?.EmploymentType).ThenBy(e => e.Employee)
        };
        Users = new ObservableCollection<User>(userList);
    }

    public void ChangeUserRole()
    {
        if (Helios is null || Charon is null || SelectedUser is null) return;
        
        var roleWindow = new SetUserRoleView(Helios, Charon, SelectedUser);

        roleWindow.ShowDialog();

        RefreshData();
    }

    public void DeactivateUser()
    {
        if (Helios is null || Charon is null || SelectedUser is null) return;

        if (MessageBox.Show(
                $"Are you sure you want to deactivate the user: {SelectedUser.Employee?.FullName ?? SelectedUser.ID.ToString()}?",
                "Confirm User Deactivation", MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
            MessageBoxResult.Yes) return;

        if (!Charon.DeactivateUser(SelectedUser))
        {
            MessageBox.Show(
                $"Unable to remove {SelectedUser.Employee?.FullName ?? SelectedUser.ID.ToString()} as a user.",
                "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show(
                $"Successfully removed {SelectedUser.Employee?.FullName ?? SelectedUser.ID.ToString()} as a user.",
                "Success", MessageBoxButton.OK);
        fullUserList.Remove(SelectedUser);
        SelectedUser = null;
        ApplyFilters();

    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}