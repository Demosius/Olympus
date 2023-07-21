using Aion.View;
using Aion.ViewModels.Commands;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Aion.ViewModels;

public enum EEmployeeSortOption
{
    EmployeeID,
    EmployeeLastName,
    EmployeeFirstName,
    Department,
    ReportsToID,
    ReportsToLastName,
    ReportsToFirstName,
    JobClassification
}

public class EmployeePageVM : INotifyPropertyChanged, IFilters, IDBInteraction, ISorting
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    private List<Employee> allEmployees;
    
    public ObservableCollection<Employee> Employees { get; set; }

    private Employee? selectedEmployee;
    public Employee? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged(nameof(SelectedEmployee));
        }
    }

    private Employee? selectedReport;
    public Employee? SelectedReport
    {
        get => selectedReport;
        set
        {
            selectedReport = value;
            OnPropertyChanged(nameof(SelectedReport));
        }
    }

    private string employeeSearchString;
    public string EmployeeSearchString
    {
        get => employeeSearchString;
        set
        {
            employeeSearchString = value;
            OnPropertyChanged(nameof(EmployeeSearchString));
        }
    }

    private string departmentSearchString;
    public string DepartmentSearchString
    {
        get => departmentSearchString;
        set
        {
            departmentSearchString = value;
            OnPropertyChanged(nameof(DepartmentSearchString));
        }
    }

    private string reportSearchString;
    public string ReportSearchString
    {
        get => reportSearchString;
        set
        {
            reportSearchString = value;
            OnPropertyChanged(nameof(ReportSearchString));
        }
    }

    private string roleSearchString;
    public string RoleSearchString
    {
        get => roleSearchString;
        set
        {
            roleSearchString = value;
            OnPropertyChanged(nameof(RoleSearchString));
        }
    }

    private EEmployeeSortOption sortOption;
    public EEmployeeSortOption SortOption
    {
        get => sortOption;
        set
        {
            sortOption = value;
            OnPropertyChanged(nameof(SortOption));
        }
    }

    /* Commands */
    public LaunchEmployeeCreatorCommand LaunchEmployeeCreatorCommand { get; set; }
    public LaunchEmployeeEditorCommand LaunchEmployeeEditorCommand { get; set; }
    public DeleteEmployeeCommand DeleteEmployeeCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplySortingCommand ApplySortingCommand { get; set; }
    public GoToEmployeeCommand GoToEmployeeCommand { get; set; }

    private EmployeePageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        allEmployees = new List<Employee>();
        Employees = new ObservableCollection<Employee>();

        employeeSearchString = string.Empty;
        departmentSearchString = string.Empty;
        reportSearchString = string.Empty;
        roleSearchString = string.Empty;

        // Commands
        LaunchEmployeeEditorCommand = new LaunchEmployeeEditorCommand(this);
        LaunchEmployeeCreatorCommand = new LaunchEmployeeCreatorCommand(this);
        DeleteEmployeeCommand = new DeleteEmployeeCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);
        ApplySortingCommand = new ApplySortingCommand(this);
        GoToEmployeeCommand = new GoToEmployeeCommand(this);
    }

    private async Task<EmployeePageVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<EmployeePageVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new EmployeePageVM(helios, charon);
        return ret.InitializeAsync();
    }

    /// <summary>
    /// Applies the listed filters for Employee Name/Department/ReportsTo/Role.
    /// </summary>
    public void ApplyFilters()
    {
        if (Charon.User is null) return;

        var employeeBase = allEmployees;

        try
        {
            FilterName(ref employeeBase);
            FilterDepartment(ref employeeBase);
            FilterReports(ref employeeBase);
            FilterRole(ref employeeBase);
        }
        catch (RegexParseException ex)
        {
            MessageBox.Show("Issue with pattern matching in filters:\n\n" +
                            $"{ex.Message}", "RegEx Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        ApplySorting(employeeBase);
    }

    private void FilterName(ref List<Employee> employeeGroup)
    {
        if (employeeSearchString == "") return;

        Regex rex = new(employeeSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        employeeGroup = employeeGroup.Where(employee => rex.IsMatch(employee.FullName) || rex.IsMatch(employee.ID.ToString())).ToList();
    }

    private void FilterDepartment(ref List<Employee> employeeGroup)
    {
        if (departmentSearchString == "") return;

        Regex rex = new(departmentSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        employeeGroup = employeeGroup.Where(employee => rex.IsMatch(employee.DepartmentName)).ToList();
    }

    private void FilterReports(ref List<Employee> employeeGroup)
    {
        if (reportSearchString == "") return;

        Regex rex = new(reportSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        employeeGroup = employeeGroup.Where(employee => rex.IsMatch(employee.ReportsTo?.FullName ?? "") || rex.IsMatch(employee.ReportsTo?.ID.ToString() ?? "")).ToList();
    }

    private void FilterRole(ref List<Employee> employeeGroup)
    {
        if (roleSearchString == "") return;

        Regex rex = new(roleSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        employeeGroup = employeeGroup.Where(employee => rex.IsMatch(employee.RoleName)).ToList();
    }

    /// <summary>
    /// Applies the selected sorting to the employee list.
    /// </summary>
    public void ApplySorting()
    {
        ApplySorting(Employees);
    }

    /// <summary>
    /// Applies the selected sorting to the given employee list.
    /// </summary>
    public void ApplySorting(IEnumerable<Employee> employeeGroup)
    {
        var employeeArray = employeeGroup as Employee[] ?? employeeGroup.ToArray();
        if (!employeeArray.Any())
        {
            Employees = new ObservableCollection<Employee>();
            return;
        }
        Employees = SortOption switch
        {
            EEmployeeSortOption.Department =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.DepartmentName)),
            EEmployeeSortOption.JobClassification =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.RoleName)),
            EEmployeeSortOption.EmployeeLastName =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)),
            EEmployeeSortOption.EmployeeFirstName =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)),
            EEmployeeSortOption.EmployeeID =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.ID)),
            EEmployeeSortOption.ReportsToLastName =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.ReportsTo?.LastName).ThenBy(e => e.ReportsTo?.FirstName)),
            EEmployeeSortOption.ReportsToFirstName =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.ReportsTo?.FirstName).ThenBy(e => e.ReportsTo?.LastName)),
            EEmployeeSortOption.ReportsToID =>
                new ObservableCollection<Employee>(employeeArray.OrderBy(e => e.ReportsTo?.ID)),
            _ => new ObservableCollection<Employee>(employeeArray)
        };
    }

    internal async Task LaunchEmployeeCreator()
    {
        EmployeeCreationWindow creator = new(Helios);
        if (creator.ShowDialog() != true) return;
        if (creator.VM?.NewEmployee is null) return;

        EmployeeEditorWindow editor = new(Helios, creator.VM!.NewEmployee, true);
        if (editor.ShowDialog() == true)
            await RefreshDataAsync();
    }

    internal void LaunchEmployeeEditor()
    {
        if (SelectedEmployee is null) return;

        EmployeeEditorWindow editorWindow = new(Helios, SelectedEmployee, false);
        editorWindow.ShowDialog();
    }

    internal static void DeleteEmployee()
    {
        MessageBox.Show("This feature is not yet implemented", "Feature Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public void GoToEmployee()
    {
        if (SelectedReport is null) return;

        if (allEmployees.All(e => e.ID != SelectedReport.ID))
        {
            MessageBox.Show(
                $"ERROR: Selected report employee, {SelectedReport.FullName}, (reports to {SelectedEmployee!.FullName}) was not found in the primary employee list.",
                "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        SelectedEmployee = allEmployees.First(e => e.ID == SelectedReport.ID);
    }

    public void ClearFilters()
    {
        EmployeeSearchString = "";
        DepartmentSearchString = "";
        ReportSearchString = "";
        RoleSearchString = "";
        ApplyFilters();
    }

    public async Task RefreshDataAsync()
    {
        allEmployees = new List<Employee>(await Helios.StaffReader.GetManagedEmployeesAsync(Charon.Employee?.ID ?? 0));
        ClearFilters();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}