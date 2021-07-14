using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Staff.Model
{
    public class Role
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Department))]
        public string DepartmentName { get; set; }
        public int Level { get; set; }
        [ForeignKey(typeof(Role))]
        public string ReportsToRoleName { get; set; }

        [ManyToOne]
        public Department Department { get; set; }
        [OneToOne]
        public Role ReprortsToRole { get; set; }
        [OneToMany]
        public List<Employee> Employees { get; set; }
        [OneToMany]
        public List<Role> Reports { get; set; }


        public void SetDepartment(Department department)
        {
            Department = department;
            DepartmentName = department.Name;
            department.Roles.Add(this);
        }

        public bool LookDown(ref int down, ref Role targetRole)
        {
            if (this == targetRole)
            {
                ++down;
                return true;
            }
            foreach (Role role in Reports)
            {
                if (role.LookDown(ref down, ref targetRole))
                {
                    ++down;
                    return true;
                }
            }
            return false;
        }

        public bool LookUp(ref int up, ref int down, Role refRole, ref Role targetRole)
        {
            ++up;
            if (this == targetRole) return true;
            foreach(Role role in Reports)
            {
                if (role != refRole)
                {
                    if (role.LookDown(ref down, ref targetRole))
                    {
                        ++down;
                        return true;
                    }
                }
            }
            if (ReprortsToRole is null) return false;
            return ReprortsToRole.LookUp(ref up, ref down, this, ref targetRole);
        }


        public override bool Equals(object obj) => this.Equals(obj as Role);

        public bool Equals(Role role)
        {
            if (role is null) return false;

            if (Object.ReferenceEquals(this, role)) return true;

            if (this.GetType() != role.GetType()) return false;

            return Name == role.Name;
        }

        public override int GetHashCode() => (Name, DepartmentName, Level, ReportsToRoleName).GetHashCode();

        public static bool operator ==(Role lhs, Role rhs)
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

        public static bool operator !=(Role lhs, Role rhs) => !(lhs == rhs);
    }
}
