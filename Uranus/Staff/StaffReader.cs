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

    public void RunInTransaction(Action action) => Chariot.RunInTransaction(action);

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
        var employees = new List<Employee>();

        void Action()
        {
            employees = Chariot.PullObjectList<Employee>();
            var roles = Chariot.PullObjectList<Role>();

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
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return employees;
    }

    public async Task<Employee?> RoleStackEmployeeAsync(int id) => (await EmployeeRoleStackAsync().ConfigureAwait(false)).FirstOrDefault(e => e.ID == id);

    /// <summary>
    /// Gets the employee with all appropriate relationships loaded.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Employee?> EmployeeLogInAsync(int id)
    {
        (await EmployeeDataSetAsync().ConfigureAwait(false)).Employees.TryGetValue(id, out var employee);
        return employee;
    }

    public async Task<List<Employee>> EmployeesAsync(Expression<Func<Employee, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter ?? (e => e.IsActive), pullType).ConfigureAwait(false);

    public List<Employee> Employees(Expression<Func<Employee, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => Chariot.PullObjectList(filter ?? (e => e.IsActive), pullType);

    public bool EmployeeExists(int id) => Chariot.ExecuteScalar<int>("SELECT count(*) FROM Employee WHERE ID=?;", id) > 0;

    public int EmployeeCount() => Chariot.ExecuteScalar<int>("SELECT count(*) FROM Employee;");

    public Role? Role(string roleName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Role>(roleName, pullType);

    public async Task<IEnumerable<Role>> RolesAsync(Expression<Func<Role, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task<List<ClockEvent>> ClockEventsAsync(Expression<Func<ClockEvent, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<ClockEvent> ClockEvents(Expression<Func<ClockEvent, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<ClockEvent> ClockEventsAsync(DateTime startDate, DateTime endDate)
    {
        return Chariot.Query<ClockEvent>(
            "SELECT * FROM ClockEvent WHERE Date BETWEEN ? AND ?;",
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"));
    }

    public async Task<List<ClockEvent>> ClockEventsAsync(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        var task = Task.Run(() =>
            Chariot.Query<ClockEvent>(
                $"SELECT * FROM ClockEvent WHERE EmployeeID in ({string.Join(", ", employeeIDs)}) AND Date BETWEEN ? AND ?;",
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")));

        return await task;
    }

    public async Task<List<ShiftEntry>> ShiftEntriesAsync(Expression<Func<ShiftEntry, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<ShiftEntry> ShiftEntries(Expression<Func<ShiftEntry, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<int> EmployeeIDs() => Chariot.Query<Employee>("SELECT DISTINCT ID FROM Employee;").Select(e => e.ID).OrderBy(c => c).ToList();

    /// <summary>
    /// Get the data required for an Aion Employee Refresh.
    /// </summary>
    /// <returns>Tuple: (Manager IDs, employees, locations, payPoints, employmentTypes, roles, departments)</returns>
    public async
        Task<(List<int>, List<Employee>, IEnumerable<string>, IEnumerable<string>, IEnumerable<EEmploymentType>,
            IEnumerable<Role>, IEnumerable<Department>)> AionEmployeeRefreshAsync()
    {
        IEnumerable<Role>? roles = null;
        List<Department>? departments = null;
        List<Employee>? employees = null;
        List<int>? managerIDList = null;
        IEnumerable<string>? locations = null;
        IEnumerable<string>? payPoints = null;
        IEnumerable<EEmploymentType>? employmentTypes = null;

        void Action()
        {
            roles = Chariot.PullObjectList<Role>().OrderBy(r => r.Name);
            departments = Chariot.PullObjectList<Department>();
            employees = Chariot.PullObjectList<Employee>();

            managerIDList = employees.Where(e => e.ReportsToID != 0).Select(e => e.ReportsToID).Distinct().ToList();
            locations = employees.Select(e => e.Location).Distinct();
            payPoints = employees.Select(e => e.PayPoint).Distinct();
            employmentTypes = employees.Select(e => e.EmploymentType).Distinct();
        }
        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        managerIDList ??= new List<int>();
        employees ??= new List<Employee>();
        locations ??= new List<string>();
        payPoints ??= new List<string>();
        employmentTypes ??= new List<EEmploymentType>();
        roles ??= new List<Role>();
        departments ??= new List<Department>();

        return (managerIDList, employees, locations, payPoints, employmentTypes, roles, departments);
    }

    public IEnumerable<int> GetManagerIDs() => Chariot.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee WHERE IsActive = ?;", true).Select(e => e.ReportsToID).ToList();

    public IEnumerable<string> Locations() => Chariot.Query<Employee>("SELECT DISTINCT Location FROM Employee WHERE IsActive = ?;", true).Select(e => e.Location);

    public async Task<IEnumerable<Employee>> GetManagedEmployeesAsync(int managerID)
    {
        var fullEmployees = (await EmployeesRecursiveReportsAsync().ConfigureAwait(false)).ToDictionary(e => e.ID, e => e);

        if (!fullEmployees.TryGetValue(managerID, out var manager)) return new List<Employee>();

        fullEmployees.Clear();

        GetReports(manager, ref fullEmployees);

        return fullEmployees.Select(d => d.Value);
    }

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
    public async Task<EmployeeDataSet> EmployeeDataSetAsync(bool includeInactive = false)
    {
        try
        {
            var dataSet = new EmployeeDataSet();

            void Action()
            {
                var empTask = Chariot.PullObjectList<Employee>(e => e.IsActive || includeInactive);
                var depTask = Chariot.PullObjectList<Department>();
                var depRosterTask = Chariot.PullObjectList<DepartmentRoster>();
                var roleTask = Chariot.PullObjectList<Role>();
                var clanTask = Chariot.PullObjectList<Clan>();
                var iconTask = EmployeeIcons();
                var avatarTask = EmployeeAvatars();
                var shiftTask = Chariot.PullObjectList<Shift>();
                var breakTask = Chariot.PullObjectList<Break>();
                var sglRuleTask = Chariot.PullObjectList<ShiftRuleSingle>();
                var recRuleTask = Chariot.PullObjectList<ShiftRuleRecurring>();
                var rstRuleTask = Chariot.PullObjectList<ShiftRuleRoster>();
                var tagTask = Chariot.PullObjectList<TempTag>();
                var useTask = Chariot.PullObjectList<TagUse>();

                dataSet = new EmployeeDataSet(empTask, depTask, depRosterTask, clanTask, roleTask, iconTask,
                    avatarTask, shiftTask, breakTask, sglRuleTask, recRuleTask, rstRuleTask, tagTask, useTask);
            }
            await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

            return dataSet;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull the EmployeeDataSet");
        }
        return new EmployeeDataSet();
    }

    /// <summary>
    /// In a single transaction, pulls all the data required for all employees and inserts it into a single data object.
    /// </summary>
    /// <returns>Employee data set, containing Departments, Roles, etc. - with established relationships.</returns>
    public EmployeeDataSet EmployeeDataSet(bool includeInactive = false)
    {
        try
        {
            var dataSet = new EmployeeDataSet();

            void Action()
            {
                var empTask = Chariot.PullObjectList<Employee>(e => e.IsActive || includeInactive);
                var depTask = Chariot.PullObjectList<Department>();
                var depRosterTask = Chariot.PullObjectList<DepartmentRoster>();
                var roleTask = Chariot.PullObjectList<Role>();
                var clanTask = Chariot.PullObjectList<Clan>();
                var iconTask = EmployeeIcons();
                var avatarTask = EmployeeAvatars();
                var shiftTask = Chariot.PullObjectList<Shift>();
                var breakTask = Chariot.PullObjectList<Break>();
                var sglRuleTask = Chariot.PullObjectList<ShiftRuleSingle>();
                var recRuleTask = Chariot.PullObjectList<ShiftRuleRecurring>();
                var rstRuleTask = Chariot.PullObjectList<ShiftRuleRoster>();
                var tagTask = Chariot.PullObjectList<TempTag>();
                var useTask = Chariot.PullObjectList<TagUse>();

                dataSet = new EmployeeDataSet(empTask, depTask, depRosterTask, clanTask, roleTask, iconTask,
                    avatarTask, shiftTask, breakTask, sglRuleTask, recRuleTask, rstRuleTask, tagTask, useTask);
            }
            Chariot.RunInTransaction(Action);

            return dataSet;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull the EmployeeDataSet");
        }
        return new EmployeeDataSet();
    }

    public IEnumerable<string> PayPoints() =>
        Chariot.Query<Employee>("SELECT DISTINCT PayPoint FROM Employee;").Select(e => e.PayPoint);

    public DepartmentRoster? DepartmentRoster(string rosterName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<DepartmentRoster>(rosterName, pullType);

    public IEnumerable<DepartmentRoster?> DepartmentRosters(string departmentName)
        => Chariot.Query<DepartmentRoster>("SELECT * FROM DepartmentRoster WHERE DepartmentName = ?;", departmentName);

    public async Task<IEnumerable<DepartmentRoster>> DepartmentRostersAsync(Expression<Func<DepartmentRoster, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task FillDepartmentRoster(DepartmentRoster departmentRoster)
    {
        try
        {
            void Action()
            {
                var departmentName = departmentRoster.DepartmentName;

                var dataSet = EmployeeDataSet(true);
                var rosters = Chariot.PullObjectList<Roster>(r => r.DepartmentRosterID == departmentRoster.ID);
                var dailyRosters =
                    Chariot.PullObjectList<DailyRoster>(r => r.DepartmentRosterID == departmentRoster.ID);
                var empRosters =
                    Chariot.PullObjectList<EmployeeRoster>(r => r.DepartmentRosterID == departmentRoster.ID);
                var shifts = Chariot.PullObjectList<Shift>(s => s.DepartmentName == departmentName);
                var employeeShifts = Chariot.PullObjectList<EmployeeShift>();
                var singleRules = Chariot.PullObjectList<ShiftRuleSingle>();
                var recurringRules = Chariot.PullObjectList<ShiftRuleRecurring>();
                var rosterRules = Chariot.PullObjectList<ShiftRuleRoster>();
                var weeklyShiftCounters =
                    Chariot.PullObjectList<WeeklyShiftCounter>(wc => wc.RosterID == departmentRoster.ID);

                var shiftIDs = shifts.Select(s => s.ID);
                var dailyIDs = dailyRosters.Select(dr => dr.ID);
                var breaks = Chariot.PullObjectList<Break>(b => shiftIDs.Contains(b.ShiftID));
                var dailyShiftCounters =
                    Chariot.PullObjectList<DailyShiftCounter>(dc => dailyIDs.Contains(dc.RosterID));

                var employees = dataSet.Employees.Values.Where(e => e.DepartmentName == departmentName)
                    .ToList();

                departmentRoster.SetData(employees, rosters, dailyRosters, empRosters, shifts, breaks, employeeShifts,
                    singleRules, recurringRules, rosterRules, weeklyShiftCounters, dailyShiftCounters);
            }

            await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
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
            var dataSet = new RosterDataSet();

            void Action()
            {
                var department = Chariot.PullObject<Department>(departmentName);
                if (department is null) return;

                var employees =
                    Chariot.PullObjectList<Employee>(e => e.DepartmentName == department.Name && e.IsActive);
                var earliestDate = startDate.AddDays(DayOfWeek.Sunday - startDate.DayOfWeek - 14);
                var latestDate = endDate.AddDays(DayOfWeek.Saturday - endDate.DayOfWeek);
                var rosters = Chariot.PullObjectList<Roster>(r =>
                    r.DepartmentName == department.Name && r.Date >= earliestDate && r.Date <= latestDate);
                var dailyRosters = Chariot.PullObjectList<DailyRoster>(r =>
                    r.DepartmentName == departmentName && r.Date >= earliestDate && r.Date <= latestDate);
                var empRosters = Chariot.PullObjectList<EmployeeRoster>(r =>
                    r.DepartmentName == departmentName && r.StartDate >= earliestDate && r.StartDate <= latestDate);
                var shifts = Chariot.PullObjectList<Shift>(s => s.DepartmentName == department.Name);
                var breaks = Chariot.PullObjectList<Break>();
                var empShifts = Chariot.PullObjectList<EmployeeShift>();
                var singleRules = Chariot.PullObjectList<ShiftRuleSingle>();
                var recRules = Chariot.PullObjectList<ShiftRuleRecurring>();
                var rosterRules = Chariot.PullObjectList<ShiftRuleRoster>();
                var dailyCounters = Chariot.PullObjectList<DailyShiftCounter>();
                var weeklyCounters = Chariot.PullObjectList<WeeklyShiftCounter>();

                dataSet = new RosterDataSet(department, startDate, endDate, employees, rosters, dailyRosters, empRosters,
                    shifts, breaks, empShifts, singleRules, recRules, rosterRules, dailyCounters, weeklyCounters);
            }

            await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

            return dataSet;
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

        void Action()
        {
            departmentLoaningList =
                Chariot.PullObjectList<EmployeeDepartmentLoaning>(loan => loan.DepartmentName == departmentName);
            var employeeIDs = departmentLoaningList.Select(loan => loan.EmployeeID);
            employees = Chariot.PullObjectList<Employee>(e => employeeIDs.Contains(e.ID));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return employees ?? Enumerable.Empty<Employee>();
    }

    public async Task<IEnumerable<EmployeeIcon>> EmployeeIconsAsync()
    {
        var icons = await Chariot.PullObjectListAsync<EmployeeIcon>().ConfigureAwait(false);
        foreach (var icon in icons)
            icon.SetDirectory(EmployeeIconDirectory);

        return icons;
    }

    public IEnumerable<EmployeeIcon> EmployeeIcons()
    {
        var icons = Chariot.PullObjectList<EmployeeIcon>();
        foreach (var icon in icons)
            icon.SetDirectory(EmployeeIconDirectory);

        return icons;
    }

    public async Task<IEnumerable<EmployeeAvatar>> EmployeeAvatarsAsync()
    {
        var avatars = await Chariot.PullObjectListAsync<EmployeeAvatar>().ConfigureAwait(false);
        foreach (var avatar in avatars)
            avatar.SetDirectory(EmployeeAvatarDirectory);

        return avatars;
    }

    public IEnumerable<EmployeeAvatar> EmployeeAvatars()
    {
        var avatars = Chariot.PullObjectList<EmployeeAvatar>();
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
        var roles = await GetReportingRolesAsync(headRole).ConfigureAwait(false);
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
        var roleDict = new Dictionary<string, Role>();
        var employeeDict = new Dictionary<string, List<Employee>>();

        try
        {
            void Action()
            {
                var roles = Chariot.PullObjectList<Role>();
                var employees = Employees(e => e.IsActive, EPullType.IncludeChildren);

                roleDict = roles.ToDictionary(r => r.Name, r => r);
                employeeDict = employees.GroupBy(e => e.RoleName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unable to pull Roles and/or Employees from {}, defaulting to empty.", Chariot.DatabaseName);
            roleDict = new Dictionary<string, Role>();
            employeeDict = new Dictionary<string, List<Employee>>();
        }

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
        var fullEmployees = await Task.Run(() => { return Chariot.Database?.Table<Employee>().Where(e => e.IsActive).ToDictionary(e => e.ID, e => e) ?? new Dictionary<int, Employee>(); }).ConfigureAwait(false);


        foreach (var (_, employee) in fullEmployees)
        {
            if (!fullEmployees.TryGetValue(employee.ReportsToID, out var manager)) continue;
            manager.Reports.Add(employee);
            employee.ReportsTo = manager;
        }

        return fullEmployees.Select(e => e.Value);
    }

    public IEnumerable<Employee> EmployeesRecursiveReports()
    {
        var fullEmployees = Chariot.Database?.Table<Employee>()
                .Where(e => e.IsActive)
                .ToDictionary(e => e.ID, e => e) ?? new Dictionary<int, Employee>();

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

        void Action()
        {
            employees = Employees().ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database
                ?.Query<ShiftEntry>(
                    "SELECT DailyEntry.* FROM DailyEntry JOIN Employee E on DailyEntry.EmployeeID = E.Code WHERE Date BETWEEN ? AND ?;",
                    startString, endString).ToList();
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

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
        return await GetFilteredEntriesAsync(minDate, maxDate, manager.ID).ConfigureAwait(false);
    }
    public async Task<List<ShiftEntry>> GetFilteredEntriesAsync(DateTime minDate, DateTime maxDate, int managerID)
    {
        Dictionary<int, Employee>? employees = null;
        List<ShiftEntry>? entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        void Action()
        {
            employees = GetManagedEmployees(managerID).ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database
                ?.Query<ShiftEntry>(
                    "SELECT * FROM ShiftEntry " +
                    $"WHERE EmployeeID IN ({string.Join(", ", employees.Select(d => d.Value.ID))}) " +
                    "AND Date BETWEEN ? AND ?;", startString, endString).ToList();
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

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
        return Chariot.Query<ClockEvent>(
            "SELECT ClockEvent.* FROM ClockEvent JOIN Employee ON ClockEvent.EmployeeID = Employee.ID " +
            "WHERE Employee.ReportsToID = ? " +
            "AND ClockEvent.Status = ?" +
            "AND Date BETWEEN ? AND ?;",
            managerCode, EClockStatus.Pending,
            fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
    }

    public int GetPendingCount(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        return Chariot.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM ClockEvent " +
            $"WHERE EmployeeID IN ({string.Join(", ", employeeIDs)}) AND " +
            "Status = ? AND " +
            $"Date BETWEEN '{startDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}';",
            EClockStatus.Pending);
    }

    /// <summary>
    /// Gets all employees that have at least one other employee as a direct report.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Employee>> GetManagersAsync()
    {
        IEnumerable<Employee>? managers = null;

        void Action()
        {
            var managerIDs = GetManagerIDs();
            managers = Chariot.Query<Employee>($"SELECT * FROM Employee WHERE ID IN ({string.Join(", ", managerIDs)});");
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return managers ?? new List<Employee>();
    }

    /// <summary>
    /// Gets a full list of today's clock events for the given employee code.
    /// </summary>
    /// <returns></returns>
    public List<ClockEvent> ClocksForToday(int employeeID)
    {
        return Chariot.Query<ClockEvent>("SELECT * FROM ClockEvent WHERE EmployeeID = ? AND Date = ? AND Status <> ?;",
            employeeID, DateTime.Now.ToString("yyyy-MM-dd"), EClockStatus.Deleted);
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
    public async Task<IEnumerable<Shift>> ShiftsAsync(Employee employee) => await Chariot.PullObjectListAsync<Shift>(s => s.DepartmentName == employee.DepartmentName).ConfigureAwait(false);

    public async Task<IEnumerable<EmployeeShift>> EmployeeShiftsAsync(Employee employee) => await Chariot.PullObjectListAsync<EmployeeShift>(es => es.EmployeeID == employee.ID).ConfigureAwait(false);

    public async Task<IEnumerable<EmployeeShift>> EmployeeShiftsAsync(Shift shift) => await Chariot.PullObjectListAsync<EmployeeShift>(es => es.ShiftID == shift.ID).ConfigureAwait(false);

    public async Task<IEnumerable<EmployeeIcon>> IconsAsync(Expression<Func<EmployeeIcon, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    /* DEPARTMENTS */
    public async Task<List<Department>> DepartmentsAsync(Expression<Func<Department, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task<List<Clan>> ClansAsync(Expression<Func<Clan, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public Department? Department(string departmentName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Department>(departmentName, pullType);

    /// <summary>
    /// Pulls all relevant sub departments according to the given department name.
    /// Will also fill relevant department data, such as shifts.
    /// </summary>
    /// <param name="departmentName"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Department>> SubDepartmentsAsync(string departmentName)
    {
        var deptDict = new Dictionary<string, Department>();
        var shiftDict = new Dictionary<string, List<Shift>>();
        var breakDict = new Dictionary<string, List<Break>>();

        try
        {
            void Action()
            {
                var departments = Chariot.PullObjectList<Department>();
                var shifts = Chariot.PullObjectList<Shift>();
                var breaks = Chariot.PullObjectList<Break>();

                deptDict = departments.ToDictionary(d => d.Name, d => d);
                shiftDict = shifts.GroupBy(s => s.DepartmentName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                breakDict = breaks.GroupBy(b => b.ShiftName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull shift and/or department data from {}. Defaulted to null values.", Chariot.DatabaseName);
        }

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
        void Action()
        {
            var clockEvents = ClockEvents();
            var employees = Employees();
            var shiftEntries = ShiftEntries();

            newSet.ClockEvents = clockEvents.ToDictionary(c => c.ID, c => c);
            newSet.Employees = employees.ToDictionary(e => e.ID, e => e);
            newSet.ShiftEntries = shiftEntries.ToDictionary(e => e.ID, e => e);
        }

        await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);
        return newSet;
    }

    /* TEMP TAGS */
    public async Task<IEnumerable<TempTag>> TempTagsAsync()
    {
        var tags = new List<TempTag>();
        var employeeDict = new Dictionary<int, Employee>();
        var tagsUse = new List<TagUse>();
        var tagDict = new Dictionary<string, TempTag>();

        void Action()
        {
            tags = Chariot.PullObjectList<TempTag>();
            tagsUse = Chariot.PullObjectList<TagUse>();
            var employees = Chariot.PullObjectList<Employee>();

            tagDict = tags.ToDictionary(e => e.RF_ID, e => e);
            employeeDict = employees.ToDictionary(e => e.ID, e => e);
        }

        await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);

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
        var tool = new TagAssignmentTool();

        void Action()
        {
            var tags = Chariot.PullObjectList<TempTag>();
            var tagsUse = Chariot.PullObjectList<TagUse>();
            var employees = Chariot.PullObjectList<Employee>();

            tool = new TagAssignmentTool(tags, employees, tagsUse);
        }

        await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);

        return tool;
    }

    /// <summary>
    /// A dictionary that assigns Employee objects according to their RF ID - and also takes into account Temp Tags.
    /// </summary>
    /// <returns></returns>
    public TagAssignmentTool TagAssignmentTool()
    {
        var tool = new TagAssignmentTool();

        void Action()
        {
            var tags = Chariot.PullObjectList<TempTag>();
            var tagsUse = Chariot.PullObjectList<TagUse>();
            var employees = Chariot.PullObjectList<Employee>();

            tool = new TagAssignmentTool(tags, employees, tagsUse);
        }

        RunInTransaction(Action);

        return tool;
    }

    public async Task<Employee?> TagUserAsync(TempTag tempTag, DateTime date) => await TagUserAsync(tempTag.RF_ID, date).ConfigureAwait(false);

    public async Task<Employee?> TagUserAsync(string tempTagRF, DateTime date) => Employee(await TagUserIDAsync(tempTagRF, date).ConfigureAwait(false));

    public async Task<int> TagUserIDAsync(TempTag tempTag, DateTime date) => await TagUserIDAsync(tempTag.RF_ID, date).ConfigureAwait(false);

    public async Task<int> TagUserIDAsync(string tempTagRF, DateTime date)
    {
        var id = 0;
        await Task.Run(async () => { var usage = await Chariot.PullObjectListAsync<TagUse>(u => u.TempTagRF_ID == tempTagRF && u.StartDate <= date && (u.EndDate == null || u.EndDate >= date)).ConfigureAwait(false); if (!usage.Any()) return; if (usage.Count == 1) { id = usage.First().EmployeeID; return; } if (usage.Any(u => u.EndDate is null)) usage = usage.Where(u => u.EndDate is null).ToList(); id = usage.OrderBy(u => u.StartDate).First().EmployeeID; }).ConfigureAwait(false);
        return id;
    }

    /* Pick Event Tracking */

    public async Task<List<PickEvent>> RawPickEventsAsync(DateTime startDate, DateTime endDate) => await Chariot.PullObjectListAsync<PickEvent>(e => e.Date >= startDate.Date && e.Date <= endDate.Date).ConfigureAwait(false);

    public async Task<List<Mispick>> RawMispicksAsync(DateTime startDate, DateTime endDate) =>
        await Chariot.PullObjectListAsync<Mispick>(mp => mp.ShipmentDate >= startDate.Date && mp.ShipmentDate <= endDate.Date).ConfigureAwait(false);

    public List<Mispick> MispicksByErrorDate(DateTime startDate, DateTime endDate) =>
        Chariot.PullObjectList<Mispick>(mp => mp.ErrorDate >= startDate.Date && mp.ErrorDate <= endDate.Date);

    public List<Mispick> MispicksByShipDate(DateTime startDate, DateTime endDate) =>
        Chariot.PullObjectList<Mispick>(mp => mp.ShipmentDate >= startDate.Date && mp.ShipmentDate <= endDate.Date);

    public List<Mispick> MispicksByPostedDate(DateTime startDate, DateTime endDate) =>
        Chariot.PullObjectList<Mispick>(mp => mp.PostedDate >= startDate.Date && mp.PostedDate <= endDate.Date);

    public async Task<List<PickEvent>> PickEventsAsync(DateTime startDate, DateTime endDate, bool includeEmployees = false)
    {
        var events = new List<PickEvent>();

        void Action()
        {
            events = Chariot.PullObjectList<PickEvent>(e => e.Date >= startDate.Date && e.Date <= endDate.Date);

            if (!includeEmployees) return;

            var employees = Employees();
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

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);


        events ??= new List<PickEvent>();

        return events;
    }

    public async Task<List<PickEvent>> PickEventsAsync(DateTime date, bool includeEmployees = false) =>
        await PickEventsAsync(date, date, includeEmployees).ConfigureAwait(false);

    public List<PickEvent> PickEvents(DateTime startDate, DateTime endDate, bool includeEmployees = false)
    {
        var events = new List<PickEvent>();

        void Action()
        {
            events = Chariot.PullObjectList<PickEvent>(e => e.Date >= startDate.Date && e.Date <= endDate.Date);

            if (!includeEmployees) return;

            var employees = Employees();
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

        Chariot.RunInTransaction(Action);

        events ??= new List<PickEvent>();

        return events;
    }

    public List<PickEvent> PickEvents(DateTime date, bool includeEmployees = false) =>
        PickEvents(date, date, includeEmployees);

    public async Task<List<PickSession>> PickSessionsAsync(DateTime startDate, DateTime endDate, bool includeEmployees = false)
    {
        var eventTask = PickEventsAsync(startDate, endDate, includeEmployees);

        var sessionTask = Chariot.PullObjectListAsync<PickSession>(s =>
            s.Date >= startDate.Date && s.Date <= endDate.Date);

        await Task.WhenAll(sessionTask, eventTask).ConfigureAwait(false);

        var events = await eventTask;
        var sessions = await sessionTask;

        await Task.Run(() => { var sessionDict = sessions.ToDictionary(s => s.ID, s => s); foreach (var pickEvent in events) { if (!sessionDict.TryGetValue(pickEvent.SessionID, out var session)) continue; session.PickEvents.Add(pickEvent); pickEvent.Session = session; if (!includeEmployees) continue; session.Operator = pickEvent.Operator; session.Operator?.PickSessions.Add(session); } }).ConfigureAwait(false);

        return sessions;
    }

    public async Task<List<PickSession>> PickSessionsAsync(DateTime date, bool includeEmployees = false) => await
        PickSessionsAsync(date, date, includeEmployees).ConfigureAwait(false);

    public List<PickSession> PickSessions(DateTime startDate, DateTime endDate, bool includeEmployees = false)
    {
        var events = new List<PickEvent>();
        var sessions = new List<PickSession>();

        void Action()
        {
            events = PickEvents(startDate, endDate, includeEmployees).ToList();
            sessions = Chariot.PullObjectList<PickSession>(s =>
                s.Date >= startDate.Date && s.Date <= endDate.Date);
        }

        RunInTransaction(Action);

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

        return sessions;
    }

    public List<PickSession> PickSessions(DateTime date, bool includeEmployees = false) =>
        PickSessions(date, date, includeEmployees);

    public async Task<List<PickDailyStats>> PickStatsAsync(DateTime startDate, DateTime endDate,
        bool includeEmployees = false)
    {
        var sessions = new List<PickSession>();
        var stats = new List<PickDailyStats>();

        void Action()
        {
            sessions = PickSessions(startDate, endDate, includeEmployees).ToList();
            stats = Chariot.PullObjectList<PickDailyStats>(s => s.Date >= startDate.Date && s.Date <= endDate.Date);
        }

        await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);

        await Task.Run(() => { var statDict = stats.ToDictionary(s => s.ID, s => s); foreach (var pickSession in sessions) { if (!statDict.TryGetValue(pickSession.StatsID, out var statistics)) continue; statistics.AddSession(pickSession); if (!includeEmployees) continue; statistics.Operator = pickSession.Operator; statistics.Operator?.PickStatistics.Add(statistics); } }).ConfigureAwait(false);

        return stats;
    }

    public async Task<IEnumerable<PickDailyStats>> PickStatsAsync(DateTime date, bool includeEmployees = false) =>
        await PickStatsAsync(date, date, includeEmployees).ConfigureAwait(false);

    public List<PickDailyStats> PickStats(DateTime startDate, DateTime endDate,
        bool includeEmployees = false)
    {
        var sessions = new List<PickSession>();
        var stats = new List<PickDailyStats>();

        void Action()
        {
            sessions = PickSessions(startDate, endDate, includeEmployees).ToList();
            stats = Chariot.PullObjectList<PickDailyStats>(s => s.Date >= startDate.Date && s.Date <= endDate.Date);
        }

        RunInTransaction(Action);

        var statDict = stats.ToDictionary(s => s.ID, s => s);

        foreach (var pickSession in sessions)
        {
            if (!statDict.TryGetValue(pickSession.StatsID, out var statistics)) continue;

            statistics.AddSession(pickSession);

            if (!includeEmployees) continue;

            statistics.Operator = pickSession.Operator;
            statistics.Operator?.PickStatistics.Add(statistics);
        }

        return stats;
    }

    public List<PickDailyStats> PickStats(DateTime date, bool includeEmployees = false) =>
        PickStats(date, date, includeEmployees);

    public async Task<Dictionary<DateTime, int>> PickEventLineCountByDate(DateTime startDate, DateTime endDate)
    {
        var items = await Chariot.QueryAsync<(DateTime, int)>("SELECT Date, COUNT(*) as Lines FROM PickEvent WHERE Date >= ? AND Date <= ? GROUP BY Date;", startDate.Ticks, endDate.Ticks).ConfigureAwait(false);

        return items.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
    }

    public async Task<Dictionary<DateTime, int>> MispickLineCountByDate(DateTime startDate, DateTime endDate)
    {
        var items = await Chariot.QueryAsync<(DateTime, int)>("SELECT ShipmentDate, COUNT(*) as Lines FROM Mispick WHERE ShipmentDate >= ? AND ShipmentDate <= ? GROUP BY ShipmentDate;", startDate.Ticks, endDate.Ticks).ConfigureAwait(false);

        return items.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
    }

    public async Task<(List<PickDailyStats>, List<Mispick>)> ErrorAssignmentDataAsync(DateTime fromDate, DateTime toDate)
    {
        var stats = new List<PickDailyStats>();
        var mispicks = new List<Mispick>();

        void Action()
        {
            // It is possible for cartons to be delayed up to 7 days, 
            // so gather pick data from 7 days earlier than the mispicks.
            mispicks = MispicksByErrorDate(fromDate, toDate);
            stats = mispicks.Any() ? PickStats(fromDate.AddDays(-7), toDate) : stats;
        }

        await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);

        return (stats, mispicks);
    }

    public async Task<(List<Mispick> mispicks, List<PickSession> sessions, TagAssignmentTool tagTool)> StatisticReportsAsync(DateTime fromDate, DateTime toDate, bool usePosted = false)
    {
        var mispicks = new List<Mispick>();
        var sessions = new List<PickSession>();
        var tagTool = new TagAssignmentTool();

        void Action()
        {
            mispicks = usePosted ? MispicksByPostedDate(fromDate, toDate) : MispicksByErrorDate(fromDate, toDate);
            sessions = Chariot.PullObjectList<PickSession>(s => s.Date >= fromDate && s.Date <= toDate);
            tagTool = TagAssignmentTool();
        }

        await Task.Run(() => RunInTransaction(Action)).ConfigureAwait(false);

        return (mispicks, sessions, tagTool);
    }
}