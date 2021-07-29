using Olympus.Uranus.Staff.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Styx.Model
{
    // Genereal simple validiont, typically extension of the (user)role class.
    public partial class Charon
    {
        public bool CanCreateDepartment() => CurrentUser.Role.CreateDepartment;
        public bool CanUpdateDepartment() => CurrentUser.Role.UpdateDepartment;
        public bool CanDeleteDepartment() => CurrentUser.Role.DeleteDepartment;

        public bool CanAssignRole() => CurrentUser.Role.AssignRole;
        public bool CanEditRoles() => CurrentUser.Role.EditRoles;

        public bool CanCreateClan() => CurrentUser.Role.CreateClan;
        public bool CanUpdateClan() => CurrentUser.Role.UpdateClan;
        public bool CanUpdateClan(Clan clan) => CurrentUser.Role.UpdateClan || clan.Leader == UserEmployee || clan.Department.Head == UserEmployee;
        public bool CanDeleteClan() => CurrentUser.Role.DeleteClan;
        public bool CanDeleteClan(Clan clan) => CurrentUser.Role.DeleteClan || clan.Department.Head == UserEmployee;

        public bool CanCreateShift() => CurrentUser.Role.CreateShift >= 1;
        public bool CanUpdateShift() => CurrentUser.Role.UpdateShift >= 1;
        public bool CanDeleteShift() => CurrentUser.Role.DeleteShift >= 1;

        public bool CanCreateShift(Department department)
        {
            if (department == UserEmployee.Department)
                return CurrentUser.Role.CreateShift >= 0;
            return CurrentUser.Role.CreateShift >= 1;
        }
        public bool CanUpdateShift(Department department)
        {
            if (department == UserEmployee.Department)
                return CurrentUser.Role.UpdateShift >= 0;
            return CurrentUser.Role.UpdateShift >= 1;
        }
        public bool CanDeleteShift(Department department)
        {
            if (department == UserEmployee.Department)
                return CurrentUser.Role.DeleteShift >= 0;
            return CurrentUser.Role.DeleteShift >= 1;
        }

        public bool CanCreateLicence() => CurrentUser.Role.CreateLicence;
        public bool CanReadLicence() => CurrentUser.Role.ReadLicence;
        public bool CanUpdateLicence() => CurrentUser.Role.UpdateLicence;
        public bool CanDeleteLicence() => CurrentUser.Role.DeleteLicence;

        public bool CanCreateVehicle() => CurrentUser.Role.CreateVehicle;
        public bool CanReadVehicle() => CurrentUser.Role.ReadVehicle;
        public bool CanUpdateVehicle() => CurrentUser.Role.UpdateVehicle;
        public bool CanDeleteVehicle() => CurrentUser.Role.DeleteVehicle;

    }
}
