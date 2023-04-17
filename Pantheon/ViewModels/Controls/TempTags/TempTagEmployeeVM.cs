using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.TempTags;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.TempTags;

public class TempTagEmployeeVM : INotifyPropertyChanged, IFilters
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public TempTagPageVM ParentVM { get; set; }

    public EmployeeDataSet DataSet { get; set; }

    private List<EmployeeVM> allEmployees;
    #region INotifyPropertyChanged Members

    public ObservableCollection<EmployeeVM> Employees { get; set; }
    public ObservableCollection<string> DepartmentNames { get; set; }

    private EmployeeVM? selectedEmployee;
    public EmployeeVM? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged();
            ParentVM.SelectedEmployee = value;
        }
    }

    private string selectedDepartmentName;
    public string SelectedDepartmentName
    {
        get => selectedDepartmentName;
        set
        {
            selectedDepartmentName = value;
            DataSet.Departments.TryGetValue(selectedDepartmentName, out var department);
            SelectedDepartment = department;
            OnPropertyChanged();
        }
    }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
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

    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public ReOrderEmployeesCommand ReOrderEmployeesCommand { get; set; }

    #endregion

    public TempTagEmployeeVM(TempTagPageVM parentVM, EmployeeDataSet dataSet)
    {
        ParentVM = parentVM;
        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;
        DataSet = dataSet;

        allEmployees = new List<EmployeeVM>();
        Employees = new ObservableCollection<EmployeeVM>();
        DepartmentNames = new ObservableCollection<string>();
        selectedDepartmentName = string.Empty;
        filterString = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        ReOrderEmployeesCommand = new ReOrderEmployeesCommand(this);

        RefreshData();
    }

    private void RefreshData()
    {
        allEmployees = DataSet.Employees.Values.Select(e => new EmployeeVM(e, Charon, Helios))
            .OrderByDescending(e => e.HasTempTag).ThenBy(e => e.FullName).ToList();

        DepartmentNames.Add("<-- All -->");
        foreach (var departmentName in DataSet.Departments.Keys.OrderBy(s => s))
            DepartmentNames.Add(departmentName);

        selectedDepartmentName = DepartmentNames.Contains("Restock") ? "Restock" : DepartmentNames[0];
        DataSet.Departments.TryGetValue(selectedDepartmentName, out var department);
        selectedDepartment = department;

        ApplyFilters();
    }

    public void RefreshData(EmployeeDataSet dataSet)
    {
        DataSet = dataSet;
        Employees.Clear();
        filterString = string.Empty;
        SelectedEmployee = null;
        DepartmentNames.Clear();
        RefreshData();
    }

    public void ReOrderEmployees()
    {
        var id = SelectedEmployee?.ID;
        allEmployees = allEmployees.OrderByDescending(e => e.HasTempTag).ThenBy(e => e.FullName).ToList();
        ApplyFilters();
        if (id is null) return;
        SelectedEmployee = Employees.FirstOrDefault(e => e.ID == id);
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
        SelectedDepartmentName = DepartmentNames[0];
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        Employees.Clear();

        var employees = allEmployees.Where(emp =>
            (SelectedDepartment is null || emp.Department == SelectedDepartment) &&
            Regex.IsMatch(emp.FullName, FilterString, RegexOptions.IgnoreCase));

        foreach (var employeeVM in employees)
            Employees.Add(employeeVM);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}