using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Locker
    {
        public string ID { get; set; }
        public string Location { get; set; }
        private Employee _employee;
        public Employee Employee 
        { 
            get { return _employee; }
            set 
            { 
                _employee.Locker = null;
                _employee = value;
            } 
        }

        public Locker() { }

        public Locker(string id, string location, Employee employee)
        {
            ID = id;
            Location = location;
            Employee = employee;
        }
    }
}
