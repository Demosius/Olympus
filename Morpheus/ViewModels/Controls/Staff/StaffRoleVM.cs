using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class StaffRoleVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public Role Role { get; set; }

    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    #region Role Access

    public string Name
    {
        get => Role.Name;
        set => _ = UpdateNameAsync(value);
    }

    public string DepartmentName
    {
        get => Role.DepartmentName;
        set
        {
            if (CanUpdate)
                Role.DepartmentName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string Level
    {
        get => Role.Level.ToString();
        set
        {
            if (CanUpdate && int.TryParse(value, out var val))
                Role.Level = val;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public string ReportsToRole
    {
        get => Role.ReportsToRoleName;
        set
        {
            if (CanUpdate)
                Role.ReportsToRoleName = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public StaffRoleVM(Role department, Helios helios, Charon charon)
    {
        Role = department;
        Helios = helios;
        Charon = charon;

        CanCreate = Charon.CanCreateStaffRole();
        CanUpdate = Charon.CanUpdateStaffRole();
        CanDelete = Charon.CanDeleteStaffRole();

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task UpdateNameAsync(string newName)
    {
        if (CanUpdate && Name != newName)
            await Helios.StaffUpdater.RoleNameChangeAsync(Role, newName);
        OnPropertyChanged(nameof(Name));
    }

    public async Task RefreshDataAsync()
    {
        Role = await Helios.StaffReader.RoleAsync(Role.Name) ?? Role;
    }

    public async Task SaveAsync()
    {
        if (CanUpdate) await Helios.StaffUpdater.RoleAsync(Role);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}