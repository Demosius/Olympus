using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class Clan
{
    [PrimaryKey] public string Name { get; set; }
    [ForeignKey(typeof(Employee))] public int LeaderID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Model.Department.Clans), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department Department { get; set; }

    [OneToOne(nameof(LeaderID), nameof(Employee.Clan), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee Leader { get; set; }

    [OneToMany(nameof(Employee.ClanName), nameof(Employee.Clan), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; } = new();
}