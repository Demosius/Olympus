using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Staff.Model
{
    public class EmployeeIcon : Image
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }

        public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.EmployeeIconDirectory, FileName);

    }
}
