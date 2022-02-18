using Aion.ViewModel.Commands;
using Aion.ViewModel.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aion.View;
using System.Windows;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
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

    public class EmployeePageVM : INotifyPropertyChanged, IFilters, IDBInteraction
    {
        public Helios Helios { get; set; }
        public Charon Charon { get; set; }

        private List<Employee> allEmployees;

        private ObservableCollection<Employee> employees;
        public ObservableCollection<Employee> Employees
        {
            get => employees;
            set
            {
                employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        private Employee selectedEmployee;
        public Employee SelectedEmployee
        {
            get => selectedEmployee;
            set
            {
                selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        private Employee selectedReport;
        public Employee SelectedReport
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
        public RepairDataCommand RepairDataCommand { get; set; }

        public EmployeePageVM()
        {
            // Commands
            LaunchEmployeeEditorCommand = new(this);
            LaunchEmployeeCreatorCommand = new(this);
            DeleteEmployeeCommand = new(this);
            ApplyFiltersCommand = new(this);
            ClearFiltersCommand = new(this);
            RefreshDataCommand = new(this);
            ApplySortingCommand = new(this);
            GoToEmployeeCommand = new(this);
            RepairDataCommand = new(this);
        }

        public void SetDataSources(Helios helios, Charon charon)
        {
            Helios = helios;
            Charon = charon;
            allEmployees ??= new(Helios.StaffReader.GetManagedEmployees(Charon.UserEmployee.ID));
            Task.Run(RefreshData);
        }

        /// <summary>
        /// Applies the listed filters for Employee Name/Department/ReportsTo/Role.
        /// </summary>
        public void ApplyFilters()
        {
            if (Charon.CurrentUser is null) return;

            IEnumerable<Employee> employeeBase = allEmployees;

            try
            {
                FilterName(ref employeeBase);
                FilterDepartment(ref employeeBase);
                // ReSharper disable once PossibleMultipleEnumeration
                FilterReports(ref employeeBase);
                // ReSharper disable once PossibleMultipleEnumeration
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

        private void FilterName(ref IEnumerable<Employee> employeeGroup)
        {
            if ((employeeSearchString ?? "") == "") return;

            Regex rex = new(employeeSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            employeeGroup = employeeGroup?.Where(employee => rex.IsMatch(employee.FullName ?? "") || rex.IsMatch(employee.ID.ToString()));
        }

        private void FilterDepartment(ref IEnumerable<Employee> employeeGroup)
        {
            if ((departmentSearchString ?? "") == "") return;

            Regex rex = new(departmentSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            employeeGroup = employeeGroup?.Where(employee => rex.IsMatch(employee.DepartmentName ?? ""));
        }

        private void FilterReports(ref IEnumerable<Employee> employeeGroup)
        {
            if ((reportSearchString ?? "") == "") return;

            Regex rex = new(reportSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            employeeGroup = employeeGroup?.Where(employee => rex.IsMatch(employee.ReportsTo?.FullName ?? "") || rex.IsMatch(employee.ReportsTo?.ID.ToString() ?? ""));
        }

        private void FilterRole(ref IEnumerable<Employee> employeeGroup)
        {
            if ((roleSearchString ?? "") == "") return;

            Regex rex = new(roleSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            employeeGroup = employeeGroup?.Where(employee => rex.IsMatch(employee.RoleName));
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
                Employees = new();
                return;
            }
            Employees = SortOption switch
            {
                EEmployeeSortOption.Department =>
                    new(employeeArray.OrderBy(e => e.DepartmentName)),
                EEmployeeSortOption.JobClassification =>
                    new(employeeArray.OrderBy(e => e.RoleName)),
                EEmployeeSortOption.EmployeeLastName =>
                    new(employeeArray.OrderBy(e => e.LastName).ThenBy(e => e.FirstName)),
                EEmployeeSortOption.EmployeeFirstName =>
                    new(employeeArray.OrderBy(e => e.FirstName).ThenBy(e => e.LastName)),
                EEmployeeSortOption.EmployeeID => 
                    new(employeeArray.OrderBy(e => e.ID)),
                EEmployeeSortOption.ReportsToLastName =>
                    new(employeeArray.OrderBy(e => e.ReportsTo?.LastName).ThenBy(e => e.ReportsTo?.FirstName)),
                EEmployeeSortOption.ReportsToFirstName =>
                    new(employeeArray.OrderBy(e => e.ReportsTo?.FirstName).ThenBy(e => e.ReportsTo?.LastName)),
                EEmployeeSortOption.ReportsToID =>
                    new(employeeArray.OrderBy(e => e.ReportsTo?.ID)),
                _ => new(employeeArray)
            };
        }

        internal void LaunchEmployeeCreator()
        {
            EmployeeCreationWindow creator = new(Helios);
            if (creator.ShowDialog() != true) return;

            EmployeeEditorWindow editor = new(Helios, creator.VM.NewEmployee, true);
            if (editor.ShowDialog() == true)
                RefreshData();
        }

        internal void LaunchEmployeeEditor()
        {
            EmployeeEditorWindow editorWindow = new(Helios, SelectedEmployee, false);
            editorWindow.ShowDialog();
        }

        internal void DeleteEmployee()
        {
            if (SelectedEmployee is null) { return; }
            MessageBox.Show("This feature is not yet implemented","Feature Missing",MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public void GoToEmployee()
        {
            if (SelectedReport is null) return;

            if (allEmployees.All(e => e.ID != selectedReport.ID))
            {
                MessageBox.Show(
                    $"ERROR: Selected report employee, {SelectedReport.FullName}, (reports to {SelectedEmployee.FullName}) was not found in the primary employee list.",
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

        public void RefreshData()
        {
            allEmployees = new(Helios.StaffReader.GetManagedEmployees(Charon.UserEmployee.ID));
            ClearFilters();
        }

        public void RepairData()
        {
            throw new System.NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
