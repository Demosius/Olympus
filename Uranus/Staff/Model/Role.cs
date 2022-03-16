using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model;

public class Role : IComparable
{
    [PrimaryKey] public string Name { get; set; }
    [ForeignKey(typeof(Department)), NotNull] public string DepartmentName { get; set; }
    public int Level { get; set; }
    [ForeignKey(typeof(Role))] public string ReportsToRoleName { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Roles), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department Department { get; set; }
    [ManyToOne(nameof(ReportsToRoleName), nameof(Reports), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Role ReportsToRole { get; set; }

    [OneToMany(nameof(Employee.RoleName), nameof(Employee.Role), CascadeOperations = CascadeOperation.All)]
    public List<Employee> Employees { get; set; }
    [OneToMany(nameof(ReportsToRoleName), nameof(ReportsToRole), CascadeOperations = CascadeOperation.All)]
    public List<Role> Reports { get; set; }

    public Role()
    {
        Name = "UniqueRole";
    }

    public Role(string name)
    {
        Name = name;
    }

    public bool LookDown(ref int down, ref Role targetRole)
    {
        if (this == targetRole)
        {
            ++down;
            return true;
        }
        foreach (var role in Reports)
        {
            if (!role.LookDown(ref down, ref targetRole)) continue;
            ++down;
            return true;
        }
        return false;
    }

    public bool LookUp(ref int up, ref int down, Role refRole, ref Role targetRole)
    {
        ++up;
        if (this == targetRole) return true;
        foreach (var role in Reports.Where(role => role != refRole))
        {
            if (!role.LookDown(ref down, ref targetRole)) continue;
            ++down;
            return true;
        }
        return ReportsToRole is not null && ReportsToRole.LookUp(ref up, ref down, this, ref targetRole);
    }

    public void AddReportingRole(Role reportingRole)
    {
        reportingRole.ReportsToRole = this;
        Reports ??= new List<Role>();
        Reports.Add(reportingRole);
    }

    public void AddEmployee(Employee employee)
    {
        if (employee is null) return;
        employee.Role = this;
        employee.RoleName = Name;
        Employees.Add(employee);
    }

    public void AddEmployees(IEnumerable<Employee> newEmployees)
    {
        if (newEmployees is null) return;
        var employeeArray = newEmployees as Employee[] ?? newEmployees.ToArray();
        foreach (var employee in employeeArray)
        {
            employee.Role = this;
            employee.RoleName = Name;
        }

        Employees ??= new List<Employee>();
        Employees.AddRange(employeeArray);
    }

    public override string ToString()
    {
        return Name;
    }

    public int CompareTo(object obj)
    {
        if (obj is not Role otherRole) return -1;

        if (Level < otherRole.Level) return 1;
        return Level > otherRole.Level ? -1 : string.Compare(Name, otherRole.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => Equals(obj as Role);

    public bool Equals(Role role)
    {
        if (role is null) return false;

        if (ReferenceEquals(this, role)) return true;

        if (GetType() != role.GetType()) return false;

        return Name == role.Name;
    }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Name.GetHashCode();

    public static bool operator ==(Role lhs, Role rhs)
    {
        if (lhs is not null) return lhs.Equals(rhs);
        return rhs is null;
    }

    public static bool operator !=(Role lhs, Role rhs) => !(lhs == rhs);

    public static bool operator >(Role lhs, Role rhs)
    {
        return lhs.Level > rhs.Level ||
               lhs.Level == rhs.Level && string.CompareOrdinal(lhs.Name, rhs.Name) > 0;
    }

    public static bool operator <(Role lhs, Role rhs)
    {
        return lhs.Level < rhs.Level ||
               lhs.Level == rhs.Level && string.CompareOrdinal(lhs.Name, rhs.Name) < 0;
    }

    public static bool operator >=(Role lhs, Role rhs) => lhs > rhs || lhs == rhs;
    public static bool operator <=(Role lhs, Role rhs) => lhs < rhs || lhs == rhs;
}