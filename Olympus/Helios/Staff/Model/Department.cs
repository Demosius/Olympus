using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Staff.Model
{
    public class Department
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Employee))]
        public int HeadID { get; set; }

        [OneToOne]
        public Employee Head { get; set; }
        [OneToMany]
        public List<Shift> Shifts { get; set; } = new List<Shift> { };
        [OneToMany]
        public List<Employee> Employees { get; set; } = new List<Employee> { };
        [OneToMany]
        public List<Clan> Clans { get; set; } = new List<Clan> { };
        [OneToMany]
        public List<Role> Roles { get; set; } = new List<Role> { };
        [ManyToMany(typeof(Employee))]
        public List<Employee> EmployeesCanBorrow { get; set; }

        public void SetHead(Employee employee)
        {
            Head = employee;
            HeadID = employee.ID;
        }

        public override bool Equals(object obj) => this.Equals(obj as Department);

        public bool Equals(Department department)
        {
            if (department is null) return false;

            if (Object.ReferenceEquals(this, department)) return true;

            if (this.GetType() != department.GetType()) return false;

            return Name == department.Name;
        }

        public override int GetHashCode() => (Name, HeadID).GetHashCode();

        public static bool operator ==(Department lhs, Department rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Department lhs, Department rhs) => !(lhs == rhs);

    }
}
