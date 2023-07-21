using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Models;

public class Clan
{
    [PrimaryKey] public string Name { get; set; }
    [ForeignKey(typeof(Employee))] public int LeaderID { get; set; }
    [ForeignKey(typeof(Department))] public string DepartmentName { get; set; }

    [ManyToOne(nameof(DepartmentName), nameof(Models.Department.Clans), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Department? Department { get; set; }

    [OneToOne(nameof(LeaderID), nameof(Employee.Clan), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Employee? Leader { get; set; }

    [OneToMany(nameof(Employee.ClanName), nameof(Employee.Clan), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Employee> Employees { get; set; } = new();

    public Clan()
    {
        Name = string.Empty;
        DepartmentName = string.Empty;
        Employees = new List<Employee>();
    }

    public Clan(string name, int leaderID, string departmentName, Department department, Employee leader)
    {
        Name = name;
        LeaderID = leaderID;
        DepartmentName = departmentName;
        Department = department;
        Leader = leader;
    }

    public override string ToString()
    {
        return $"{Name}{(DepartmentName != string.Empty ? $" ({DepartmentName})" : "")}";
    }
}