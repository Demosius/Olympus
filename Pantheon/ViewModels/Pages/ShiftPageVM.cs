using Pantheon.ViewModels.Commands.Shifts;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
    
    public ObservableCollection<ShiftVM> ShiftVMs { get; set; }

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
    
    public ObservableCollection<Department> Departments { get; set; }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged(nameof(SelectedDepartment));
            ShiftVMs.Clear();
            foreach (var shift in SelectedDepartment?.Shifts.Select(s => new ShiftVM(s, this)) ?? new List<ShiftVM>())
                ShiftVMs.Add(shift);
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
    public CreateShiftCommand CreateShiftCommand { get; set; }

    #endregion

    private ShiftPageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        Departments = new ObservableCollection<Department>();
        ShiftVMs = new ObservableCollection<ShiftVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        CreateShiftCommand = new CreateShiftCommand(this);
    }

    private async Task<ShiftPageVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<ShiftPageVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new ShiftPageVM(helios, charon);
        return ret.InitializeAsync();
    }

    public static ShiftPageVM CreateEmpty(Helios helios, Charon charon) => new(helios, charon);

    public async Task RefreshDataAsync()
    {
        Departments.Clear();

        foreach (var department in await Helios.StaffReader.SubDepartmentsAsync(Charon.Employee?.DepartmentName ?? ""))
            Departments.Add(department);

        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.Employee?.DepartmentName);
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