using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    // Denotes an employees eligibility for a particular shift.
    public class EmployeeShift 
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        [ForeignKey(typeof(Shift))]
        public string ShiftName { get; set; }
    }
}
