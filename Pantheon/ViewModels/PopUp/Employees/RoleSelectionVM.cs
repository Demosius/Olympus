using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Commands.Generic;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class RoleSelectionVM : INotifyPropertyChanged, ICreationMode, ISelector, IDepartments
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }

    public bool UserCanCreate { get; }
    public bool CanCreate => UserCanCreate && NewRoleName.Length > 0 && !Roles.Select(r => r.Name).Contains(NewRoleName);
    public bool UserCanDelete { get; }
    public bool CanDelete => UserCanDelete && SelectedRole is not null && SelectedRole.IsDeletable;
    public bool CanConfirm => SelectedRole is not null;

    public bool ShowCreationOption => !InCreation && UserCanCreate;
    public bool ShowNew => InCreation && UserCanCreate;

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
        }
    }

    private int roleLevel;
    public int RoleLevel
    {
        get => roleLevel;
        set
        {
            roleLevel = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ActivateCreationCommand ActivateCreationCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }

    #endregion

    public RoleSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        UserCanCreate = Charon.CanCreateStaffRole();
        UserCanDelete = Charon.CanDeleteStaffRole();

        Roles = new ObservableCollection<Role>(Helios.StaffReader.Roles().OrderBy(r => r.DepartmentName).ThenBy(r => r.Name));

        newRoleName = string.Empty;

        ActivateCreationCommand = new ActivateCreationCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
    }

    public void ActivateCreation()
    {
        InCreation = !InCreation;
    }

    public void Create()
    {
        throw new System.NotImplementedException();
    }

    public void Delete()
    {
        throw new System.NotImplementedException();
    }

    public void SelectDepartment()
    {
        throw new System.NotImplementedException();
    }

    public void ClearDepartment()
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