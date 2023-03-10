using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Generic;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Pantheon.ViewModels.PopUp.Employees;

public class EmployeeSelectionVM : INotifyPropertyChanged, IFilters, ISelector
{
    private const string MANAGER_SELECTION = "Manager Selection";
    private const string EMPLOYEE_SELECTION = "Employee Selection";
    private const string ANY_DEP_STR = "<Any>";

    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    
    public List<EmployeeVM> FullEmployeeList { get; set; }
    public List<EmployeeVM> Managers { get; set; }

    public string SelectionName { get; }

    public bool CanCreate => false;
    public bool CanDelete => false;
    public bool CanConfirm => SelectedEmployee is not null;

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
            OnPropertyChanged(nameof(CanConfirm));
        }
    }

    // Filters

    private bool managersOnly;
    public bool ManagersOnly
    {
        get => managersOnly;
        set
        {
            managersOnly = value;
            OnPropertyChanged();
            ApplyFilters();
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

    public ObservableCollection<string> DepartmentNames { get; set; }

    private string selectedDepartmentName;
    public string SelectedDepartmentName
    {
        get => selectedDepartmentName;
        set
        {
            selectedDepartmentName = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands
    
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }

    #endregion

    public EmployeeSelectionVM(Helios helios, Charon charon, bool managers = false, string? departmentName = null)
    {
        Helios = helios;
        Charon = charon;

        managersOnly = managers;
        filterString = string.Empty;

        SelectionName = managers ? MANAGER_SELECTION : EMPLOYEE_SELECTION;

        FullEmployeeList = Helios.StaffReader.Employees().OrderBy(e => e.FullName).Select(e => new EmployeeVM(e, Charon, Helios)).ToList();

        var managerIDs = FullEmployeeList.Where(e => e.ReportsToID != 0).Select(e => e.ReportsToID).Distinct();

        Managers = FullEmployeeList.Where(e => managerIDs.Contains(e.ID)).ToList();

        Employees = new ObservableCollection<EmployeeVM>();

        DepartmentNames = new ObservableCollection<string> { ANY_DEP_STR };

        var departments = FullEmployeeList.Select(e => e.DepartmentName).Distinct().OrderBy(n => n);

        selectedDepartmentName = departmentName ?? ANY_DEP_STR;

        foreach (var department in departments) DepartmentNames.Add(department);
        
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand  = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);

        ApplyFilters();
    }
    
    public void ClearFilters()
    {
        managersOnly = SelectionName == MANAGER_SELECTION;
        filterString = string.Empty;
        selectedDepartmentName = ANY_DEP_STR;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        Employees.Clear();
        IEnumerable<EmployeeVM> list = ManagersOnly ? Managers : FullEmployeeList;

        if (SelectedDepartmentName != ANY_DEP_STR) list = list.Where(e => e.DepartmentName == SelectedDepartmentName);

        list = list.Where(e => 
            Regex.IsMatch(e.FullName, FilterString) ||
            Regex.IsMatch(e.RoleName, FilterString));

        foreach (var employeeVM in list)
        {
            Employees.Add(employeeVM);
        }
    }

    public void Create() { }

    public void Delete() { }

    public void ApplySorting()
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