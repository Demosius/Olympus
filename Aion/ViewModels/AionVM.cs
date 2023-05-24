using Aion.View;
using Aion.ViewModels.Commands;
using Aion.ViewModels.Utility;
using Styx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Aion.ViewModels;

public class AionVM : INotifyPropertyChanged, IDBInteraction, IDBRepair
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ShiftEntryPage? ShiftEntryPage { get; set; }
    public EmployeePage? EmployeePage { get; set; }

    private Page? currentPage;

    public Page? CurrentPage
    {
        get => currentPage;
        set
        {
            currentPage = value;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
    private string databasePath;
    public string DatabasePath
    {
        get => databasePath;
        set
        {
            databasePath = value;
            OnPropertyChanged(nameof(DatabasePath));
        }
    }

    /* page checks */
    private bool entryView;
    public bool EntryView { get => entryView; set { entryView = value; OnPropertyChanged(nameof(EntryView)); } }
    private bool entryEdit;
    public bool EntryEdit { get => entryEdit; set { entryEdit = value; OnPropertyChanged(nameof(EntryEdit)); } }
    private bool isEntry;
    public bool IsEntry { get => isEntry; set { isEntry = value; OnPropertyChanged(nameof(IsEntry)); } }
    private bool employeeView;
    public bool EmployeeView { get => employeeView; set { employeeView = value; OnPropertyChanged(nameof(EmployeeView)); } }

    /* Commands */
    public ShowEntriesCommand ShowEntriesCommand { get; set; }
    public ShowEmployeesCommand ShowEmployeesCommand { get; set; }
    public ImportOldDataCommand ImportOldDataCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    public AionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        databasePath = string.Empty;

        // Commands
        ShowEntriesCommand = new ShowEntriesCommand(this);
        ShowEmployeesCommand = new ShowEmployeesCommand(this);
        ImportOldDataCommand = new ImportOldDataCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);

        SetChecks();
    }
    
    public void ShowEntryPage()
    {
        ShiftEntryPage = new ShiftEntryPage(Helios, Charon);
        CurrentPage = ShiftEntryPage;
        SetChecks();
    }

    public void ShowEmployeePage()
    {
        EmployeePage ??= new EmployeePage(Helios, Charon);
        CurrentPage = EmployeePage;
        SetChecks();
    }

    /// <summary>
    /// Set the bool check values - that bind to the menu options - based on what page is showing.
    /// </summary>
    private void SetChecks()
    {
        if (CurrentPage is null)
        {
            EntryView = false;
            EmployeeView = false;
        }
        else
        {
            EntryView = CurrentPage is ShiftEntryPage;
            EmployeeView = CurrentPage is EmployeePage;
        }
        IsEntry = EntryView || EntryEdit;
    }

    public async Task RefreshDataAsync()
    {
        var tasks = new List<Task>();

        if (EmployeePage?.VM is not null) tasks.Add(EmployeePage.VM.RefreshDataAsync());
        if (ShiftEntryPage?.VM is not null) tasks.Add(ShiftEntryPage.VM.RefreshDataAsync());

        await Task.WhenAll(tasks);
    }

    public async Task RepairDataAsync()
    {
        if (ShiftEntryPage?.VM is null) return;

        await ShiftEntryPage.VM.RepairDataAsync().ConfigureAwait(false);
    }

    public async Task ImportOldData()
    {
        try
        {
            var archivedData = OldDataUtil.GetArchivedData();
            if (!archivedData.HasData()) return;

            var currentData = await Helios.StaffReader.GetAionDataSetAsync();

            // Remove non-unique data - where keys or specific unique combinations already exist.
            if (archivedData.HasEmployees())
                archivedData.Employees = archivedData.Employees.Select(e => e.Value)
                    .Where(e => !currentData.Employees.ContainsKey(e.ID)).ToDictionary(e => e.ID, e => e);
            if (archivedData.HasClockEvents())
                archivedData.ClockEvents = archivedData.ClockEvents.Select(c => c.Value)
                    .Where(c => !currentData.ClockEvents.ContainsKey(c.ID)).ToDictionary(c => c.ID, c => c);
            if (archivedData.HasShiftEntries())
                archivedData.ShiftEntries = archivedData.ShiftEntries.Select(e => e.Value)
                    .Where(e => !currentData.ShiftEntries.ContainsKey(e.ID) &&
                                !currentData.ShiftEntries.Select(d => d.Value).ToDictionary(se => (se.EmployeeID, se.Date)).ContainsKey((e.EmployeeID, e.Date)))
                    .ToDictionary(e => e.ID, e => e);

            var lines = await Helios.StaffCreator.AionDataSetAsync(archivedData);

            await Helios.StaffUpdater.ApplyPendingClockEventsAsync();

            MessageBox.Show($"Successfully converted and added {lines} new line of data.", "Update Successful", MessageBoxButton.OK);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception when Importing Old Aion Data:\n\n{ex.Message}", "Unexpected Exception",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}