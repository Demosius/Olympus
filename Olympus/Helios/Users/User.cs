using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Olympus.Helios.Staff;

namespace Olympus.Helios.Users
{
    public class User
    {
        public int Number { get; set; }
        public Role Role { get; set; }
        public Employee Employee { get; set; }

        public User() { }

        public User(int userNumber, Role role, Employee employee)
        {
            Number = userNumber;
            Role = role;
            Employee = employee;
        }
    }

    public class Role
    {
        public string Name { get; set; }
        
        public int CreateUser { get; set; }
        public int DeleteUser { get; set; }
        public int ViewUser { get; set; }
        public int ModifyUser { get; set; }

        public int CreateEmployee { get; set; }
        public int DeleteEmployee { get; set; }
        public int ViewEmployee { get; set; }
        public int ModifyEmployee { get; set; }

        public bool CreateDepartment { get; set; }
        public bool DeleteDepartment { get; set; }
        public bool ModifyDepartment { get; set; }

        public bool AssignRole { get; set; }

        public bool CreateClan { get; set; }
        public bool DeleteClan { get; set; }
        public bool ModifyClan { get; set; }

        public int CreateShift { get; set; }
        public int DeleteShift { get; set; }
        public int ModifyShift { get; set; }

        public bool CreateLicence { get; set; }
        public bool DeleteLicence { get; set; }
        public bool ViewLicence { get; set; }
        public bool ModifyLicence { get; set; }

        public bool CreateVehicle { get; set; }
        public bool DeleteVehicle { get; set; }
        public bool ViewVehicle { get; set; }
        public bool ModifyVehicle { get; set; }

        public Role() { }

        public Role(string name,
                    int createUser=-2, int deleteUser=-2, int viewUser=-5, int modifyUser=-2,
                    int createEmployee=-2, int deleteEmployee=-2, int viewEmployee=-5, int modifyEmployee=-2,
                    bool createDepartment=false, bool deleteDepartment = false, bool modifyDepartment = false,
                    bool assignRole = false,
                    bool createClan = false, bool deleteClan = false,  bool modifyClan = false,
                    int createShift=-1, int deleteShift=-1, int modifyShift=-1,
                    bool createLicence = false, bool deleteLicence = false, bool viewLicence = false, bool modifyLicence = false,
                    bool createVehicle = false, bool deleteVehicle = false, bool viewVehicle = false, bool modifyVehicle = false)
        {
            Name = name;
            CreateUser = createUser; DeleteUser = deleteUser; ViewUser = viewUser; ModifyUser = modifyUser;
            CreateEmployee = createEmployee; DeleteEmployee = deleteEmployee; ViewEmployee = viewEmployee; ModifyEmployee = modifyEmployee;
            CreateDepartment = createDepartment; DeleteDepartment = deleteDepartment; ModifyDepartment = modifyDepartment;
            AssignRole = assignRole;
            CreateClan = createClan; DeleteClan = deleteClan; ModifyClan = modifyClan;
            CreateShift = createShift; DeleteShift = deleteShift; ModifyShift = modifyShift;
            CreateLicence = createLicence; DeleteLicence = deleteLicence; ViewLicence = viewLicence; ModifyLicence = modifyLicence;
            CreateVehicle = createVehicle; DeleteVehicle = deleteVehicle; ViewVehicle = viewVehicle; ModifyVehicle = modifyVehicle;
        }
    }
}
