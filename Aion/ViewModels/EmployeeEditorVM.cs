﻿using Aion.ViewModels.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Morpheus.Views.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Aion.ViewModels;

public class EmployeeEditorVM : INotifyPropertyChanged, IDBInteraction

{
    public Helios Helios { get; set; }

    private readonly Employee employee;

    public bool IsNew;

    private List<Employee> employees;
    private List<int> managerIDs;

    private bool useManagers;
    public bool UseManagers
    {
        get => useManagers;
        set
        {
            useManagers = value;
            OnPropertyChanged(nameof(UseManagers));
            SetReportList();
        }
    }
    
    public ObservableCollection<string> Locations { get; set; }
    
    public ObservableCollection<Employee> Reports { get; set; }
    
    public ObservableCollection<Department> Departments { get; set; }
    
    public ObservableCollection<string> PayPoints { get; set; }
    
    public ObservableCollection<EEmploymentType> EmploymentTypes { get; set; }
    
    public ObservableCollection<Role> JobClassifications { get; set; }

    private int code;
    public int Code
    {
        get => code;
        set
        {
            code = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    private string firstName;
    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }

    private string surname;
    public string Surname
    {
        get => surname;
        set
        {
            surname = value;
            OnPropertyChanged(nameof(Surname));
        }
    }

    private string location;
    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged(nameof(location));
        }
    }

    private Employee? reportsTo;
    public Employee? ReportsTo
    {
        get => reportsTo;
        set
        {
            reportsTo = value;
            OnPropertyChanged(nameof(ReportsTo));
        }
    }

    private Department? department;
    public Department? Department
    {
        get => department;
        set
        {
            department = value;
            PayPoint = department?.PayPoint ?? "";
            OnPropertyChanged(nameof(Department));
        }
    }

    private string payPoint;
    public string PayPoint
    {
        get => payPoint;
        set
        {
            payPoint = value;
            OnPropertyChanged(nameof(PayPoint));
        }
    }

    private EEmploymentType employmentType;
    public EEmploymentType EmploymentType
    {
        get => employmentType;
        set
        {
            employmentType = value;
            OnPropertyChanged(nameof(EmploymentType));
        }
    }

    private Role? jobClassification;
    public Role? JobClassification
    {
        get => jobClassification;
        set
        {
            jobClassification = value;
            OnPropertyChanged(nameof(JobClassification));
        }
    }

    public ConfirmEmployeeEditCommand ConfirmEmployeeEditCommand { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }
    public AddLocationCommand AddLocationCommand { get; set; }
    public AddPayPointCommand AddPayPointCommand { get; set; }

    private EmployeeEditorVM(Helios helios, Employee employee, bool isNew)
    {
        Helios = helios;
        IsNew = isNew;
        this.employee = employee;

        employees = new List<Employee>();
        managerIDs = new List<int>();
        Locations = new ObservableCollection<string>();
        Reports = new ObservableCollection<Employee>();
        Departments = new ObservableCollection<Department>();
        PayPoints = new ObservableCollection<string>();
        EmploymentTypes = new ObservableCollection<EEmploymentType>();
        JobClassifications = new ObservableCollection<Role>();
        firstName = string.Empty;
        surname = string.Empty;
        location = string.Empty;
        payPoint = string.Empty;

        ConfirmEmployeeEditCommand = new ConfirmEmployeeEditCommand(this);
        RefreshDataCommand = new RefreshDataCommand(this);
        AddLocationCommand = new AddLocationCommand(this);
        AddPayPointCommand = new AddPayPointCommand(this);
        UseManagers = true;
    }

    private async Task<EmployeeEditorVM> InitializeAsync()
    {
        await SetInitialData();
        return this;
    }

    public static Task<EmployeeEditorVM> CreateAsync(Helios helios, Employee employee, bool isNew)
    {
        var ret = new EmployeeEditorVM(helios, employee, isNew);
        return ret.InitializeAsync();
    }
    public void SetEmployee()
    {
        Code = employee.ID;
        FirstName = employee.FirstName;
        Surname = employee.LastName;
        Location = employee.Location;
        Department = Departments.FirstOrDefault(d => d.Name == employee.DepartmentName);
        ReportsTo = employees.FirstOrDefault(e => e.ID == employee.ReportsToID);
        PayPoint = employee.PayPoint;
        EmploymentType = employee.EmploymentType;
        JobClassification = JobClassifications.FirstOrDefault(r => r.Name == employee.RoleName);
    }

    private async Task SetInitialData()
    {
        await RefreshDataAsync();
        await Task.Run(SetEmployee);
    }

    /// <summary>
    /// Sets the ReportTo list based on whether UseManagers is limiting the list to those who already have reports.
    /// </summary>
    public void SetReportList()
    {
        Reports = useManagers
            ? new ObservableCollection<Employee>(employees.Where(e => managerIDs.Contains(e.ID)).OrderBy(e => e.FullName))
            : new ObservableCollection<Employee>(employees.OrderBy(e => e.FullName));
    }

    public async Task RefreshDataAsync()
    {
        (managerIDs, employees, var locationsList, var payPointList,
                var employmentTypeList, var roleList, var departmentList)
            = await Helios.StaffReader.AionEmployeeRefreshAsync();

        await Task.Run(() =>
        {
            Locations = new ObservableCollection<string>(locationsList.OrderBy(s => s));
            PayPoints = new ObservableCollection<string>(payPointList.OrderBy(s => s));
            EmploymentTypes = new ObservableCollection<EEmploymentType>(employmentTypeList);
            JobClassifications = new ObservableCollection<Role>(roleList.OrderBy(r => r.Name));
            Departments = new ObservableCollection<Department>(departmentList.OrderBy(d => d.Name));

            SetReportList();
        });
    }

    public void ConfirmEdit()
    {
        employee.FirstName = FirstName;
        employee.LastName = Surname;
        employee.Location = Location;
        employee.ReportsToID = ReportsTo?.ID ?? 0;
        employee.ReportsTo = ReportsTo;
        employee.PayPoint = PayPoint;
        employee.EmploymentType = EmploymentType;
        employee.RoleName = JobClassification?.Name ?? string.Empty;
        employee.Role = JobClassification;

        Helios.StaffUpdater.Employee(employee);
    }

    public void AddLocation()
    {
        InputWindow input = new();
        if (input.ShowDialog() != true) return;

        Locations.Add(input.InputText);
        Locations = new ObservableCollection<string>(Locations.OrderBy(s => s));
        Location = input.InputText;
    }

    public void AddPayPoint()
    {
        InputWindow input = new();
        if (input.ShowDialog() != true) return;

        PayPoints.Add(input.InputText);
        PayPoints = new ObservableCollection<string>(PayPoints.OrderBy(s => s));
        PayPoint = input.InputText;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}