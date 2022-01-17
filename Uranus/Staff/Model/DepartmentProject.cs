using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    public class DepartmentProject
    {
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        [ForeignKey(typeof(Project))]
        public string ProjectName { get; set; }
    }
}
