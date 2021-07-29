using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff.Model
{
    public class EmployeeProject
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        [ForeignKey(typeof(Project))]
        public string ProjectName { get; set; }
    }
}
