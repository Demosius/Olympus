using Uranus.Staff.Model;

namespace Styx;

// General simple validation, typically extension of the (user)role class.
public partial class Charon
{
    // DEPARTMENTS
    public bool CanCreateDepartment() {
        return CurrentUser is not null && CurrentUser.Role.CreateDepartment;
    }
    public bool CanUpdateDepartment() {
        return CurrentUser is not null && CurrentUser.Role.UpdateDepartment;
    }
    public bool CanDeleteDepartment() {
        return CurrentUser is not null && CurrentUser.Role.DeleteDepartment;
    }

    // USER ROLES
    public bool CanCreateStaffRole() => CanCreateDepartment();
    public bool CanUpdateStaffRole() => CanUpdateDepartment();
    public bool CanDeleteStaffRole() => CanDeleteDepartment();

    public bool CanCreateClan() {
        return CurrentUser is not null && CurrentUser.Role.CreateClan;
    }
    public bool CanUpdateClan() {
        return CurrentUser is not null && CurrentUser.Role.UpdateClan;
    }
    public bool CanUpdateClan(Clan clan)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role.UpdateClan || clan.Leader == UserEmployee || clan.Department.Head == UserEmployee;
    }
    public bool CanDeleteClan() {
        return CurrentUser is not null && CurrentUser.Role.DeleteClan;
    }
    public bool CanDeleteClan(Clan clan)
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role.DeleteClan || clan.Department.Head == UserEmployee;
    }

    public bool CanCreateShift()
    {
        if (CurrentUser is null) return false;
        return CurrentUser.Role.CreateShift >= 1;
    }
    public bool CanUpdateShift() {
        if (CurrentUser is null) return false;
        return CurrentUser.Role.UpdateShift >= 1;
    }
    public bool CanDeleteShift() {
        if (CurrentUser is null) return false;
        return CurrentUser.Role.DeleteShift >= 1;
    }

    public bool CanCreateShift(Department department)
    {
        if (CurrentUser is null) return false;
        if (department == UserEmployee.Department)
            return CurrentUser.Role.CreateShift >= 0;
        return CurrentUser.Role.CreateShift >= 1;
    }
    public bool CanUpdateShift(Department department)
    {
        if (CurrentUser is null) return false;
        if (department == UserEmployee.Department)
            return CurrentUser.Role.UpdateShift >= 0;
        return CurrentUser.Role.UpdateShift >= 1;
    }
    public bool CanDeleteShift(Department department)
    {
        if (CurrentUser is null) return false;
        if (department == UserEmployee.Department)
            return CurrentUser.Role.DeleteShift >= 0;
        return CurrentUser.Role.DeleteShift >= 1;
    }

    public bool CanCreateLicence() {
        return CurrentUser is not null && CurrentUser.Role.CreateLicence;
    }
    public bool CanReadLicence() {
        return CurrentUser is not null && CurrentUser.Role.ReadLicence;
    }
    public bool CanUpdateLicence() {
        return CurrentUser is not null && CurrentUser.Role.UpdateLicence;
    }
    public bool CanDeleteLicence() {
        return CurrentUser is not null && CurrentUser.Role.DeleteLicence;
    }

    public bool CanCreateVehicle() {
        return CurrentUser is not null && CurrentUser.Role.CreateVehicle;
    }
    public bool CanReadVehicle() {
        return CurrentUser is not null && CurrentUser.Role.ReadVehicle;
    }
    public bool CanUpdateVehicle() {
        return CurrentUser is not null && CurrentUser.Role.UpdateVehicle;
    }
    public bool CanDeleteVehicle() {
        return CurrentUser is not null && CurrentUser.Role.DeleteVehicle;
    }

    public bool CanCopyDatabase() {
        return CurrentUser is not null && CurrentUser.Role.CopyDatabase;
    }

    public bool CanMoveDatabase() {
        return CurrentUser is not null && CurrentUser.Role.MoveDatabase;
    }

    // USER ROLES
    public bool CanEditUserRole()
    {
        return CurrentUser is not null && CurrentUser.Role.EditRoles;
    }

    public bool CanCreateUserRole() => CanEditUserRole();
    public bool CanDeleteUserRole() => CanEditUserRole();

    public bool CanAssignUserRole()
    {
        return CurrentUser is not null && CurrentUser.Role.AssignRole;
    }

}