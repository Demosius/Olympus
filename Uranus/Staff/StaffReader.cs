using Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Uranus.Staff
{
    public class StaffReader
    {
        public StaffChariot Chariot { get; set; }

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

        public bool EmployeeExists(int id) => Chariot.Database.Execute("SELECT count(*) FROM Employee WHERE ID=?;", id) > 0;

        public int EmployeeCount() => Chariot.Database.Execute("SELECT count(*) FROM Employee;");

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
        public List<Project> Projects(Expression<Func<Project, bool>> filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    }
}
