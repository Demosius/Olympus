using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Uranus.Staff.Model;

public class Shift : INotifyPropertyChanged
{
    #region fields

    private List<Break> breaks;

    #endregion

    [PrimaryKey] public string ID { get; set; } // {DepartmentName}|{Name}
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool Default { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.ShiftName), nameof(Employee.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    [OneToMany(nameof(Roster.ShiftName), nameof(Roster.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Roster> Rosters { get; set; }

    [OneToMany(nameof(Break.ShiftID), nameof(Break.Shift),
        CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Break> Breaks
    {
        get => breaks;
        set
        {
            breaks = value;
            OnPropertyChanged(nameof(Breaks));
        }
    }

    private string? startString;
    [Ignore]
    public string StartString
    {
        get => startString ??= StartTime.ToString();
        set
        {
            startString = value;
            if (TimeSpan.TryParse(value, out var newSpan))
                StartTime = newSpan;
        }
    }

    private string? endString;
    [Ignore] public string EndString
    {
        get => endString ??= EndTime.ToString();
        set
        {
            endString = value;
            if (TimeSpan.TryParse(value, out var newSpan))
                EndTime = newSpan;
        }
    }

    public Shift()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Employees = new List<Employee>();
        Rosters = new List<Roster>();
        breaks = new List<Break>();
        ID = string.Empty;
    }

    public Shift(Department department, string name)
    {
        Name = name;
        DepartmentName = department.Name;
        Department = department;
        ID = $"{DepartmentName}|{Name}";
        Employees = new List<Employee>();
        Rosters = new List<Roster>();
        breaks = new List<Break>
        {
            new (this)
        };
    }

    public Shift(string name, string departmentName, TimeSpan startTime, TimeSpan endTime, Department department, List<Employee> employees, List<Roster> rosters, List<Break> breaks)
    {
        Name = name;
        DepartmentName = departmentName;
        StartTime = startTime;
        EndTime = endTime;
        Department = department;
        Employees = employees;
        Rosters = rosters;
        this.breaks = breaks;
        ID = $"{DepartmentName}|{Name}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
