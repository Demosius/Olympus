using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Interfaces;
using Pantheon.Views.PopUp.Employees;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class RoleSelectionVM : INotifyPropertyChanged, ICreationMode, ISelector, IDepartments, IRoles, IFilters, IDBInteraction
{
    private const string ANY_DEP_STR = "<Any>";

    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private readonly string? initialDepartmentName;

    public bool UserCanCreate { get; }

    public bool CanCreate => UserCanCreate &&
                             NewRoleName.Length > 0 &&
                             !FullRoles.Select(r => r.Name).Contains(NewRoleName) &&
                             RoleDepartment is not null;
    public bool UserCanDelete { get; }
    public bool CanDelete => UserCanDelete && SelectedRole is not null && SelectedRole.IsDeletable;
    public bool CanConfirm => SelectedRole is not null;

    public bool ShowCreationOption => !InCreation && UserCanCreate;
    public bool ShowNew => InCreation && UserCanCreate;

    public List<Role> FullRoles { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<Role> Roles { get; set; }
    
    private Role? selectedRole;
    public Role? SelectedRole
    {
        get => selectedRole;
        set
        {
            selectedRole = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDelete));
            OnPropertyChanged();
        }
    }

    // Filters

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
    
    public ObservableCollection<string> DepartmentNames { get; set; }

    private string? selectedDepartmentName;
    public string? SelectedDepartmentName
    {
        get => selectedDepartmentName;
        set
        {
            selectedDepartmentName = value == ANY_DEP_STR ? null : value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }


    // Creating a new Role

    private bool inCreation;
    public bool InCreation
    {
        get => inCreation;
        set
        {
            inCreation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowCreationOption));
            OnPropertyChanged(nameof(ShowNew));
        }
    }

    private string newRoleName;
    public string NewRoleName
    {
        get => newRoleName;
        set
        {
            newRoleName = value;
            OnPropertyChanged();
        }
    }

    private Department? roleDepartment;
    public Department? RoleDepartment
    {
        get => roleDepartment;
        set
        {
            roleDepartment = value;
            OnPropertyChanged();
        }
    }

    private Role? reportsToRole;
    public Role? ReportsToRole
    {
        get => reportsToRole;
        set
        {
            reportsToRole = value;
            OnPropertyChanged();
            if (ReportsToRole is not null && ReportsToRole.Level <= roleLevel) RoleLevel = (ReportsToRole.Level - 1).ToString();
        }
    }

    private int roleLevel;
    public string RoleLevel
    {
        get => roleLevel.ToString();
        set
        {
            if (!int.TryParse(value, out var i)) i = 0;
            if (ReportsToRole is not null && ReportsToRole.Level <= i) i = (ReportsToRole?.Level ?? 0) - 1;
            roleLevel = i;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ActivateCreationCommand ActivateCreationCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }
    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }
    public SelectRoleCommand SelectRoleCommand { get; set; }
    public ClearRoleCommand ClearRoleCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public RoleSelectionVM(Helios helios, Charon charon, string? departmentName = null)
    {
        Helios = helios;
        Charon = charon;

        UserCanCreate = Charon.CanCreateStaffRole();
        UserCanDelete = Charon.CanDeleteStaffRole();

        Roles = new ObservableCollection<Role>();

        DepartmentNames = new ObservableCollection<string> { ANY_DEP_STR };
        FullRoles = new List<Role>();

        filterString = string.Empty;
        newRoleName = string.Empty;

        initialDepartmentName = departmentName;

        ActivateCreationCommand = new ActivateCreationCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        SelectDepartmentCommand = new SelectDepartmentCommand(this);
        ClearDepartmentCommand = new ClearDepartmentCommand(this);
        SelectRoleCommand = new SelectRoleCommand(this);
        ClearRoleCommand = new ClearRoleCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);

        Task.Run(RefreshDataAsync);
    }

    public async Task RefreshDataAsync()
    {
        FullRoles = (await Helios.StaffReader.RolesAsync(pullType: EPullType.IncludeChildren))
            .OrderBy(r => r.DepartmentName).ThenBy(r => r.Name).ToList();

        var departments = FullRoles.Select(d => d.DepartmentName).Distinct().OrderBy(n => n);

        foreach (var department in departments)
            DepartmentNames.Add(department);
        
        if (initialDepartmentName is null || DepartmentNames.Contains(initialDepartmentName)) SelectedDepartmentName = initialDepartmentName;

        ApplyFilters();
    }

    public void ActivateCreation()
    {
        InCreation = !InCreation;
    }

    public void Create()
    {
        if (!CanCreate) return;

        // Add to database.
        var newRole = new Role
        {
            Name = NewRoleName,
            DepartmentName = RoleDepartment!.Name,
            Level = roleLevel,
            ReportsToRoleName = ReportsToRole?.Name ?? string.Empty
        };
        Helios.StaffCreator.Role(newRole);

        // Reset data to include new role.
        Roles.Clear();
        var roleList = AsyncHelper.RunSync(() => Helios.StaffReader.RolesAsync(pullType: EPullType.IncludeChildren))
            .OrderBy(r => r.DepartmentName)
            .ThenBy(r => r.Name);
        foreach (var role in roleList) Roles.Add(role);

        // Select newly created role (using role name)
        SelectedRole = Roles.FirstOrDefault(r => r.Name == NewRoleName);

        // Clear role creation data.
        NewRoleName = string.Empty;
        RoleDepartment = null;
        ReportsToRole = null;
        RoleLevel = "0";
        InCreation = false;

    }

    public void Delete()
    {
        // Check that it can be deleted.
        if (SelectedRole is null || !CanDelete) return;

        // Confirm desire to delete.
        if (MessageBox.Show(
                $"Are you sure that you would like to delete the {SelectedRole.Name} role for {SelectedRole.DepartmentName}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        // Delete from database.
        Helios.StaffDeleter.Role(SelectedRole);

        // Delete from Roles
        Roles.Remove(SelectedRole);

        // Deselect
        SelectedRole = null;
    }

    public void SelectDepartment()
    {
        var departmentSelector = new DepartmentSelectionWindow(Helios, Charon);
        departmentSelector.ShowDialog();

        if (departmentSelector.DialogResult != true) return;

        var department = departmentSelector.VM.SelectedDepartment;

        if (department is null) return;

        RoleDepartment = department;
    }

    public void ClearDepartment()
    {
        RoleDepartment = null;
    }

    public void SelectRole()
    {
        var roleSelector = new RoleSelectionWindow(Helios, Charon);
        roleSelector.ShowDialog();

        if (roleSelector.DialogResult != true) return;

        var role = roleSelector.VM.SelectedRole;

        if (role is null) return;

        ReportsToRole = role;
    }

    public void ClearRole()
    {
        ReportsToRole = null;
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        selectedDepartmentName = null;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var fRoles = SelectedDepartmentName is null ? FullRoles : FullRoles.Where(x => x.DepartmentName == selectedDepartmentName);

        fRoles = fRoles.Where(r => Regex.IsMatch(r.Name, FilterString));

        Roles.Clear();

        foreach (var role in fRoles)
            Roles.Add(role);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}