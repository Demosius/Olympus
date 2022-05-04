using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Users.Model;

public class Role : IEquatable<Role>
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

    public string Description { get; set; }

    [OneToMany(nameof(User.RoleName), nameof(User.Role), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<User> Users { get; set; }

    [Ignore] public int UserPermissionsTotal => CreateUser + ReadUser + UpdateUser + DeleteUser;
    [Ignore] public int UserCount => Users.Count;

    public const string Default = "Default";
    public const string Master = "DatabaseManager";

    public Role()
    {
        Name = string.Empty;
        Users = new List<User>();
        Description = string.Empty;
    }

    public Role(string name, int createUser, int readUser, int updateUser, int deleteUser, bool createEmployee, int readEmployee,
        int readEmployeeSensitive, int readEmployeeVerySensitive, int updateEmployee, int deleteEmployee, bool createDepartment,
        bool updateDepartment, bool deleteDepartment, bool assignRole, bool editRoles, bool createClan, bool updateClan,
        bool deleteClan, int createShift, int updateShift, int deleteShift, bool createLicence, bool readLicence, bool updateLicence,
        bool deleteLicence, bool createVehicle, bool readVehicle, bool updateVehicle, bool deleteVehicle, bool copyDatabase,
        bool moveDatabase, List<User> users, string description)
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
        Description = description;
    }

    public void SetDefault()
    {
        Name = Default;

        CreateUser = -1;
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
        Name = Master;

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

    /// <summary>
    /// Checks if the Role is a standard (default/DatabaseManager) and sets the Properties accordingly.
    /// </summary>
    /// <returns>The name of the Role.</returns>
    public string CheckStandards()
    {
        switch (Name)
        {
            case Default:
                SetDefault();
                break;
            case Master:
                SetMaster();
                break;
        }

        return Name;
    }

    public override string ToString() => Name;

    /// <summary>
    /// Determines if this role is master to - has a higher or equal value in every field - the given other role.
    /// Must have at least one higher, and none lower.
    /// </summary>
    /// <param name="otherRole"></param>
    /// <returns></returns>
    public bool IsMasterTo(Role? otherRole)
    {
        if (otherRole is null || ReferenceEquals(this, otherRole)) return false;
        if (Name == "DatabaseManager") return true;

        var higher = false;
        // Users
        if (CreateUser < otherRole.CreateUser) return false;
        higher |= CreateUser > otherRole.CreateUser;
        if (ReadUser < otherRole.ReadUser) return false;
        higher |= ReadUser > otherRole.ReadUser;
        if (UpdateUser < otherRole.UpdateUser) return false;
        higher |= UpdateUser > otherRole.UpdateUser;
        if (DeleteUser < otherRole.DeleteUser) return false;
        higher |= DeleteUser > otherRole.DeleteUser;
        // Employees
        if (!CreateEmployee && otherRole.CreateEmployee) return false;
        higher |= CreateEmployee && !otherRole.CreateEmployee;
        if (ReadEmployee < otherRole.ReadEmployee) return false;
        higher |= ReadEmployee > otherRole.ReadEmployee;
        if (ReadEmployeeSensitive < otherRole.ReadEmployeeSensitive) return false;
        higher |= ReadEmployeeSensitive > otherRole.ReadEmployeeSensitive;
        if (ReadEmployeeVerySensitive < otherRole.ReadEmployeeVerySensitive) return false;
        higher |= ReadEmployeeVerySensitive > otherRole.ReadEmployeeVerySensitive;
        if (UpdateEmployee < otherRole.UpdateEmployee) return false;
        higher |= UpdateEmployee > otherRole.UpdateEmployee;
        if (DeleteEmployee < otherRole.DeleteEmployee) return false;
        higher |= DeleteEmployee > otherRole.DeleteEmployee;
        // Departments
        if (!CreateDepartment && otherRole.CreateDepartment) return false;
        higher |= CreateDepartment && !otherRole.CreateDepartment;
        if (!UpdateDepartment && otherRole.UpdateDepartment) return false;
        higher |= UpdateDepartment && !otherRole.UpdateDepartment;
        if (!DeleteDepartment && otherRole.DeleteDepartment) return false;
        higher |= DeleteDepartment && !otherRole.DeleteDepartment;
        // User Roles
        if (!AssignRole && otherRole.AssignRole) return false;
        higher |= AssignRole && !otherRole.AssignRole;
        if (!EditRoles && otherRole.EditRoles) return false;
        higher |= EditRoles && !otherRole.EditRoles;
        // Clans
        if (!CreateClan && otherRole.CreateClan) return false;
        higher |= CreateClan && !otherRole.CreateClan;
        if (!UpdateClan && otherRole.UpdateClan) return false;
        higher |= UpdateClan && !otherRole.UpdateClan;
        if (!DeleteClan && otherRole.DeleteClan) return false;
        higher |= DeleteClan && !otherRole.DeleteClan;
        // Shifts
        if (CreateShift < otherRole.CreateShift) return false;
        higher |= CreateShift > otherRole.CreateShift;
        if (UpdateShift < otherRole.UpdateShift) return false;
        higher |= UpdateShift > otherRole.UpdateShift;
        if (DeleteShift < otherRole.DeleteShift) return false;
        higher |= DeleteShift > otherRole.DeleteShift;
        // Licences
        if (!CreateLicence && otherRole.CreateLicence) return false;
        higher |= CreateLicence && !otherRole.CreateLicence;
        if (!ReadLicence && otherRole.ReadLicence) return false;
        higher |= ReadLicence && !otherRole.ReadLicence;
        if (!UpdateLicence && otherRole.UpdateLicence) return false;
        higher |= UpdateLicence && !otherRole.UpdateLicence;
        if (!DeleteLicence && otherRole.DeleteLicence) return false;
        higher |= DeleteLicence && !otherRole.DeleteLicence;
        // Vehicles
        if (!CreateVehicle && otherRole.CreateVehicle) return false;
        higher |= CreateVehicle && !otherRole.CreateVehicle;
        if (!ReadVehicle && otherRole.ReadVehicle) return false;
        higher |= ReadVehicle && !otherRole.ReadVehicle;
        if (!UpdateVehicle && otherRole.UpdateVehicle) return false;
        higher |= UpdateVehicle && !otherRole.UpdateVehicle;
        if (!DeleteVehicle && otherRole.DeleteVehicle) return false;
        higher |= DeleteVehicle && !otherRole.DeleteVehicle;
        // Other
        if (!ManageLockers && otherRole.ManageLockers) return false;
        higher |= ManageLockers && !otherRole.ManageLockers;
        if (!CopyDatabase && otherRole.CopyDatabase) return false;
        higher |= CopyDatabase && !otherRole.CopyDatabase;
        if (!MoveDatabase && otherRole.MoveDatabase) return false;
        higher |= MoveDatabase && !otherRole.MoveDatabase;

        return higher;
    }

    public bool Equals(Role? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is Role otherRole && Equals(otherRole);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Name.GetHashCode();
    }

    public static bool operator ==(Role lh, Role rh) => lh.Equals(rh);
    public static bool operator !=(Role lh, Role rh) => !(lh == rh);
}