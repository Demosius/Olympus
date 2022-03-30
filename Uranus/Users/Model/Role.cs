using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Users.Model;

public class Role
{
    [PrimaryKey] public string Name { get; set; }

    // Use int to refer to the difference between the user and the target user employee levels.
    public int CreateUser { get; set; }
    public int ReadUser { get; set; }
    public int UpdateUser { get; set; }
    public int DeleteUser { get; set; }

    // Use int to refer to the difference between the user and the target employee levels.
    public bool CreateEmployee { get; set; }
    public int ReadEmployee { get; set; }               // Basic visibility beyond name and department.
    public int ReadEmployeeSensitive { get; set; }      // Such as Phone, Address, etc.
    public int ReadEmployeeVerySensitive { get; set; }  // Pay rate and similar.
    public int UpdateEmployee { get; set; }
    public int DeleteEmployee { get; set; } // Can only remove employee if there is no associated User.

    public bool CreateDepartment { get; set; }
    public bool UpdateDepartment { get; set; }
    public bool DeleteDepartment { get; set; } // Can only be deleted if there are no employees associated.

    public bool AssignRole { get; set; }
    public bool EditRoles { get; set; } // Includes creation and deletion.

    public bool CreateClan { get; set; }
    public bool UpdateClan { get; set; }
    public bool DeleteClan { get; set; } // Can be done while employees are a part of the clan - but SHOULD also remove clan from the employee.

    // Three options: -1: No, 0: Same Department Only, 1: For Any.
    public int CreateShift { get; set; }
    public int UpdateShift { get; set; }
    public int DeleteShift { get; set; }

    public bool CreateLicence { get; set; }
    public bool ReadLicence { get; set; }
    public bool UpdateLicence { get; set; }
    public bool DeleteLicence { get; set; }

    public bool CreateVehicle { get; set; }
    public bool ReadVehicle { get; set; }
    public bool UpdateVehicle { get; set; }
    public bool DeleteVehicle { get; set; }

    public bool ManageLockers { get; set; }

    // Whole database operations.
    public bool CopyDatabase { get; set; }  // Most
    public bool MoveDatabase { get; set; }  // Only db admin/manager.

    [OneToMany(nameof(User.RoleName), nameof(User.Role), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<User> Users { get; set; }

    public Role()
    {
        Name = string.Empty;
        Users = new List<User>();
    }

    public Role(string name, int createUser, int readUser, int updateUser, int deleteUser, bool createEmployee, int readEmployee,
        int readEmployeeSensitive, int readEmployeeVerySensitive, int updateEmployee, int deleteEmployee, bool createDepartment,
        bool updateDepartment, bool deleteDepartment, bool assignRole, bool editRoles, bool createClan, bool updateClan,
        bool deleteClan, int createShift, int updateShift, int deleteShift, bool createLicence, bool readLicence, bool updateLicence,
        bool deleteLicence, bool createVehicle, bool readVehicle, bool updateVehicle, bool deleteVehicle, bool copyDatabase,
        bool moveDatabase, List<User> users)
    {
        Name = name;
        CreateUser = createUser;
        ReadUser = readUser;
        UpdateUser = updateUser;
        DeleteUser = deleteUser;
        CreateEmployee = createEmployee;
        ReadEmployee = readEmployee;
        ReadEmployeeSensitive = readEmployeeSensitive;
        ReadEmployeeVerySensitive = readEmployeeVerySensitive;
        UpdateEmployee = updateEmployee;
        DeleteEmployee = deleteEmployee;
        CreateDepartment = createDepartment;
        UpdateDepartment = updateDepartment;
        DeleteDepartment = deleteDepartment;
        AssignRole = assignRole;
        EditRoles = editRoles;
        CreateClan = createClan;
        UpdateClan = updateClan;
        DeleteClan = deleteClan;
        CreateShift = createShift;
        UpdateShift = updateShift;
        DeleteShift = deleteShift;
        CreateLicence = createLicence;
        ReadLicence = readLicence;
        UpdateLicence = updateLicence;
        DeleteLicence = deleteLicence;
        CreateVehicle = createVehicle;
        ReadVehicle = readVehicle;
        UpdateVehicle = updateVehicle;
        DeleteVehicle = deleteVehicle;
        CopyDatabase = copyDatabase;
        MoveDatabase = moveDatabase;
        Users = users;
    }

    public void SetDefault()
    {
        Name = "Default";

        CreateUser = -3;
        ReadUser = -3;
        UpdateUser = -3;
        DeleteUser = -3;

        CreateEmployee = false;
        ReadEmployee = -3;
        ReadEmployeeSensitive = -5;
        ReadEmployeeVerySensitive = -10;
        UpdateEmployee = -3;
        DeleteEmployee = -3;

        CreateDepartment = false;
        UpdateDepartment = false;
        DeleteDepartment = false;

        AssignRole = false;
        EditRoles = false;

        CreateClan = false;
        UpdateClan = false;
        DeleteClan = false;

        CreateShift = -1;
        UpdateShift = -1;
        DeleteShift = -1;

        CreateLicence = false;
        ReadLicence = false;
        UpdateLicence = false;
        DeleteLicence = false;

        CreateVehicle = false;
        ReadVehicle = false;
        UpdateVehicle = false;
        DeleteVehicle = false;

        CopyDatabase = false;
        MoveDatabase = false;
    }

    public void SetMaster()
    {
        Name = "DatabaseManager";

        CreateUser = 1000;
        ReadUser = 1000;
        UpdateUser = 1000;
        DeleteUser = 1000;

        CreateEmployee = true;
        ReadEmployee = 1000;
        ReadEmployeeSensitive = 1000;
        ReadEmployeeVerySensitive = 1000;
        UpdateEmployee = 1000;
        DeleteEmployee = 1000;

        CreateDepartment = true;
        UpdateDepartment = true;
        DeleteDepartment = true;

        AssignRole = true;
        EditRoles = true;

        CreateClan = true;
        UpdateClan = true;
        DeleteClan = true;

        CreateShift = 1;
        UpdateShift = 1;
        DeleteShift = 1;

        CreateLicence = true;
        ReadLicence = true;
        UpdateLicence = true;
        DeleteLicence = true;

        CreateVehicle = true;
        ReadVehicle = true;
        UpdateVehicle = true;
        DeleteVehicle = true;

        CopyDatabase = true;
        MoveDatabase = true;
    }
}