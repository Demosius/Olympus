using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Staff.Model;

public class Break : IComparable
{
    // {ShiftID}|{Name}
    [PrimaryKey] public string ID { get; set; }
    // {DepartmentName}|{ShiftName}
    [ForeignKey(typeof(Shift))] public string ShiftID { get; set; }
    public string ShiftName { get; set; }
    public string Name { get; set; }
    public TimeSpan StartTime { get; set; }
    public int Length { get; set; } // in minutes

    [ManyToOne(nameof(ShiftID), nameof(Model.Shift.Breaks), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Shift? Shift { get; set; }

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

    public Break()
    {
        Name = "Lunch";
        ShiftID = string.Empty;
        ShiftName = string.Empty;
        ID = string.Empty;
    }

    public Break(Shift shift)
    {
        Name = "Lunch";
        ShiftID = shift.ID;
        ShiftName = shift.Name;
        Shift = shift;
        ID = $"{ShiftID}|{Name}";
    }

    public Break(Shift shift, string name)
    {
        Name = name;
        ShiftID = shift.ID;
        ShiftName = shift.Name;
        Shift = shift;
        ID = $"{ShiftID}|{Name}";
    }

    public Break(string name, DateTime startTime, int length)
    {
        Name = name;
        ShiftID = string.Empty;
        ShiftName = string.Empty;
        ID = string.Empty;
        StartTime = startTime.TimeOfDay;
        Length = length;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not Break otherBreak) return -1;
        return StartTime.CompareTo(otherBreak.StartTime);
    }
}