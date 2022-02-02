using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Model
{
    public class Role
    {
        [PrimaryKey]
        public string Name { get; set; }
        [ForeignKey(typeof(Department)), NotNull]
        public string DepartmentName { get; set; }
        public int Level { get; set; }
        [ForeignKey(typeof(Role))]
        public string ReportsToRoleName { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Department Department { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Role ReportsToRole { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Employee> Employees { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
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

        public override string ToString()
        {
            return Name;
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
    }
}
