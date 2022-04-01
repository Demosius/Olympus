using Pantheon.Properties;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    
    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public RosterPageVM()
    {
        departments = new ObservableCollection<Department>();
        ReportingEmployees = new List<Employee>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshData();
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
        
        Departments = new ObservableCollection<Department>(Helios.StaffReader.SubDepartments(Charon.UserEmployee?.DepartmentName ?? ""));
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name == Charon.UserEmployee?.DepartmentName);
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