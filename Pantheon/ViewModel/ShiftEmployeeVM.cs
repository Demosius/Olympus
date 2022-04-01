using Pantheon.Annotations;
using Pantheon.ViewModel.Commands;
using Pantheon.ViewModel.Pages;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel
{
    internal class ShiftEmployeeVM : INotifyPropertyChanged
    {
        public ShiftPageVM? ParentVM { get; set; }
        public Shift? Shift { get; set; }
        public Helios? Helios { get; set; }
        public Charon? Charon { get; set; }

        #region Notifiable Properties

        private ObservableCollection<EmployeeShift> employeeShifts;
        public ObservableCollection<EmployeeShift> EmployeeShifts
        {
            get => employeeShifts;
            set
            {
                employeeShifts = value;
                OnPropertyChanged(nameof(EmployeeShifts));
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
                ApplyFilters();
            }
        }

        #endregion

        private List<EmployeeShift> empShifts;

        public Dictionary<int, Employee> Employees { get; set; }

        public ConfirmEmployeeAssignmentCommand ConfirmEmployeeAssignmentCommand { get; set; }

        public ShiftEmployeeVM()
        {
            employeeShifts = new ObservableCollection<EmployeeShift>();
            Employees = new Dictionary<int, Employee>();
            employeeSearchString = string.Empty;
            empShifts = new List<EmployeeShift>();

            ConfirmEmployeeAssignmentCommand = new ConfirmEmployeeAssignmentCommand(this);
        }

        public void SetData(ShiftPageVM shiftPageVM, Shift shift)
        {
            ParentVM = shiftPageVM;
            Shift = shift;
            Helios = ParentVM.Helios;
            Charon = ParentVM.Charon;

            if (Helios is null || ParentVM.SelectedDepartment is null) return;

            empShifts = Helios.StaffReader.EmployeeShifts(Shift).ToList();
            Employees = Helios.StaffReader.Employees(e => e.DepartmentName == ParentVM.SelectedDepartment.Name).ToDictionary(e => e.ID, e => e);

            // Set current connections as active and original.
            foreach (var employeeShift in empShifts)
            {
                employeeShift.Active = true;
                employeeShift.Original = true;
                employeeShift.Shift = Shift;
                if (Employees.TryGetValue(employeeShift.EmployeeID, out var employee))
                    employeeShift.Employee = employee;
            }

            // Create remaining potential employee connections.
            var currentConnections = empShifts.Select(es => es.EmployeeID);
            foreach (var employee in Employees.Values.Where(e => !currentConnections.Contains(e.ID)))
            {
                var newConn = new EmployeeShift(employee, Shift);
                empShifts.Add(newConn);
            }
            
            // Sort empShifts.
            empShifts.Sort();

            ApplyFilters();
        }

        public void ApplyFilters()
        {
            var regex = new Regex(EmployeeSearchString, RegexOptions.IgnoreCase);
            EmployeeShifts = new ObservableCollection<EmployeeShift>(empShifts.Where(e => regex.IsMatch(e.Employee?.FullName ?? "")));
        }

        public void ConfirmEmployeeAssignment()
        {
            if (Helios is null || Shift is null) return;

            Helios.StaffUpdater.EmployeeToShiftConnections(EmployeeShifts);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
