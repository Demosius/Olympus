using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

public class ShiftRuleRoster : ShiftRule
{
    public string? ShiftID { get; set; }
    public bool? Monday { get; set; }
    public bool? Tuesday { get; set; }
    public bool? Wednesday { get; set; }
    public bool? Thursday { get; set; }
    public bool? Friday { get; set; }
    public bool? Saturday { get; set; }
    public bool? Sunday { get; set; }

    public int MinDays { get; set; }
    public int MaxDays { get; set; }

    public bool SetShift { get; set; }

    public bool Rotation { get; set; }
    // If using rotation.
    public DateTime? FromDate { get; set; }
    public int? WeekRotation { get; set; }
    public string WeekNumbers { get; set; }

    [ManyToOne(nameof(ShiftID), nameof(Models.Shift.RosterRules), CascadeOperations = CascadeOperation.None)]
    public Shift? Shift { get; set; }
    [ManyToOne(nameof(EmployeeID), nameof(Models.Employee.RosterRules), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Employee { get; set; }

    [Ignore]
    public List<int> WeekNumberList
    {
        get => WeekNumbers == "" ? new List<int>() : WeekNumbers.Split(",").Select(int.Parse).ToList();
        set
        {
            value.Sort();
            WeekNumbers = string.Join(",", value);
        }
    }

    [Ignore]
    public string Summary => $"{(Rotation ? RotationString : "Roster: ")}{DayString}";

    private string RotationString =>
        $"Roster for week{(WeekNumberList.Count > 1 ? "s" : "")}: {WeekNumbers} on {WeekRotation} week rotation.\n";

    private string DayString =>
        $"(({MinDays}-{MaxDays} Days: M:{AtStr(Monday)} T:{AtStr(Tuesday)} W:{AtStr(Wednesday)} Th:{AtStr(Thursday)} F:{AtStr(Friday)} Sa:{AtStr(Saturday)} Su{AtStr(Sunday)}))";

    private static string AtStr(bool? b) => b is null ? "-" : (bool)b ? "✔" : "❌";


    public ShiftRuleRoster()
    {
        Sunday = false;
        Saturday = false;
        WeekNumbers = string.Empty;
    }

    public ShiftRuleRoster(Employee employee) : this()
    {
        Employee = employee;
        EmployeeID = employee.ID;
    }

    /// <summary>
    /// Use a sample roster to set initial important values to match - such as start date and week rotation.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="sampleRosterRule"></param>
    public ShiftRuleRoster(Employee employee, ShiftRuleRoster sampleRosterRule) : this(employee)
    {
        Rotation = sampleRosterRule.Rotation;
        FromDate = sampleRosterRule.FromDate;
    }
    /// <summary>
    /// Compares two roster rules to check if their rotation is harmonious - making sure that they don't clash in any
    /// way assuming they were both to be applied to the same employee.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool IsHarmoniousRotation(ShiftRuleRoster other)
        => EmployeeID == other.EmployeeID &&
           Rotation && other.Rotation &&
           WeekRotation == other.WeekRotation && WeekRotation > 1 &&
           FromDate == other.FromDate &&
           /*WeekNumbers is not null && other.WeekNumbers is not null &&*/
           !WeekNumbers.Intersect(other.WeekNumbers).Any();

    public ShiftRuleRoster Copy()
    {
        return new ShiftRuleRoster
        {
            Employee = Employee,
            Rotation = Rotation,
            FromDate = FromDate,
            WeekRotation = WeekRotation,
            WeekNumbers = WeekNumbers,
            ShiftID = ShiftID,
            Shift = Shift,
            Monday = Monday,
            Tuesday = Tuesday,
            Wednesday = Wednesday,
            Thursday = Thursday,
            Friday = Friday,
            Saturday = Saturday,
            Sunday = Sunday,
            MinDays = MinDays,
            MaxDays = MaxDays,
            ID = ID,
            EmployeeID = EmployeeID,
            Description = Description
        };
    }

    public override (Shift, int)? ShiftDedication()
    {
        if (!SetShift || Shift is null) return null;
        return (Shift, MinDays == 0 ? 1 : MinDays);
    }

    public override bool AppliesToWeek(DateTime weeksStartDate)
    {
        if (WeekRotation == 1) return true;
        if (FromDate > weeksStartDate) return false;
        if (!Rotation || FromDate is null) return true;
        var weeksFromStart = (weeksStartDate - (DateTime)FromDate).Days / 7;
        var weekInRotation = weeksFromStart % (WeekRotation ?? 1);
        return WeekNumbers.Split(",").ToList().Contains(weekInRotation.ToString());
    }

    public override bool AppliesToDay(DateTime date) => AppliesToWeek(date.AddDays(date.DayOfWeek - DayOfWeek.Monday));

    /// <summary>
    /// Given the day of week, returns the appropriate workday object.
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public bool? Day(DayOfWeek dayOfWeek)
    {
        return (dayOfWeek) switch
        {
            DayOfWeek.Sunday => Sunday,
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
        };
    }
}