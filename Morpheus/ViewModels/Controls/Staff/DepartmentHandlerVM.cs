using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Morpheus.Views.Windows;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class DepartmentHandlerVM : INotifyPropertyChanged, IDBInteraction, IFilters, ICreateDelete<DepartmentVM>
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public List<DepartmentVM> AllDepartments { get; set; }

    public bool CanCreateDepartment { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<DepartmentVM> Departments { get; set; }

    public bool CanDeleteDepartment => SelectedItem?.CanDelete ?? false;

    private DepartmentVM? selectedItem;
    public DepartmentVM? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDeleteDepartment));
        }
    }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public CreateNewItemCommand CreateNewItemCommand { get; set; }
    public DeleteSelectedItemCommand<DepartmentVM> DeleteSelectedItemCommand { get; set; }

    #endregion

    private DepartmentHandlerVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanCreateDepartment = Charon.CanCreateDepartment();
        filterString = string.Empty;

        AllDepartments = new List<DepartmentVM>();
        Departments = new ObservableCollection<DepartmentVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        CreateNewItemCommand = new CreateNewItemCommand(this);
        DeleteSelectedItemCommand = new DeleteSelectedItemCommand<DepartmentVM>(this);
    }

    private async Task<DepartmentHandlerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<DepartmentHandlerVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new DepartmentHandlerVM(helios, charon);
        return ret.InitializeAsync();
    }

    public static DepartmentHandlerVM CreateEmpty(Helios helios, Charon charon) => new(helios, charon);

    public async Task RefreshDataAsync()
    {
        var departments = await Helios.StaffReader.DepartmentsAsync();
        var empDict = (await Helios.StaffReader.EmployeesAsync()).ToDictionary(e => e.ID, e => e);
        foreach (var department in departments)
        {
            if (empDict.TryGetValue(department.HeadID, out var head))
                department.Head = head;
        }
        AllDepartments = departments.Select(r => new DepartmentVM(r, Helios, Charon)).ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        var departments = AllDepartments.Where(r =>
            Regex.IsMatch(r.Name, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.Head, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.HeadID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.OverDepartmentName, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.PayPoint, FilterString, RegexOptions.IgnoreCase));

        Departments.Clear();
        foreach (var staffDepartmentVM in departments)
            Departments.Add(staffDepartmentVM);
    }

    public async Task CreateNewItemAsync()
    {
        if (!CanCreateDepartment) return;

        var input = new InputWindow("Enter new Department Name:", "New Department");

        if (input.ShowDialog() != true) return;

        var newDepartmentName = input.InputText;

        if (await Helios.StaffReader.DepartmentExistsAsync(newDepartmentName))
        {
            MessageBox.Show($"Department - {newDepartmentName} - already exists.", "Failed to Create Department", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var department = new Department(newDepartmentName);

        await Helios.StaffCreator.DepartmentAsync(department);

        var departmentVM = new DepartmentVM(department, Helios, Charon);

        AllDepartments.Add(departmentVM);
        Departments.Add(departmentVM);
        SelectedItem = departmentVM;
    }

    public async Task DeleteSelectedItemAsync()
    {
        if (SelectedItem is null || !CanDeleteDepartment) return;
        if (MessageBox.Show($"Are you sure that you want to delete {SelectedItem.Department}?", "Confirm Department Deletion",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        await Helios.StaffDeleter.DepartmentAsync(SelectedItem.Department);
        await RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}