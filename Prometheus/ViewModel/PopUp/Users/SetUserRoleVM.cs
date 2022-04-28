using Prometheus.ViewModel.Commands.Users;
using Serilog;
using Serilog.Events;
using Styx;
using Styx.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Users.Model;

namespace Prometheus.ViewModel.PopUp.Users;

internal class SetUserRoleVM : INotifyPropertyChanged, IDataSource, IDBInteraction
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members

    private User? user;
    public User? User
    {
        get => user;
        set
        {
            user = value;
            OnPropertyChanged();
        }
    }

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

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public ConfirmRoleCommand ConfirmRoleCommand { get; set; }

    #endregion

    public SetUserRoleVM()
    {
        roles = new ObservableCollection<Role>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        ConfirmRoleCommand = new ConfirmRoleCommand(this);
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        User = new User();

        RefreshData();
    }

    public void SetDataSources(Helios helios, Charon charon, User newUser)
    {
        Helios = helios;
        Charon = charon;
        User = newUser;

        RefreshData();
    }

    public void RefreshData()
    {
        if (Helios is null || Charon is null || User is null) return;

        // Get role list.
        Roles = new ObservableCollection<Role>(Helios.UserReader.Roles().OrderBy(r => r.Name));

        // Set selected role to be equal to the user's current role.
        SelectedRole = Roles.FirstOrDefault(r => r.Name == User.RoleName);
        User.Role ??= SelectedRole;
    }

    public bool ConfirmRole()
    {
        if (Charon is null || User is null || SelectedRole is null) return false;

        var success = Charon.SetRole(User, SelectedRole);

        if (success)
        {
            MessageBox.Show($"Successfully set {User.Employee}'s Role to {SelectedRole}.", "Success",
                MessageBoxButton.OK);
            return true;
        }

        MessageBox.Show($"Unable to set role for {User.Employee} to {SelectedRole}.", "Failure",
                MessageBoxButton.OK, MessageBoxImage.Warning);

        Log.Write(LogEventLevel.Information,
            "Failed to set role for {User.Employee} to {Role} when considered conditions should have allowed.",
            User.Employee, SelectedRole);
        
        return false;
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
}