using Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Uranus.Staff
{
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
            returnDict ??= new();

            if (employee.Reports is null) return;

            foreach (var report in employee.Reports)
            {
                if (returnDict.ContainsKey(report.ID)) continue;
                returnDict.Add(report.ID, report);
                GetReports(report, ref returnDict);
            }
        }

        public IEnumerable<Employee> EmployeesRecursiveReports()
        {
            var fullEmployees = Chariot.Database.Table<Employee>().ToDictionary(e => e.ID, e => e);

            foreach (var (_, employee) in fullEmployees)
            {
                if (!fullEmployees.TryGetValue(employee.ReportsToID, out var manager)) continue;
                manager.Reports ??= new();
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
    }
}
