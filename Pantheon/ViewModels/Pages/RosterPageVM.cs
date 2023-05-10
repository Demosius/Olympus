using Microsoft.Win32;
using Pantheon.ViewModels.Commands.Rosters;
using Pantheon.ViewModels.Controls.Rosters;
using Pantheon.ViewModels.Utility;
using Pantheon.Views.PopUp.Rosters;
using Serilog;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Pages;

public class RosterPageVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public EmployeeDataSet EmployeeDataSet { get; set; }
    public RosterDataSet? RosterDataSet { get; set; }

    public List<Employee> ReportingEmployees { get; set; }

    public Dictionary<Guid, DepartmentRosterVM> LoadedRosters { get; set; }

    #region NotifiableProperties

    private DateTime minDate;
    public DateTime MinDate
    {
        get => minDate;
        set
        {
            minDate = value;
            OnPropertyChanged();
        }
    }

    private DateTime maxDate;
    public DateTime MaxDate
    {
        get => maxDate;
        set
        {
            maxDate = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<Department> Departments { get; set; }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged();
            SetRosters();
        }
    }
    private bool showSaturdays;
    public bool ShowSaturdays
    {
        get => showSaturdays;
        set
        {
            showSaturdays = value;
            OnPropertyChanged();
        }
    }

    private bool showSundays;
    public bool ShowSundays
    {
        get => showSundays;
        set
        {
            showSundays = value;
            OnPropertyChanged();
        }
    }

    private DataTable? rosterTable;
    public DataTable? RosterTable
    {
        get => rosterTable;
        set
        {
            rosterTable = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<DepartmentRoster> Rosters { get; set; }

    private DepartmentRoster? selectedRoster;
    public DepartmentRoster? SelectedRoster
    {
        get => selectedRoster;
        set
        {
            selectedRoster = value;
            OnPropertyChanged();
            LoadedRoster = null;
        }
    }

    private DepartmentRosterVM? loadedRoster;
    public DepartmentRosterVM? LoadedRoster
    {
        get => loadedRoster;
        set
        {
            loadedRoster = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsLoaded));
        }
    }

    public bool IsLoaded => LoadedRoster is not null;

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public LoadRosterCommand LoadRosterCommand { get; set; }
    public NewRosterCommand NewRosterCommand { get; set; }
    public SaveRosterCommand SaveRosterCommand { get; set; }
    public DeleteRosterCommand DeleteRosterCommand { get; set; }
    public ExportRosterCommand ExportRosterCommand { get; set; }

    #endregion

    public RosterPageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        EmployeeDataSet = new EmployeeDataSet();
        ReportingEmployees = new List<Employee>();
        LoadedRosters = new Dictionary<Guid, DepartmentRosterVM>();
        Departments = new ObservableCollection<Department>();
        Rosters = new ObservableCollection<DepartmentRoster>();

        RefreshDataCommand = new RefreshDataCommand(this);
        LoadRosterCommand = new LoadRosterCommand(this);
        NewRosterCommand = new NewRosterCommand(this);
        SaveRosterCommand = new SaveRosterCommand(this);
        DeleteRosterCommand = new DeleteRosterCommand(this);
        ExportRosterCommand = new ExportRosterCommand(this);

        Task.Run(RefreshDataAsync);
    }

    private void SetRosters()
    {
        Rosters.Clear();

        if (SelectedDepartment is null) return;

        Rosters = new ObservableCollection<DepartmentRoster>(SelectedDepartment.DepartmentRosters.OrderBy(r => r.StartDate));

        SelectedRoster = Rosters.FirstOrDefault(r =>
            Math.Abs(DateTime.Now.Date.Subtract(r.StartDate).TotalDays -
                     Rosters.Min(dr => DateTime.Now.Date.Subtract(dr.StartDate).TotalDays)) < .05);
    }

    public async Task NewRoster()
    {
        if (SelectedDepartment is null) return;

        var rosterCreator = new RosterCreationWindow(SelectedDepartment, Helios, Charon);
        if (rosterCreator.ShowDialog() != true) return;

        var newRoster = rosterCreator.VM.Roster;

        if (newRoster is null) return;

        SelectedDepartment.DepartmentRosters.Add(newRoster);
        Rosters.Add(newRoster);
        SelectedRoster = newRoster;

        await LoadRoster();
    }

    public async Task RefreshDataAsync()
    {
        EmployeeDataSet = await Helios.StaffReader.EmployeeDataSetAsync();

        // Make sure that the user has an assigned role.
        if (Charon.Employee is not null && Charon.Employee.Role is null)
            if (EmployeeDataSet.Roles.TryGetValue(Charon.Employee.RoleName, out var role))
                Charon.Employee.Role = role;

        // Reporting employees (and other collections for filtering that list) is base purely on the employees that report to the current user.
        ReportingEmployees = EmployeeDataSet.GetReportsByRole(Charon.Employee?.ID ?? 0).ToList();

        Departments.Clear();
        foreach (var subDepartment in EmployeeDataSet.SubDepartments(Charon.Employee?.DepartmentName ?? "")) 
            Departments.Add(subDepartment);

        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.Employee?.DepartmentName);
        
        Rosters.Clear();
        
        minDate = DateTime.Today.AddDays(DayOfWeek.Sunday - DateTime.Today.DayOfWeek + 1);   // Default to Monday of the current week. (Sunday will get the next monday)
        maxDate = minDate.AddDays(4);   // Default to the next friday.
        LoadedRosters = new Dictionary<Guid, DepartmentRosterVM>();
        LoadedRoster = null;
    }

    public async Task LoadRoster()
    {
        if (LoadedRoster is not null || SelectedRoster is null) return;

        if (!LoadedRosters.TryGetValue(SelectedRoster.ID, out var vm))
            vm = new DepartmentRosterVM(SelectedRoster, Helios, Charon);

        Mouse.OverrideCursor = Cursors.Wait;
        await vm.Initialize();

        if (!LoadedRosters.ContainsKey(SelectedRoster.ID)) LoadedRosters.Add(SelectedRoster.ID, vm);

        LoadedRoster = vm;
    }

    public async Task ExportRoster()
    {
        // Must have a selected roster.
        if (SelectedRoster is null) return;

        var vm = LoadedRoster ?? new DepartmentRosterVM(SelectedRoster, Helios, Charon);
        await vm.Initialize();
        var depRoster = vm.DepartmentRoster;

        // Prompt for file/directory.
        SaveFileDialog sfd = new()
        {
            OverwritePrompt = true,
            FileName = $"{depRoster.Name}.csv",
            CheckFileExists = false,
            CheckPathExists = true
        };
        if (sfd.ShowDialog() != true) return;
        var fullFilePath = sfd.FileName;

        // Create data table.
        var table = depRoster.DataTable();

        // Export to csv.
        table.WriteToCsvFile(fullFilePath);

        MessageBox.Show($"Successfully exported '{depRoster.Name}' to {Path.GetDirectoryName(fullFilePath)}.",
            "Successful Export", MessageBoxButton.OK, MessageBoxImage.None);
    }

    public async Task SaveRoster()
    {
        if (LoadedRoster is null || !LoadedRoster.IsInitialized) return;

        try
        {
            LoadedRoster.DepartmentRoster.RegenerateRosters();
            await Helios.StaffUpdater.DepartmentRosterAsync(LoadedRoster.DepartmentRoster);
            MessageBox.Show("Saved Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.None);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save Department Roster to Database.");
            MessageBox.Show("Failed to save roster.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void DeleteRoster()
    {
        if (SelectedRoster is null) return;

        // Confirm delete.
        if (MessageBox.Show($"Are you sure you want to delete the roster: {SelectedRoster}?", "Confirm Deletion",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        // Delete from database.
        Helios.StaffDeleter.DepartmentRoster(SelectedRoster);

        // Delete from active data.
        SelectedDepartment?.DepartmentRosters.Remove(SelectedRoster);
        Rosters.Remove(SelectedRoster);
        SelectedRoster = null;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}