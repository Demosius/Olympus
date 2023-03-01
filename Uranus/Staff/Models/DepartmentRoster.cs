using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Uranus.Extensions;

namespace Uranus.Staff.Models;

public class DepartmentRoster
{
    [PrimaryKey] public Guid ID { get; set; }
    public string Name { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }
    public DateTime StartDate { get; set; } // Monday
    public bool UseSaturdays { get; set; }
    public bool UseSundays { get; set; }
    public bool ExceedTargets { get; set; }

    [ForeignKey(typeof(DailyRoster))] public Guid MondayRosterID { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid TuesdayRosterID { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid WednesdayRosterID { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid ThursdayRosterID { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid FridayRosterID { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid SaturdayRosterID { get; set; }
    [ForeignKey(typeof(DailyRoster))] public Guid SundayRosterID { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.DepartmentRosters), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [OneToMany(nameof(Roster.DepartmentRosterID), nameof(Roster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(EmployeeRoster.DepartmentRosterID), nameof(EmployeeRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<EmployeeRoster> EmployeeRosters { get; set; }
    [OneToMany(nameof(WeeklyShiftCounter.RosterID), nameof(WeeklyShiftCounter.Roster), CascadeOperations = CascadeOperation.None)]
    public List<WeeklyShiftCounter> ShiftCounters { get; set; }

    [OneToOne(nameof(MondayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? MondayRoster { get; set; }
    [OneToOne(nameof(TuesdayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? TuesdayRoster { get; set; }
    [OneToOne(nameof(WednesdayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? WednesdayRoster { get; set; }
    [OneToOne(nameof(ThursdayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? ThursdayRoster { get; set; }
    [OneToOne(nameof(FridayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? FridayRoster { get; set; }
    [OneToOne(nameof(SaturdayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? SaturdayRoster { get; set; }
    [OneToOne(nameof(SundayRosterID), nameof(DailyRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead)]
    public DailyRoster? SundayRoster { get; set; }

    [Ignore] public Dictionary<int, Employee> EmployeeDict { get; set; }
    [Ignore] public Dictionary<string, Shift> ShiftDict { get; set; }
    [Ignore] public Dictionary<string, List<Break>> BreakDict { get; set; }
    [Ignore] public Dictionary<(int, DateTime), Roster> RosterDict { get; set; }
    [Ignore] public Dictionary<Guid, EmployeeRoster> EmployeeRosterDict { get; set; }
    [Ignore] public IEnumerable<EmployeeShift> EmpShiftConnections { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRuleSingle>> SingleRuleDict { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRuleRecurring>> RecurringRuleDict { get; set; }
    [Ignore] public Dictionary<int, List<ShiftRuleRoster>> RosterRuleDict { get; set; }
    [Ignore] public Dictionary<string, WeeklyShiftCounter> ShiftCounterDict { get; set; }
    [Ignore] public Dictionary<Guid, Dictionary<string, DailyShiftCounter>> DailyCounterDict { get; set; }

    [Ignore] public Shift? DefaultShift { get; set; }

    [Ignore] public Dictionary<Shift, int> ShiftCounter { get; set; }
    [Ignore] public Dictionary<Shift, int> TargetShiftCounts { get; set; }

    [Ignore] public bool IsLoaded { get; set; }

    public DepartmentRoster()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Rosters = new List<Roster>();
        EmployeeRosters = new List<EmployeeRoster>();
        ShiftCounters = new List<WeeklyShiftCounter>();

        EmployeeDict = new Dictionary<int, Employee>();
        ShiftDict = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        RosterDict = new Dictionary<(int, DateTime), Roster>();
        EmployeeRosterDict = new Dictionary<Guid, EmployeeRoster>();
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        ShiftCounter = new Dictionary<Shift, int>();
        TargetShiftCounts = new Dictionary<Shift, int>();
        ShiftCounterDict = new Dictionary<string, WeeklyShiftCounter>();
        DailyCounterDict = new Dictionary<Guid, Dictionary<string, DailyShiftCounter>>();
    }

    public DepartmentRoster(string name, DateTime startDate, bool useSaturdays, bool useSundays, Department department)
    {
        ID = Guid.NewGuid();
        Name = name;
        StartDate = startDate;
        UseSaturdays = useSaturdays;
        UseSundays = useSundays;
        Department = department;
        DepartmentName = department.Name;

        Rosters = new List<Roster>();
        EmployeeRosters = new List<EmployeeRoster>();
        ShiftCounters = new List<WeeklyShiftCounter>();

        EmployeeDict = new Dictionary<int, Employee>();
        ShiftDict = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        RosterDict = new Dictionary<(int, DateTime), Roster>();
        EmployeeRosterDict = new Dictionary<Guid, EmployeeRoster>();
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        ShiftCounter = new Dictionary<Shift, int>();
        TargetShiftCounts = new Dictionary<Shift, int>();
        ShiftCounterDict = new Dictionary<string, WeeklyShiftCounter>();
        DailyCounterDict = new Dictionary<Guid, Dictionary<string, DailyShiftCounter>>();
    }

    public void SetData(IEnumerable<Employee> employees, IEnumerable<Roster> rosters, IEnumerable<DailyRoster> dailyRosters, IEnumerable<EmployeeRoster> employeeRosters,
        IEnumerable<Shift> shifts, IEnumerable<Break> breaks, IEnumerable<EmployeeShift> empShiftCons,
        IEnumerable<ShiftRuleSingle> singleRules, IEnumerable<ShiftRuleRecurring> recurringRules, IEnumerable<ShiftRuleRoster> rosterRules,
        IEnumerable<WeeklyShiftCounter> weeklyShiftCounters, IEnumerable<DailyShiftCounter> dailyShiftCounters)
    {
        EmployeeDict = employees.ToDictionary(e => e.ID, e => e);
        ShiftDict = shifts.ToDictionary(s => s.ID, s => s);
        BreakDict = breaks.GroupBy(b => b.ShiftID).ToDictionary(g => g.Key, g => g.ToList());
        EmpShiftConnections = empShiftCons;
        SingleRuleDict = singleRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RecurringRuleDict = recurringRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RosterRuleDict = rosterRules.GroupBy(rule => rule.EmployeeID)
            .ToDictionary(group => group.Key, group => group.ToList());
        RosterDict = rosters.ToDictionary(r => (r.EmployeeID, r.Date), r => r);
        EmployeeRosterDict = employeeRosters.ToDictionary(er => er.ID, er => er);

        // Daily Rosters
        dailyRosters = dailyRosters.Where(dr => dr.DepartmentRosterID == ID);
        foreach (var dailyRoster in dailyRosters) SetDaily(dailyRoster);

        DailyCounterDict = dailyShiftCounters.GroupBy(counter => counter.RosterID).ToDictionary(group => group.Key,
            group => group.ToDictionary(counter => counter.ShiftID, counter => counter));

        ShiftCounterDict = weeklyShiftCounters.Where(c => c.RosterID == ID).ToDictionary(c => c.ShiftID, c => c);
        ShiftCounters = ShiftCounterDict.Values.ToList();

        SetRelationships();
        GenerateMissingRosters();
        EmployeeRosters.Sort();

        IsLoaded = true;
    }

    public void AddCount(Shift shift)
    {
        ShiftCounterDict[shift.ID].Count++;
    }

    public void SubCount(Shift shift)
    {
        ShiftCounterDict[shift.ID].Count--;
    }

    /// <summary>
    /// Using the dictionaries, assign relevant data as references to each other.
    /// </summary>
    public void SetRelationships()
    {
        SetFromShifts();
        SetFromConnections();
        SetFromEmployees();
        SetFromRosters();
        SetFromDailyRosters();
        SetFromEmployeeRosters();
    }

    /// <summary>
    /// Generates an empty roster for each that should exist based on employees and date range.
    /// </summary>
    private void GenerateMissingRosters()
    {
        if (Department is null) throw new DataException("Department Roster initialized without Department.");

        FillMissingDailyRosters();

        // Convert EmpRoster list to dictionary to compare to our employee list (which should be all required for the department).
        var empRosterDict = EmployeeRosters.ToDictionary(er => er.EmployeeID, er => er);

        // Create employeeRoster for each employee - only for those that do not already exist.
        foreach (var (_, employee) in EmployeeDict)
        {
            if (empRosterDict.TryGetValue(employee.ID, out _)) continue;

            EmployeeRoster newEmployeeRoster = new(Department, this, employee, StartDate);

            // Get Roster for each day.
            for (var i = 0; i < 7; i++)
            {
                var date = StartDate.AddDays(i);
                var dailyRoster = GetDaily(date.DayOfWeek);
                if (dailyRoster is null) continue;
                var roster = new Roster(Department, this, newEmployeeRoster, employee, date);
                newEmployeeRoster.SetDaily(roster);
                dailyRoster.AddRoster(roster);
                Rosters.Add(roster);
                RosterDict.Add((roster.EmployeeID, date), roster);
            }
            
            EmployeeRosters.Add(newEmployeeRoster);
            EmployeeRosterDict.Add(newEmployeeRoster.ID, newEmployeeRoster);
        }

    }

    public DailyRoster? GetDaily(DayOfWeek weekDay)
    {
        return weekDay switch
        {
            DayOfWeek.Sunday => SundayRoster,
            DayOfWeek.Monday => MondayRoster,
            DayOfWeek.Tuesday => TuesdayRoster,
            DayOfWeek.Wednesday => WednesdayRoster,
            DayOfWeek.Thursday => ThursdayRoster,
            DayOfWeek.Friday => FridayRoster,
            DayOfWeek.Saturday => SaturdayRoster,
            _ => throw new ArgumentOutOfRangeException(nameof(weekDay), weekDay, null)
        };
    }

    public bool SetDaily(DailyRoster dailyRoster)
    {
        var date = dailyRoster.Date;
        if (date < StartDate || date > StartDate.AddDays(6)) return false;

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                SundayRoster = dailyRoster;
                break;
            case DayOfWeek.Monday:
                MondayRoster = dailyRoster;
                break;
            case DayOfWeek.Tuesday:
                TuesdayRoster = dailyRoster;
                break;
            case DayOfWeek.Wednesday:
                WednesdayRoster = dailyRoster;
                break;
            case DayOfWeek.Thursday:
                ThursdayRoster = dailyRoster;
                break;
            case DayOfWeek.Friday:
                FridayRoster = dailyRoster;
                break;
            case DayOfWeek.Saturday:
                SaturdayRoster = dailyRoster;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dailyRoster), dailyRoster, null);
        }
        return true;
    }

    private void SetFromShifts()
    {
        foreach (var (_, shift) in ShiftDict)
        {
            if (BreakDict.TryGetValue(shift.ID, out var breaks))
            {
                shift.Breaks = breaks;
                foreach (var @break in breaks)
                    @break.Shift = shift;
            }

            if (ShiftCounterDict.TryGetValue(shift.ID, out var counter))
            {
                counter.Shift = shift;
                if (counter.RosterID == ID) counter.Roster = this;
            }
            else
            {
                counter = new WeeklyShiftCounter(this, shift, shift.DailyTarget);
                ShiftCounterDict.Add(shift.ID, counter);
                ShiftCounters.Add(counter);
            }

            if (Department is not null && !Department.Shifts.Select(s => s.Name).Contains(shift.Name))
                Department?.Shifts.Add(shift);

            ShiftCounter[shift] = 0;
            TargetShiftCounts[shift] = shift.DailyTarget;
            shift.Department = Department;
            if (shift.Default) DefaultShift = shift;
        }
    }

    private void SetFromConnections()
    {
        foreach (var empShiftConnection in EmpShiftConnections)
        {
            EmployeeDict.TryGetValue(empShiftConnection.EmployeeID, out var employee);
            ShiftDict.TryGetValue(empShiftConnection.ShiftID, out var shift);

            empShiftConnection.Employee = employee;
            empShiftConnection.Shift = shift;

            if (employee is null || shift is null) continue;

            if (shift != DefaultShift) employee.Shifts.Add(shift);
            shift.Employees.Add(employee);
        }
    }

    private void SetFromEmployees()
    {
        foreach (var (_, employee) in EmployeeDict)
        {
            if (employee.DepartmentName == Department?.Name) employee.Department = Department;

            if (DefaultShift is not null) employee.Shifts.Add(DefaultShift);

            if (SingleRuleDict.TryGetValue(employee.ID, out var shiftRules))
            {
                employee.SingleRules = shiftRules;
                foreach (var shiftRule in shiftRules)
                    shiftRule.Employee = employee;
            }

            if (RecurringRuleDict.TryGetValue(employee.ID, out var recurringRules))
            {
                employee.RecurringRules = recurringRules;
                foreach (var shiftRule in recurringRules)
                    shiftRule.Employee = employee;
            }

            if (!RosterRuleDict.TryGetValue(employee.ID, out var rosterRules)) continue;

            employee.RosterRules = rosterRules;
            foreach (var shiftRule in rosterRules)
            {
                shiftRule.Employee = employee;
                if (!ShiftDict.TryGetValue(shiftRule.ShiftID ?? "", out var shift)) continue;

                shiftRule.Shift = shift;
                shift.RosterRules.Add(shiftRule);
            }
        }
    }

    private void SetFromRosters()
    {
        var nullIDs = new List<(int, DateTime)>();

        foreach (var (key, roster) in RosterDict)
        {
            if (roster.DepartmentName == Department?.Name) roster.Department = Department;

            if (EmployeeDict.TryGetValue(roster.EmployeeID, out var employee))
            {
                employee.Rosters.Add(roster);
                roster.Employee = employee;
            }
            else    // Make sure to remove rosters that are trying to use non-existent/inactive employees.
            {
                nullIDs.Add(key);
                continue;
            }

            if (ShiftDict.TryGetValue(roster.ShiftID, out var shift))
            {
                roster.Shift = shift;
                shift.Rosters.Add(roster);
            }

            if (EmployeeRosterDict.TryGetValue(roster.EmployeeRosterID, out var employeeRoster))
            {
                roster.EmployeeRoster = employeeRoster;
                employeeRoster.SetDaily(roster);
            }

            if (roster.DepartmentRosterID == ID)
            {
                Rosters.Add(roster);
                roster.DepartmentRoster = this;
                if (roster.Shift is not null && ShiftCounter.TryGetValue(roster.Shift, out _))
                    ShiftCounter[roster.Shift]++;
            }

            GetDaily(roster.Day)?.AddRoster(roster);
        }

        foreach (var rosterKey in nullIDs)
        {
            RosterDict.Remove(rosterKey);
        }
    }

    private void SetFromDailyRosters()
    {
        foreach (var dailyRoster in new List<DailyRoster?>
                 {
                     MondayRoster, TuesdayRoster, WednesdayRoster, ThursdayRoster, 
                     FridayRoster, SaturdayRoster, SundayRoster
                 }.Where(dailyRoster => dailyRoster is not null))
        {
            if (dailyRoster!.DepartmentName == DepartmentName)
            {
                dailyRoster.Department = Department;
                Department?.DailyRosters.Add(dailyRoster);
            }

            DailyCounterDict.TryGetValue(dailyRoster.ID, out var counters);
            counters ??= new Dictionary<string, DailyShiftCounter>();

            foreach (var (id, shift) in ShiftDict)
            {
                counters.TryGetValue(id, out var counter);
                counter ??= new DailyShiftCounter(dailyRoster, shift, ShiftCounterDict[id].Target);

                counter.Shift = shift;
                counter.Roster = dailyRoster;
                dailyRoster.ShiftCounters.Add(counter);
            }
        }

        // Build missing Daily Rosters.
        FillMissingDailyRosters();
    }

    private void FillMissingDailyRosters()
    {
        if (Department is null) throw new DataException("Department Roster initialized without Department.");

        // Ensure all daily rosters exist.
        MondayRoster ??= new DailyRoster(Department, this, StartDate);
        TuesdayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(1));
        WednesdayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(2));
        ThursdayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(3));
        FridayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(4));
        SaturdayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(5));
        SundayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(6));
    }

    private void SetFromEmployeeRosters()
    {
        var nullIDs = new List<Guid>();
        foreach (var (_, employeeRoster) in EmployeeRosterDict)
        {
            if (EmployeeDict.TryGetValue(employeeRoster.EmployeeID, out var employee))
            {
                employee.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Employee = employee;
            }
            else
            {
                nullIDs.Add(employeeRoster.ID);
                continue;
            }

            if (employeeRoster.DepartmentName == DepartmentName)
            {
                employeeRoster.Department = Department;
                Department?.EmployeeRosters.Add(employeeRoster);
            }

            if (employeeRoster.DepartmentRosterID == ID)
            {
                employeeRoster.DepartmentRoster = this;
                EmployeeRosters.Add(employeeRoster);
            }

            if (ShiftDict.TryGetValue(employeeRoster.ShiftID, out var shift))
            {
                shift.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Shift = shift;
            }


        }

        foreach (var id in nullIDs)
        {
            EmployeeRosterDict.Remove(id);
        }
    }

    /// <summary>
    /// Converts the contained array of rosters into a DataTable.
    /// </summary>
    /// <returns></returns>
    public DataTable DataTable()
    {
        var table = new DataTable();

        // Establish columns.
        table.Columns.Add("NAME");
        for (var i = 0; i < 7; i++)
            table.Columns.Add(StartDate.AddDays(i).ToString("dd-MMM"));

        // Day of week row.
        var row = table.NewRow();
        for (var i = 0; i < 7; i++)
        {
            row[i] = StartDate.AddDays(i).ToString("dddd").ToUpper();
        }

        table.Rows.Add(row);

        // Fill in employees.
        foreach (var employeeRoster in EmployeeRosters)
        {
            row = table.NewRow();
            var c = 0;
            row[c] = employeeRoster.Employee?.FullName ?? "";

            for (var d = 0; d < 7; d++)
            {
                c++;
                row[c] = employeeRoster.GetDaily(StartDate.AddDays(d).DayOfWeek)?.ToString();
            }

            table.Rows.Add(row);
        }

        return table;
    }

    public override string ToString() => Name;

    public void GenerateRosterAssignments()
    {
        ApplyShiftRules();
        AssignDefaults();
        CountToTargets();
        ApplyDepartmentDefault();
        CheckPublicHolidays();
    }

    private void CheckPublicHolidays()
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Applies shift rules to employees and their weekly/daily shifts as appropriate.
    /// </summary>
    private void ApplyShiftRules()
    {
        foreach (var employeeRoster in EmployeeRosters)
        {
            // TODO: 
            employeeRoster.ApplyShiftRules();
        }
    }

    /// <summary>
    /// Assigns shifts to employees based on their defined defaults - only if they do not already have assigned shifts..
    /// </summary>
    public void AssignDefaults()
    {
        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.Shift is null))
            employeeRoster.SetDefault();
    }

    /// <summary>
    /// Attempt to reach the targeted number for each shift.
    /// </summary>
    public void CountToTargets()
    {
        if (Department is null) throw new DataException("Department Roster has null value for department.");

        // Get every non-default shift with a target above 0.
        var targetShifts = Department.Shifts.Where(s => !s.Default && s.DailyTarget > 0)
            .ToDictionary(s => s, _ => new List<EmployeeRoster>());

        foreach (var (shift, _) in targetShifts)
            targetShifts[shift] = EmployeeRosters
                .Where(er => er.Employee is not null && er.Employee.Shifts.Contains(shift)).ToList();

        // Order by most needed (discrepancy between those available and number required to reach target.
        targetShifts = targetShifts.OrderBy(s => s.Value.Count - ShiftCounterDict[s.Key.ID].Discrepancy)
            .ToDictionary(e => e.Key, e => e.Value);

        foreach (var (shift, empRosters) in targetShifts)
        {
            if (ShiftCounterDict[shift.ID].Discrepancy <= 0) continue;
            // Randomize employees #TODO: Check against history to rotate through staff (instead of randomizing).
            empRosters.Shuffle();

            foreach (var employeeRoster in empRosters.Where(employeeRoster => employeeRoster.Shift is null).TakeWhile(_ => ShiftCounterDict[shift.ID].Lacking))
                employeeRoster.Shift = shift;
        }
    }

    /// <summary>
    /// Checks all employees to see if they are assigned. If they aren't, use department defaults if they exist,
    /// otherwise assign a shift that they are eligible for that is closest to its target count.
    /// </summary>
    public void ApplyDepartmentDefault()
    {
        foreach (var employeeRoster in EmployeeRosters.Where(employeeRoster => employeeRoster.Shift is null))
        {
            if (DefaultShift is not null)
            {
                employeeRoster.Shift = DefaultShift;
                continue;
            }

            if (employeeRoster.Employee is null)
                throw new DataException("Employee roster does not have employee initialized.");

            var bestShift = ShiftCounters.First(c => c.Discrepancy == ShiftCounters.Min(counter => counter.Discrepancy)).Shift;

            employeeRoster.Shift = bestShift;
        }
    }

    /// <summary>
    /// Get all Daily rosters within a single data structure.
    /// </summary>
    /// <returns>IEnumerable of Daily Roster. Should always be 7 (representing Mon-Sun)</returns>
    public IEnumerable<DailyRoster> DailyRosters()
    {
        if (Department is null) throw new DataException("Department Roster initialized without Department.");

        return new List<DailyRoster>
        {
            MondayRoster ?? new DailyRoster(Department, this, StartDate),
            TuesdayRoster ?? new DailyRoster(Department, this, StartDate.AddDays(1)),
            WednesdayRoster ?? new DailyRoster(Department, this, StartDate.AddDays(2)),
            ThursdayRoster ?? new DailyRoster(Department, this, StartDate.AddDays(3)),
            FridayRoster ?? new DailyRoster(Department, this, StartDate.AddDays(4)),
            SaturdayRoster ?? new DailyRoster(Department, this, StartDate.AddDays(5)),
            SundayRoster ?? new DailyRoster(Department, this, StartDate.AddDays(6))
        };
    }

    public IEnumerable<DailyShiftCounter> DailyShiftCounters()
    {
        return DailyRosters().SelectMany(dr => dr.ShiftCounters);
    }

    public IEnumerable<DailyShiftCounter> DailyShiftCounters(string shiftID)
    {
        return DailyRosters().SelectMany(dr => dr.ShiftCounters).Where(dc => dc.ShiftID == shiftID);
    }
}