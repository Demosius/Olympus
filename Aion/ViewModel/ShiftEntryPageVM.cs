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
        public ApplyClocksCommand ApplyClocksCommand { get; set; }
        public DeleteClockCommand DeleteClockCommand { get; set; }
        public DeleteShiftCommand DeleteShiftCommand { get; set; }
        public ExportEntriesToCSVCommand ExportEntriesToCSVCommand { get; set; }
        public LaunchShiftCreatorCommand LaunchShiftCreatorCommand { get; set; }
        public ReSummarizeEntriesCommand ReSummarizeEntriesCommand { get; set; }
        public ReSummarizeEntryCommand ReSummarizeEntryCommand { get; set; }
        public CreateNewClockCommand CreateNewClockCommand { get; set; }

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
            ApplyClocksCommand = new(this);
            DeleteClockCommand = new(this);
            DeleteShiftCommand = new(this);
            ExportEntriesToCSVCommand = new(this);
            LaunchShiftCreatorCommand = new(this);
            ReSummarizeEntriesCommand = new(this);
            ReSummarizeEntryCommand = new(this);
            CreateNewClockCommand = new(this);
        }

        public void SetDataSources(Helios helios, Charon charon)
        {
            Helios = helios;
            Charon = charon;
            Manager = charon.UserEmployee;
            var temp = Helios.StaffReader.GetManagedEmployees(Manager).ToList();
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
            Task.Run(() =>
            {
                FullClockDictionary = Helios.StaffReader.ClockEvents(startDate, endDate)
                    .GroupBy(c => (c.EmployeeID, c.Date))
                    .ToDictionary(g => g.Key, g => g.ToList());
            });

            FullEntries = Helios.StaffReader.GetFilteredEntries(StartDate, EndDate, Manager.ID);
            
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
        /// </summary>
        public void ApplyPendingClocks()
        {
            Helios.StaffUpdater.ApplyPendingClockEvents();
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
                Helios.StaffDeleter.ShiftEntries(DeletedEntries);
            });

            Task.WaitAll(uTask, dTask);

            RefreshData(true);
        }

        public void LaunchSimpleShiftCreator()
        {
            SimpleEntryCreationWindow window = new(Helios, this);
            window.ShowDialog();
            ApplySorting();
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
            if (FullClockDictionary.TryGetValue((selectedEntry.EmployeeID, SelectedEntry.Date), out var newClockList))
                Clocks = new(newClockList.Where(c => c.Status != EClockStatus.Deleted));
            Clocks = new();
        }
        
        public void DeleteSelectedClock()
        {
            SelectedClock.Status = EClockStatus.Deleted;
            Clocks.Remove(SelectedClock);
        }

        public void DeleteShift()
        {
            throw new NotImplementedException();
        }

        public void ExportEntriesAsCSV()
        {
            throw new NotImplementedException();
        }
        
        public void LaunchShiftCreator()
        {
            throw new NotImplementedException();
        }

        public void ReSummarizeEntries()
        {
            throw new NotImplementedException();
        }

        public void ReSummarizeEntry()
        {
            throw new NotImplementedException();
        }

        public void CreateNewClock()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

    }
}
