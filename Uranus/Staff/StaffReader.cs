using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Uranus.Staff.Model;

namespace Uranus.Staff;

public class StaffReader
{
    private StaffChariot Chariot { get; }

    public StaffReader(ref StaffChariot chariot)
    {
        Chariot = chariot;
    }

    /* DIRECTORIES */
    public string BaseDirectory => Chariot.BaseDataDirectory;
    public string EmployeeIconDirectory => Chariot.EmployeeIconDirectory;
    public string EmployeeAvatarDirectory => Chariot.EmployeeAvatarDirectory;
    public string ProjectIconDirectory => Chariot.ProjectIconDirectory;
    public string LicenceImageDirectory => Chariot.LicenceImageDirectory;

    /* EMPLOYEES */
    public Employee? Employee(int id, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Employee>(id, pullType);

    /// <summary>
    /// Gets the employee with all appropriate relationships loaded.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Employee? EmployeeLogIn(int id)
    {
        EmployeeDataSet().Employees.TryGetValue(id, out var employee);
        return employee;
    }

    public List<Employee> Employees(Expression<Func<Employee, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public bool EmployeeExists(int id) => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM Employee WHERE ID=?;", id) > 0;

    public int EmployeeCount() => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM Employee;") ?? 0;

    public Role? Role(string roleName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Role>(roleName, pullType);

    public IEnumerable<Role> Roles(Expression<Func<Role, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<ClockEvent> ClockEvents(Expression<Func<ClockEvent, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<ClockEvent> ClockEvents(DateTime startDate, DateTime endDate)
    {
        return Chariot.Database?.Query<ClockEvent>(
            "SELECT * FROM ClockEvent WHERE Date BETWEEN ? AND ?;",
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"))
               ?? new List<ClockEvent>();
    }

    public List<ClockEvent> ClockEvents(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        return Chariot.Database?.Query<ClockEvent>
        ($"SELECT * FROM ClockEvent WHERE EmployeeID in ({string.Join(", ", employeeIDs)}) AND Date BETWEEN ? AND ?;",
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"))
            ?? new List<ClockEvent>();
    }

    public List<ShiftEntry> ShiftEntries(Expression<Func<ShiftEntry, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<int> EmployeeIDs() => Chariot.Database?.Query<Employee>("SELECT DISTINCT ID FROM Employee;").Select(e => e.ID).OrderBy(c => c).ToList() ?? new List<int>();

    public void AionEmployeeRefresh(out List<int> managerIDList, out List<Employee> employeeList, out IEnumerable<string> locationList,
        out IEnumerable<string> payPointList, out IEnumerable<EEmploymentType> employmentTypeList, out IEnumerable<Role> roleList, out IEnumerable<Department> departmentList)
    {
        IEnumerable<int> managerIDs = new List<int>();
        IEnumerable<Employee> employees = new List<Employee>();
        IEnumerable<string> locations = new List<string>();
        IEnumerable<string> payPoints = new List<string>();
        IEnumerable<EEmploymentType> employmentTypes = new List<EEmploymentType>();
        IEnumerable<Role> roles = new List<Role>();
        IEnumerable<Department> departments = new List<Department>();

        Chariot.Database?.RunInTransaction(() =>
        {
            managerIDs = Chariot.Database?.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;").Select(e => e.ReportsToID) ?? managerIDs;
            employees = Chariot.Database?.Table<Employee>() ?? employees;
            locations = Chariot.Database?.Query<Employee>("SELECT DISTINCT Location FROM Employee;").Select(e => e.Location) ?? locations;
            payPoints = Chariot.Database?.Query<Employee>("SELECT DISTINCT PayPoint FROM Employee;").Select(e => e.PayPoint) ?? payPoints;
            employmentTypes = Chariot.Database?.Query<Employee>("SELECT DISTINCT EmploymentType FROM Employee;").Select(e => e.EmploymentType) ?? employmentTypes;
            roles = Chariot.PullObjectList<Role>().OrderBy(r => r.Name);
            departments = Chariot.Database?.Table<Department>() ?? departments;
        });

        managerIDList = managerIDs.ToList();
        employeeList = employees.ToList();
        locationList = locations;
        payPointList = payPoints;
        employmentTypeList = employmentTypes;
        roleList = roles;
        departmentList = departments;
    }

    public List<int> GetManagerIDs() => Chariot.Database?.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;").Select(e => e.ReportsToID).ToList() ?? new List<int>();

    public IEnumerable<string> Locations() => Chariot.Database?.Query<Employee>("SELECT DISTINCT Location FROM Employee;").Select(e => e.Location) ?? new List<string>();

    public IEnumerable<Employee> GetManagedEmployees(int managerID)
    {
        var fullEmployees = EmployeesRecursiveReports().ToDictionary(e => e.ID, e => e);

        if (!fullEmployees.TryGetValue(managerID, out var manager)) return new List<Employee>();

        fullEmployees.Clear();

        GetReports(manager, ref fullEmployees);

        return fullEmployees.Select(d => d.Value);
    }

    /// <summary>
    /// Recursively go through the employees and their reports to return a full dictionary of
    /// employees.
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="returnDict"></param>
    private static void GetReports(Employee employee, ref Dictionary<int, Employee> returnDict)
    {
        foreach (var report in employee.Reports)
        {
            if (returnDict.ContainsKey(report.ID)) continue;
            returnDict.Add(report.ID, report);
            GetReports(report, ref returnDict);
        }
    }

    /// <summary>
    /// In a single transaction, pulls all the data required for all employees and inserts it into a single data object.
    /// </summary>
    /// <returns>Employee data set, containing Departments, Roles, etc. - with established relationships.</returns>
    public EmployeeDataSet EmployeeDataSet()
    {
        try
        {
            EmployeeDataSet? data = null;
            Chariot.Database?.RunInTransaction(() =>
            {
                var employees = Chariot.PullObjectList<Employee>();
                var departments = Chariot.PullObjectList<Department>();
                var departmentRosters = Chariot.PullObjectList<DepartmentRoster>();
                var roles = Chariot.PullObjectList<Role>();
                var clans = Chariot.PullObjectList<Clan>();
                var icons = EmployeeIcons();
                var avatars = EmployeeAvatars();
                var shifts = Chariot.PullObjectList<Shift>();
                var breaks = Chariot.PullObjectList<Break>();
                data = new EmployeeDataSet(employees, departments, departmentRosters, clans, roles, icons, avatars, shifts, breaks);
            });
            if (data is not null) return data;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull the EmployeeDataSet");
        }
        return new EmployeeDataSet();
    }

    public DepartmentRoster? DepartmentRoster(string rosterName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<DepartmentRoster>(rosterName, pullType);

    public IEnumerable<DepartmentRoster?> DepartmentRosters(string departmentName) 
        => Chariot.Database?.Query<DepartmentRoster>("SELECT * FROM DepartmentRoster WHERE DepartmentName = ?;", departmentName) ?? new List<DepartmentRoster>();

    public IEnumerable<DepartmentRoster> DepartmentRosters(Expression<Func<DepartmentRoster, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => Chariot.PullObjectList(filter, pullType);

    public void FillDepartmentRoster(DepartmentRoster departmentRoster)
    {
        try
        {
            var departmentName = departmentRoster.DepartmentName;

            var startDate = departmentRoster.StartDate;
            // startDate should be monday, but it might not be, so account for that possibility.
            // Earliest date should cover up to 2 weeks before hand.
            var earliestDate = startDate.AddDays(DayOfWeek.Monday - startDate.DayOfWeek - 14);
            // Latest date should be the sunday after the earliest.
            var latestDate = earliestDate.AddDays(DayOfWeek.Saturday - earliestDate.DayOfWeek + 1);

            // Declare variables.
            List<Employee>? employees = null;
            List<Roster>? rosters = null;
            List<DailyRoster>? dailyRosters = null;
            List<EmployeeRoster>? employeeRosters = null;
            List<Shift>? shifts = null;
            List<Break>? breaks = null;
            List<EmployeeShift>? employeeShiftConnections = null;
            List<ShiftRule>? shiftRules = null;

            Chariot.Database?.RunInTransaction(() =>
            {
                //employees = Chariot.PullObjectList<Employee>(e => e.DepartmentName == departmentName && e.EmploymentType != EEmploymentType.SA);
                employees = EmployeeDataSet().Employees.Values.Where(e => e.DepartmentName == departmentName && e.EmploymentType != EEmploymentType.SA).ToList();
                rosters = Chariot.PullObjectList<Roster>(r =>
                    r.DepartmentName == departmentName && r.Date >= earliestDate && r.Date <= latestDate);
                dailyRosters = Chariot.PullObjectList<DailyRoster>(r =>
                        r.DepartmentName == departmentName && r.Date >= earliestDate && r.Date <= latestDate);
                employeeRosters = Chariot.PullObjectList<EmployeeRoster>(r =>
                    r.DepartmentName == departmentName && r.StartDate >= earliestDate && r.StartDate <= latestDate);
                shifts = Chariot.PullObjectList<Shift>(s => s.DepartmentName == departmentName);
                breaks = Chariot.PullObjectList<Break>();
                employeeShiftConnections = Chariot.PullObjectList<EmployeeShift>();
                shiftRules = Chariot.PullObjectList<ShiftRule>();
            });

            // Assign variables that may have been missed.
            employees ??= new List<Employee>();
            rosters ??= new List<Roster>();
            dailyRosters ??= new List<DailyRoster>();
            employeeRosters ??= new List<EmployeeRoster>();
            shifts ??= new List<Shift>();
            breaks ??= new List<Break>();
            employeeShiftConnections ??= new List<EmployeeShift>();
            shiftRules ??= new List<ShiftRule>();

            departmentRoster.SetData(employees, rosters, dailyRosters, employeeRosters, shifts, breaks, employeeShiftConnections, shiftRules);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull Roster Data Set.");
        }
    }

    public RosterDataSet RosterDataSet(string departmentName, DateTime startDate, DateTime endDate)
    {
        try
        {
            RosterDataSet? data = null;
            Chariot.Database?.RunInTransaction(() =>
            {
                var department = Chariot.PullObject<Department>(departmentName);
                if (department is null) return;
                var employees = Chariot.PullObjectList<Employee>(e => e.DepartmentName == department.Name);
                var earliestDate = startDate.AddDays(DayOfWeek.Sunday - startDate.DayOfWeek - 14);
                var latestDate = endDate.AddDays(DayOfWeek.Saturday - endDate.DayOfWeek);
                var rosters = Chariot.PullObjectList<Roster>(r =>
                    r.DepartmentName == department.Name && r.Date >= earliestDate && r.Date <= latestDate);
                var dailyRosters = Chariot.PullObjectList<DailyRoster>(r =>
                    r.DepartmentName == departmentName && r.Date >= earliestDate && r.Date <= latestDate);
                var employeeRosters = Chariot.PullObjectList<EmployeeRoster>(r =>
                    r.DepartmentName == departmentName && r.StartDate >= earliestDate && r.StartDate <= latestDate);
                var shifts = Chariot.PullObjectList<Shift>(s => s.DepartmentName == department.Name);
                var breaks = Chariot.PullObjectList<Break>();
                var employeeShiftConnections = Chariot.PullObjectList<EmployeeShift>();
                var shiftRules = Chariot.PullObjectList<ShiftRule>();
                data = new RosterDataSet(department, startDate, endDate, employees, rosters, dailyRosters, employeeRosters,
                    shifts, breaks, employeeShiftConnections, shiftRules);
            });
            if (data is not null) return data;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull Roster Data Set.");
        }

        return new RosterDataSet();
    }

    public IEnumerable<EmployeeIcon> EmployeeIcons()
    {
        var icons = Chariot.PullObjectList<EmployeeIcon>();
        foreach (var icon in icons)
            icon.SetDirectory(EmployeeIconDirectory);

        return icons;
    }

    public IEnumerable<EmployeeAvatar> EmployeeAvatars()
    {
        var avatars = Chariot.PullObjectList<EmployeeAvatar>();
        foreach (var icon in avatars)
            icon.SetDirectory(EmployeeAvatarDirectory);

        return avatars;
    }

    /// <summary>
    /// Get all the employees that recursively have roles that report to the given role.
    /// </summary>
    /// <param name="headRole">The role of the employee for whom we are gathering reporting employees.</param>
    /// <returns></returns>
    public IEnumerable<Employee> GetReportsByRole(Role headRole)
    {
        var roles = GetReportingRoles(headRole);
        var employees = roles.SelectMany(r => r.Employees);

        return employees;
    }

    /// <summary>
    /// Get all the reporting roles based on the given role.
    /// </summary>
    /// <param name="headRole"></param>
    /// <returns>A collection of roles, each of which containing the relevant employees.</returns>
    public IEnumerable<Role> GetReportingRoles(Role headRole)
    {
        Dictionary<string, Role>? roleDict = null;
        Dictionary<string, List<Employee>>? employeeDict = null;

        try
        {
            Chariot.Database?.RunInTransaction(() =>
            {
                roleDict = Chariot.PullObjectList<Role>()
                    .ToDictionary(r => r.Name, r => r);
                employeeDict = Employees(pullType: EPullType.IncludeChildren)
                    .GroupBy(e => e.RoleName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unable to pull Roles and/or Employees from {}, defaulting to empty.", Chariot.DatabaseName);
            roleDict = new Dictionary<string, Role>();
            employeeDict = new Dictionary<string, List<Employee>>();
        }

        if (roleDict is null || employeeDict is null) return new List<Role>();

        foreach (var (_, role) in roleDict)
        {
            if (roleDict.TryGetValue(role.ReportsToRoleName, out var upperRole))
                upperRole.AddReportingRole(role);
        }

        foreach (var (roleName, employees) in employeeDict)
        {
            if (roleDict.TryGetValue(roleName, out var role))
                role.AddEmployees(employees);
        }

        if (!roleDict.TryGetValue(headRole.Name, out var newHeadRole)) return new List<Role>();

        roleDict.Clear();

        GetRoleReports(newHeadRole, ref roleDict);

        return roleDict.Values;

    }

    /// <summary>
    /// Recursively go through the roles and their reports to return a full dictionary of
    /// roles.
    /// </summary>
    /// <param name="role">The role as a starting point to get reports.</param>
    /// <param name="returnDict">A dictionary of reports</param>
    private static void GetRoleReports(Role role, ref Dictionary<string, Role> returnDict)
    {
        foreach (var report in role.Reports)
        {
            if (returnDict.ContainsKey(report.Name)) continue;
            returnDict.Add(report.Name, report);
            GetRoleReports(report, ref returnDict);
        }
    }

    public IEnumerable<Employee> EmployeesRecursiveReports()
    {
        var fullEmployees = Chariot.Database?.Table<Employee>().ToDictionary(e => e.ID, e => e) ?? new Dictionary<int, Employee>();

        foreach (var (_, employee) in fullEmployees)
        {
            if (!fullEmployees.TryGetValue(employee.ReportsToID, out var manager)) continue;
            manager.Reports.Add(employee);
            employee.ReportsTo = manager;
        }

        return fullEmployees.Select(e => e.Value);
    }

    public List<ShiftEntry> GetFilteredEntries(DateTime minDate, DateTime maxDate)
    {
        Dictionary<int, Employee>? employees = null;
        List<ShiftEntry>? entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        Chariot.Database?.RunInTransaction(() =>
        {
            employees = Employees().ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database?.Query<ShiftEntry>("SELECT DailyEntry.* FROM DailyEntry JOIN Employee E on DailyEntry.EmployeeID = E.Code WHERE Date BETWEEN ? AND ?;",
                startString, endString).ToList();

        });

        employees ??= new Dictionary<int, Employee>();
        entries ??= new List<ShiftEntry>();

        foreach (var shiftEntry in entries)
        {
            if (employees.TryGetValue(shiftEntry.EmployeeID, out var employee))
                shiftEntry.Employee = employee;
        }

        return entries;
    }
    public List<ShiftEntry> GetFilteredEntries(DateTime minDate, DateTime maxDate, Employee manager)
    {
        return GetFilteredEntries(minDate, maxDate, manager.ID);
    }
    public List<ShiftEntry> GetFilteredEntries(DateTime minDate, DateTime maxDate, int managerID)
    {
        Dictionary<int, Employee>? employees = null;
        List<ShiftEntry>? entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        Chariot.Database?.RunInTransaction(() =>
        {
            employees = GetManagedEmployees(managerID).ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database?.Query<ShiftEntry>("SELECT * FROM ShiftEntry " +
                                                         $"WHERE EmployeeID IN ({string.Join(", ", employees.Select(d => d.Value.ID))}) " +
                                                         "AND Date BETWEEN ? AND ?;",
                startString, endString).ToList();

        });

        employees ??= new Dictionary<int, Employee>();
        entries ??= new List<ShiftEntry>();

        foreach (var shiftEntry in entries)
        {
            if (employees.TryGetValue(shiftEntry.EmployeeID, out var employee))
                shiftEntry.Employee = employee;
        }

        return entries;
    }

    public IEnumerable<ClockEvent> GetPendingClocks(int managerCode, DateTime fromDate, DateTime toDate)
    {
        return Chariot.Database?.Query<ClockEvent>(
            "SELECT ClockEvent.* FROM ClockEvent JOIN Employee ON ClockEvent.EmployeeID = Employee.ID " +
            "WHERE Employee.ReportsToID = ? " +
            "AND ClockEvent.Status = ?" +
            "AND Date BETWEEN ? AND ?;",
            managerCode, EClockStatus.Pending,
            fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"))
               ?? new List<ClockEvent>();
    }

    public int GetPendingCount(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        return Chariot.Database?.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM ClockEvent " +
            $"WHERE EmployeeID IN ({string.Join(", ", employeeIDs)}) AND " +
            "Status = ? AND " +
            $"Date BETWEEN '{startDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}';",
            EClockStatus.Pending) ?? 0;
    }

    /// <summary>
    /// Gets all employees that have at least one other employee as a direct report.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Employee> GetManagers()
    {
        IEnumerable<Employee>? managers = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            var managerIDs = GetManagerIDs();
            managers = Chariot.Database?.Query<Employee>(
                $"SELECT * FROM Employee WHERE ID IN ({string.Join(", ", managerIDs)});");
        });

        return managers ?? new List<Employee>();
    }

    /// <summary>
    /// Gets a full list of today's clock events for the given employee code.
    /// </summary>
    /// <returns></returns>
    public List<ClockEvent> ClocksForToday(int employeeID)
    {
        return Chariot.Database?.Query<ClockEvent>("SELECT * FROM ClockEvent WHERE EmployeeID = ? AND Date = ? AND Status <> ?;",
            employeeID, DateTime.Now.ToString("yyyy-MM-dd"), EClockStatus.Deleted) ?? new List<ClockEvent>();
    }

    /// <summary>
    /// Gets count of clocks for today.
    /// </summary>
    /// <param name="employeeCode"></param>
    /// <returns></returns>
    public int GetClockCount(int employeeCode)
    {
        return ClocksForToday(employeeCode).Count;
    }

    /// <summary>
    /// Returns a list of all employees that have direct reports.
    /// </summary>
    /// <returns>List of Employees</returns>
    public List<Employee> Managers()
    {
        var conn = Chariot.Database;
        //List<int> employeeIDs = conn.Query<int>("SELECT DISTINCT ReportsToID FROM Employee;");
        var employeeIDs = conn?.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;").Select(e => e.ReportsToID).ToList() ?? new List<int>();

        return conn?.Query<Employee>($"SELECT * FROM Employee WHERE ID IN ({string.Join(", ", employeeIDs)});") ?? new List<Employee>();
    }

    /// <summary>
    /// Gets all of the shifts applicable to the given employee - based on department.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<Shift> Shifts(Employee employee) => Chariot.PullObjectList<Shift>(s => s.DepartmentName == employee.DepartmentName);

    public IEnumerable<EmployeeShift> EmployeeShifts(Employee employee) => Chariot.PullObjectList<EmployeeShift>(es => es.EmployeeID == employee.ID);

    public IEnumerable<EmployeeShift> EmployeeShifts(Shift shift) => Chariot.PullObjectList<EmployeeShift>(es => es.ShiftID == shift.ID);

    public IEnumerable<EmployeeIcon> Icons(Expression<Func<EmployeeIcon, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* DEPARTMENTS */
    public List<Department> Departments(Expression<Func<Department, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public Department? Department(string departmentName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Department>(departmentName, pullType);

    /// <summary>
    /// Pulls all relevant sub departments according to the given department name.
    /// Will also fill relevant department data, such as shifts.
    /// </summary>
    /// <param name="departmentName"></param>
    /// <returns></returns>
    public IEnumerable<Department> SubDepartments(string departmentName)
    {
        Dictionary<string, Department>? deptDict = null;
        Dictionary<string, List<Shift>>? shiftDict = null;
        Dictionary<string, List<Break>>? breakDict = null;

        try
        {
            Chariot.Database?.RunInTransaction(() =>
            {
                deptDict = Chariot.PullObjectList<Department>().ToDictionary(d => d.Name, d => d);
                shiftDict = Chariot.PullObjectList<Shift>()
                    .GroupBy(s => s.DepartmentName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                breakDict = Chariot.PullObjectList<Break>()
                    .GroupBy(b => b.ShiftName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull shift and/or department data from {}. Defaulted to null values.", Chariot.DatabaseName);
            deptDict = new Dictionary<string, Department>();
            shiftDict = new Dictionary<string, List<Shift>>();
            breakDict = new Dictionary<string, List<Break>>();
        }

        if (deptDict is null || shiftDict is null || breakDict is null) return Array.Empty<Department>();

        foreach (var shift in shiftDict.SelectMany(d => d.Value))
        {
            if (!breakDict.TryGetValue(shift.Name, out var breaks)) continue;

            shift.SetBreaks(breaks);
            foreach (var @break in breaks)
            {
                @break.Shift = shift;
            }
        }

        foreach (var (deptName, shifts) in shiftDict)
        {
            if (deptDict.TryGetValue(deptName, out var department))
                department.AddShifts(shifts);
        }

        foreach (var (_, department) in deptDict)
        {
            if (deptDict.TryGetValue(department.OverDepartmentName, out var overDepartment))
                overDepartment.AddSubDepartment(department);
        }

        if (!deptDict.TryGetValue(departmentName, out var headDepartment)) return new List<Department>();

        deptDict.Clear();

        GetSubDepartments(headDepartment, ref deptDict);

        deptDict.Add(departmentName, headDepartment);

        return deptDict.Select(d => d.Value);
    }

    /// <summary>
    /// Gets a dict of departments that are all  recursively under the given (potential) head department.
    /// </summary>
    /// <param name="department"></param>
    /// <param name="returnDict"></param>
    private static void GetSubDepartments(Department department, ref Dictionary<string, Department> returnDict)
    {
        foreach (var sub in department.SubDepartments)
        {
            if (returnDict.ContainsKey(sub.Name)) continue;
            returnDict.Add(sub.Name, sub);
            GetSubDepartments(sub, ref returnDict);
        }
    }

    /* PROJECTS */
    public IEnumerable<Project> Projects(Expression<Func<Project, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType).OrderBy(p => p.Name);

    public AionDataSet GetAionDataSet()
    {
        AionDataSet newSet = new();
        Chariot.Database?.RunInTransaction(() =>
        {
            newSet.ClockEvents = ClockEvents().ToDictionary(c => c.ID, c => c);
            newSet.Employees = Employees().ToDictionary(e => e.ID, e => e);
            newSet.ShiftEntries = ShiftEntries().ToDictionary(e => e.ID, e => e);
        });
        return newSet;
    }
}