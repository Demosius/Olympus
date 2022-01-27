using SQLiteNetExtensions.Attributes;
using System.IO;

namespace Uranus.Staff.Model
{
    public class EmployeeAvatar : Image
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Employee Employee { get; set; }

        public override string GetImageFilePath(StaffReader reader) => Path.Combine(reader.EmployeeAvatarDirectory, FileName);

    }
}
