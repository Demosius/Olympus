using Pantheon.Properties;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Pages;

internal class ShiftPageVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region NotifiableProperties

    private ObservableCollection<Shift> shifts;
    public ObservableCollection<Shift> Shifts
    {
        get => shifts;
        set
        {
            shifts = value;
            OnPropertyChanged(nameof(Shifts));
        }
    }

    private Shift? selectedShift;
    public Shift? SelectedShift
    {
        get => selectedShift;
        set
        {
            selectedShift = value;
            OnPropertyChanged(nameof(SelectedShift));
        }
    }

    private ObservableCollection<Department> departments;
    public ObservableCollection<Department> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged(nameof(departments));
        }
    }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged(nameof(SelectedDepartment));
            Shifts = new ObservableCollection<Shift>(SelectedDepartment?.Shifts ?? new List<Shift>());
        }
    }

    #endregion

    public ShiftPageVM()
    {
        departments = new ObservableCollection<Department>();
        shifts = new ObservableCollection<Shift>();
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        Departments = new ObservableCollection<Department>(Helios.StaffReader.SubDepartments(Charon.UserEmployee.DepartmentName));
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.UserEmployee.DepartmentName);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}