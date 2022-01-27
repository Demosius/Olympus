using Uranus.Inventory;
using Uranus.Staff;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Equipment.Model
{
    public class MachineType
    {
        [PrimaryKey]
        public string Code { get; set; }
        public string Description { get; set; }
        public ELicence? LicenceRequired { get; set; }
        public EAccessLevel AccessLevel { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Machine> Machines { get; set; }
    }
}
