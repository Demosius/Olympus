using Olympus.Uranus.Staff.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff
{
    public class StaffReader
    {
        public StaffChariot Chariot { get; set; }

        public StaffReader(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

        /* DIRECTORIES */
        public string BaseDirectory { get => Chariot.BaseDataDirectory; }
        public string EmployeeIconDirectory { get => Chariot.EmployeeIconDirectory; }
        public string EmployeeAvatarDirectory { get => Chariot.EmployeeAvatarDirectory; }
        public string ProjectIconDirectory { get => Chariot.ProjectIconDirectory; }
        public string LicenceImageDirectory { get => Chariot.LicenceImageDirectory; }
         
        /* EMPLOYEES */
        public Employee Employee(int ID, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<Employee>(ID, pullType);

        public bool EmployeeExists(int ID) => Chariot.Database.Execute("SELECT count(*) FROM Employee WHERE ID=?;", ID) > 0;

        public int EmployeeCount() => Chariot.Database.Execute("SELECT count(*) FROM Employee;");

        /* DEPARTMENTS */
        public List<Department> Departments(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<Department>(pullType);

        /* PROJECTS */
        public List<Project> Projects(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<Project>(pullType);
    }
}
