﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Aion.Models;

[Table("Employee")]
public class BrokeEmployee : INotifyPropertyChanged
{
    /* Fields - for notify property changed */
    private string surname;
    private string firstName;
    private string location;
    private int reportsToCode;
    private BrokeEmployee? reportsTo;
    private string payPoint;
    private string employmentType;
    // ReSharper disable once IdentifierTypo
    private string jobClasification;

    /* Properties */
    [PrimaryKey]
    public int Code { get; set; }
    public string Surname
    {
        get => surname;
        set
        {
            surname = value;
            OnPropertyChanged(nameof(Surname));
        }
    }
    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }
    public string Location
    {
        get => location;
        set
        {
            location = value;
            OnPropertyChanged(nameof(Location));
        }
    }
    [ForeignKey(typeof(BrokeEmployee))]
    public int ReportsToCode
    {
        get => reportsToCode;
        set
        {
            reportsToCode = value;
            OnPropertyChanged(nameof(ReportsToCode));
        }
    }
    public string PayPoint
    {
        get => payPoint;
        set
        {
            payPoint = value;
            OnPropertyChanged(nameof(PayPoint));
        }
    }
    public string EmploymentType
    {
        get => employmentType;
        set
        {
            employmentType = value;
            OnPropertyChanged(nameof(EmploymentType));
        }
    }
    // ReSharper disable once IdentifierTypo
    public string JobClasification
    {
        get => jobClasification;
        set
        {
            jobClasification = value;
            OnPropertyChanged(nameof(JobClasification));
        }
    }

    [OneToMany(inverseProperty: "ReportsTo", CascadeOperations = CascadeOperation.All)]
    public List<BrokeEmployee> Reports { get; set; }
    [ManyToOne(inverseProperty: "Reports")]
    public BrokeEmployee? ReportsTo
    {
        get => reportsTo;
        set
        {
            reportsTo = value;
            OnPropertyChanged(nameof(ReportsTo));
        }
    }
    [OneToMany(inverseProperty: "Employee", CascadeOperations = CascadeOperation.All)]
    public List<ClockEvent> ClockTimes { get; set; }
    [OneToMany(inverseProperty: "Employee", CascadeOperations = CascadeOperation.All)]
    public List<ShiftEntry> ShiftEntries { get; set; }

    [Ignore]
    public string FullName => $"{FirstName} {Surname}";

    public BrokeEmployee()
    {
        surname = string.Empty;
        firstName = string.Empty;
        location = string.Empty;
        payPoint = string.Empty;
        employmentType = string.Empty;
        jobClasification = string.Empty;
        Reports = new List<BrokeEmployee>();
        ClockTimes = new List<ClockEvent>();
        ShiftEntries = new List<ShiftEntry>();
    }

    /// <summary>
    /// Adds a newly created timestamp (clock time event) for the employee for current time/date.
    /// </summary>
    /// <returns>The newly created clock time.</returns>
    public ClockEvent AddTimestamp()
    {
        ClockEvent clock = new() { EmployeeID = Code, Status = Uranus.Staff.Models.EClockStatus.Pending };
        clock.StampTime();
        ClockTimes.Add(clock);
        return clock;
    }

    /// <summary>
    /// Converts the employee's ClockTimes to ShiftEntries. 
    /// </summary>
    public void ConvertClockToEntries()
    {
        var pendingClocks = ClockTimes.Where(c => c.Status == Uranus.Staff.Models.EClockStatus.Pending).GroupBy(c => c.DtDate.ToString("yyyy-MM-dd")).ToDictionary(g => g.Key, g => g.ToList());
        if (pendingClocks.Count == 0) return;

        // Get existing Daily Entries in dictionary form for quick look up.
        var currentEntries = ShiftEntries.ToDictionary(d => d.Date, d => d);

        foreach (var (key, value) in pendingClocks)
        {
            if (currentEntries.ContainsKey(key))
            {
                currentEntries[key].ApplyClockTimes(value);
            }
            else
            {
                ShiftEntries.Add(new ShiftEntry(new Employee(), value));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public override string ToString()
    {
        return $"{FirstName} {Surname}";
    }
}