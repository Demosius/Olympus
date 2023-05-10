using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Uranus.Staff.Models;

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
    /// Pulls a full list of employees that have all appropriate connections established through their roles.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Employee>> EmployeeRoleStackAsync()
    {
        List<Employee>? employees = null;
        List<Role>? roles = null;

        async void Action()
        {
            var empTask = Chariot.PullObjectListAsync<Employee>();
            var roleTask = Chariot.PullObjectListAsync<Role>();

            await Task.WhenAll(empTask, roleTask);

            employees = await empTask;
            roles = await roleTask;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        employees ??= new List<Employee>();
        roles ??= new List<Role>();

        var employeeDict = employees.ToDictionary(e => e.ID, e => e);
        var roleDict = roles.ToDictionary(r => r.Name, r => r);

        foreach (var role in roles)
            if (roleDict.TryGetValue(role.ReportsToRoleName, out var parentRole))
                parentRole.AddReportingRole(role);

        foreach (var employee in employees)
        {
            if (roleDict.TryGetValue(employee.RoleName, out var role)) role.AddEmployee(employee);

            if (!employeeDict.TryGetValue(employee.ReportsToID, out var manager)) continue;

            employee.ReportsTo = manager;
            manager.Reports.Add(employee);
        }

        return employees;
    }

    public async Task<Employee?> RoleStackEmployeeAsync(int id) => (await EmployeeRoleStackAsync()).FirstOrDefault(e => e.ID == id);

    /// <summary>
    /// Gets the employee with all appropriate relationships loaded.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Employee?> EmployeeLogInAsync(int id)
    {
        (await EmployeeDataSetAsync()).Employees.TryGetValue(id, out var employee);
        return employee;
    }

    public async Task<List<Employee>> EmployeesAsync(Expression<Func<Employee, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter ?? (e => e.IsActive), pullType);

    public bool EmployeeExists(int id) => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM Employee WHERE ID=?;", id) > 0;

    public int EmployeeCount() => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM Employee;") ?? 0;

    public Role? Role(string roleName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Role>(roleName, pullType);

    public async Task<IEnumerable<Role>> RolesAsync(Expression<Func<Role, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task<List<ClockEvent>> ClockEventsAsync(Expression<Func<ClockEvent, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public List<ClockEvent> ClockEventsAsync(DateTime startDate, DateTime endDate)
    {
        return Chariot.Database?.Query<ClockEvent>(
            "SELECT * FROM ClockEvent WHERE Date BETWEEN ? AND ?;",
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"))
               ?? new List<ClockEvent>();
    }

    public async Task<List<ClockEvent>> ClockEventsAsync(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        var task = new Task<List<ClockEvent>>(() =>
            Chariot.Database?.Query<ClockEvent>(
                $"SELECT * FROM ClockEvent WHERE EmployeeID in ({string.Join(", ", employeeIDs)}) AND Date BETWEEN ? AND ?;",
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")) ?? new List<ClockEvent>());

        return await task;
    }

    public async Task<List<ShiftEntry>> ShiftEntriesAsync(Expression<Func<ShiftEntry, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public List<int> EmployeeIDs() => Chariot.Database?.Query<Employee>("SELECT DISTINCT ID FROM Employee;").Select(e => e.ID).OrderBy(c => c).ToList() ?? new List<int>();

    /// <summary>
    /// Get the data required for an Aion Employee Refresh.
    /// </summary>
    /// <returns>Tuple: (Manager IDs, employees, locations, payPoints, employmentTypes, roles, departments)</returns>
    public async
        Task<(List<int>, List<Employee>, IEnumerable<string>, IEnumerable<string>, IEnumerable<EEmploymentType>,
            IEnumerable<Role>, IEnumerable<Department>)> AionEmployeeRefreshAsync()
    {
        IEnumerable<int> managerIDs = new List<int>();
        IEnumerable<Employee> employees = new List<Employee>();
        IEnumerable<string> locations = new List<string>();
        IEnumerable<string> payPoints = new List<string>();
        IEnumerable<EEmploymentType> employmentTypes = new List<EEmploymentType>();
        IEnumerable<Role> roles = new List<Role>();
        IEnumerable<Department> departments = new List<Department>();

        async void Action()
        {
            managerIDs = Chariot.Database?.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;")
                .Select(e => e.ReportsToID) ?? managerIDs;
            employees = Chariot.Database?.Table<Employee>() ?? employees;
            locations = Chariot.Database?.Query<Employee>("SELECT DISTINCT Location FROM Employee;")
                .Select(e => e.Location) ?? locations;
            payPoints = Chariot.Database?.Query<Employee>("SELECT DISTINCT PayPoint FROM Employee;")
                .Select(e => e.PayPoint) ?? payPoints;
            employmentTypes = Chariot.Database?.Query<Employee>("SELECT DISTINCT EmploymentType FROM Employee;")
                .Select(e => e.EmploymentType) ?? employmentTypes;
            roles = (await Chariot.PullObjectListAsync<Role>()).OrderBy(r => r.Name);
            departments = Chariot.Database?.Table<Department>() ?? departments;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        var managerIDList = managerIDs.ToList();
        var employeeList = employees.ToList();
        var locationList = locations;
        var payPointList = payPoints;
        var employmentTypeList = employmentTypes;
        var roleList = roles;
        var departmentList = departments;

        return (managerIDList, employeeList, locationList, payPointList, employmentTypeList, roleList, departmentList);
    }

    public IEnumerable<int> GetManagerIDs() => Chariot.Database?.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee WHERE IsActive = ?;", true).Select(e => e.ReportsToID).ToList() ?? new List<int>();

    public IEnumerable<string> Locations() => Chariot.Database?.Query<Employee>("SELECT DISTINCT Location FROM Employee WHERE IsActive = ?;", true).Select(e => e.Location) ?? new List<string>();

    public async Task<IEnumerable<Employee>> GetManagedEmployeesAsync(int managerID)
    {
        var fullEmployees = (await EmployeesRecursiveReportsAsync()).ToDictionary(e => e.ID, e => e);

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
    public async Task<EmployeeDataSet> EmployeeDataSetAsync(bool includeInactive = false)
    {
        try
        {
            EmployeeDataSet? data = null;

            async void Action()
            {
                var empTask = Chariot.PullObjectListAsync<Employee>(e => e.IsActive || includeInactive);
                var depTask = Chariot.PullObjectListAsync<Department>();
                var depRosterTask = Chariot.PullObjectListAsync<DepartmentRoster>();
                var roleTask = Chariot.PullObjectListAsync<Role>();
                var clanTask = Chariot.PullObjectListAsync<Clan>();
                var iconTask = EmployeeIconsAsync();
                var avatarTask = EmployeeAvatarsAsync();
                var shiftTask = Chariot.PullObjectListAsync<Shift>();
                var breakTask = Chariot.PullObjectListAsync<Break>();
                var sglRuleTask = Chariot.PullObjectListAsync<ShiftRuleSingle>();
                var recRuleTask = Chariot.PullObjectListAsync<ShiftRuleRecurring>();
                var rstRuleTask = Chariot.PullObjectListAsync<ShiftRuleRoster>();
                var tagTask = Chariot.PullObjectListAsync<TempTag>();
                var useTask = Chariot.PullObjectListAsync<TagUse>();

                await Task.WhenAll(empTask, depTask, depRosterTask, roleTask, clanTask, iconTask, avatarTask, shiftTask, breakTask, sglRuleTask, recRuleTask, rstRuleTask, tagTask, useTask);

                data = new EmployeeDataSet(await empTask, await depTask, await depRosterTask, await clanTask, await roleTask, await iconTask, await avatarTask, await shiftTask, await breakTask, await sglRuleTask, await recRuleTask, await rstRuleTask, await tagTask, await useTask);
            }

            await new Task(() => Chariot.Database?.RunInTransaction(Action));

            if (data is not null) return data;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull the EmployeeDataSet");
        }
        return new EmployeeDataSet();
    }

    public IEnumerable<string> PayPoints() =>
        Chariot.Database?.Query<Employee>("SELECT DISTINCT PayPoint FROM Employee;").Select(e => e.PayPoint) ??
        new List<string>();

    public DepartmentRoster? DepartmentRoster(string rosterName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<DepartmentRoster>(rosterName, pullType);

    public IEnumerable<DepartmentRoster?> DepartmentRosters(string departmentName)
        => Chariot.Database?.Query<DepartmentRoster>("SELECT * FROM DepartmentRoster WHERE DepartmentName = ?;", departmentName) ?? new List<DepartmentRoster>();

    public async Task<IEnumerable<DepartmentRoster>> DepartmentRostersAsync(Expression<Func<DepartmentRoster, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task FillDepartmentRoster(DepartmentRoster departmentRoster)
    {
        try
        {
            var departmentName = departmentRoster.DepartmentName;

            // Declare variables.
            List<Employee>? employees = null;
            List<Roster>? rosters = null;
            List<DailyRoster>? dailyRosters = null;
            List<EmployeeRoster>? employeeRosters = null;
            List<Shift>? shifts = null;
            List<Break>? breaks = null;
            List<EmployeeShift>? employeeShiftConnections = null;
            List<ShiftRuleSingle>? singleRules = null;
            List<ShiftRuleRecurring>? recurringRules = null;
            List<ShiftRuleRoster>? rosterRules = null;
            List<WeeklyShiftCounter>? weeklyShiftCounters = null;
            List<DailyShiftCounter>? dailyShiftCounters = null;

            async void Action()
            {
                var empTask = EmployeeDataSetAsync(true);
                var rosterTask = Chariot.PullObjectListAsync<Roster>(r => r.DepartmentRosterID == departmentRoster.ID);
                var dailyRosterTask = Chariot.PullObjectListAsync<DailyRoster>(r => r.DepartmentRosterID == departmentRoster.ID);
                var empRosterTask = Chariot.PullObjectListAsync<EmployeeRoster>(r => r.DepartmentRosterID == departmentRoster.ID);
                var shiftTask = Chariot.PullObjectListAsync<Shift>(s => s.DepartmentName == departmentName);
                var empShiftTask = Chariot.PullObjectListAsync<EmployeeShift>();
                var singleRuleTask = Chariot.PullObjectListAsync<ShiftRuleSingle>();
                var recRuleTask = Chariot.PullObjectListAsync<ShiftRuleRecurring>();
                var rosterRuleTask = Chariot.PullObjectListAsync<ShiftRuleRoster>();
                var weeklyCounterTask = Chariot.PullObjectListAsync<WeeklyShiftCounter>(wc => wc.RosterID == departmentRoster.ID);

                await Task.WhenAll(shiftTask, dailyRosterTask);

                shifts = await shiftTask;
                dailyRosters = await dailyRosterTask;
                var shiftIDs = shifts.Select(s => s.ID);
                var dailyIDs = dailyRosters.Select(dr => dr.ID);
                var breakTask = Chariot.PullObjectListAsync<Break>(b => shiftIDs.Contains(b.ShiftID));
                var dailyCounterTask = Chariot.PullObjectListAsync<DailyShiftCounter>(dc => dailyIDs.Contains(dc.RosterID));

                await Task.WhenAll(empTask, rosterTask, empRosterTask, breakTask, empShiftTask, singleRuleTask, recRuleTask, rosterRuleTask, weeklyCounterTask, dailyCounterTask);

                employees = (await empTask).Employees.Values.Where(e => e.DepartmentName == departmentName).ToList();
                rosters = await rosterTask;
                employeeRosters = await empRosterTask;
                breaks = await breakTask;
                employeeShiftConnections = await empShiftTask;
                singleRules = await singleRuleTask;
                recurringRules = await recRuleTask;
                rosterRules = await rosterRuleTask;
                weeklyShiftCounters = await weeklyCounterTask;
                dailyShiftCounters = await dailyCounterTask;
            }

            await new Task(() => Chariot.Database?.RunInTransaction(Action));

            // Assign variables that may have been missed.
            employees ??= new List<Employee>();
            rosters ??= new List<Roster>();
            dailyRosters ??= new List<DailyRoster>();
            employeeRosters ??= new List<EmployeeRoster>();
            shifts ??= new List<Shift>();
            breaks ??= new List<Break>();
            employeeShiftConnections ??= new List<EmployeeShift>();
            singleRules ??= new List<ShiftRuleSingle>();
            recurringRules ??= new List<ShiftRuleRecurring>();
            rosterRules ??= new List<ShiftRuleRoster>();
            weeklyShiftCounters ??= new List<WeeklyShiftCounter>();
            dailyShiftCounters ??= new List<DailyShiftCounter>();

            departmentRoster.SetData(
                employees, rosters, dailyRosters, employeeRosters, shifts, breaks,
                employeeShiftConnections, singleRules, recurringRules, rosterRules,
                weeklyShiftCounters, dailyShiftCounters);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull Roster Data Set.");
        }
    }

    public async Task<RosterDataSet> RosterDataSet(string departmentName, DateTime startDate, DateTime endDate)
    {
        try
        {
            RosterDataSet? data = null;

            async void Action()
            {
                var department = Chariot.PullObject<Department>(departmentName);
                if (department is null) return;

                var empTask = Chariot.PullObjectListAsync<Employee>(e => e.DepartmentName == department.Name && e.IsActive);
                var earliestDate = startDate.AddDays(DayOfWeek.Sunday - startDate.DayOfWeek - 14);
                var latestDate = endDate.AddDays(DayOfWeek.Saturday - endDate.DayOfWeek);
                var rosterTask = Chariot.PullObjectListAsync<Roster>(r => r.DepartmentName == department.Name && r.Date >= earliestDate && r.Date <= latestDate);
                var dailyRosterTask = Chariot.PullObjectListAsync<DailyRoster>(r => r.DepartmentName == departmentName && r.Date >= earliestDate && r.Date <= latestDate);
                var empRosterTask = Chariot.PullObjectListAsync<EmployeeRoster>(r => r.DepartmentName == departmentName && r.StartDate >= earliestDate && r.StartDate <= latestDate);
                var shiftTask = Chariot.PullObjectListAsync<Shift>(s => s.DepartmentName == department.Name);
                var breakTask = Chariot.PullObjectListAsync<Break>();
                var empShiftTask = Chariot.PullObjectListAsync<EmployeeShift>();
                var singleRuleTask = Chariot.PullObjectListAsync<ShiftRuleSingle>();
                var recRuleTask = Chariot.PullObjectListAsync<ShiftRuleRecurring>();
                var rosterRuleTask = Chariot.PullObjectListAsync<ShiftRuleRoster>();
                var dailyCounterTask = Chariot.PullObjectListAsync<DailyShiftCounter>();
                var weeklyCounterTask = Chariot.PullObjectListAsync<WeeklyShiftCounter>();

                await Task.WhenAll(empTask, rosterTask, dailyRosterTask, empRosterTask, shiftTask, breakTask,
                    empShiftTask, singleRuleTask, recRuleTask, rosterRuleTask, dailyCounterTask, weeklyCounterTask);

                data = new RosterDataSet(department, startDate, endDate, await empTask, await rosterTask,
                    await dailyRosterTask, await empRosterTask, await shiftTask, await breakTask, await empShiftTask,
                    await singleRuleTask, await recRuleTask, await rosterRuleTask, await dailyCounterTask,
                    await weeklyCounterTask);
            }

            await new Task(() => Chariot.Database?.RunInTransaction(Action));

            if (data is not null) return data;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull Roster Data Set.");
        }

        return new RosterDataSet();
    }

    public async Task<IEnumerable<Employee>> BorrowableEmployeesAsync(string departmentName)
    {
        List<EmployeeDepartmentLoaning>? departmentLoaningList;
        List<Employee>? employees = null;

        async void Action()
        {
            departmentLoaningList = await Chariot.PullObjectListAsync<EmployeeDepartmentLoaning>(loan => loan.DepartmentName == departmentName);
            var employeeIDs = departmentLoaningList.Select(loan => loan.EmployeeID);
            employees = await Chariot.PullObjectListAsync<Employee>(e => employeeIDs.Contains(e.ID));
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return employees ?? Enumerable.Empty<Employee>();
    }

    public async Task<IEnumerable<EmployeeIcon>> EmployeeIconsAsync()
    {
        var icons = await Chariot.PullObjectListAsync<EmployeeIcon>();
        foreach (var icon in icons)
            icon.SetDirectory(EmployeeIconDirectory);

        return icons;
    }

    public async Task<IEnumerable<EmployeeAvatar>> EmployeeAvatarsAsync()
    {
        var avatars = await Chariot.PullObjectListAsync<EmployeeAvatar>();
        foreach (var avatar in avatars)
            avatar.SetDirectory(EmployeeAvatarDirectory);

        return avatars;
    }

    /// <summary>
    /// Get all the employees that recursively have roles that report to the given role.
    /// </summary>
    /// <param name="headRole">The role of the employee for whom we are gathering reporting employees.</param>
    /// <returns></returns>
    public async Task<IEnumerable<Employee>> GetReportsByRole(Role headRole)
    {
        var roles = await GetReportingRolesAsync(headRole);
        var employees = roles.SelectMany(r => r.Employees);

        return employees;
    }

    /// <summary>
    /// Get all the reporting roles based on the given role.
    /// </summary>
    /// <param name="headRole"></param>
    /// <returns>A collection of roles, each of which containing the relevant employees.</returns>
    public async Task<IEnumerable<Role>> GetReportingRolesAsync(Role headRole)
    {
        Dictionary<string, Role>? roleDict = null;
        Dictionary<string, List<Employee>>? employeeDict = null;

        try
        {
            async void Action()
            {
                var roleTask = Chariot.PullObjectListAsync<Role>();
                var empTask = EmployeesAsync(e => e.IsActive, EPullType.IncludeChildren);

                await Task.WhenAll(roleTask, empTask);

                roleDict = (await roleTask).ToDictionary(r => r.Name, r => r);
                employeeDict = (await empTask).GroupBy(e => e.RoleName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            await new Task(() => Chariot.Database?.RunInTransaction(Action));
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

    public async Task<IEnumerable<Employee>> EmployeesRecursiveReportsAsync()
    {
        var fullEmployees = await new Task<Dictionary<int, Employee>>(() =>
        {
            return Chariot.Database?.Table<Employee>()
                .Where(e => e.IsActive)
                .ToDictionary(e => e.ID, e => e) ?? new Dictionary<int, Employee>();
        });


        foreach (var (_, employee) in fullEmployees)
        {
            if (!fullEmployees.TryGetValue(employee.ReportsToID, out var manager)) continue;
            manager.Reports.Add(employee);
            employee.ReportsTo = manager;
        }

        return fullEmployees.Select(e => e.Value);
    }

    public async Task<List<ShiftEntry>> GetFilteredEntriesAsync(DateTime minDate, DateTime maxDate)
    {
        Dictionary<int, Employee>? employees = null;
        List<ShiftEntry>? entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        async void Action()
        {
            employees = (await EmployeesAsync()).ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database
                ?.Query<ShiftEntry>(
                    "SELECT DailyEntry.* FROM DailyEntry JOIN Employee E on DailyEntry.EmployeeID = E.Code WHERE Date BETWEEN ? AND ?;",
                    startString, endString).ToList();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        employees ??= new Dictionary<int, Employee>();
        entries ??= new List<ShiftEntry>();

        foreach (var shiftEntry in entries)
        {
            if (employees.TryGetValue(shiftEntry.EmployeeID, out var employee))
                shiftEntry.Employee = employee;
        }

        return entries;
    }
    public async Task<List<ShiftEntry>> GetFilteredEntriesAsync(DateTime minDate, DateTime maxDate, Employee manager)
    {
        return await GetFilteredEntriesAsync(minDate, maxDate, manager.ID);
    }
    public async Task<List<ShiftEntry>> GetFilteredEntriesAsync(DateTime minDate, DateTime maxDate, int managerID)
    {
        Dictionary<int, Employee>? employees = null;
        List<ShiftEntry>? entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        await new Task(() =>
        {
            async void Action()
            {
                employees = (await GetManagedEmployeesAsync(managerID)).ToDictionary(e => e.ID, e => e);
                entries = Chariot.Database
                    ?.Query<ShiftEntry>(
                        "SELECT * FROM ShiftEntry " +
                        $"WHERE EmployeeID IN ({string.Join(", ", employees.Select(d => d.Value.ID))}) " +
                        "AND Date BETWEEN ? AND ?;", startString, endString).ToList();
            }

            Chariot.Database?.RunInTransaction(Action);
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
    public async Task<IEnumerable<Employee>> GetManagersAsync()
    {
        IEnumerable<Employee>? managers = null;

        async void Action()
        {
            await new Task(() =>
            {
                var managerIDs = GetManagerIDs();
                managers = Chariot.Database?.Query<Employee>($"SELECT * FROM Employee WHERE ID IN ({string.Join(", ", managerIDs)});");
            });
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

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
    /// Gets all of the shifts applicable to the given employee - based on department.
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<Shift>> ShiftsAsync(Employee employee) => await Chariot.PullObjectListAsync<Shift>(s => s.DepartmentName == employee.DepartmentName);

    public async Task<IEnumerable<EmployeeShift>> EmployeeShiftsAsync(Employee employee) => await Chariot.PullObjectListAsync<EmployeeShift>(es => es.EmployeeID == employee.ID);

    public async Task<IEnumerable<EmployeeShift>> EmployeeShiftsAsync(Shift shift) => await Chariot.PullObjectListAsync<EmployeeShift>(es => es.ShiftID == shift.ID);

    public async Task<IEnumerable<EmployeeIcon>> IconsAsync(Expression<Func<EmployeeIcon, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /* DEPARTMENTS */
    public async Task<List<Department>> DepartmentsAsync(Expression<Func<Department, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task<List<Clan>> ClansAsync(Expression<Func<Clan, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public Department? Department(string departmentName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Department>(departmentName, pullType);

    /// <summary>
    /// Pulls all relevant sub departments according to the given department name.
    /// Will also fill relevant department data, such as shifts.
    /// </summary>
    /// <param name="departmentName"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Department>> SubDepartmentsAsync(string departmentName)
    {
        Dictionary<string, Department>? deptDict = null;
        Dictionary<string, List<Shift>>? shiftDict = null;
        Dictionary<string, List<Break>>? breakDict = null;

        try
        {
            async void Action()
            {
                var deptTask = Chariot.PullObjectListAsync<Department>();
                var shiftTask = Chariot.PullObjectListAsync<Shift>();
                var breakTask = Chariot.PullObjectListAsync<Break>();

                await Task.WhenAll(deptTask, shiftTask, breakTask);

                deptDict = (await deptTask).ToDictionary(d => d.Name, d => d);
                shiftDict = (await shiftTask).GroupBy(s => s.DepartmentName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                breakDict = (await breakTask).GroupBy(b => b.ShiftName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            await new Task(() => Chariot.Database?.RunInTransaction(Action));
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
    public async Task<IEnumerable<Project>> ProjectsAsync(Expression<Func<Project, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) =>
        (await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false)).OrderBy(p => p.Name);

    public async Task<AionDataSet> GetAionDataSetAsync()
    {
        AionDataSet newSet = new();

        async void Action()
        {
            var eventTask = ClockEventsAsync();
            var empTask = EmployeesAsync();
            var shiftTask = ShiftEntriesAsync();

            await Task.WhenAll(eventTask, shiftTask, empTask);

            newSet.ClockEvents = (await eventTask).ToDictionary(c => c.ID, c => c);
            newSet.Employees = (await empTask).ToDictionary(e => e.ID, e => e);
            newSet.ShiftEntries = (await shiftTask).ToDictionary(e => e.ID, e => e);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return newSet;
    }

    /* TEMP TAGS */
    public async Task<IEnumerable<TempTag>> TempTagsAsync()
    {
        List<TempTag>? tags = null;
        List<TagUse>? tagsUse = null;
        List<Employee>? employees = null;

        async void Action()
        {
            var tagTask = Chariot.PullObjectListAsync<TempTag>();
            var useTask = Chariot.PullObjectListAsync<TagUse>();
            var empTask = Chariot.PullObjectListAsync<Employee>();

            await Task.WhenAll(tagTask, empTask, useTask);

            tags = await tagTask;
            tagsUse = await useTask;
            employees = await empTask;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        tags ??= new List<TempTag>();
        tagsUse ??= new List<TagUse>();
        employees ??= new List<Employee>();

        var tagDict = tags.ToDictionary(e => e.RF_ID, e => e);
        var employeeDict = employees.ToDictionary(e => e.ID, e => e);

        foreach (var tempTag in tags)
            if (tempTag.EmployeeID != 0 && employeeDict.TryGetValue(tempTag.EmployeeID, out var employee))
                tempTag.Employee = employee;

        foreach (var tagUse in tagsUse)
        {
            if (tagDict.TryGetValue(tagUse.TempTagRF_ID, out var tag))
            {
                tag.TagUse.Add(tagUse);
                tagUse.TempTag = tag;
            }

            if (!employeeDict.TryGetValue(tagUse.EmployeeID, out var employee)) continue;

            employee.TagUse.Add(tagUse);
            tagUse.Employee = employee;
        }

        return tags;
    }

    /// <summary>
    /// A dictionary that assigns Employee objects according to their RF ID - and also takes into account Temp Tags.
    /// </summary>
    /// <returns></returns>
    public async Task<TagAssignmentTool> TagAssignmentToolAsync()
    {
        List<TempTag>? tags = null;
        List<TagUse>? tagsUse = null;
        List<Employee>? employees = null;

        async void Action()
        {
            var tagTask = Chariot.PullObjectListAsync<TempTag>();
            var useTask = Chariot.PullObjectListAsync<TagUse>();
            var empTask = Chariot.PullObjectListAsync<Employee>();

            await Task.WhenAll(tagTask, empTask, useTask);

            tags = await tagTask;
            tagsUse = await useTask;
            employees = await empTask;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        tags ??= new List<TempTag>();
        employees ??= new List<Employee>();
        tagsUse ??= new List<TagUse>();

        var tool = new TagAssignmentTool(tags, employees, tagsUse);

        return tool;
    }

    public async Task<TagUse?> GetValidUsageAsync(Employee employee, TempTag tempTag, DateTime date) =>
        await GetValidUsageAsync(employee.ID, tempTag.RF_ID, date);

    public async Task<TagUse?> GetValidUsageAsync(int employeeID, string tempTagRFID, DateTime date) =>
        (await Chariot.PullObjectListAsync<TagUse>(u =>
            u.EmployeeID == employeeID && u.TempTagRF_ID == tempTagRFID && u.StartDate <= date &&
            (u.EndDate == null || u.EndDate >= date))).MinBy(u => u.StartDate);

    public async Task<Employee?> TagUserAsync(TempTag tempTag, DateTime date) => await TagUserAsync(tempTag.RF_ID, date);

    public async Task<Employee?> TagUserAsync(string tempTagRF, DateTime date) => Employee(await TagUserIDAsync(tempTagRF, date));

    public async Task<int> TagUserIDAsync(TempTag tempTag, DateTime date) => await TagUserIDAsync(tempTag.RF_ID, date);

    public async Task<int> TagUserIDAsync(string tempTagRF, DateTime date)
    {
        var id = 0;
        await new Task(() =>
        {
            async void Action()
            {
                var usage = await Chariot.PullObjectListAsync<TagUse>(u => u.TempTagRF_ID == tempTagRF && u.StartDate <= date && (u.EndDate == null || u.EndDate >= date));
                if (!usage.Any()) return;

                if (usage.Count == 1)
                {
                    id = usage.First().EmployeeID;
                    return;
                }

                if (usage.Any(u => u.EndDate is null)) usage = usage.Where(u => u.EndDate is null).ToList();
                id = usage.OrderBy(u => u.StartDate).First().EmployeeID;
            }

            Chariot.Database?.RunInTransaction(Action);
        });
        return id;
    }

    /* Pick Event Tracking */

    public async Task<IEnumerable<PickEvent>> RawPickEventsAsync(DateTime startDate, DateTime endDate) => await Chariot.PullObjectListAsync<PickEvent>(e => e.Date >= startDate.Date && e.Date <= endDate.Date);

    public async Task<IEnumerable<MissPick>> RawMissPicksAsync(DateTime startDate, DateTime endDate) =>
        await Chariot.PullObjectListAsync<MissPick>(mp => mp.ShipmentDate >= startDate.Date && mp.ShipmentDate <= endDate.Date);

    public async Task<IEnumerable<PickEvent>> PickEventsAsync(DateTime startDate, DateTime endDate, bool includeEmployees = false)
    {
        IEnumerable<PickEvent>? events = null;

        await new Task(() =>
        {
            async void Action()
            {
                events = await Chariot.PullObjectListAsync<PickEvent>(e => e.Date >= startDate.Date && e.Date <= endDate.Date);

                if (!includeEmployees) return;

                var employees = await EmployeesAsync();
                var empDict = employees.ToDictionary(e => e.ID, e => e);

                // Get ID Mapping dictionary to use for reference between RF/Dematic ID to employee ID.
                Dictionary<string, int> idDict = new();
                foreach (var employee in employees)
                {
                    if (employee.DematicID != string.Empty && employee.DematicID != "0000" && !idDict.ContainsKey(employee.DematicID)) idDict.Add(employee.DematicID, employee.ID);
                    if (employee.RF_ID != string.Empty && !idDict.ContainsKey(employee.RF_ID)) idDict.Add(employee.RF_ID, employee.ID);
                }

                // Assign actual OperatorID.
                foreach (var pickEvent in events)
                {
                    if (pickEvent.OperatorID == 0)
                    {
                        if (idDict.TryGetValue(pickEvent.OperatorDematicID, out var id))
                            pickEvent.OperatorID = id;
                        else if (idDict.TryGetValue(pickEvent.OperatorRF_ID, out id)) pickEvent.OperatorID = id;
                    }

                    if (!empDict.TryGetValue(pickEvent.OperatorID, out var employee)) continue;

                    employee.PickEvents.Add(pickEvent);
                    pickEvent.Operator = employee;
                }
            }

            Chariot.Database?.RunInTransaction(Action);
        });

        events ??= new List<PickEvent>();

        return events;
    }

    public async Task<IEnumerable<PickEvent>> PickEventsAsync(DateTime date, bool includeEmployees = false) =>
        await PickEventsAsync(date, date, includeEmployees);

    public async Task<IEnumerable<PickSession>> PickSessionsAsync(DateTime startDate, DateTime endDate, bool includeEmployees = false)
    {
        IEnumerable<PickSession>? sessions = null;

        await new Task(() =>
        {
            async void Action()
            {
                var eventTask = PickEventsAsync(startDate, endDate, includeEmployees);

                var sessionTask = Chariot.PullObjectListAsync<PickSession>(s =>
                    s.Date >= startDate.Date && s.Date <= endDate.Date);

                await Task.WhenAll(sessionTask, eventTask);

                var events = await eventTask;
                sessions = await sessionTask;

                var sessionDict = sessions.ToDictionary(s => s.ID, s => s);

                foreach (var pickEvent in events)
                {
                    if (!sessionDict.TryGetValue(pickEvent.SessionID, out var session)) continue;

                    session.PickEvents.Add(pickEvent);
                    pickEvent.Session = session;

                    if (!includeEmployees) continue;

                    session.Operator = pickEvent.Operator;
                    session.Operator?.PickSessions.Add(session);
                }
            }

            Chariot.Database?.RunInTransaction(Action);
        });

        sessions ??= new List<PickSession>();

        return sessions;
    }

    public async Task<IEnumerable<PickSession>> PickSessionsAsync(DateTime date, bool includeEmployees = false) => await
        PickSessionsAsync(date, date, includeEmployees);

    public async Task<IEnumerable<PickDailyStats>> PickStatsAsync(DateTime startDate, DateTime endDate,
        bool includeEmployees = false)
    {
        IEnumerable<PickDailyStats>? stats = null;

        await new Task(() =>
        {
            async void Action()
            {
                var sessionTask = PickSessionsAsync(startDate, endDate, includeEmployees);
                var statTask = Chariot.PullObjectListAsync<PickDailyStats>(s => s.Date >= startDate.Date && s.Date <= endDate.Date);

                await Task.WhenAll(sessionTask, statTask);

                var sessions = await sessionTask;
                stats = await statTask;

                var statDict = stats.ToDictionary(s => s.ID, s => s);

                foreach (var pickSession in sessions)
                {
                    if (!statDict.TryGetValue(pickSession.StatsID, out var statistics)) continue;

                    statistics.AddSession(pickSession);

                    if (!includeEmployees) continue;

                    statistics.Operator = pickSession.Operator;
                    statistics.Operator?.PickStatistics.Add(statistics);
                }
            }

            Chariot.Database?.RunInTransaction(Action);
        });

        stats ??= new List<PickDailyStats>();

        return stats;
    }

    public async Task<IEnumerable<PickDailyStats>> PickStatsAsync(DateTime date, bool includeEmployees = false) =>
        await PickStatsAsync(date, date, includeEmployees);
}