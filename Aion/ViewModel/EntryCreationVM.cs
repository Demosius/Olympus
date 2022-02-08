using ExportEntry = Aion.Model.ExportEntry;
using Aion.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel
{
    public class EntryCreationVM : INotifyPropertyChanged
    {
        public Helios Helios { get; set; }

        private List<ExportEntry> newEntries = new();
        private List<ExportEntry> deletedEntries = new();

        private DateTime MinDate { get; set; }
        private DateTime MaxDate { get; set; }

        public ShiftEntryPageVM EditorVM { get; set; }

        private ObservableCollection<Employee> employees;
        public ObservableCollection<Employee> Employees
        {
            get => employees;
            set
            {
                employees = value;
                OnPropertyChanged(nameof(Employees));
                SelectedEmployee = employees.FirstOrDefault();
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
                SetEntries();
            }
        }

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

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get => selectedDate;
            set
            {
                if (value < MinDate) value = MinDate;
                if (value > MaxDate) value = MaxDate;
                selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        private string startShiftTime;
        public string StartShiftTime
        {
            get => startShiftTime;
            set
            {
                startShiftTime = value;
                OnPropertyChanged(nameof(StartShiftTime));
            }
        }

        private string startLunchTime;
        public string StartLunchTime
        {
            get => startLunchTime;
            set
            {
                startLunchTime = value;
                OnPropertyChanged(nameof(StartLunchTime));
            }
        }

        private string endLunchTime;
        public string EndLunchTime
        {
            get => endLunchTime;
            set
            {
                endLunchTime = value;
                OnPropertyChanged(nameof(EndLunchTime));
            }
        }

        private string endShiftTime;
        public string EndShiftTime
        {
            get => endShiftTime;
            set
            {
                endShiftTime = value;
                OnPropertyChanged(nameof(EndShiftTime));
            }
        }

        private string comment;
        public string Comment
        {
            get => comment;
            set
            {
                comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        /* Commands */
        public CreateEntryCommand CreateEntryCommand { get; set; }
        public DeleteEntryCommand DeleteEntryCommand { get; set; }
        public ConfirmEntryCreationCommand ConfirmEntryCreationCommand { get; set; }

        public EntryCreationVM()
        {
            CreateEntryCommand = new(this);
            DeleteEntryCommand = new(this);
            ConfirmEntryCreationCommand = new(this);
        }

        public EntryCreationVM(ShiftEntryPageVM editorVM)
        {
            SetEditorSourceVM(editorVM);
        }

        public void SetDataSource(Helios helios)
        {
            Helios = helios;
        }

        public void SetEditorSourceVM(ShiftEntryPageVM editorVM)
        {
            EditorVM = editorVM;
            Employees = editorVM.Employees;
            MinDate = editorVM.StartDate;
            MaxDate = editorVM.EndDate;
            SelectedDate = MinDate;
            SetEntries();
        }

        public void SetData(Helios helios, ShiftEntryPageVM editorVM)
        {
            Helios = helios;
            SetEditorSourceVM(editorVM);
        }

        /// <summary>
        /// Creates the list of entries based on the selected employee.
        /// </summary>
        public void SetEntries()
        {
            Entries = new();
            if (SelectedEmployee is not null && SelectedEmployee.ID != -1)
            {
                var shiftEntries = Helios.StaffReader.ShiftEntries(entry => entry.EmployeeID == SelectedEmployee.ID 
                    && string.CompareOrdinal(entry.Date, MinDate.ToString("yyyy-MM-dd")) >= 0 
                    && string.CompareOrdinal(entry.Date, MaxDate.ToString("yyyy-MM-dd")) <= 0)
                    .OrderBy(e => e.Date); 

                Dictionary<(int, DateTime), List<ClockEvent>> clocks = Helios.StaffReader.ClockEvents()
                    .GroupBy(c => (c.EmployeeID, DTDate: c.DtDate))
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var entry in shiftEntries)
                {
                    if (clocks.TryGetValue((entry.EmployeeID, DateTime.Parse(entry.Date)), out var clockList))
                        entry.ApplyClockTimes(clockList);
                    ExportEntry export = new(new());
                    Entries.Add(export);
                }
            }
            newEntries = new();
            deletedEntries = new();
        }

        public void CreateEntry()
        {
            if (SelectedEmployee.ID == -1) return;

            // I there already exists an entry for this date, move to the next date that has no entry.
            if (entries.Any(e => e.ShiftEntry.Date == SelectedDate.ToString("yyyy-MM-dd")))
            {
                SelectedDate = SelectedDate.AddDays(1);
                while (entries.Any(e => e.ShiftEntry.Date == SelectedDate.ToString("yyyy-MM-dd")) && SelectedDate < MaxDate)
                    SelectedDate = SelectedDate.AddDays(1);
                return;
            }

            ShiftEntry newEntry = new(SelectedEmployee, SelectedDate);
            AddEntry(newEntry);
            Entries = new(Entries.OrderBy(e => e.Date));

            // Default to next valid date.
            while (entries.Any(e => e.ShiftEntry.Date == SelectedDate.ToString("yyyy-MM-dd")) && SelectedDate < MaxDate)
                SelectedDate = SelectedDate.AddDays(1);
        }

        public void AddEntry(ShiftEntry daily)
        {
            ExportEntry export = new(daily)
            {
                In = StartShiftTime,
                OutToLunch = StartLunchTime,
                InFromLunch = EndLunchTime,
                Out = EndShiftTime,
                Comments = Comment
            };

            newEntries.Add(export);
            Entries.Add(export);
        }

        public void DeleteSelectedEntry()
        {
            if (SelectedEntry is null) return;

            if (!newEntries.Remove(SelectedEntry))
                deletedEntries.Add(SelectedEntry);

            Entries.Remove(SelectedEntry);
        }

        /// <summary>
        /// Confirms the changes from the VM, including both the newly created entries, and the deleted entries.
        /// </summary>
        public void ConfirmAll()
        {
            // Add new ShiftEntries to the database and parent vm.
            /*DBUtil.Conn().RunInTransaction(() =>
            {
                using var conn = DBUtil.Conn();
                foreach (var e in newEntries) { conn.Insert(e.ShiftEntry); }
                EditorVM.AddEntries(newEntries);
                newEntries = new();
                // Remove Deleted Entries from the database and the parent (editor) VM.
                foreach (var e in deletedEntries)
                {
                    conn.Delete(e.ShiftEntry);
                    EditorVM.RemoveEntries(deletedEntries.Select(entry => entry.ShiftEntry).ToList());
                }
                deletedEntries = new();
            });*/
            _ = deletedEntries;
            deletedEntries = new();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
    }
}
