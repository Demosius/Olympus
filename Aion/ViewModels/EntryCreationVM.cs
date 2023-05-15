using Aion.View;
using Aion.ViewModels.Commands;
using Aion.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Aion.ViewModels;

public class EntryCreationVM : INotifyPropertyChanged, IDateRange
{
    public Helios Helios { get; set; }

    private List<ShiftEntry> newEntries = new();
    private List<ShiftEntry> deletedEntries = new();

    public ShiftEntryPageVM EditorVM { get; set; }
    
    public ObservableCollection<Employee> Employees { get; set; }

    private Employee? selectedEmployee;
    public Employee? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged(nameof(SelectedEmployee));
            SetEntries();
        }
    }
    
    public ObservableCollection<ShiftEntry> Entries { get; set; }

    private ShiftEntry? selectedEntry;
    public ShiftEntry? SelectedEntry
    {
        get => selectedEntry;
        set
        {
            selectedEntry = value;
            OnPropertyChanged(nameof(SelectedEntry));
        }
    }

    private DateTime? selectedDate;
    public DateTime? SelectedDate
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

    /* Commands */
    public CreateEntryCommand CreateEntryCommand { get; set; }
    public DeleteEntryCommand DeleteEntryCommand { get; set; }
    public ConfirmEntryCreationCommand ConfirmEntryCreationCommand { get; set; }
    public LaunchDateRangeCommand LaunchDateRangeCommand { get; set; }

    public EntryCreationVM(Helios helios, ShiftEntryPageVM editorVM)
    {
        Helios = helios;

        CreateEntryCommand = new CreateEntryCommand(this);
        DeleteEntryCommand = new DeleteEntryCommand(this);
        ConfirmEntryCreationCommand = new ConfirmEntryCreationCommand(this);
        LaunchDateRangeCommand = new LaunchDateRangeCommand(this);
        SelectedEmployee = null;

        Employees = new ObservableCollection<Employee>();
        Entries = new ObservableCollection<ShiftEntry>();
        startShiftTime = string.Empty;
        startLunchTime = string.Empty;
        endLunchTime = string.Empty;
        endShiftTime = string.Empty;
        comment = string.Empty;

        EditorVM = editorVM;
        MinDate = editorVM.MinDate;
        MaxDate = editorVM.MaxDate;
        Employees = new ObservableCollection<Employee>(editorVM.Employees);
        SelectedDate = MinDate;
        SetEntries();
    }

    /// <summary>
    /// Creates the list of entries based on the selected employee.
    /// </summary>
    public void SetEntries()
    {
        Entries.Clear();
        var entries = SelectedEmployee is not null && SelectedEmployee.ID != -1
            ? new List<ShiftEntry>(EditorVM.FullEntries.Where(s => s.EmployeeID == SelectedEmployee.ID)
                .OrderBy(s => s.Date))
            : new List<ShiftEntry>();

        foreach (var entry in entries)
            Entries.Add(entry);

        newEntries = new List<ShiftEntry>();
        deletedEntries = new List<ShiftEntry>();
    }

    public void CreateEntry()
    {
        if (SelectedEmployee is null || SelectedEmployee.ID == -1 || SelectedDate is null) return;

        // I there already exists an entry for this date, move to the next date that has no entry.
        if (Entries.Any(e => e.Date == ((DateTime)SelectedDate).ToString("yyyy-MM-dd")))
        {
            SelectedDate = ((DateTime) SelectedDate).AddDays(1);
            while (Entries.Any(e => e.Date == ((DateTime)SelectedDate).ToString("yyyy-MM-dd")) && SelectedDate < MaxDate)
                SelectedDate = ((DateTime)SelectedDate).AddDays(1);
            return;
        }

        ShiftEntry shiftEntry = new(SelectedEmployee, ((DateTime)SelectedDate))
        {
            ShiftStartTime = StartShiftTime,
            ShiftEndTime = EndShiftTime,
            LunchStartTime = StartLunchTime,
            LunchEndTime = EndLunchTime,
            Comments = Comment
        };
        shiftEntry.SummarizeShift();
        AddEntry(shiftEntry);
        var entries = new List<ShiftEntry>(Entries.OrderBy(e => e.Date));
        Entries.Clear();
        foreach (var entry in entries)
            Entries.Add(entry);

        // Default to next valid date.
        while (Entries.Any(e => e.Date == ((DateTime)SelectedDate).ToString("yyyy-MM-dd")) && SelectedDate < MaxDate)
            SelectedDate = ((DateTime)SelectedDate).AddDays(1);
    }

    public void AddEntry(ShiftEntry shiftEntry)
    {
        newEntries.Add(shiftEntry);
        Entries.Add(shiftEntry);
    }

    public void DeleteSelectedEntry()
    {
        if (SelectedEntry is null) return;
        if (!newEntries.Remove(SelectedEntry))
            deletedEntries.Add(SelectedEntry);

        Entries.Remove(SelectedEntry);
    }

    public async Task<bool> CheckDateChange()
    {
        var result = MessageBox.Show("Changing the working date range will reset the data.\n\n" +
                                     "Would you like to save your changes before you continue.",
            "Caution: Data Reset", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
            await EditorVM.SaveEntryChanges();

        return result != MessageBoxResult.Cancel;
    }

    public async Task LaunchDateRangeWindowAsync()
    {
        if (!await CheckDateChange()) return;

        DateRangeWindow datePicker = new(EditorVM);

        datePicker.ShowDialog();

        MinDate = EditorVM.MinDate;
        MaxDate = EditorVM.MaxDate;

        SetEntries();
    }

    /// <summary>
    /// Confirms the changes from the VM, including both the newly created entries, and the deleted entries.
    /// Apply these changes to the parent VM, from where it can then be saved to the database.
    /// </summary>
    public void ConfirmAll()
    {
        EditorVM.RemoveEntries(deletedEntries);
        EditorVM.AddEntries(newEntries);
        SetEntries();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}