using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class Department
{
    [PrimaryKey]
    public string Name { get; set; }
    [ForeignKey(typeof(Employee))]
    public int HeadID { get; set; }
    [ForeignKey(typeof(Department))]
    public string OverDepartmentName { get; set; }
    public string PayPoint { get; set; }

    [OneToOne]
    public Employee Head { get; set; }

    [OneToMany(inverseProperty: "Department", CascadeOperations = CascadeOperation.All)]
    public List<Shift> Shifts { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<Employee> Employees { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<Clan> Clans { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public List<Role> Roles { get; set; }
    [OneToMany(nameof(Roster.DepartmentName), nameof(Roster.Department), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }

    [OneToMany(inverseProperty: "OverDepartment")]
    public List<Department> SubDepartments { get; set; }
    [ManyToOne(inverseProperty: "SubDepartments")]
    public Department OverDepartment { get; set; }

    [ManyToMany(typeof(EmployeeDepartmentLoaning), "EmployeeID", "DepartmentsCanWorkIn", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> EmployeesCanLoan { get; set; }
    [ManyToMany(typeof(DepartmentProject), "ProjectName", "Departments", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Project> Projects { get; set; }

    public Department() {}

    public Department(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj) => Equals(obj as Department);

    public bool Equals(Department department)
    {
        if (department is null) return false;

        if (ReferenceEquals(this, department)) return true;

        if (GetType() !=  department.GetType()) return false;

        return Name == department.Name;
    }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();

    public static bool operator ==(Department lhs, Department rhs)
    {
        if (lhs is not null) return lhs.Equals(rhs);
        return rhs is null;
    }

    public static bool operator !=(Department lhs, Department rhs) => !(lhs == rhs);

}