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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Aion.Annotations;
using CsvHelper;
using Ookii.Dialogs.Wpf;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public enum EEntrySortOption
    {
        EmployeeDate,
        DateEmployee,
        DepartmentDateEmployee,
        DayDateEmployee
    }

    public class ShiftEntryPageVM : INotifyPropertyChanged, IDBInteraction, IFilters, IDateRange
    {
        public Helios Helios { get; set; }
        public Charon Charon { get; set; }
        public List<Employee> Employees { get; set; }

        private EEntrySortOption sortOption;
        public EEntrySortOption SortOption
        {
            get => sortOption;
            set
            {
                sortOption = value;
                OnPropertyChanged(nameof(SortOption));
            }
        }

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

        private List<Day> days;
        public List<Day> Days
        {
            get => days;
            set
            {
                days = value;
                OnPropertyChanged(nameof(Days));
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

        public List<ShiftEntry> FullEntries { get; set; }
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

        private int pendingClockCount;
        public int PendingClockCount
        {
            get => pendingClockCount;
            set
            {
                pendingClockCount = value;
                OnPropertyChanged(nameof(PendingClockCount));
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
            }
        }

        public string ExportString { get; set; }

        /* Filtering Strings */
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

        private string commentSearchString;

        public string CommentSearchString
        {
            get => commentSearchString;
            set
            {
                commentSearchString = value;
                OnPropertyChanged(nameof(CommentSearchString));
            }
        }

        private string daySearchString;
        public string DaySearchString
        {
            get => daySearchString;
            set
            {
                daySearchString = value;
                OnPropertyChanged(nameof(DaySearchString));
            }
        }

        /* Commands */
        public RefreshDataCommand RefreshDataCommand { get; set; }
        public SaveEntryChangesCommand SaveEntryChangesCommand { get; set; }
        public DeleteSelectedShiftsCommand DeleteSelectedShiftsCommand { get; set; }
        public ApplyClocksCommand ApplyClocksCommand { get; set; }
        public DeleteClockCommand DeleteClockCommand { get; set; }
        public ExportEntriesToCSVCommand ExportEntriesToCSVCommand { get; set; }
        public LaunchShiftCreatorCommand LaunchShiftCreatorCommand { get; set; }
        public ReSummarizeEntriesCommand ReSummarizeEntriesCommand { get; set; }
        public ReSummarizeEntryCommand ReSummarizeEntryCommand { get; set; }
        public CreateNewClockCommand CreateNewClockCommand { get; set; }
        public LaunchDateRangeCommand LaunchDateRangeCommand { get; set; }
        public ClearFiltersCommand ClearFiltersCommand { get; set; }
        public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
        public CreateMissingShiftsCommand CreateMissingShiftsCommand { get; set; }
        public ApplySortingCommand ApplySortingCommand { get; set; }
        public RepairDataCommand RepairDataCommand { get; set; }

        public ShiftEntryPageVM()
        {
            minDate = DateTime.Now.AddDays(-((int)DateTime.Now.DayOfWeek - 1) - 7 - ((int)DateTime.Now.DayOfWeek <= 1 ? 7 : 0)).Date;
            maxDate = DateTime.Now.AddDays(-1).Date;
            sortOption = EEntrySortOption.EmployeeDate;

            Entries = new();

            // Commands
            RefreshDataCommand = new(this);
            SaveEntryChangesCommand = new(this);
            DeleteSelectedShiftsCommand = new(this);
            ApplyClocksCommand = new(this);
            DeleteClockCommand = new(this);
            ExportEntriesToCSVCommand = new(this);
            LaunchShiftCreatorCommand = new(this);
            ReSummarizeEntriesCommand = new(this);
            ReSummarizeEntryCommand = new(this);
            CreateNewClockCommand = new(this);
            LaunchDateRangeCommand = new(this);
            ClearFiltersCommand = new(this);
            ApplyFiltersCommand = new(this);
            CreateMissingShiftsCommand = new(this);
            ApplySortingCommand = new(this);
            RepairDataCommand = new(this);
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
            Employees = Helios.StaffReader.GetManagedEmployees(Manager.ID).ToList();

            Task.Run(SetEntries);
        }

        /// <summary>
        /// Sets the entries according to the _manager.
        /// </summary>
        private void SetEntries()
        {
            StartDate = minDate;
            EndDate = maxDate;

            var dTask = Task.Run(() =>
            {
                FullClockDictionary = Helios.StaffReader.ClockEvents(Employees.Select(e => e.ID), startDate, endDate)
                    .Where(c => c.Status != EClockStatus.Deleted)
                    .GroupBy(c => (c.EmployeeID, c.Date))
                    .ToDictionary(g => g.Key, g => g.ToList());
            });

            FullEntries = Helios.StaffReader.GetFilteredEntries(StartDate, EndDate, Manager.ID);

            Locations = new(FullEntries.Select(e => e.Location).Distinct().Where(e => e is not null or ""));

            Days = new()
            {
                new(DayOfWeek.Sunday),
                new(DayOfWeek.Monday),
                new(DayOfWeek.Tuesday),
                new(DayOfWeek.Wednesday),
                new(DayOfWeek.Thursday),
                new(DayOfWeek.Friday),
                new(DayOfWeek.Saturday)
            };

            DeletedEntries = new();
            DeletedClocks = new();

            // Check for duplicate entries.
            if (CheckForDuplicateEntries()) return;

            dTask.Wait();

            CheckData();
            ApplyFilters();
        }

        /// <summary>
        /// Checks for duplicate (date-employee) entries.
        /// Will offer to run a repair if there is.
        /// </summary>
        /// <returns>True if there is still duplicates by the end of the function.</returns>
        private bool CheckForDuplicateEntries()
        {
            if (!FullEntries.GroupBy(x => new { x.Date, x.EmployeeID }).Any(x => x.Skip(1).Any()))
                return false;

            var result = MessageBox.Show(
                "There are duplicate values in the Shift Entries. This will need to be rectified before we can continue.\n\n" +
                "Would you like to run a data repair now?\n\n" +
                "(It may be possible to continue with a different date range, depending on the extent of the fault.)", "Duplicate Fault Found",
                MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result != MessageBoxResult.Yes) return true;

            var linesAffected = Helios.StaffUpdater.RepairAionData();

            MessageBox.Show($"The repair process affected {linesAffected} rows of data.", "Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);

            FullEntries = new(Helios.StaffReader.GetFilteredEntries(StartDate, EndDate, Manager.ID));


            if (!FullEntries.GroupBy(x => new { x.Date, x.EmployeeID }).Any(x => x.Skip(1).Any()))
                return false;

            MessageBox.Show(
                "Duplicates still found after repair. Please Aion Development or Database managers to rectify this issue.\n\n" +
                "(It may be possible to continue with a different date range, depending on the extent of the fault.)");
            return true;

        }

        public void ClearFilters()
        {
            EmployeeSearchString = "";
            DepartmentSearchString = "";
            DaySearchString = "";
            CommentSearchString = "";
            StartDate = MinDate;
            EndDate = MaxDate;
            ApplyFilters();
        }

        /// <summary>
        /// Applies the listed filters for start and end dates, and the selected employee, if applicable.
        /// </summary>
        public void ApplyFilters()
        {
            if (Manager is null) return;

            IEnumerable<ShiftEntry> shiftEntries = FullEntries;

            try
            {
                FilterEmployee(ref shiftEntries);
                FilterDepartment(ref shiftEntries);
                // ReSharper disable once PossibleMultipleEnumeration
                FilterComment(ref shiftEntries);
            }
            catch (RegexParseException ex)
            {
                MessageBox.Show("Issue with pattern matching in filters:\n\n" +
                                $"{ex.Message}", "RegEx Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            FilterDay(ref shiftEntries);
            FilterDate(ref shiftEntries);
            
            ApplySorting(shiftEntries);

            SetExportString();
        }

        private void FilterEmployee(ref IEnumerable<ShiftEntry> shiftEntries)
        {
            if ((employeeSearchString ?? "") == "") return;

            Regex rex = new(employeeSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            shiftEntries = shiftEntries?.Where(s => rex.IsMatch(s.EmployeeName ?? "") || rex.IsMatch(s.EmployeeID.ToString()));
        }

        private void FilterDepartment(ref IEnumerable<ShiftEntry> shiftEntries)
        {
            if ((departmentSearchString ?? "") == "") return;

            Regex rex = new(departmentSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            shiftEntries = shiftEntries?.Where(s => rex.IsMatch(s.Department ?? ""));
        }

        private void FilterComment(ref IEnumerable<ShiftEntry> shiftEntries)
        {
            if ((commentSearchString ?? "") == "") return;

            Regex rex = new(commentSearchString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            shiftEntries = shiftEntries?.Where(s => rex.IsMatch(s.Comments ?? ""));
        }

        private void FilterDay(ref IEnumerable<ShiftEntry> shiftEntries)
        {
            shiftEntries = shiftEntries?.Where(e => Days.Where(d => d.InUse).Select(d => d.DayOfWeek).Contains(e.Day));
        }

        private void FilterDate(ref IEnumerable<ShiftEntry> shiftEntries)
        {
            var startString = startDate.ToString("yyyy-MM-dd");
            var endString = endDate.ToString("yyyy-MM-dd");

            shiftEntries = shiftEntries?.Where(e =>
                string.CompareOrdinal(e.Date, startString) >= 0 && string.CompareOrdinal(e.Date, endString) <= 0);
        }

        /// <summary>
        /// Converts all the clock times for Employees into Daily Entries.
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

            if (!newEntry) return;

            ApplyFilters();
            CheckData();
        }

        /// <summary>
        /// Removes given list of daily entries - which may represent an internal list, or sourced externally.
        /// </summary>
        /// <param name="entryList">A list of the base daily entries that are to be removed.</param>
        public void RemoveEntries(List<ShiftEntry> entryList)
        {
            FullEntries.RemoveAll(e => entryList.Select(s => s.ID).Contains(e.ID));
            DeletedEntries.AddRange(entryList.Where(s => !DeletedEntries.Select(d => d.ID).Contains(s.ID)));
            // Set associated clocks to pending.
            foreach (var shiftEntry in entryList)
            {
                if (!FullClockDictionary.TryGetValue((shiftEntry.EmployeeID, shiftEntry.Date), out var clockEvents)) continue;

                foreach (var clockEvent in clockEvents)
                {
                    clockEvent.Status = clockEvent.Status != EClockStatus.Deleted
                        ? EClockStatus.Pending
                        : EClockStatus.Deleted;
                }
            }
            ApplyFilters();
            CheckData();
        }

        /// <summary>
        /// Adds new shift entries from an external source.
        /// </summary>
        /// <param name="entryList">List of new export entries - which should each reference a daily entry.</param>
        public void AddEntries(List<ShiftEntry> entryList)
        {
            FullEntries.AddRange(entryList.Where(s => !FullEntries.Select(f => f.ID).Contains(s.ID)));
            ApplyFilters();
            CheckData();
        }

        /// <summary>
        /// Applies the selected sorting to the entry list.
        /// </summary>
        public void ApplySorting()
        {
            ApplySorting(Entries);
        }

        /// <summary>
        /// Applies the selected sorting to the given shift list.
        /// </summary>
        public void ApplySorting(IEnumerable<ShiftEntry> shiftEntries)
        {
            var shifts = shiftEntries as ShiftEntry[] ?? shiftEntries.ToArray();
            if (!shifts.Any())
            {
                Entries = new();
                return;
            }
            Entries = SortOption switch
            {
                EEntrySortOption.DateEmployee =>
                    new(shifts.OrderBy(e => e.Date).ThenBy(e => e.EmployeeName)),
                EEntrySortOption.DayDateEmployee =>
                    new(shifts.OrderBy(e => e.Day).ThenBy(e => e.Date).ThenBy(e => e.EmployeeName)),
                EEntrySortOption.DepartmentDateEmployee =>
                    new(shifts.OrderBy(e => e.Department).ThenBy(e => e.Date).ThenBy(e => e.EmployeeName)),
                EEntrySortOption.EmployeeDate =>
                    new(shifts.OrderBy(e => e.EmployeeName).ThenBy(e => e.Date)),
                _ => new(shifts)
            };
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

        /// <summary>
        /// Repairs data in the database, checking for know potential issues, such as duplicate data,
        /// or missing fields in tables.
        /// </summary>
        /// <returns>The number of rows of data affected by the repairs.</returns>
        public void RepairData()
        {
            if (MessageBox.Show(
                    "This action will reset the data, and undo any unsaved changes that have been made.\n\nDo you want to continue?",
                    "Refresh Data?", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            var linesAffected = Helios.StaffUpdater.RepairAionData();
            RefreshData(true);
            MessageBox.Show($"The repair process affected {linesAffected} rows of data.", "Complete",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SaveEntryChanges()
        {
            Helios.StaffUpdater.EntriesAndClocks(FullEntries, FullClockDictionary.SelectMany(d => d.Value));
            Helios.StaffDeleter.EntriesAndClocks(DeletedEntries, DeletedClocks);

            RefreshData(true);
        }

        public void DeleteSelectedShifts(List<ShiftEntry> selectedShifts)
        {
            RemoveEntries(selectedShifts);
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
            // We will only delete the clock event from the DB if the timestamp does not match the date - denoting that it was not created by a physical clock event (but through the Aion Manager instead).
            // (In practice, actual clock events should remain recorded, for posterity.)
            if (SelectedClock.Date != DateTime.Parse(SelectedClock.Timestamp).ToString("yyyy-MM-dd")) DeletedClocks.Add(SelectedClock);
            Clocks.Remove(SelectedClock);
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
            entryCreator.Show();
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

            FullClockDictionary[(SelectedEntry.EmployeeID, selectedEntry.Date)] = new(Clocks);
        }

        /// <summary>
        /// Checks data for missing shifts, pending clocks, etc.
        /// </summary>
        public void CheckData()
        {
            CheckForMissingShifts();
            CheckForPendingClocks();
        }

        public void CheckForPendingClocks()
        {
            // Set any clocks with no shifts assigned to be pending.
            Dictionary<(int, string), ShiftEntry> entryDict =
                FullEntries.ToDictionary(e => (e.EmployeeID, e.Date), e => e);

            foreach (var (_, clockEvents) in FullClockDictionary.Where(dictEntry => !entryDict.ContainsKey(dictEntry.Key)))
            {
                foreach (var clockEvent in clockEvents)
                {
                    clockEvent.Status = clockEvent.Status == EClockStatus.Deleted
                        ? EClockStatus.Deleted
                        : EClockStatus.Pending;
                }
            }

            // Set the count based on total clocks with pending status.
            PendingClockCount = (int) FullClockDictionary?.SelectMany(d => d.Value).Count(c => c.Status == EClockStatus.Pending);
        }

        /// <summary>
        /// Checks for missing shift entries amongst the employees within the date range.
        /// Does not check beyond today - as they are expected to yet be created.
        /// </summary>
        public void CheckForMissingShifts()
        {
            var count = 0;

            var lastDate = endDate.Date > DateTime.Now.Date ? DateTime.Now.Date : endDate.Date;
            Dictionary<(int, string), ShiftEntry> entryDict;

            try
            {
                entryDict = FullEntries.ToDictionary(e => (e.EmployeeID, e.Date), e => e);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    $"There appears to be an issue with the data:\n\n{ex.Message}\n\nA common cause is duplicate shift entries.\nPlease run a repair when possible.",
                    "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MissingEntryCount = -1;
                return;
            }


            foreach (var employee in Employees)
            {
                if (employee.EmploymentType is not (EEmploymentType.FP or EEmploymentType.SA)) continue;
                var checkDate = startDate;
                while (checkDate.Date <= lastDate.Date)
                {
                    if (checkDate.DayOfWeek is not (DayOfWeek.Sunday or DayOfWeek.Saturday))
                        if (!entryDict.ContainsKey((employee.ID, checkDate.ToString("yyyy-MM-dd")))) ++count;
                    checkDate = checkDate.AddDays(1);
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
                    if (checkDate.DayOfWeek is not (DayOfWeek.Sunday or DayOfWeek.Saturday))
                    {
                        if (!entryDict.ContainsKey((employee.ID, checkDate.ToString("yyyy-MM-dd"))))
                        {
                            if (FullClockDictionary.TryGetValue((employee.ID, checkDate.ToString("yyyy-MM-dd")),
                                    out var clockEvents))
                                FullEntries.Add(new(employee, clockEvents) { Comments = comment });
                            else
                                FullEntries.Add(new(employee, checkDate) { Comments = comment });

                            newEntries = true;
                        }
                    }
                    checkDate = checkDate.AddDays(1);
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

    public class Day : INotifyPropertyChanged
    {
        private DayOfWeek dayOfWeek;
        public DayOfWeek DayOfWeek
        {
            get => dayOfWeek;
            set
            {
                dayOfWeek = value;
                OnPropertyChanged(nameof(DayOfWeek));
            }
        }

        private bool inUse;
        public bool InUse
        {
            get => inUse;
            set
            {
                inUse = value;
                OnPropertyChanged(nameof(InUse));
            }
        }

        public Day()
        {
            DayOfWeek = DayOfWeek.Sunday;
            InUse = true;
        }

        public Day(DayOfWeek weekDay)
        {
            DayOfWeek = weekDay;
            InUse = true;
        }

        public Day(DayOfWeek weekDay, bool isInUse)
        {
            DayOfWeek = weekDay;
            inUse = isInUse;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
