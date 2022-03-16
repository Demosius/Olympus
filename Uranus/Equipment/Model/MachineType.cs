using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using Uranus.Inventory;
using Uranus.Staff;

namespace Uranus.Equipment.Model;

public class MachineType
{
    [PrimaryKey] public string Code { get; set; }
    public string Description { get; set; }
    public ELicence? LicenceRequired { get; set; }
    public EAccessLevel AccessLevel { get; set; }

    [OneToMany(nameof(Machine.TypeCode), nameof(Machine.Type), CascadeOperations = CascadeOperation.All)]
    public List<Machine> Machines { get; set; }
    [OneToMany(nameof(Checklist.TypeCode), nameof(Checklist.MachineType), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Checklist> Checklists { get; set; }
}