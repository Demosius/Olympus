using Aion.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Aion.View;
using Aion.ViewModel.Interfaces;
using Uranus;
using Uranus.Staff.Model;

namespace Aion.ViewModel;

public class EntryCreationVM : INotifyPropertyChanged, IDateRange
{
    public Helios Helios { get; set; }

    private List<ShiftEntry> newEntries = new();
    private List<ShiftEntry> deletedEntries = new();
        
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

    public EntryCreationVM()
    {
        CreateEntryCommand = new CreateEntryCommand(this);
        DeleteEntryCommand = new DeleteEntryCommand(this);
        ConfirmEntryCreationCommand = new ConfirmEntryCreationCommand(this);
        LaunchDateRangeCommand = new LaunchDateRangeCommand(this);
        SelectedEmployee = null;
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
        MinDate = editorVM.MinDate;
        MaxDate = editorVM.MaxDate;
        Employees = new ObservableCollection<Employee>(editorVM.Employees);
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
        Entries = new ObservableCollection<ShiftEntry>();
        if (SelectedEmployee is not null && SelectedEmployee.ID != -1)
            Entries = new ObservableCollection<ShiftEntry>(EditorVM.FullEntries.Where(s => s.EmployeeID == selectedEmployee.ID).OrderBy(s => s.Date));
        else
            Entries = new ObservableCollection<ShiftEntry>();
            
        newEntries = new List<ShiftEntry>();
        deletedEntries = new List<ShiftEntry>();
    }

    public void CreateEntry()
    {
        if (SelectedEmployee.ID == -1) return;

        // I there already exists an entry for this date, move to the next date that has no entry.
        if (entries.Any(e => e.Date == SelectedDate.ToString("yyyy-MM-dd")))
        {
            SelectedDate = SelectedDate.AddDays(1);
            while (entries.Any(e => e.Date == SelectedDate.ToString("yyyy-MM-dd")) && SelectedDate < MaxDate)
                SelectedDate = SelectedDate.AddDays(1);
            return;
        }

        ShiftEntry shiftEntry = new(SelectedEmployee, SelectedDate)
        {
            ShiftStartTime = StartShiftTime,
            ShiftEndTime = EndShiftTime,
            LunchStartTime = StartLunchTime,
            LunchEndTime = EndLunchTime,
            Comments = Comment
        };
        shiftEntry.SummarizeShift();
        AddEntry(shiftEntry);
        Entries = new ObservableCollection<ShiftEntry>(Entries.OrderBy(e => e.Date));

        // Default to next valid date.
        while (entries.Any(e => e.Date == SelectedDate.ToString("yyyy-MM-dd")) && SelectedDate < MaxDate)
            SelectedDate = SelectedDate.AddDays(1);
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

    public bool CheckDateChange()
    {
        var result = MessageBox.Show("Changing the working date range will reset the data.\n\n" +
                                     "Would you like to save your changes before you continue.",
            "Caution: Data Reset", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
            EditorVM.SaveEntryChanges();

        return result != MessageBoxResult.Cancel;
    }

    public void LaunchDateRangeWindow()
    {
        if (!CheckDateChange()) return;

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

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}