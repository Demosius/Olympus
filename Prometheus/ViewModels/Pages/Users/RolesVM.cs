using Prometheus.ViewModels.Commands.Users;
using Styx;
using Styx.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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

internal class RolesVM : INotifyPropertyChanged, IDataSource, IDBInteraction, IFilters, ISorting
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

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
    public RepairDataCommand RepairDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public DeleteRoleCommand DeleteRoleCommand { get; set; }
    public SaveRolesCommand SaveRolesCommand { get; set; }

    #endregion

    public RolesVM()
    {
        allRoles = new List<Role>();
        roles = new ObservableCollection<Role>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        DeleteRoleCommand = new DeleteRoleCommand(this);
        SaveRolesCommand = new SaveRolesCommand(this);
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

        var roleDict = Helios.UserReader.Roles().ToDictionary(role => role.Name, role => role);
        var users = Helios.UserReader.Users().ToList();

        foreach (var user in users)
        {
            if (!roleDict.TryGetValue(user.RoleName, out var role)) continue;
            user.Role = role;
            role.Users.Add(user);
        }

        allRoles = roleDict.Values.ToList();

        ApplyFilters();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
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

    public void SaveRoles()
    {
        if (Charon?.User?.Role is null || Helios is null || !Charon.CanEditUserRole()) return;

        // Check each role to see if the current user has permission to edit them - or that they have no assigned users.
        var savable = Roles.Where(role => role.Users.Count == 0 || Charon.User.Role.IsMasterTo(role)).ToList();

        Helios.UserUpdater.Roles(savable);

        RefreshData();

        MessageBox.Show("Save Successful.\n\n(Some data may revert based on your permissions and current users.)", "Data Saved", MessageBoxButton.OK);
    }

    public void DeleteRole()
    {
        if (Helios is null || SelectedRole is null || SelectedRole.Users.Count > 0 || !(Charon?.CanDeleteUserRole() ?? false)) return;

        // Remove from database.
        if (!Helios.UserDeleter.Role(SelectedRole))
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