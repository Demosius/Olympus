using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Staff.Models;

namespace Uranus.Users.Models;

public class User
{
    [PrimaryKey] public int ID { get; set; }
    [ForeignKey(typeof(Role))] public string RoleName { get; set; }

    [ManyToOne(nameof(RoleName), nameof(Models.Role.Users), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Role? Role { get; set; }

    [Ignore]
    public Employee? Employee { get; set; }

    public User()
    {
        RoleName = string.Empty;
    }

    public User(int id, string roleName)
    {
        ID = id;
        RoleName = roleName;
    }

    public User(Employee employee, Role role)
    {
        Employee = employee;
        Role = role;
        ID = employee.ID;
        RoleName = role.Name;
    }

    public void SetRole(Role role)
    {
        Role = role;
        RoleName = role.Name;
    }
}