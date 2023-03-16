using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.DepartmentRosters), CascadeOperations = CascadeOperation.CascadeRead)]
    public Department? Department { get; set; }

    [OneToMany(nameof(Roster.DepartmentRosterID), nameof(Roster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }
    [OneToMany(nameof(EmployeeRoster.DepartmentRosterID), nameof(EmployeeRoster.DepartmentRoster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<EmployeeRoster> EmployeeRosters { get; set; }
    [OneToMany(nameof(WeeklyShiftCounter.RosterID), nameof(WeeklyShiftCounter.Roster), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
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

    [Ignore] public Dictionary<int, Employee> EmployeeDict { get; set; }    // All Employees for the department.
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

    [Ignore] public List<Employee> ActiveEmployees { get; }        // Employees that are both active and not salary - and therefore should have a roster associated.
    [Ignore] public List<Employee> EmployeesWithoutRoster { get; } // Active employees that should, but do not, have a roster.

    [Ignore] public Shift? DefaultShift { get; set; }

    [Ignore] public bool IsLoaded { get; set; }

    [Ignore] public List<Shift> Shifts => Department?.Shifts ?? new List<Shift>();

    /// <summary>
    /// Create basic DepartmentRoster object. Empty.
    /// </summary>
    public DepartmentRoster()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Rosters = new List<Roster>();
        EmployeeRosters = new List<EmployeeRoster>();
        ShiftCounters = new List<WeeklyShiftCounter>();

        EmployeeDict = new Dictionary<int, Employee>();
        ActiveEmployees = new List<Employee>();
        EmployeesWithoutRoster = new List<Employee>();
        ShiftDict = new Dictionary<string, Shift>();
        BreakDict = new Dictionary<string, List<Break>>();
        RosterDict = new Dictionary<(int, DateTime), Roster>();
        EmployeeRosterDict = new Dictionary<Guid, EmployeeRoster>();
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = new Dictionary<int, List<ShiftRuleSingle>>();
        RecurringRuleDict = new Dictionary<int, List<ShiftRuleRecurring>>();
        RosterRuleDict = new Dictionary<int, List<ShiftRuleRoster>>();
        ShiftCounterDict = new Dictionary<string, WeeklyShiftCounter>();
        DailyCounterDict = new Dictionary<Guid, Dictionary<string, DailyShiftCounter>>();
    }

    /// <summary>
    /// Create a Department Roster from scratch. Fill anything and everything that can be filled from given data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="startDate"></param>
    /// <param name="useSaturdays"></param>
    /// <param name="useSundays"></param>
    /// <param name="department">How full this is determines how complete the roster will be.</param>
    public DepartmentRoster(string name, DateTime startDate, bool useSaturdays, bool useSundays, Department department)
    {
        ID = Guid.NewGuid();
        Name = name;
        StartDate = startDate;
        UseSaturdays = useSaturdays;
        UseSundays = useSundays;
        Department = department;
        DepartmentName = department.Name;

        ActiveEmployees = department.Employees.Where(e => e.IsActive && e.EmploymentType != EEmploymentType.SA).ToList();
        EmployeeRosters = ActiveEmployees.Select(e => new EmployeeRoster(department, this, e, startDate)).ToList();
        EmployeesWithoutRoster = new List<Employee>();

        ShiftCounters = department.Shifts.Select(s => new WeeklyShiftCounter(this, s, s.DailyTarget)).ToList();

        EmployeeDict = department.Employees.ToDictionary(e => e.ID, e => e);
        ShiftDict = department.Shifts.ToDictionary(s => s.Name, s => s);
        BreakDict = department.Shifts.ToDictionary(s => s.Name, s => s.Breaks);
        EmployeeRosterDict = EmployeeRosters.ToDictionary(er => er.ID, er => er);
        EmpShiftConnections = new List<EmployeeShift>();
        SingleRuleDict = department.Employees.ToDictionary(e => e.ID, e => e.SingleRules);
        RecurringRuleDict = department.Employees.ToDictionary(e => e.ID, e => e.RecurringRules);
        RosterRuleDict = department.Employees.ToDictionary(e => e.ID, e => e.RosterRules);
        ShiftCounterDict = ShiftCounters.ToDictionary(c => c.ShiftID, c => c);

        DailyCounterDict = new Dictionary<Guid, Dictionary<string, DailyShiftCounter>>();

        FillMissingDailyRosters();
        CreateDailyCounterDict();

        Rosters = new List<Roster>();

        foreach (var employeeRoster in EmployeeRosters)
        {
            foreach (DayOfWeek value in Enum.GetValues(typeof(DayOfWeek)))
            {
                var daily = GetDaily(value);
                if (daily is null) continue;

                var roster = new Roster(this, employeeRoster, daily);

                employeeRoster.SetDaily(roster);
                Rosters.Add(roster);
            }
        }

        RosterDict = Rosters.ToDictionary(r => (r.EmployeeID, r.Date), r => r);

        IsLoaded = true;
    }

    public void SetData(IEnumerable<Employee> employees, IEnumerable<Roster> rosters, IEnumerable<DailyRoster> dailyRosters, IEnumerable<EmployeeRoster> employeeRosters,
        IEnumerable<Shift> shifts, IEnumerable<Break> breaks, IEnumerable<EmployeeShift> empShiftCons,
        IEnumerable<ShiftRuleSingle> singleRules, IEnumerable<ShiftRuleRecurring> recurringRules, IEnumerable<ShiftRuleRoster> rosterRules,
        IEnumerable<WeeklyShiftCounter> weeklyShiftCounters, IEnumerable<DailyShiftCounter> dailyShiftCounters)
    {
        var employeeList = employees.ToList();

        EmployeeRosters = employeeRosters.ToList();

        ActiveEmployees.Clear();
        ActiveEmployees.AddRange(employeeList.Where(e => e.IsActive && e.EmploymentType != EEmploymentType.SA));
        CheckEmployeeRosters();
        EmployeeDict = employeeList.ToDictionary(e => e.ID, e => e);

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
        EmployeeRosterDict = EmployeeRosters.ToDictionary(er => er.ID, er => er);

        // Daily Rosters
        dailyRosters = dailyRosters.Where(dr => dr.DepartmentRosterID == ID);
        foreach (var dailyRoster in dailyRosters) SetDaily(dailyRoster);

        DailyCounterDict = dailyShiftCounters.GroupBy(counter => counter.RosterID).ToDictionary(group => group.Key,
            group => group.ToDictionary(counter => counter.ShiftID, counter => counter));

        ShiftCounterDict = weeklyShiftCounters.Where(c => c.RosterID == ID).ToDictionary(c => c.ShiftID, c => c);
        ShiftCounters = ShiftCounterDict.Values.ToList();

        SetRelationships();
        EmployeeRosters.Sort();

        IsLoaded = true;
    }

    private void CreateDailyCounterDict()
    {
        DailyCounterDict = DailyCounterList().GroupBy(counter => counter.RosterID).ToDictionary(group => group.Key,
            group => group.ToDictionary(counter => counter.ShiftID, counter => counter));
    }

    private IEnumerable<DailyShiftCounter> DailyCounterList() => DailyRosterList().SelectMany(dr => dr.ShiftCounters).ToList();

    private IEnumerable<DailyRoster> DailyRosterList() => new List<DailyRoster?>
    {
        MondayRoster, TuesdayRoster, WednesdayRoster, ThursdayRoster,
        FridayRoster, SaturdayRoster, SundayRoster
    }.Where(dailyRoster => dailyRoster is not null).ToList()!;

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
    /// <returns>The newly created Employee Rosters.</returns>
    public IEnumerable<EmployeeRoster> CreateMissingRosters(IEnumerable<Employee>? employees = null)
    {
        if (Department is null) throw new DataException("Department Roster initialized without Department.");

        var returnList = new List<EmployeeRoster>();

        FillMissingDailyRosters();

        var createList = employees ?? EmployeesWithoutRoster;

        // Convert EmpRoster list to dictionary to compare to our employee list (which should be all required for the department).
        var empRosterDict = EmployeeRosters.ToDictionary(er => er.EmployeeID, er => er);

        // Create employeeRoster for each employee - only for those that do not already exist.
        foreach (var employee in createList)
        {
            if (empRosterDict.TryGetValue(employee.ID, out _)) continue;

            EmployeeRoster newEmployeeRoster = new(Department, this, employee, StartDate);

            // Get Roster for each day.
            for (var i = 0; i < 7; i++)
            {
                var date = StartDate.AddDays(i);
                // Make sure that we are not re-creating a dangling (existing but unattached) roster.
                if (!RosterDict.TryGetValue((employee.ID, date), out var roster))
                {
                    roster = new Roster(Department, this, newEmployeeRoster, employee, date);
                    RosterDict.Add((roster.EmployeeID, date), roster);
                    Rosters.Add(roster);
                }
                // Attach DailyRoster as appropriate.
                var dailyRoster = GetDaily(date.DayOfWeek);
                if (dailyRoster is null) continue;
                newEmployeeRoster.SetDaily(roster);
                dailyRoster.AddRoster(roster);
                roster.DailyRoster = dailyRoster;
            }

            returnList.Add(newEmployeeRoster);
            EmployeeRosterDict.Add(newEmployeeRoster.ID, newEmployeeRoster);
            EmployeeRosters.Add(newEmployeeRoster);
        }

        CheckEmployeeRosters();

        return returnList;
    }

    /// <summary>
    /// Check employees against employee rosters to determine which employees do not have rosters.
    /// </summary>
    private void CheckEmployeeRosters()
    {
        EmployeesWithoutRoster.Clear();
        EmployeesWithoutRoster.AddRange(ActiveEmployees.Where(e => !EmployeeRosters.Select(er => er.EmployeeID).Contains(e.ID)));
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
                if (roster.Shift is not null) ShiftCounter(roster.Shift).Count++;
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
        foreach (var dailyRoster in DailyRosters())
        {
            if (dailyRoster.DepartmentName == DepartmentName)
            {
                dailyRoster.Department = Department;
                Department?.DailyRosters.Add(dailyRoster);
            }

            DailyCounterDict.TryGetValue(dailyRoster.ID, out var counters);
            counters ??= new Dictionary<string, DailyShiftCounter>();

            // Make sure that there is a shift counter for each shift for each day.
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
        if (UseSaturdays) SaturdayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(5));
        if (UseSundays) SundayRoster ??= new DailyRoster(Department, this, StartDate.AddDays(6));

        MondayRosterID = MondayRoster.ID;
        TuesdayRosterID = TuesdayRoster.ID;
        WednesdayRosterID = WednesdayRoster.ID;
        ThursdayRosterID = ThursdayRoster.ID;
        FridayRosterID = FridayRoster.ID;
        SaturdayRosterID = SaturdayRoster?.ID ?? Guid.Empty;
        SundayRosterID = SundayRoster?.ID ?? Guid.Empty;
    }

    private void SetFromEmployeeRosters()
    {
        var nullIDs = new List<Guid>();
        foreach (var employeeRoster in EmployeeRosters)
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

            if (ShiftDict.TryGetValue(employeeRoster.ShiftID, out var shift))
            {
                shift.EmployeeRosters.Add(employeeRoster);
                employeeRoster.Shift = shift;
            }

            if (employeeRoster.DepartmentRosterID == ID) employeeRoster.DepartmentRoster = this;

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

    public WeeklyShiftCounter ShiftCounter(Shift shift)
    {
        if (ShiftCounterDict.TryGetValue(shift.ID, out var counter)) return counter;

        counter = new WeeklyShiftCounter(this, shift, 0);
        ShiftCounters.Add(counter);
        ShiftCounterDict.Add(shift.ID, counter);

        return counter;
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

    public IEnumerable<DailyShiftCounter> DailyShiftCounters() => DailyRosters().SelectMany(dr => dr.ShiftCounters);

    public IEnumerable<DailyShiftCounter> DailyShiftCounters(string shiftID) =>
        DailyShiftCounters().Where(dc => dc.ShiftID == shiftID);

}