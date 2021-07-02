using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympus.Helios.Users;
using Olympus.Helios.Staff;

namespace Olympus.Styx.Model
{
    public class Charon
    {
        public User User { get; set; }
        public Employee Employee { get; set; }

        public Charon() { }

        public Charon(User user, Employee employee)
        {
            User = user;
            Employee = employee;
        }

    }
}
