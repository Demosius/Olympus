using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model;

public class Department : IComparable
{
    [PrimaryKey] public string Name { get; set; }
    [ForeignKey(typeof(Employee))] public int HeadID { get; set; }
    [ForeignKey(typeof(Department))] public string OverDepartmentName { get; set; }
    public string PayPoint { get; set; }

    [OneToOne(nameof(HeadID), nameof(Employee.Department), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Head { get; set; }

    [OneToMany(nameof(Shift.DepartmentName), nameof(Shift.Department), CascadeOperations = CascadeOperation.All)]
    public List<Shift> Shifts { get; set; }
    [OneToMany(nameof(Employee.DepartmentName), nameof(Employee.Department), CascadeOperations = CascadeOperation.All)]
    public List<Employee> Employees { get; set; }
    [OneToMany(nameof(Clan.DepartmentName), nameof(Clan.Department), CascadeOperations = CascadeOperation.All)]
    public List<Clan> Clans { get; set; }
    [OneToMany(nameof(Role.DepartmentName), nameof(Role.Department), CascadeOperations = CascadeOperation.All)]
    public List<Role> Roles { get; set; }
    [OneToMany(nameof(Roster.DepartmentName), nameof(Roster.Department), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Roster> Rosters { get; set; }

    [OneToMany(nameof(OverDepartmentName), nameof(OverDepartment), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Department> SubDepartments { get; set; }
    [ManyToOne(nameof(OverDepartmentName), nameof(SubDepartments), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department OverDepartment { get; set; }

    [ManyToMany(typeof(EmployeeDepartmentLoaning), nameof(EmployeeDepartmentLoaning.DepartmentName), nameof(Employee.DepartmentsCanWorkIn), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> EmployeesCanLoan { get; set; }
    [ManyToMany(typeof(DepartmentProject), nameof(DepartmentProject.DepartmentName), nameof(Project.Departments), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Project> Projects { get; set; }

    public Department() { }

    public Department(string name)
    {
        Name = name;
    }

    public void AddSubDepartment(Department department)
    {
        if (department is null) return;
        department.OverDepartmentName = Name;
        department.OverDepartment = this;
        SubDepartments ??= new List<Department>();
        SubDepartments.Add(department);
    }

    public void AddShifts(IEnumerable<Shift> shifts)
    {
        var shiftArr = shifts as Shift[] ?? shifts.ToArray();

        if (!shiftArr.Any()) return;

        foreach (var shift in shiftArr)
        {
            shift.DepartmentName = Name;
            shift.Department = this;
        }

        Shifts.AddRange(shiftArr);
    }

    public override string ToString()
    {
        return Name;
    }

    public int CompareTo(object obj)
    {
        if (obj is not Department otherDepartment) return -1;

        return string.Compare(Name, otherDepartment.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => Equals(obj as Department);

    public bool Equals(Department department)
    {
        if (department is null) return false;

        if (ReferenceEquals(this, department)) return true;

        if (GetType() != department.GetType()) return false;

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