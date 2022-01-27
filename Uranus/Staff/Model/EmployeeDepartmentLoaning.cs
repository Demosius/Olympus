using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Model
{
    public class EmployeeDepartmentLoaning
    {
        [ForeignKey(typeof(Employee))]
        public int EmployeeID { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
    }
}
