using Pantheon.ViewModels.Commands.Shifts;
using Pantheon.Views;
using Pantheon.Views.PopUp.Shifts;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Pantheon.Annotations;
using Pantheon.ViewModels.Controls.Shifts;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Pages;

public class ShiftPageVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public Shift? SelectedShift => SelectedShiftVM?.Shift;

    public Break? SelectedBreak => SelectedBreakVM?.Break;

    #region NotifiableProperties

    private ObservableCollection<ShiftVM> shiftVMs;
    public ObservableCollection<ShiftVM> ShiftVMs
    {
        get => shiftVMs;
        set
        {
            shiftVMs = value;
            OnPropertyChanged(nameof(ShiftVMs));
        }
    }

    private ShiftVM? selectedShiftVM;
    public ShiftVM? SelectedShiftVM
    {
        get => selectedShiftVM;
        set
        {
            selectedShiftVM = value;
            OnPropertyChanged(nameof(SelectedShiftVM));
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
            ShiftVMs = new ObservableCollection<ShiftVM>(SelectedDepartment?.Shifts.Select(s => new ShiftVM(s)) ?? new List<ShiftVM>());
        }
    }

    private BreakVM? selectedBreakVM;
    public BreakVM? SelectedBreakVM
    {
        get => selectedBreakVM;
        set
        {
            selectedBreakVM = value;
            OnPropertyChanged(nameof(SelectedBreakVM));
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
        shiftVMs = new ObservableCollection<ShiftVM>();

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

        Departments = new ObservableCollection<Department>(Helios.StaffReader.SubDepartments(Charon.Employee?.DepartmentName ?? ""));
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.Employee?.DepartmentName);
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
        ShiftVMs.Add(new ShiftVM(shift));

        Helios.StaffCreator.Shift(shift);
    }

    public void DeleteShift(ShiftVM shiftVM)
    {
        var shift = shiftVM.Shift;
        if (MessageBox.Show($"Are you sure you want to delete the {shift.Name} shift?", "Confirm Delete",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        SelectedDepartment?.Shifts.Remove(shift);
        ShiftVMs.Remove(shiftVM); 

        Helios?.StaffDeleter.Shift(shift);
    }

    public void AddBreak(ShiftVM shift)
    {
        if (Helios is null) return;

        var inputBox = new InputWindow("Enter Break Name", "New Break");

        if (inputBox.ShowDialog() != true) return;

        var breakName = inputBox.VM.Input;

        if (shift.Breaks.Any(b => b.Name == breakName)) return;

        var @break = new Break(shift.Shift, breakName);

        shift.AddBreak(@break);

        Helios.StaffCreator.Break(@break);
    }

    public void RemoveBreak(BreakVM breakVM)
    {
        breakVM.Remove();
        Helios?.StaffDeleter.Break(breakVM.Break);
    }

    public void LaunchShiftEmployeeWindow(Shift shift)
    {
        if (Helios is null || Charon is null) return;

        var employeeWindow = new ShiftEmployeeWindow(this, shift);
        employeeWindow.ShowDialog();
    }
}