using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model
{
    public class EmployeeProject
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        [ForeignKey(typeof(Project))]
        public string ProjectName { get; set; }
    }
}
