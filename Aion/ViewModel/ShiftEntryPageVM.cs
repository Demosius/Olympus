using Aion.View;
using Aion.ViewModel.Commands;
using Aion.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Styx;
using Uranus;
using ExportEntry = Aion.Model.ExportEntry;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public class ShiftEntryPageVM : INotifyPropertyChanged, IDBInteraction
    {
        public Helios Helios { get; set; }
        public Charon Charon { get; set; }

        private readonly DateTime minDate = DateTime.Now.AddMonths(-2).Date;
        private readonly DateTime maxDate = DateTime.Now.AddYears(1).Date;

        private Employee manager;
        public Employee Manager
        {
            get => manager;
            set
            {
                manager = value;
                OnPropertyChanged(nameof(Manager));
            }
        }

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
                ApplyFilters();
            }
        }

        private List<ShiftEntry> FullEntries { get; set; }
        private List<ExportEntry> EditableEntries { get; set; }
        private List<ExportEntry> DeletedEntries { get; set; }

        private ObservableCollection<ExportEntry> entries;
        public ObservableCollection<ExportEntry> Entries
        {
            get => entries;
            set
            {
                entries = value;
                OnPropertyChanged(nameof(Entries));
            }
        }

        private ExportEntry selectedEntry;
        public ExportEntry SelectedEntry
        {
            get => selectedEntry;
            set
            {
                selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (value > maxDate) value = maxDate;
                if (value < minDate) value = minDate;
                startDate = value;
                if (startDate > endDate)
                {
                    endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
                OnPropertyChanged(nameof(StartDate));
                ApplyFilters();
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get => endDate;
            set
            {
                if (value > maxDate) value = maxDate;
                if (value < minDate) value = minDate;
                endDate = value;
                if (endDate < startDate)
                {
                    startDate = endDate;
                    OnPropertyChanged(nameof(StartDate));
                }
                OnPropertyChanged(nameof(EndDate));
                ApplyFilters();
            }
        }

        public string ExportString { get; set; }

        /* Commands */
        public RefreshDataCommand RefreshDataCommand { get; set; }
        public SaveEntryChangesCommand SaveEntryChangesCommand { get; set; }
        public DeleteSelectedShiftsCommand DeleteSelectedShiftsCommand { get; set; }
        public LaunchSimpleShiftCreatorCommand LaunchSimpleShiftCreatorCommand { get; set; }

        public ShiftEntryPageVM()
        {
            startDate = DateTime.Now.AddDays(-((int)DateTime.Now.DayOfWeek - 1) - 7 - ((int)DateTime.Now.DayOfWeek <= 1 ? 7 : 0));
            endDate = DateTime.Now.AddDays(-1).Date;

            Entries = new();
            // Commands
            RefreshDataCommand = new(this);
            SaveEntryChangesCommand = new(this);
            DeleteSelectedShiftsCommand = new(this);
            LaunchSimpleShiftCreatorCommand = new(this);
        }

        public void SetDataSources(Helios helios, Charon charon)
        {
            Helios = helios;
            Charon = charon;
            Manager = charon.UserEmployee;
            var temp = Helios.StaffReader.GetManagedEmployees(Manager).ToList();
            selectedEmployee = new() { FirstName = "<- None", LastName = "Selected ->", ID = -1 };
            temp.Insert(0, selectedEmployee);
            Employees = new(temp);

            Task.Run(SetEntries);
        }

        /// <summary>
        /// Sets the entries according to the _manager.
        /// </summary>
        private void SetEntries()
        {
            if (Manager is null) return;
            // Update with pending clocks first.
            UpdateEntries();

            // Get the required entries with applied clock times.
            FullEntries = Helios.StaffReader.GetFilteredEntries(minDate, maxDate, Manager.ID);

            // Get the rejected clock times.
            EditableEntries = new();
            DeletedEntries = new();

            ApplyFilters();
        }

        /// <summary>
        /// Applies the listed filters for start and end dates, and the selected employee, if applicable.
        /// </summary>
        private void ApplyFilters()
        {
            if (Manager is null) return;
            var startString = startDate.ToString("yyyy-MM-dd");
            var endString = endDate.ToString("yyyy-MM-dd");
            if ((SelectedEmployee?.ID ?? -1) != -1)
            {
                Entries = new
                (
                    EditableEntries.Where(e => e.AssociateNumber == SelectedEmployee.ID && string.CompareOrdinal(e.Date, startString) >= 0 && String.CompareOrdinal(e.Date, endString) <= 0)
                );
            }
            else
            {
                Entries = new
                (
                    EditableEntries.Where(e => String.CompareOrdinal(e.Date, startString) >= 0 && String.CompareOrdinal(e.Date, endString) <= 0)
                );
            }
            SetExportString();
            ApplySorting();
        }

        /// <summary>
        /// Converts all the clock times for Employees into Daily Entries, updating the database in the process.
        /// </summary>
        public void UpdateEntries()
        {
            // Get all pending clocks. Sort into employee groups.
            var clockList = Helios.StaffReader.GetPendingClocks(Manager.ID, minDate, maxDate).ToList();
            var pendingClocks = clockList.GroupBy(c => c.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());

            // Get list of relevant employees, and pull those from DB.
            var empCodes = pendingClocks.Select(e => e.Key).ToList();
            var employeeList = Helios.StaffReader.Employees(e => empCodes.Contains(e.ID));

            var validEntries = Helios.StaffReader.ShiftEntries(e => empCodes.Contains(e.EmployeeID))
                .GroupBy(e => e.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());

            // For keeping track of the adjusted entries.
            List<ShiftEntry> entryConfirmation = new();

            // Loop through employees and assign clocks and DEs before running their clock convert function.
            foreach (var employee in employeeList)
            {
                var employeeClockEvents = pendingClocks[employee.ID];
                _ = validEntries.TryGetValue(employee.ID, out var empEntries);
                employee.ShiftEntries = empEntries ?? new();
                ConvertClockToEntries(employee, employeeClockEvents);
                entryConfirmation = entryConfirmation.Concat(employee.ShiftEntries).ToList();
            }

            Helios.StaffUpdater.ShiftEntries(entryConfirmation);
            Helios.StaffUpdater.ClockEvents(clockList);
        }

        private static void ConvertClockToEntries(Employee employee, List<ClockEvent> clocks)
        {
            if (clocks is null || employee is null) return;
            var pendingClocks = clocks.Where(c => c.Status == EClockStatus.Pending)
                .GroupBy(c => c.DtDate.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key, g => g.ToList());
            if (pendingClocks.Count == 0) return;

            // Get existing Daily Entries in dictionary form for quick look up.
            var currentEntries = employee.ShiftEntries.ToDictionary(d => d.Date, d => d);

            foreach (var (key, value) in pendingClocks)
            {
                if (currentEntries.ContainsKey(key))
                {
                    currentEntries[key].ApplyClockTimes(value);
                }
                else
                {
                    employee.ShiftEntries.Add(new(employee, value));
                }
            }
        }

        /// <summary>
        /// Removes daily entries offered from an external source.
        /// These should represent entries deleted from the database.
        /// </summary>
        /// <param name="entryList">A list of the base daily entries that are to be removed.</param>
        public void RemoveEntries(List<ShiftEntry> entryList)
        {
            foreach (var daily in entryList)
            {
                FullEntries.Remove(daily);
                // Get the ExportEntry representing this daily.
                var export = EditableEntries.FirstOrDefault(e => e.ShiftEntry == daily);
                EditableEntries.Remove(export);
                Entries.Remove(export);
                DeletedEntries.Remove(export);
            }
        }

        /// <summary>
        /// Adds new shift entries from an external source.
        /// These should represent entries that have been added to the database.
        /// </summary>
        /// <param name="entryList">List of new export entries - which should each reference a daily entry.</param>
        public void AddEntries(List<ExportEntry> entryList)
        {
            foreach (var export in entryList)
            {
                FullEntries.Add(export.ShiftEntry);
                EditableEntries.Add(export);
                Entries.Add(export);
            }
        }

        /// <summary>
        /// Applies the selected sorting to the entry list.
        /// (Temporarily applies simple single sorting style.)
        /// </summary>
        public void ApplySorting()
        {
            Entries = new(Entries.OrderBy(e => e.Date).ThenBy(e => e.Name));
        }

        public void SetExportString()
        {
            ExportString = StartDate == EndDate
                ? $"{(Manager?.ID == -1 ? "All" : Manager?.ToString())}_{StartDate:ddMMMyyyy}"
                : $"{(Manager?.ID == -1 ? "All" : Manager?.ToString())}_{StartDate:ddMMMyyyy}-{EndDate:ddMMMyyyy}";
        }

        public void RefreshData()
        {
            RefreshData(false);
        }

        public void RefreshData(bool force)
        {
            if (force || MessageBox.Show("Refreshing will undo any unsaved changes you have made.\n" +
                "Would you like to continue?", "Confirm Refresh", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                Task.Run(SetEntries);
        }

        public void SaveEntryChanges()
        {
            foreach (var e in EditableEntries) { e.ConvertEntry(); }

            Helios.StaffUpdater.ShiftEntries(EditableEntries.Select(e => e.ShiftEntry));
            Helios.StaffDeleter.ShiftEntries(DeletedEntries.Select(e => e.ShiftEntry));

            RefreshData(true);
        }

        public void LaunchSimpleShiftCreator()
        {
            SimpleEntryCreationWindow window = new(Helios, this);
            window.ShowDialog();
            ApplySorting();
        }

        public void DeleteSelectedShifts(List<ExportEntry> selectedShifts)
        {
            foreach (var s in selectedShifts)
            {
                EditableEntries.Remove(s);
                DeletedEntries.Add(s);
                Entries.Remove(s);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

    }
}
