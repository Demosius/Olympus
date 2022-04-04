using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Uranus.Staff.Model;

public class Shift : INotifyPropertyChanged
{
    [PrimaryKey] public string ID { get; set; } // {DepartmentName}|{Name}
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DailyTarget { get; set; }

    private bool def;
    public bool Default
    {
        get => def;
        set
        {
            def = value;
            OnPropertyChanged(nameof(Default));
            if (!def || Department?.Shifts is null) return;
            foreach (var shift in Department.Shifts.Where(shift => shift.Name != Name))
                shift.Default = false;
        }

    }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [ManyToMany(typeof(EmployeeShift), nameof(EmployeeShift.ShiftID), nameof(Employee.Shifts), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; }

    [OneToMany(nameof(Roster.ShiftID), nameof(Roster.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Roster> Rosters { get; set; }

    [OneToMany(nameof(Break.ShiftID), nameof(Break.Shift), CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    public List<Break> Breaks { get; set; }

    [OneToMany(nameof(Employee.DefaultShiftID), nameof(Employee.DefaultShift), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> DefaultEmployees { get; set; }

    #region Notifiable Properties

    [Ignore] public ObservableCollection<Break> BreaksObservable { get; set; }

    #endregion

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
    [Ignore]
    public string EndString
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
        Breaks = new List<Break>();
        BreaksObservable = new ObservableCollection<Break>(Breaks);
        DefaultEmployees = new List<Employee>();
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

        // Select initial times.
        StartTime = TimeSpan.FromHours(8);
        EndTime = TimeSpan.FromHours(16);

        Breaks = new List<Break>
        {
            new (this)
        };
        BreaksObservable = new ObservableCollection<Break>(Breaks);
        DefaultEmployees = new List<Employee>();
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
        Breaks = breaks;
        BreaksObservable = new ObservableCollection<Break>(Breaks);
        DefaultEmployees = new List<Employee>();
        ID = $"{DepartmentName}|{Name}";
    }

    public void SetBreaks(IEnumerable<Break> newBreaks)
    {
        Breaks = newBreaks.ToList();
        Breaks.Sort();
        BreaksObservable = new ObservableCollection<Break>(Breaks);
    }

    public void AddBreaks(IEnumerable<Break> newBreaks)
    {
        foreach (var newBreak in newBreaks)
        {
            Breaks.Add(newBreak);
            BreaksObservable.Add(newBreak);
        }
    }

    public void AddBreak(Break newBreak)
    {
        Breaks.Add(newBreak);
        Breaks.Sort();
        BreaksObservable = new ObservableCollection<Break>(Breaks);
        OnPropertyChanged(nameof(BreaksObservable));
    }

    public void RemoveBreak(Break deletedBreak)
    {
        Breaks.Remove(deletedBreak);
        BreaksObservable.Remove(deletedBreak);
    }

    public void SortBreaks()
    {
        Breaks.Sort();
        BreaksObservable = new ObservableCollection<Break>(Breaks);
        OnPropertyChanged(nameof(BreaksObservable));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
