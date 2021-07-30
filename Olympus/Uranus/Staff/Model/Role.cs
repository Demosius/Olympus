using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Staff.Model
{
    public class Role
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Department)), NotNull]
        public string DepartmentName { get; set; }
        [NotNull]
        public int Level { get; set; }
        [ForeignKey(typeof(Role))]
        public string ReportsToRoleName { get; set; }

        //private Department department;
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Department Department { get; set; }
        //{
        //    get => department; 
        //    set
        //    {
        //        department = value;
        //        DepartmentName = value.Name;
        //        value.Roles.Add(this);
        //    }
        //}
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Role ReprortsToRole { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Employee> Employees { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Role> Reports { get; set; }

        public Role()
        {
            //Employees = new List<Employee> { };
            //Reports = new List<Role> { };
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


        public override bool Equals(object obj) => Equals(obj as Role);

        public bool Equals(Role role)
        {
            if (role is null) return false;

            if (ReferenceEquals(this, role)) return true;

            if (GetType() != role.GetType()) return false;

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
