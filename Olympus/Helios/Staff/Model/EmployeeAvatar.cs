using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class EmployeeAvatar : Image
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }

        public override void GetImageFilePath() => Path.Combine(App.Charioteer.StaffReader.EmployeeAvatarDirectory, FileName);

    }
}
