using Olympus.Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff
{
    public class StaffCreator
    {
        public StaffChariot Chariot { get; set; }

        public StaffCreator(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

        public bool Employee(Employee employee, PushType pushType = PushType.ObjectOnly) => Chariot.Create(employee, pushType);

        public bool Department(Department department, PushType pushType = PushType.ObjectOnly) => Chariot.Create(department, pushType);

        public bool Role(Role role, PushType pushType = PushType.ObjectOnly) => Chariot.Create(role, pushType);
    }
}
