using Pantheon.Properties;
using Pantheon.View;
using Pantheon.ViewModel.Commands;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Pages;

internal class ShiftPageVM : INotifyPropertyChanged, IDBInteraction
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

    private Break? selectedBreak;
    public Break? SelectedBreak
    {
        get => selectedBreak;
        set
        {
            selectedBreak = value;
            OnPropertyChanged(nameof(SelectedBreak));
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public CreateShiftCommand CreateShiftCommand { get; set; }
    public SaveShiftCommand SaveShiftCommand { get; set; }
    public DeleteShiftCommand DeleteShiftCommand { get; set; }
    public AddRemoveBreakCommand AddRemoveBreakCommand { get; set; }
    public LaunchShiftEmployeeWindowCommand LaunchShiftEmployeeWindowCommand { get; set; }

    #endregion

    public ShiftPageVM()
    {
        departments = new ObservableCollection<Department>();
        shifts = new ObservableCollection<Shift>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        CreateShiftCommand = new CreateShiftCommand(this);
        SaveShiftCommand = new SaveShiftCommand(this);
        DeleteShiftCommand = new DeleteShiftCommand(this);
        AddRemoveBreakCommand = new AddRemoveBreakCommand(this);
        LaunchShiftEmployeeWindowCommand = new LaunchShiftEmployeeWindowCommand(this);
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

        Departments = new ObservableCollection<Department>(Helios.StaffReader.SubDepartments(Charon.UserEmployee?.DepartmentName ?? ""));
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.UserEmployee?.DepartmentName);
    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void CreateShift()
    {
        if (SelectedDepartment is null || Helios is null) return;

        var inputBox = new InputWindow("Enter Shift Name", "New Shift");

        if (inputBox.ShowDialog() != true) return;

        var shiftName = inputBox.VM.Input;

        if (SelectedDepartment.Shifts.Any(s => s.Name == shiftName)) return;

        var shift = new Shift(SelectedDepartment, shiftName);
        if (!SelectedDepartment.Shifts.Any()) shift.Default = true;

        SelectedDepartment.Shifts.Add(shift);
        Shifts.Add(shift);

        Helios.StaffCreator.Shift(shift);
    }

    public void DeleteShift(Shift shift)
    {
        if (MessageBox.Show($"Are you sure you want to delete the {shift.Name} shift?", "Confirm Delete",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        SelectedDepartment?.Shifts.Remove(shift);
        Shifts.Remove(shift);

        Helios?.StaffDeleter.Shift(shift);
    }

    public void AddBreak(Shift shift)
    {
        if (Helios is null) return;

        var inputBox = new InputWindow("Enter Break Name", "New Break");

        if (inputBox.ShowDialog() != true) return;

        var breakName = inputBox.VM.Input;

        if (shift.Breaks.Any(b => b.Name == breakName)) return;

        var @break = new Break(shift, breakName);

        shift.AddBreak(@break);

        Helios.StaffCreator.Break(@break);
    }

    public void RemoveBreak(Break @break)
    {
        @break.Shift?.RemoveBreak(@break);
        Helios?.StaffDeleter.Break(@break);
    }

    public void LaunchShiftEmployeeWindow(Shift shift)
    {
        if (Helios is null || Charon is null) return;

        var employeeWindow = new ShiftEmployeeWindow(Helios, Charon, shift);
        employeeWindow.ShowDialog();
    }
}