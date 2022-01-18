using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uranus.Inventory.Model;
using Uranus.Staff.Model;
using SQLite;

namespace Vulcan.Model
{
    class Operator
    {
        [Ignore]
        public Employee Employee { get; set; }
        [Ignore]
        public int MyProperty { get; set; }


    }
}
