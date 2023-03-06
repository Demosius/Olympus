using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Employees;
using Styx;
using Uranus;

namespace Pantheon.ViewModels.PopUp.Employees;

public class ManagerSelectionVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    
    public List<EmployeeVM> FullEmployeeList { get; set; }
    public List<EmployeeVM> Managers { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<EmployeeVM> Employees { get; set; }

    private EmployeeVM? selectedManager;
    public EmployeeVM? SelectedManager
    {
        get => selectedManager;
        set
        {
            selectedManager = value;
            OnPropertyChanged();
        }
    }


    private bool useAllAsManagers;
    public bool UseAllAsManagers
    {
        get => useAllAsManagers;
        set
        {
            useAllAsManagers = value;
            OnPropertyChanged();
            SetList();
        }
    }

    #endregion

    #region Commands

    public ConfirmManagerSelectionCommand ConfirmManagerSelectionCommand { get; set; }

    #endregion

    public ManagerSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        FullEmployeeList = Helios.StaffReader.Employees().OrderBy(e => e.FullName).Select(e => new EmployeeVM(e, Charon, Helios)).ToList();
        var managerIDs = FullEmployeeList.Where(e => e.ReportsToID != 0).Select(e => e.ReportsToID).Distinct();
        Managers = FullEmployeeList.Where(e => managerIDs.Contains(e.ID)).ToList();

        Employees = new ObservableCollection<EmployeeVM>();
        
        SetList();

        ConfirmManagerSelectionCommand = new ConfirmManagerSelectionCommand(this);
    }

    /// <summary>
    /// Set the observable employee list based on the UseAllManagers bool value.
    /// </summary>
    private void SetList()
    {
        Employees.Clear();
        var list = UseAllAsManagers ? FullEmployeeList : Managers;
        foreach (var employeeVM in list)
        {
            Employees.Add(employeeVM);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}