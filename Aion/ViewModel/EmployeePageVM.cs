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

        public ObservableCollection<Employee> Managers { get; set; }

        private Employee selectedManager;
        public Employee SelectedManager
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                OnPropertyChanged(nameof(SelectedManager));
                ApplyFilters();
            }
        }

        private string empSearchString;
        public string EmpSearchString
        {
            get => empSearchString;
            set
            {
                empSearchString = value;
                OnPropertyChanged(nameof(EmpSearchString));
            }
        }

        /* Commands */
        public LaunchEmployeeCreatorCommand LaunchEmployeeCreatorCommand { get; set; }
        public LaunchEmployeeEditorCommand LaunchEmployeeEditorCommand { get; set; }
        public DeleteEmployeeCommand DeleteEmployeeCommand { get; set; }
        public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
        public ClearFiltersCommand ClearFiltersCommand { get; set; }
        public RefreshDataCommand RefreshDataCommand { get; set; }

        public EmployeePageVM()
        {
            // Commands
            LaunchEmployeeEditorCommand = new(this);
            LaunchEmployeeCreatorCommand = new(this);
            DeleteEmployeeCommand = new(this);
            ApplyFiltersCommand = new(this);
            ClearFiltersCommand = new(this);
        }

        public void SetDataSources(Helios helios, Charon charon)
        {
            Helios = helios;
            Charon = charon;
            allEmployees ??= new(Helios.StaffReader.GetManagedEmployees(Charon.UserEmployee.ID));
            Task.Run(RefreshData);
        }

        /// <summary>
        /// Apply appropriate filters to the viewable employee list.
        /// </summary>
        public void ApplyFilters()
        {
            if ((empSearchString ?? "") != "")
            {
                Regex rex = new(EmpSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                Employees = new(allEmployees.Where(e => rex.IsMatch(e.ToString())));
            }
            else
                Employees = new(allEmployees);

            if (SelectedManager.ID != -1)
                Employees = new(Employees.Where(e => e.ReportsToID == SelectedManager.ID));
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

        public void ClearFilters()
        {
            SelectedManager = Managers[0];
            EmpSearchString = "";
            ApplyFilters();
        }

        public void RefreshData()
        {
            allEmployees = new(Helios.StaffReader.GetManagedEmployees(Charon.UserEmployee.ID));
            // Set managers, including a 'clear' one for removing manager filter.
            Managers = new(Helios.StaffReader.GetManagers().OrderBy(m => m.ToString()));
            SelectedManager = new() { FirstName = "<- None", LastName = "Selected ->", ID = -1 };
            Managers.Insert(0, SelectedManager);
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
