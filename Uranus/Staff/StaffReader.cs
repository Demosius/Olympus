using Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serilog;

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
    public Employee Employee(int id, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Employee>(id, pullType);

    public List<Employee> Employees(Expression<Func<Employee, bool>> filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public bool EmployeeExists(int id) => Chariot.Database.Execute("SELECT count(*) FROM Employee WHERE ID=?;", id) > 0;

    public int EmployeeCount() => Chariot.Database.Execute("SELECT count(*) FROM Employee;");

    public Role Role(string roleName, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<Role>(roleName, pullType);
    
    public List<ClockEvent> ClockEvents(Expression<Func<ClockEvent, bool>> filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<ClockEvent> ClockEvents(DateTime startDate, DateTime endDate)
    {
        return Chariot.Database.Query<ClockEvent>("SELECT * FROM ClockEvent WHERE Date BETWEEN ? AND ?;", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public List<ClockEvent> ClockEvents(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        return Chariot.Database.Query<ClockEvent>($"SELECT * FROM ClockEvent WHERE EmployeeID in ({string.Join(", ", employeeIDs)}) AND Date BETWEEN ? AND ?;",
            startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }

    public List<ShiftEntry> ShiftEntries(Expression<Func<ShiftEntry, bool>> filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<int> EmployeeIDs() => Chariot.Database.Query<Employee>("SELECT DISTINCT ID FROM Employee;").Select(e => e.ID).OrderBy(c => c).ToList();

    public void AionEmployeeRefresh(out List<int> managerIDList, out List<Employee> employeeList, out IEnumerable<string> locationList,
        out IEnumerable<string> payPointList, out IEnumerable<EEmploymentType> employmentTypeList, out IEnumerable<Role> roleList, out IEnumerable<Department> departmentList)
    {
        IEnumerable<int> managerIDs = null;
        IEnumerable<Employee> employees = null;
        IEnumerable<string> locations = null;
        IEnumerable<string> payPoints = null;
        IEnumerable<EEmploymentType> employmentTypes = null;
        IEnumerable<Role> roles = null;
        IEnumerable<Department> departments = null;

        Chariot.Database.RunInTransaction(() =>
        {
            managerIDs = Chariot.Database.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;").Select(e => e.ReportsToID);
            employees = Chariot.Database.Table<Employee>();
            locations = Chariot.Database.Query<Employee>("SELECT DISTINCT Location FROM Employee;").Select(e => e.Location);
            payPoints = Chariot.Database.Query<Employee>("SELECT DISTINCT PayPoint FROM Employee;").Select(e => e.PayPoint);
            employmentTypes = Chariot.Database.Query<Employee>("SELECT DISTINCT EmploymentType FROM Employee;").Select(e => e.EmploymentType);
            roles = Chariot.PullObjectList<Role>().OrderBy(r => r.Name);
            departments = Chariot.Database.Table<Department>();
        });

        managerIDList = managerIDs.ToList();
        employeeList = employees.ToList();
        locationList = locations;
        payPointList = payPoints;
        employmentTypeList = employmentTypes;
        roleList = roles;
        departmentList = departments;
    }

    public List<int> GetManagerIDs() => Chariot.Database.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;").Select(e => e.ReportsToID).ToList();

    public IEnumerable<string> Locations() => Chariot.Database.Query<Employee>("SELECT DISTINCT Location FROM Employee;").Select(e => e.Location);

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
        returnDict ??= new Dictionary<int, Employee>();

        if (employee.Reports is null) return;

        foreach (var report in employee.Reports)
        {
            if (returnDict.ContainsKey(report.ID)) continue;
            returnDict.Add(report.ID, report);
            GetReports(report, ref returnDict);
        }
    }

    /// <summary>
    /// Get all the employees that recursively have roles that report to the given role.
    /// </summary>
    /// <param name="headRole">The role of the employee for whom we are gathering reporting employees.</param>
    /// <returns></returns>
    public IEnumerable<Employee> GetReportsByRole(Role headRole)
    {
        var roles = GetReportingRoles(headRole);
        var employees = roles.SelectMany(r => r.Employees ?? new List<Employee>());
        
        return employees;
    }

    /// <summary>
    /// Get all the reporting roles based on the given role.
    /// </summary>
    /// <param name="headRole"></param>
    /// <returns>A collection of roles, each of which containing the relevant employees.</returns>
    public IEnumerable<Role> GetReportingRoles(Role headRole)
    {
        Dictionary<string, Role> roleDict = null;
        Dictionary<string, List<Employee>> employeeDict = null;

        try
        {
            Chariot.Database.RunInTransaction(() =>
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
            Log.Error(ex, "Unable to pull Roles and/or Employees from {}, defaulting to empty.",Chariot.DatabaseName);
            roleDict = new Dictionary<string, Role>();
            employeeDict = new Dictionary<string, List<Employee>>();
        }

        if (roleDict is null || employeeDict is null) return new List<Role>();

        foreach (var (_, role) in roleDict)
        {
            if (roleDict.TryGetValue(role.ReportsToRoleName, out var upperRole))
                upperRole.AddReportingRole(role);
        }

        foreach (var (roleName,employees) in employeeDict)
        {
            if (roleDict.TryGetValue(roleName, out var role))
                role.AddEmployees(employees);
        }

        roleDict.TryGetValue(headRole.Name, out headRole);

        roleDict.Clear();

        GetRoleReports(headRole, ref roleDict);

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
        returnDict ??= new Dictionary<string, Role>();

        if (role.Reports is null) return;

        foreach (var report in role.Reports)
        {
            if (returnDict.ContainsKey(report.Name)) continue;
            returnDict.Add(report.Name, report);
            GetRoleReports(report, ref returnDict);
        }
    }

    public IEnumerable<Employee> EmployeesRecursiveReports()
    {
        var fullEmployees = Chariot.Database.Table<Employee>().ToDictionary(e => e.ID, e => e);

        foreach (var (_, employee) in fullEmployees)
        {
            if (!fullEmployees.TryGetValue(employee.ReportsToID, out var manager)) continue;
            manager.Reports ??= new List<Employee>();
            manager.Reports.Add(employee);
            employee.ReportsTo = manager;
        }

        return fullEmployees.Select(e => e.Value);
    }

    public List<ShiftEntry> GetFilteredEntries(DateTime minDate, DateTime maxDate)
    {
        Dictionary<int, Employee> employees = null;
        List<ShiftEntry> entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        Chariot.Database.RunInTransaction(() =>
        {
            employees = Employees().ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database.Query<ShiftEntry>("SELECT DailyEntry.* FROM DailyEntry JOIN Employee E on DailyEntry.EmployeeID = E.Code WHERE Date BETWEEN ? AND ?;",
                startString, endString).ToList();

        });

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
        Dictionary<int, Employee> employees = null;
        List<ShiftEntry> entries = null;

        var startString = minDate.ToString("yyyy-MM-dd");
        var endString = maxDate.ToString("yyyy-MM-dd");

        Chariot.Database.RunInTransaction(() =>
        {
            employees = GetManagedEmployees(managerID).ToDictionary(e => e.ID, e => e);
            entries = Chariot.Database.Query<ShiftEntry>("SELECT * FROM ShiftEntry " +
                                                         $"WHERE EmployeeID IN ({string.Join(", ", employees.Select(d => d.Value.ID))}) " +
                                                         "AND Date BETWEEN ? AND ?;",
                startString, endString).ToList();

        });

        foreach (var shiftEntry in entries)
        {
            if (employees.TryGetValue(shiftEntry.EmployeeID, out var employee))
                shiftEntry.Employee = employee;
        }

        return entries;
    }

    public IEnumerable<ClockEvent> GetPendingClocks(int managerCode, DateTime fromDate, DateTime toDate)
    {
        return Chariot.Database.Query<ClockEvent>(
            "SELECT ClockEvent.* FROM ClockEvent JOIN Employee ON ClockEvent.EmployeeID = Employee.ID " +
            "WHERE Employee.ReportsToID = ? " +
            "AND ClockEvent.Status = ?" +
            "AND Date BETWEEN ? AND ?;",
            managerCode, EClockStatus.Pending,
            fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
    }

    public int GetPendingCount(IEnumerable<int> employeeIDs, DateTime startDate, DateTime endDate)
    {
        return Chariot.Database.ExecuteScalar<int>(
            $"SELECT COUNT(*) FROM ClockEvent " +
            $"WHERE EmployeeID IN ({string.Join(", ", employeeIDs)}) AND " +
            $"Status = ? AND " +
            $"Date BETWEEN '{startDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}';",
            EClockStatus.Pending);
    }

    /// <summary>
    /// Gets all employees that have at least one other employee as a direct report.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Employee> GetManagers()
    {
        IEnumerable<Employee> managers = null;

        Chariot.Database.RunInTransaction(() =>
        {
            var managerIDs = GetManagerIDs();
            managers = Chariot.Database.Query<Employee>(
                $"SELECT * FROM Employee WHERE ID IN ({string.Join(", ", managerIDs)});");
        });

        return managers;
    }

    /// <summary>
    /// Gets a full list of today's clock events for the given employee code.
    /// </summary>
    /// <returns></returns>
    public List<ClockEvent> ClocksForToday(int employeeID)
    {
        return Chariot.Database.Query<ClockEvent>("SELECT * FROM ClockEvent WHERE EmployeeID = ? AND Date = ? AND Status <> ?;",
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
    /// Returns a list of all employees that have direct reports.
    /// </summary>
    /// <returns>List of Employees</returns>
    public List<Employee> Managers()
    {
        var conn = Chariot.Database;
        //List<int> employeeIDs = conn.Query<int>("SELECT DISTINCT ReportsToID FROM Employee;");
        var employeeIDs = conn.Query<Employee>("SELECT DISTINCT ReportsToID FROM Employee;").Select(e => e.ReportsToID).ToList();

        return conn.Query<Employee>($"SELECT * FROM Employee WHERE ID IN ({string.Join(", ", employeeIDs)});");
    }

    /* DEPARTMENTS */
    public List<Department> Departments(Expression<Func<Department, bool>> filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /// <summary>
    /// Pulls all relevant sub departments according to the given department name.
    /// Will also fill relevant department data, such as shifts.
    /// </summary>
    /// <param name="departmentName"></param>
    /// <returns></returns>
    public IEnumerable<Department> SubDepartments(string departmentName)
    {
        Dictionary<string, Department> deptDict = null;
        Dictionary<string, List<Shift>> shiftDict = null;

        try
        {
            Chariot.Database.RunInTransaction(() =>
            {
                deptDict = Chariot.PullObjectList<Department>().ToDictionary(d => d.Name, d => d);
                shiftDict = Chariot.PullObjectList<Shift>()
                    .GroupBy(s => s.DepartmentName)
                    .ToDictionary(g => g.Key, g => g.ToList());
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to pull shift and/or department data from {}. Defaulted to null values.", Chariot.DatabaseName);
            deptDict = new Dictionary<string, Department>();
            shiftDict = new Dictionary<string, List<Shift>>();
        }

        if (deptDict is null || shiftDict is null) return Array.Empty<Department>();

        foreach (var (deptName, shifts) in shiftDict)
        {
            if (deptDict.TryGetValue(deptName, out var department))
                department.AddShifts(shifts);
        }

        foreach (var (_, department) in deptDict)
        {
            if (deptDict.TryGetValue(department.OverDepartmentName ?? "", out var overDepartment))
                overDepartment.AddSubDepartment(department);
        }

        deptDict.TryGetValue(departmentName, out var headDepartment);

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
        returnDict ??= new Dictionary<string, Department>();

        if (department.SubDepartments is null) return;

        foreach (var sub in department.SubDepartments)
        {
            if (returnDict.ContainsKey(sub.Name)) continue;
            returnDict.Add(sub.Name, sub);
            GetSubDepartments(sub, ref returnDict);
        }
    }

    /* PROJECTS */
    public IEnumerable<Project> Projects(Expression<Func<Project, bool>> filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType).OrderBy(p => p.Name);

    public AionDataSet GetAionDataSet()
    {
        AionDataSet newSet = new();
        Chariot.Database.RunInTransaction(() =>
        {
            newSet.ClockEvents = ClockEvents().ToDictionary(c => c.ID, c => c);
            newSet.Employees = Employees().ToDictionary(e => e.ID, e => e);
            newSet.ShiftEntries = ShiftEntries().ToDictionary(e => e.ID, e => e);
        });
        return newSet;
    }

    /// <summary>
    /// Gets all of the shifts applicable to the current User - based on department.
    /// </summary>
    /// <param name="currentUser"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IEnumerable<Shift> Shifts(Employee currentUser)
    {
        // Get a list of all relevant departments.
        var departments = SubDepartments(currentUser.DepartmentName);



        return departments.SelectMany(d => d.Shifts);
    }
}