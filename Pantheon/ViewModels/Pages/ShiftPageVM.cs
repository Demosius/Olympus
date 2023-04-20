using Pantheon.ViewModels.Commands.Shifts;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Morpheus.Views.Windows;
using Pantheon.ViewModels.Controls.Shifts;
using Pantheon.ViewModels.Interfaces;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Pages;

public class ShiftPageVM : INotifyPropertyChanged, IDBInteraction, IShiftManagerVM
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

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
            ShiftVMs = new ObservableCollection<ShiftVM>(SelectedDepartment?.Shifts.Select(s => new ShiftVM(s, this)) ?? new List<ShiftVM>());
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

    #endregion

    public ShiftPageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        departments = new ObservableCollection<Department>(Helios.StaffReader.SubDepartments(Charon.Employee?.DepartmentName ?? "")); 
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.Employee?.DepartmentName);
        shiftVMs = new ObservableCollection<ShiftVM>(SelectedDepartment?.Shifts.Select(s => new ShiftVM(s, this)) ?? new List<ShiftVM>());

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        CreateShiftCommand = new CreateShiftCommand(this);
    }
    
    public void RefreshData()
    {
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
        if (SelectedDepartment is null) return;

        var inputBox = new InputWindow("Enter Shift Name", "New Shift");

        if (inputBox.ShowDialog() != true) return;

        var shiftName = inputBox.InputText;

        if (SelectedDepartment.Shifts.Any(s => s.Name == shiftName)) return;

        var shift = new Shift(SelectedDepartment, shiftName);
        if (!SelectedDepartment.Shifts.Any()) shift.Default = true;

        SelectedDepartment.Shifts.Add(shift);
        ShiftVMs.Add(new ShiftVM(shift, this));

        Helios.StaffCreator.Shift(shift);
    }

    public void DeleteShift(ShiftVM shiftVM)
    {
        var shift = shiftVM.Shift;
        if (MessageBox.Show($"Are you sure you want to delete the {shift.Name} shift?", "Confirm Delete",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        SelectedDepartment?.Shifts.Remove(shift);
        ShiftVMs.Remove(shiftVM); 

        Helios.StaffDeleter.Shift(shift);
    }

    public void RemoveBreak(BreakVM breakVM)
    {
        breakVM.Remove();
        Helios.StaffDeleter.Break(breakVM.Break);
    }
}