using Olympus.Helios.Staff.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff
{
    public class StaffReader
    {
        public StaffChariot Chariot { get; set; }

        public StaffReader(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

        /* EMPLOYEES */
        public Employee Employee(int ID, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<Employee>(ID, pullType);

        public bool EmployeeExists(int ID) => Chariot.Database.Execute("SELECT count(*) FROM Employee WHERE ID=?;", ID) > 0;

        public int EmployeeCount() => Chariot.Database.Execute("SELECT count(*) FROM Employee;");
    }
}
