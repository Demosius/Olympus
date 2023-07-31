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
using Morpheus.Views.Windows;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class StaffRoleHandlerVM : INotifyPropertyChanged, IDBInteraction, IFilters, ICreateDelete<StaffRoleVM>
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public List<StaffRoleVM> AllRoles { get; set; }

    public bool CanCreateRole { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<StaffRoleVM> Roles { get; set; }

    public bool CanDeleteRole => SelectedItem?.CanDelete ?? false;

    private StaffRoleVM? selectedItem;
    public StaffRoleVM? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDeleteRole));
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

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public CreateNewItemCommand CreateNewItemCommand { get; set; }
    public DeleteSelectedItemCommand<StaffRoleVM> DeleteSelectedItemCommand { get; set; }

    #endregion

    private StaffRoleHandlerVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanCreateRole = Charon.CanCreateStaffRole();
        filterString = string.Empty;

        AllRoles = new List<StaffRoleVM>();
        Roles = new ObservableCollection<StaffRoleVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        CreateNewItemCommand = new CreateNewItemCommand(this);
        DeleteSelectedItemCommand = new DeleteSelectedItemCommand<StaffRoleVM>(this);
    }

    private async Task<StaffRoleHandlerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<StaffRoleHandlerVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new StaffRoleHandlerVM(helios, charon);
        return ret.InitializeAsync();
    }

    public static StaffRoleHandlerVM CreateEmpty(Helios helios, Charon charon) => new(helios, charon);

    public async Task RefreshDataAsync()
    {
        AllRoles = (await Helios.StaffReader.RolesAsync()).Select(r => new StaffRoleVM(r, Helios, Charon)).ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        var roles = AllRoles.Where(r => 
            Regex.IsMatch(r.Name, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.DepartmentName, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.ReportsToRole, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.Level, FilterString, RegexOptions.IgnoreCase));

        Roles.Clear();
        foreach (var staffRoleVM in roles)
            Roles.Add(staffRoleVM);
    }

    public async Task CreateNewItemAsync()
    {
        if (!CanCreateRole) return;

        var input = new InputWindow("Enter new Role Name:", "New Role");

        if (input.ShowDialog() != true) return;

        var newRoleName = input.InputText;

        if (await Helios.StaffReader.RoleExistsAsync(newRoleName))
        {
            MessageBox.Show($"Role - {newRoleName} - already exists.", "Failed to Create Role", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var role = new Role(newRoleName);

        await Helios.StaffCreator.RoleAsync(role);

        var roleVM = new StaffRoleVM(role, Helios, Charon);

        AllRoles.Add(roleVM);
        Roles.Add(roleVM);
        SelectedItem = roleVM;
    }

    public async Task DeleteSelectedItemAsync()
    {
        if (SelectedItem is null || !CanDeleteRole) return;
        if (MessageBox.Show($"Are you sure that you want to delete {SelectedItem.Role}?", "Confirm Role Deletion",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        await Helios.StaffDeleter.RoleAsync(SelectedItem.Role);
        await RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}