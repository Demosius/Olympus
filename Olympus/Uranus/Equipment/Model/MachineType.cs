using Olympus.Uranus.Inventory;
using Olympus.Uranus.Staff;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Equipment.Model
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
