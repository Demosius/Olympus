using Uranus.Staff.Models;
using UserRole = Uranus.Users.Models.Role;

namespace Styx;

// General simple validation, typically extension of the (user)role class.
public partial class Charon
{
    // DEPARTMENTS
    public bool CanCreateDepartment() => User?.Role?.CreateDepartment ?? false;

    public bool CanUpdateDepartment() => User?.Role?.UpdateDepartment ?? false;

    public bool CanDeleteDepartment() => User?.Role?.DeleteDepartment ?? false;

    // STAFF ROLES
    public bool CanCreateStaffRole() => CanCreateDepartment();
    public bool CanUpdateStaffRole() => CanUpdateDepartment();
    public bool CanDeleteStaffRole() => CanDeleteDepartment();

    public bool CanCreateClan() => User?.Role?.CreateClan ?? false;

    public bool CanUpdateClan() => User?.Role?.UpdateClan ?? false;

    public bool CanUpdateClan(Clan clan)
    {
        if (User?.Role is null) return false;
        return User.Role.UpdateClan || clan.Leader == Employee || clan.Department?.Head == Employee;
    }

    public bool CanDeleteClan() => User?.Role?.DeleteClan ?? false;

    public bool CanDeleteClan(Clan clan)
    {
        if (User?.Role is null) return false;
        return User.Role.DeleteClan || clan.Department?.Head == Employee;
    }

    public bool CanCreateShift() => (User?.Role?.CreateShift ?? -1) >= 1;

    public bool CanUpdateShift() => (User?.Role?.UpdateShift ?? -1) >= 1;

    public bool CanDeleteShift() => (User?.Role?.DeleteShift ?? -1) >= 1;

    public bool CanCreateShift(Department department)
    {
        if (User?.Role is null || Employee is null) return false;
        if (department == Employee.Department)
            return User.Role.CreateShift >= 0;
        return User.Role.CreateShift >= 1;
    }

    public bool CanUpdateShift(Department department)
    {
        if (User?.Role is null || Employee is null) return false;
        if (department == Employee.Department)
            return User.Role.UpdateShift >= 0;
        return User.Role.UpdateShift >= 1;
    }

    public bool CanDeleteShift(Department department)
    {
        if (User?.Role is null || Employee is null) return false;
        if (department == Employee.Department)
            return User.Role.DeleteShift >= 0;
        return User.Role.DeleteShift >= 1;
    }

    public bool CanCreateLicence() => User?.Role?.CreateLicence ?? false;

    public bool CanReadLicence() => User?.Role?.ReadLicence ?? false;

    public bool CanUpdateLicence() => User?.Role?.UpdateLicence ?? false;

    public bool CanDeleteLicence() => User?.Role?.DeleteLicence ?? false;

    public bool CanCreateVehicle() => User?.Role?.CreateVehicle ?? false;

    public bool CanReadVehicle() => User?.Role?.ReadVehicle ?? false;

    public bool CanUpdateVehicle() => User?.Role?.UpdateVehicle ?? false;

    public bool CanDeleteVehicle() => User?.Role?.DeleteVehicle ?? false;

    public bool CanManageLockers() => User?.Role?.ManageLockers ?? false;

    public bool CanCopyDatabase() => User?.Role?.CopyDatabase ?? false;

    public bool CanMoveDatabase() => User?.Role?.MoveDatabase ?? false;

    // USER ROLES
    public bool CanEditUserRole() => User?.Role?.EditRoles ?? false;

    public bool CanCreateUserRole() => CanEditUserRole();
    public bool CanDeleteUserRole() => CanEditUserRole();

    public bool CanAssignUserRole() => User?.Role?.AssignRole ?? false;

    public bool CanAssignUserRole(UserRole fromRole, UserRole toRole) =>
        CanAssignUserRole() && User!.Role!.IsMasterTo(fromRole) && User.Role.IsMasterTo(toRole);
}