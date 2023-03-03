using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Generic;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class DepartmentSelectionVM : INotifyPropertyChanged, ICreationMode, ISelector
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public bool CanCreate { get; }
    public bool CanDelete { get; }
    public bool CanConfirm => SelectedDepartment is not null;

    public bool ShowNew => CanCreate && !InCreation;

    #region INotifyPropertChanged

    public ObservableCollection<Department> Departments { get; set; }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged();
        }
    }

    private bool inCreation;
    public bool InCreation
    {
        get => inCreation;
        set
        {
            inCreation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowNew));
        }
    }

    #endregion

    #region Commands

    public ActivateCreationCommand ActivateCreationCommand { get; set; }

    #endregion

    public DepartmentSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanCreate = Charon.CanCreateDepartment();
        CanDelete = Charon.CanDeleteDepartment();



        ActivateCreationCommand = new ActivateCreationCommand(this);
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}