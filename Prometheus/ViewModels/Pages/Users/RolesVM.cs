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
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Role = Uranus.Users.Models.Role;

namespace Prometheus.ViewModels.Pages.Users;

public enum ERoleSortMethod
{
    Name,
    UserPermissions,
    UserCount
}

public class RolesVM : INotifyPropertyChanged, IDBInteraction, IFilters, ISorting
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private List<Role> allRoles;

    #region INotifyPropertyChanged Members

    private ObservableCollection<Role> roles;
    public ObservableCollection<Role> Roles
    {
        get => roles;
        set
        {
            roles = value;
            OnPropertyChanged();
        }
    }

    private Role? selectedRole;
    public Role? SelectedRole
    {
        get => selectedRole;
        set
        {
            selectedRole = value;
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

    private ERoleSortMethod selectedSortMethod;
    public ERoleSortMethod SelectedSortMethod
    {
        get => selectedSortMethod;
        set
        {
            selectedSortMethod = value;
            OnPropertyChanged();
            ApplySorting();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public DeleteRoleCommand DeleteRoleCommand { get; set; }
    public SaveRolesCommand SaveRolesCommand { get; set; }

    #endregion

    private RolesVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        allRoles = new List<Role>();
        roles = new ObservableCollection<Role>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        DeleteRoleCommand = new DeleteRoleCommand(this);
        SaveRolesCommand = new SaveRolesCommand(this);
    }

    private async Task<RolesVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<RolesVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new RolesVM(helios, charon);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        var roleTask = Helios.UserReader.RolesAsync();
        var userTask = Helios.UserReader.UsersAsync();

        await Task.WhenAll(roleTask, userTask);

        var roleDict = (await roleTask).ToDictionary(role => role.Name, role => role);
        var users = (await userTask).ToList();

        foreach (var user in users)
        {
            if (!roleDict.TryGetValue(user.RoleName, out var role)) continue;
            user.Role = role;
            role.Users.Add(user);
        }

        allRoles = roleDict.Values.ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = "";
        ApplySorting();
    }

    public void ApplyFilters()
    {
        IEnumerable<Role> roleList = allRoles;

        if (FilterString != "")
        {
            var regex = new Regex(filterString, RegexOptions.IgnoreCase);
            roleList = roleList.Where(role => regex.IsMatch(role.Name));
        }

        ApplySorting(roleList);
    }

    public void ApplySorting()
    {
        ApplySorting(allRoles);
    }

    public void ApplySorting(IEnumerable<Role> roleList)
    {
        roleList = SelectedSortMethod switch
        {
            ERoleSortMethod.Name => roleList.OrderBy(role => role.Name),
            ERoleSortMethod.UserCount => roleList.OrderBy(role => role.Users.Count),
            ERoleSortMethod.UserPermissions => roleList.OrderBy(role => role.UserPermissionsTotal),
            _ => roleList.OrderBy(e => e.Name)
        };
        Roles = new ObservableCollection<Role>(roleList);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task SaveRoles()
    {
        if (Charon.User?.Role is null || !Charon.CanEditUserRole()) return;

        // Check each role to see if the current user has permission to edit them - or that they have no assigned users.
        var savable = Roles.Where(role => role.Users.Count == 0 || Charon.User.Role.IsMasterTo(role)).ToList();

        await Helios.UserUpdater.RolesAsync(savable);

        await RefreshDataAsync();

        MessageBox.Show("Save Successful.\n\n(Some data may revert based on your permissions and current users.)", "Data Saved", MessageBoxButton.OK);
    }

    public async Task DeleteRole()
    {
        if (SelectedRole is null || SelectedRole.Users.Count > 0 || !Charon.CanDeleteUserRole()) return;

        // Remove from database.
        if (!await Helios.UserDeleter.RoleAsync(SelectedRole))
        {
            MessageBox.Show(
                "Failed to delete user role.\n" +
                "Likely due to current active users having the role assigned.\n" +
                "Make sure that no users are assigned to the role before trying to delete it again.",
                "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        // Remove from active page data.
        allRoles.Remove(SelectedRole);
        Roles.Remove(SelectedRole);
        SelectedRole = null;
        MessageBox.Show("Role Deleted", "Success", MessageBoxButton.OK);
    }
}