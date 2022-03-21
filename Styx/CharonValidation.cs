using Uranus.Staff.Model;

namespace Styx;

// General simple validation, typically extension of the (user)role class.
public partial class Charon
{
    // DEPARTMENTS
    public bool CanCreateDepartment() => CurrentUser?.Role?.CreateDepartment ?? false;

    public bool CanUpdateDepartment() => CurrentUser?.Role?.UpdateDepartment ?? false;

    public bool CanDeleteDepartment() => CurrentUser?.Role?.DeleteDepartment ?? false;

    // USER ROLES
    public bool CanCreateStaffRole() => CanCreateDepartment();
    public bool CanUpdateStaffRole() => CanUpdateDepartment();
    public bool CanDeleteStaffRole() => CanDeleteDepartment();

    public bool CanCreateClan() => CurrentUser?.Role?.CreateClan ?? false;

    public bool CanUpdateClan() => CurrentUser?.Role?.UpdateClan ?? false;

    public bool CanUpdateClan(Clan clan)
    {
        if (CurrentUser?.Role is null) return false;
        return CurrentUser.Role.UpdateClan || clan.Leader == UserEmployee || clan.Department?.Head == UserEmployee;
    }

    public bool CanDeleteClan() => CurrentUser?.Role?.DeleteClan ?? false;

    public bool CanDeleteClan(Clan clan)
    {
        if (CurrentUser?.Role is null) return false;
        return CurrentUser.Role.DeleteClan || clan.Department?.Head == UserEmployee;
    }

    public bool CanCreateShift() => (CurrentUser?.Role?.CreateShift ?? -1) >= 1;

    public bool CanUpdateShift() => (CurrentUser?.Role?.UpdateShift ?? -1) >= 1;

    public bool CanDeleteShift() => (CurrentUser?.Role?.DeleteShift ?? -1) >= 1;

    public bool CanCreateShift(Department department)
    {
        if (CurrentUser?.Role is null || UserEmployee is null) return false;
        if (department == UserEmployee.Department)
            return CurrentUser.Role.CreateShift >= 0;
        return CurrentUser.Role.CreateShift >= 1;
    }

    public bool CanUpdateShift(Department department)
    {
        if (CurrentUser?.Role is null || UserEmployee is null) return false;
        if (department == UserEmployee.Department)
            return CurrentUser.Role.UpdateShift >= 0;
        return CurrentUser.Role.UpdateShift >= 1;
    }

    public bool CanDeleteShift(Department department)
    {
        if (CurrentUser?.Role is null || UserEmployee is null) return false;
        if (department == UserEmployee.Department)
            return CurrentUser.Role.DeleteShift >= 0;
        return CurrentUser.Role.DeleteShift >= 1;
    }

    public bool CanCreateLicence() => CurrentUser?.Role?.CreateLicence ?? false;

    public bool CanReadLicence() => CurrentUser?.Role?.ReadLicence ?? false;

    public bool CanUpdateLicence() => CurrentUser?.Role?.UpdateLicence ?? false;

    public bool CanDeleteLicence() => CurrentUser?.Role?.DeleteLicence ?? false;

    public bool CanCreateVehicle() => CurrentUser?.Role?.CreateVehicle ?? false;

    public bool CanReadVehicle() => CurrentUser?.Role?.ReadVehicle ?? false;

    public bool CanUpdateVehicle() => CurrentUser?.Role?.UpdateVehicle ?? false;

    public bool CanDeleteVehicle() => CurrentUser?.Role?.DeleteVehicle ?? false;

    public bool CanCopyDatabase() => CurrentUser?.Role?.CopyDatabase ?? false;

    public bool CanMoveDatabase() => CurrentUser?.Role?.MoveDatabase ?? false;

    // USER ROLES
    public bool CanEditUserRole() => CurrentUser?.Role?.EditRoles ?? false;

    public bool CanCreateUserRole() => CanEditUserRole();
    public bool CanDeleteUserRole() => CanEditUserRole();

    public bool CanAssignUserRole() => CurrentUser?.Role?.AssignRole ?? false;

}