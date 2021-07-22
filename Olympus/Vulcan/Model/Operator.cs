using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Olympus.Helios.Inventory.Model;
using Olympus.Helios.Staff.Model;
using SQLite;

namespace Olympus.Vulcan.Model
{
    class Operator
    {
        [Ignore]
        public Employee Employee { get; set; }
        [Ignore]
        public int MyProperty { get; set; }


    }
}
