using Microsoft.Win32;
using Pantheon.Properties;
using Pantheon.View;
using Pantheon.ViewModel.Commands;
using Pantheon.ViewModel.Controls;
using Pantheon.ViewModel.Utility;
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
using System.Windows;
using System.Windows.Input;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel.Pages;

internal class RosterPageVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }
    public EmployeeDataSet? EmployeeDataSet { get; set; }
    public RosterDataSet? RosterDataSet { get; set; }

    public List<Employee> ReportingEmployees { get; set; }

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

    private ObservableCollection<Department> departments;
    public ObservableCollection<Department> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged();
        }
    }

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

    private ObservableCollection<DepartmentRoster> rosters;
    public ObservableCollection<DepartmentRoster> Rosters
    {
        get => rosters;
        set
        {
            rosters = value;
            OnPropertyChanged();
        }
    }

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
    public RepairDataCommand RepairDataCommand { get; set; }
    public LoadRosterCommand LoadRosterCommand { get; set; }
    public NewRosterCommand NewRosterCommand { get; set; }
    public SaveRosterCommand SaveRosterCommand { get; set; }
    public DeleteRosterCommand DeleteRosterCommand { get; set; }
    public ExportRosterCommand ExportRosterCommand { get; set; }

    #endregion

    public RosterPageVM()
    {
        departments = new ObservableCollection<Department>();
        ReportingEmployees = new List<Employee>();
        minDate = DateTime.Today.AddDays(DayOfWeek.Sunday - DateTime.Today.DayOfWeek + 1);   // Default to Monday of the current week. (Sunday will get the next monday)
        maxDate = minDate.AddDays(4);   // Default to the next friday.
        rosters = new ObservableCollection<DepartmentRoster>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        LoadRosterCommand = new LoadRosterCommand(this);
        NewRosterCommand = new NewRosterCommand(this);
        SaveRosterCommand = new SaveRosterCommand(this);
        DeleteRosterCommand = new DeleteRosterCommand(this);
        ExportRosterCommand = new ExportRosterCommand(this);
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshData();
    }

    private void SetRosters()
    {
        Rosters.Clear();

        if (Helios is null || SelectedDepartment is null) return;

        Rosters = new ObservableCollection<DepartmentRoster>(SelectedDepartment.DepartmentRosters.OrderBy(r => r.StartDate));

        SelectedRoster = Rosters.First(r =>
            Math.Abs(DateTime.Now.Date.Subtract(r.StartDate).TotalDays -
                     Rosters.Min(dr => DateTime.Now.Date.Subtract(dr.StartDate).TotalDays)) < .05);
    }

    public void NewRoster()
    {
        if (SelectedDepartment is null || Helios is null || Charon is null) return;

        var rosterCreator = new RosterCreationWindow(SelectedDepartment, Helios, Charon);
        if (rosterCreator.ShowDialog() != true) return;

        var newRoster = rosterCreator.VM.Roster;

        if (newRoster is null) return;

        SelectedDepartment.DepartmentRosters.Add(newRoster);
        Rosters.Add(newRoster);
        SelectedRoster = newRoster;
    }

    public void RefreshData()
    {
        if (Charon is null || Helios is null) return;

        EmployeeDataSet = Helios.StaffReader.EmployeeDataSet();

        // Make sure that the user has an assigned role.
        if (Charon.UserEmployee is not null && Charon.UserEmployee.Role is null)
            if (EmployeeDataSet.Roles.TryGetValue(Charon.UserEmployee.RoleName, out var role))
                Charon.UserEmployee.Role = role;

        // Reporting employees (and other collections for filtering that list) is base purely on the employees that report to the current user.
        ReportingEmployees = EmployeeDataSet.GetReportsByRole(Charon.UserEmployee?.ID ?? 0).ToList();

        Departments = new ObservableCollection<Department>(EmployeeDataSet.SubDepartments(Charon.UserEmployee?.DepartmentName ?? ""));
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.UserEmployee?.DepartmentName);
    }

    public void LoadRoster()
    {
        if (LoadedRoster is not null || SelectedRoster is null || Helios is null) return;
        var vm = new DepartmentRosterVM(SelectedRoster, Helios);
        Mouse.OverrideCursor = Cursors.Wait;
        vm.Initialize();
        LoadedRoster = vm;
    }

    public void ExportRoster()
    {
        // Must have a selected roster.
        if (SelectedRoster is null || Helios is null) return;

        var vm = LoadedRoster ?? new DepartmentRosterVM(SelectedRoster, Helios);
        vm.Initialize();
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

    public void SaveRoster()
    {
        if (Helios is null || LoadedRoster is null || !LoadedRoster.IsInitialized) return;

        try
        {
            Helios.StaffUpdater.DepartmentRoster(LoadedRoster.DepartmentRoster);
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
        if (Helios is null || SelectedRoster is null) return;

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

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}