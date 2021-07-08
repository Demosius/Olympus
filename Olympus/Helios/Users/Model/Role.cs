using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Users.Model
{
    public class Role
    {
        [PrimaryKey]
        public string Name { get; set; }

        public int CreateUser { get; set; }
        public int DeleteUser { get; set; }
        public int ReadUser { get; set; }
        public int UpdateUser { get; set; }

        public int CreateEmployee { get; set; }
        public int DeleteEmployee { get; set; }
        public int ReadEmployee { get; set; }
        public int UpdateEmployee { get; set; }

        public bool CreateDepartment { get; set; }
        public bool DeleteDepartment { get; set; }
        public bool UpdateDepartment { get; set; }

        public bool AssignRole { get; set; }

        public bool CreateClan { get; set; }
        public bool DeleteClan { get; set; }
        public bool UpdateClan { get; set; }

        public int CreateShift { get; set; }
        public int DeleteShift { get; set; }
        public int UpdateShift { get; set; }

        public bool CreateLicence { get; set; }
        public bool DeleteLicence { get; set; }
        public bool ReadLicence { get; set; }
        public bool UpdateLicence { get; set; }

        public bool CreateVehicle { get; set; }
        public bool DeleteVehicle { get; set; }
        public bool ReadVehicle { get; set; }
        public bool UpdateVehicle { get; set; }

        [OneToMany]
        public List<User> Users { get; set; }
    }
}
