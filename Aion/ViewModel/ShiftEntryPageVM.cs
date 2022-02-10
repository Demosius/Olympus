using Aion.View;
using Aion.ViewModel.Commands;
using Aion.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using Ookii.Dialogs.Wpf;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public class ShiftEntryPageVM : INotifyPropertyChanged, IDBInteraction, IFilters
    {
        public Helios Helios { get; set; }
        public Charon Charon { get; set; }

        private DateTime minDate;
        public DateTime MinDate
        {
            get => minDate;
            set
            {
                minDate = value;
                OnPropertyChanged(nameof(MinDate));
            }
        }

        private DateTime maxDate;
        public DateTime MaxDate
        {
            get => maxDate;
            set
            {
                maxDate = value;
                OnPropertyChanged(nameof(MaxDate));
            }
        }

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

        private ObservableCollection<string> locations;
        public ObservableCollection<string> Locations
        {
            get => locations;
            set
            {
                locations = value;
                OnPropertyChanged(nameof(Locations));
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
        private List<ShiftEntry> DeletedEntries { get; set; }

        private ObservableCollection<ShiftEntry> entries;
        public ObservableCollection<ShiftEntry> Entries
        {
            get => entries;
            set
            {
                entries = value;
                OnPropertyChanged(nameof(Entries));
            }
        }

        private ShiftEntry selectedEntry;
        public ShiftEntry SelectedEntry
        {
            get => selectedEntry;
            set
            {
                selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
                SetClocks();
            }
        }

        private int missingEntryCount;
        public int MissingEntryCount
        {
            get => missingEntryCount;
            set
            {
                missingEntryCount = value;
                OnPropertyChanged(nameof(MissingEntryCount));
            }
        }

        private Dictionary<(int, string), List<ClockEvent>> fullClockDictionary;
        public Dictionary<(int, string), List<ClockEvent>> FullClockDictionary
        {
            get => fullClockDictionary;
            set
            {
                fullClockDictionary = value;
                OnPropertyChanged(nameof(FullClockDictionary));
            }
        }

        private List<ClockEvent> DeletedClocks { get; set; }

        private ObservableCollection<ClockEvent> clocks;
        public ObservableCollection<ClockEvent> Clocks
        {
            get => clocks;
            set
            {
                clocks = value;
                OnPropertyChanged(nameof(Clocks));
            }
        }

        private ClockEvent selectedClock;
        public ClockEvent SelectedClock
        {
            get => selectedClock;
            set
            {
                selectedClock = value;
                OnPropertyChanged(nameof(selectedClock));
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (value.Date > maxDate) value = maxDate;
                if (value.Date < minDate) value = minDate;
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
                if (value.Date > maxDate) value = maxDate;
                if (value.Date < minDate) value = minDate;
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
        public ApplyClocksCommand ApplyClocksCommand { get; set; }
        public DeleteClockCommand DeleteClockCommand { get; set; }
        public DeleteShiftCommand DeleteShiftCommand { get; set; }
        public ExportEntriesToCSVCommand ExportEntriesToCSVCommand { get; set; }
        public LaunchShiftCreatorCommand LaunchShiftCreatorCommand { get; set; }
        public ReSummarizeEntriesCommand ReSummarizeEntriesCommand { get; set; }
        public ReSummarizeEntryCommand ReSummarizeEntryCommand { get; set; }
        public CreateNewClockCommand CreateNewClockCommand { get; set; }
        public LaunchDateRangeCommand LaunchDateRangeCommand { get; set; }
        public ClearFiltersCommand ClearFiltersCommand { get; set; }
        public ApplyFiltersCommand ApplyFiltersCommand { get; set; }

        public ShiftEntryPageVM()
        {
            minDate = DateTime.Now.AddDays(-((int)DateTime.Now.DayOfWeek - 1) - 7 - ((int)DateTime.Now.DayOfWeek <= 1 ? 7 : 0));
            maxDate = DateTime.Now.AddDays(-1).Date;

            Entries = new();

            // Commands
            RefreshDataCommand = new(this);
            SaveEntryChangesCommand = new(this);
            DeleteSelectedShiftsCommand = new(this);
            ApplyClocksCommand = new(this);
            DeleteClockCommand = new(this);
            DeleteShiftCommand = new(this);
            ExportEntriesToCSVCommand = new(this);
            LaunchShiftCreatorCommand = new(this);
            ReSummarizeEntriesCommand = new(this);
            ReSummarizeEntryCommand = new(this);
            CreateNewClockCommand = new(this);
            LaunchDateRangeCommand = new(this);
            ClearFiltersCommand = new(this);
            ApplyFiltersCommand = new(this);
        }

        public bool CheckDateChange()
        {
            var result = MessageBox.Show("Changing the working date range will reset the data.\n\n" +
                            "Would you like to save your changes before you continue.",
                "Caution: Data Reset", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                SaveEntryChanges();

            return result != MessageBoxResult.Cancel;
        }

        public void SetDataSources(Helios helios, Charon charon)
        {
            Helios = helios;
            Charon = charon;
            Manager = charon.UserEmployee;
            var temp = Helios.StaffReader.GetManagedEmployees(Manager.ID).ToList();
            selectedEmployee = new() { FirstName = "<- None", LastName = "Selected ->", ID = -1 };
            OnPropertyChanged(nameof(SelectedEmployee));
            temp.Insert(0, selectedEmployee);
            Employees = new(temp);

            Task.Run(SetEntries);
        }

        /// <summary>
        /// Sets the entries according to the _manager.
        /// </summary>
        private void SetEntries()
        {
            startDate = minDate;
            endDate = maxDate;

            Task.Run(() =>
            {
                FullClockDictionary = Helios.StaffReader.ClockEvents(startDate, endDate)
                    .Where(c => c.Status != EClockStatus.Deleted)
                    .GroupBy(c => (c.EmployeeID, c.Date))
                    .ToDictionary(g => g.Key, g => g.ToList());
            });

            FullEntries = Helios.StaffReader.GetFilteredEntries(StartDate, EndDate, Manager.ID);

            Locations = new(FullEntries.Select(e => e.Location).Distinct().Where(e => e is not null or ""));

            DeletedEntries = new();
            DeletedClocks = new();

            CheckForMissingShifts();

            ApplyFilters();
        }
        
        public void ClearFilters()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies the listed filters for start and end dates, and the selected employee, if applicable.
        /// </summary>
        public void ApplyFilters()
        {
            if (Manager is null) return;
            var startString = startDate.ToString("yyyy-MM-dd");
            var endString = endDate.ToString("yyyy-MM-dd");
            if ((SelectedEmployee?.ID ?? -1) != -1)
            {
                Entries = new
                (
                    FullEntries.Where(e => e.EmployeeID == SelectedEmployee.ID && string.CompareOrdinal(e.Date, startString) >= 0 && string.CompareOrdinal(e.Date, endString) <= 0)
                );
            }
            else
            {
                Entries = new
                (
                    FullEntries.Where(e => string.CompareOrdinal(e.Date, startString) >= 0 && string.CompareOrdinal(e.Date, endString) <= 0)
                );
            }
            SetExportString();
            ApplySorting();
        }

        /// <summary>
        /// Converts all the clock times for Employees into Daily Entries, updating the database in the process.
        /// Will then refresh the data.
        /// </summary>
        public void ApplyPendingClocks()
        {
            var newEntry = false;

            Dictionary<(int, string), ShiftEntry> entryDict =
                FullEntries.ToDictionary(e => (e.EmployeeID, e.Date), e => e);

            var employeeDict = Employees.ToDictionary(e => e.ID, e => e);

            foreach (var (_, value) in FullClockDictionary.Where(dictEntry => !entryDict.ContainsKey(dictEntry.Key)))
            {
                if (!employeeDict.TryGetValue(value.First().EmployeeID, out var employee))
                    employee = new(value.First().EmployeeID);

                FullEntries.Add(new(employee, value));
                newEntry = true;
            }

            if (newEntry) ApplyFilters();
        }

        /// <summary>
        /// Removes daily entries offered from an external source.
        /// These should represent entries deleted from the database.
        /// </summary>
        /// <param name="entryList">A list of the base daily entries that are to be removed.</param>
        public void RemoveEntries(List<ShiftEntry> entryList)
        {
            foreach (var entry in entryList)
            {
                FullEntries.Remove(entry);
                Entries.Remove(entry);
                DeletedEntries.Remove(entry);
            }
        }

        /// <summary>
        /// Adds new shift entries from an external source.
        /// These should represent entries that have been added to the database.
        /// </summary>
        /// <param name="entryList">List of new export entries - which should each reference a daily entry.</param>
        public void AddEntries(List<ShiftEntry> entryList)
        {
            foreach (var entry in entryList)
            {
                FullEntries.Add(entry);
                Entries.Add(entry);
            }
        }

        /// <summary>
        /// Applies the selected sorting to the entry list.
        /// (Temporarily applies simple single sorting style.)
        /// </summary>
        public void ApplySorting()
        {
            Entries = new(Entries.OrderBy(e => e.Date).ThenBy(e => e.Employee.FullName));
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
            var uTask = Task.Run(() =>
            {
                Helios.StaffUpdater.EntriesAndClocks(FullEntries, FullClockDictionary.SelectMany(d => d.Value));
            });
            var dTask = Task.Run(() =>
            {
                Helios.StaffDeleter.EntriesAndClocks(DeletedEntries, DeletedClocks);
            });

            Task.WaitAll(uTask, dTask);

            RefreshData(true);
        }

        public void DeleteSelectedShifts(List<ShiftEntry> selectedShifts)
        {
            foreach (var s in selectedShifts)
            {
                FullEntries.Remove(s);
                DeletedEntries.Add(s);
                Entries.Remove(s);
            }
        }

        /// <summary>
        /// Changes the observable clocks based on the Entry Selection.
        /// </summary>
        private void SetClocks()
        {
            if (selectedEntry is not null && FullClockDictionary.TryGetValue((selectedEntry.EmployeeID, SelectedEntry.Date), out var newClockList))
                Clocks = new(newClockList.Where(c => c.Status != EClockStatus.Deleted));
            else
                Clocks = new();
        }

        public void DeleteSelectedClock()
        {
            SelectedClock.Status = EClockStatus.Deleted;
            Clocks.Remove(SelectedClock);
            if (SelectedClock.Date != DateTime.Parse(SelectedClock.Timestamp).ToString("yyyy-MM-dd")) DeletedClocks.Add(SelectedClock);
        }

        public void DeleteShift()
        {
            if (SelectedEntry is null) return;

            // Set relevant clocks to pending.
            foreach (var clock in Clocks)
                clock.Status = EClockStatus.Pending;

            // Remove entry from list.
            Entries.Remove(SelectedEntry);
        }

        public void ExportEntriesAsCSV()
        {
            try
            {
                VistaFolderBrowserDialog folderBrowserDialog = new()
                {
                    Description = "Select Export Folder",
                    UseDescriptionForTitle = true
                };
                if (!folderBrowserDialog.ShowDialog() == true) return;

                var exportLocation = folderBrowserDialog.SelectedPath;
                var exportFileName = $"{ExportString}_[{DateTime.Now:yyyy-MM-dd-HHmm}].csv";
                var fullFilePath = Path.Combine(exportLocation, exportFileName);

                using (var writer = new StreamWriter(fullFilePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(Entries);
                }

                // Success.
                MessageBox.Show($"Successfully Exported to file:\n\n{fullFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                // Failure.
                MessageBox.Show($"Failed to export file.\n\n{ex}", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void LaunchShiftCreator()
        {
            EntryCreationWindow entryCreator = new(Helios, this);
            entryCreator.ShowDialog();
            ApplySorting();
        }

        public void ReSummarizeEntries()
        {
            Task.Run(() =>
            {
                foreach (var shiftEntry in Entries)
                {
                    if (FullClockDictionary.TryGetValue((shiftEntry.EmployeeID, shiftEntry.Date), out var clockEvents))
                    {
                        shiftEntry.ApplyClockTimes(clockEvents);
                    }
                }
            });
        }

        public void ReSummarizeEntry()
        {
            SelectedEntry?.ApplyClockTimes(Clocks is null ? new() : Clocks.ToList());
        }

        public void CreateNewClock()
        {
            if (SelectedEntry is null) return;

            Clocks.Add(new()
            {
                EmployeeID = SelectedEntry.EmployeeID,
                Employee = SelectedEntry.Employee,
                Date = SelectedEntry.Date,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Time = "00:00:00"
            });

            Clocks = new(Clocks.OrderBy(c => c.Time));
        }

        /// <summary>
        /// Checks for missing shift entries amongst the employees within the date range.
        /// Does not check beyond today - as they are expected to yet be created.
        /// </summary>
        public void CheckForMissingShifts()
        {
            var count = 0;

            var lastDate = endDate.Date > DateTime.Now.Date ? DateTime.Now.Date : endDate.Date;

            var entryDict = FullEntries.ToDictionary(e => (e.EmployeeID, e.Date), e => e);

            foreach (var employee in Employees)
            {
                if (employee.EmploymentType is not (EEmploymentType.FP or EEmploymentType.SA)) continue;
                var checkDate = startDate;
                while (checkDate.Date <= lastDate.Date)
                {
                    checkDate = checkDate.AddDays(1);
                    if (checkDate.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday) continue;
                    if (!entryDict.ContainsKey((employee.ID, checkDate.ToString("yyyy-MM-dd")))) ++count;
                }
            }

            MissingEntryCount = count;
        }

        /// <summary>
        /// Finds missing shift entry data, and generates the appropriate shifts.
        /// </summary>
        public void CreateMissingShifts()
        {
            InputWindow inputWindow = new("Enter a common comment to apply to newly created shifts:",
                "New Shift Comments");

            if (inputWindow.ShowDialog() != true) return;

            var comment = inputWindow.Input.Text;

            var newEntries = false;

            var lastDate = endDate.Date > DateTime.Now.Date ? DateTime.Now.Date : endDate.Date;

            var entryDict = FullEntries.ToDictionary(e => (e.EmployeeID, e.Date), e => e);

            foreach (var employee in Employees)
            {
                if (employee.EmploymentType is not (EEmploymentType.FP or EEmploymentType.SA)) continue;
                var checkDate = startDate;
                while (checkDate.Date <= lastDate.Date)
                {
                    checkDate = checkDate.AddDays(1);
                    if (checkDate.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday) continue;
                    if (entryDict.ContainsKey((employee.ID, checkDate.ToString("yyyy-MM-dd")))) continue;

                    if (FullClockDictionary.TryGetValue((employee.ID, checkDate.ToString("yyyy-MM-dd")), out var clockEvents))
                        FullEntries.Add(new(employee, clockEvents) { Comments = comment });
                    FullEntries.Add(new(employee, checkDate) { Comments = comment });

                    newEntries = true;
                }
            }

            if (newEntries) ApplyFilters();
            MissingEntryCount = 0;  // Assume it has worked and there are no more missing shifts.
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public void LaunchDateRangeWindow()
        {
            if (!CheckDateChange()) return;

            DateRangeWindow datePicker = new(this);

            datePicker.ShowDialog();
        }
    }
}
