using Pantheon.Annotations;
using Pantheon.ViewModel.Commands;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel
{
    internal class EmployeeShiftVM : INotifyPropertyChanged
    {
        public Employee? Employee { get; set; }
        public Helios? Helios { get; set; }
        public Charon? Charon { get; set; }

        #region Notifiable Properties

        private ObservableCollection<string> shiftNames;
        public ObservableCollection<string> ShiftNames
        {
            get => shiftNames;
            set
            {
                shiftNames = value;
                OnPropertyChanged(nameof(ShiftNames));
            }
        }

        private string selectedShiftName;
        public string SelectedShiftName
        {
            get => selectedShiftName;
            set
            {
                selectedShiftName = value;
                OnPropertyChanged(nameof(SelectedShiftName));
            }
        }

        private ObservableCollection<EmployeeShift> empShifts;
        public ObservableCollection<EmployeeShift> EmpShifts
        {
            get => empShifts;
            set
            {
                empShifts = value;
                OnPropertyChanged(nameof(EmpShifts));
            }
        }

        #endregion

        public Dictionary<string, Shift?> Shifts { get; set; }

        public ConfirmShiftAdjustmentsCommand ConfirmShiftAdjustmentsCommand { get; set; }

        public EmployeeShiftVM()
        {
            shiftNames = new ObservableCollection<string>();
            empShifts = new ObservableCollection<EmployeeShift>();
            Shifts = new Dictionary<string, Shift?>();

            ConfirmShiftAdjustmentsCommand = new ConfirmShiftAdjustmentsCommand(this);
        }

        public void SetData(Helios helios, Charon charon, Employee employee)
        {
            Employee = employee;
            Helios = helios;
            Charon = charon;

            Employee.DepartmentName = Employee.Department?.Name ?? Employee.DepartmentName;

            var shifts = Helios.StaffReader.Shifts(Employee).ToList();
            Shifts = shifts.ToDictionary(s => s.Name, s => s)!;
            Shifts.Add("<-- No Default -->", null);

            ShiftNames = new ObservableCollection<string>(Shifts.Keys);
            SelectedShiftName = Shifts.TryGetValue(Employee.DefaultShiftID, out _)
                ? Employee.DefaultShiftID
                : "<-- No Default -->";

            EmpShifts = new ObservableCollection<EmployeeShift>(Helios.StaffReader.EmployeeShifts(employee));

            // Set current connections as active and original.
            foreach (var employeeShift in EmpShifts)
            {
                employeeShift.Active = true;
                employeeShift.Original = true;
                employeeShift.Employee = Employee;
                if (Shifts.TryGetValue(employeeShift.ShiftID, out var shift))
                    employeeShift.Shift = shift;
            }

            // Create remaining potential shift connections.
            var currentConnections = empShifts.Select(es => es.ShiftID);
            foreach (var shift in Shifts.Values.Where(s => s is not null && !currentConnections.Contains(s.ID)))
            {
                var newConn = new EmployeeShift(Employee, shift!);
                EmpShifts.Add(newConn);
            }
        }

        public void ConfirmShiftAdjustments()
        {
            if (Helios is null || Employee is null) return;

            Helios.StaffUpdater.EmployeeToShiftConnections(EmpShifts);

            if (Employee.DefaultShift != Shifts[SelectedShiftName])
            {
                var shift = Shifts[SelectedShiftName];
                Employee.DefaultShift = shift;
                Employee.DefaultShiftID = shift?.ID ?? "";
                Helios.StaffUpdater.Employee(Employee);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
