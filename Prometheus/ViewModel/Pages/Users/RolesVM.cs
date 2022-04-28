using Styx;
using Styx.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Role = Uranus.Users.Model.Role;

namespace Prometheus.ViewModel.Pages.Users;

public enum ERoleSortMethod
{
    Name,
    UserPermissions,
    UserCount
}

internal class RolesVM : INotifyPropertyChanged, IDataSource, IDBInteraction, IFilters
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
        throw new System.NotImplementedException();
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
}