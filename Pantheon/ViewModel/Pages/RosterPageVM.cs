using Pantheon.Properties;
using Pantheon.ViewModel.Commands;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.View;
using Pantheon.ViewModel.Controls;
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

    private ObservableCollection<Department> departments;
    public ObservableCollection<Department> Departments
    {
        get => departments;
        set
        {
            departments = value;
            OnPropertyChanged(nameof(Departments));
        }
    }

    private Department? selectedDepartment;
    public Department? SelectedDepartment
    {
        get => selectedDepartment;
        set
        {
            selectedDepartment = value;
            OnPropertyChanged(nameof(SelectedDepartment));
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
            OnPropertyChanged(nameof(ShowSaturdays));
        }
    }

    private bool showSundays;
    public bool ShowSundays
    {
        get => showSundays;
        set
        {
            showSundays = value;
            OnPropertyChanged(nameof(ShowSundays));
        }
    }

    private DataTable? rosterTable;
    public DataTable? RosterTable
    {
        get => rosterTable;
        set
        {
            rosterTable = value;
            OnPropertyChanged(nameof(RosterTable));
        }
    }

    private ObservableCollection<DepartmentRosterVM> rosters;
    public ObservableCollection<DepartmentRosterVM> Rosters
    {
        get => rosters;
        set
        {
            rosters = value;
            OnPropertyChanged(nameof(Rosters));
        }
    }

    private DepartmentRosterVM? selectedRoster;
    public DepartmentRosterVM? SelectedRoster
    {
        get => selectedRoster;
        set
        {
            value?.Initialize();
            selectedRoster = value;
            OnPropertyChanged(nameof(SelectedRoster));
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public LoadRosterCommand LoadRosterCommand { get; set; }
    public GenerateRosterCommand GenerateRosterCommand { get; set; }
    public NewRosterCommand NewRosterCommand { get; set; }

    #endregion

    public RosterPageVM()
    {
        departments = new ObservableCollection<Department>();
        ReportingEmployees = new List<Employee>();
        minDate = DateTime.Today.AddDays(DayOfWeek.Sunday - DateTime.Today.DayOfWeek + 1);   // Default to Monday of the current week. (Sunday will get the next monday)
        maxDate = minDate.AddDays(4);   // Default to the next friday.
        rosters = new ObservableCollection<DepartmentRosterVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        GenerateRosterCommand = new GenerateRosterCommand(this);
        LoadRosterCommand = new LoadRosterCommand(this);
        NewRosterCommand = new NewRosterCommand(this);
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

        foreach (var departmentRoster in SelectedDepartment.DepartmentRosters)
        {
            Rosters.Add(new DepartmentRosterVM(departmentRoster, Helios));
        }
    }

    public void NewRoster()
    {
        if (SelectedDepartment is null || Helios is null || Charon is null) return;

        var rosterCreator = new RosterCreationWindow(SelectedDepartment, Helios, Charon);
        if (rosterCreator.ShowDialog() != true) return;

        var newRoster = rosterCreator.VM.Roster;

        if (newRoster is null) return;

        SelectedDepartment.DepartmentRosters.Add(newRoster);
        var vm = new DepartmentRosterVM(newRoster, Helios);
        Rosters.Add(vm);
        SelectedRoster = vm;
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

    public void LoadRoster(DepartmentRosterVM roster)
    {
        roster.Initialize();
    }

    public void GenerateRoster()
    {
        if (Helios is null || SelectedRoster is null) return;

        SelectedRoster.Initialize();

        SelectedRoster.GenerateRosterAssignments();
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