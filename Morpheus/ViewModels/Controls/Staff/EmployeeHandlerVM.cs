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

public class EmployeeHandlerVM : INotifyPropertyChanged, IDBInteraction, IFilters, ICreate
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public List<EmployeeVM> AllEmployees { get; set; }

    public bool CanCreateEmployee => Charon.CanCreateEmployee();

    #region INotifyPropertyChanged Members

    public ObservableCollection<EmployeeVM> Employees { get; set; }

    private EmployeeVM? selectedEmployee;
    public EmployeeVM? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged();
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

    private bool showDeleted;
    public bool ShowDeleted
    {
        get => showDeleted && CanCreateEmployee;
        set
        {
            showDeleted = value;
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

    #endregion

    private EmployeeHandlerVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        AllEmployees = new List<EmployeeVM>();
        Employees = new ObservableCollection<EmployeeVM>();
        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        CreateNewItemCommand = new CreateNewItemCommand(this);
    }

    private async Task<EmployeeHandlerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<EmployeeHandlerVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new EmployeeHandlerVM(helios, charon);
        return ret.InitializeAsync();
    }

    public static EmployeeHandlerVM CreateEmpty(Helios helios, Charon charon) => new(helios, charon);

    public async Task RefreshDataAsync()
    {
        var employees = await Helios.StaffReader.EmployeesAsync(e => e.ID > 0);
        var empDict = employees.ToDictionary(e => e.ID, e => e);

        foreach (var employee in employees)
        {
            if (empDict.TryGetValue(employee.ReportsToID, out var boss))
                employee.ReportsTo = boss;
        }

        AllEmployees = employees.Select(e => new EmployeeVM(e, Helios, Charon)).Where(e => CanCreateEmployee || e.IsActive).ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        showDeleted = false;
        OnPropertyChanged(nameof(FilterString));
        OnPropertyChanged(nameof(ShowDeleted));
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var employees = AllEmployees.Where(e => 
            (ShowDeleted || e.IsActive) &&
            (Regex.IsMatch(e.Employee.FullName, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.ID.ToString(), FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.DepartmentName, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.PC_ID, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.RF_ID, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.DematicID, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.ClanName, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.Location, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.PayPoint, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.ReportsToID, FilterString, RegexOptions.IgnoreCase) ||
             Regex.IsMatch(e.ReportsTo, FilterString, RegexOptions.IgnoreCase)));

        Employees.Clear();
        foreach (var employeeVM in employees)
            Employees.Add(employeeVM);
    }

    public async Task CreateNewItemAsync()
    {
        if (!CanCreateEmployee) return;

        var input = new InputWindow("Enter new Employee ID (5-digit number):", "New Employee");

        if (input.ShowDialog() != true) return;

        var inputText = input.InputText;

        if (inputText.Length != 5 || !int.TryParse(inputText, out var id))
        {
            MessageBox.Show("Employee ID must be 5 digits long. Include leading 0's if necessary.",
                "Failed to Create Employee", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (Helios.StaffReader.EmployeeExists(id))
        {
            MessageBox.Show($"Employee ID: {id} - already exists.", "Failed to Create Employee", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var emp = new Employee(id) {IsActive = true};

        await Helios.StaffCreator.EmployeeAsync(emp);

        var empVM = new EmployeeVM(emp, Helios, Charon);

        AllEmployees.Add(empVM);
        Employees.Add(empVM);
        SelectedEmployee = empVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}