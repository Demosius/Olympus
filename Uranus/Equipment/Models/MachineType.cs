using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Inventory;
using Uranus.Staff;

namespace Uranus.Equipment.Models;

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

    public MachineType()
    {
        Code = string.Empty;
        Description = string.Empty;
        Machines = new List<Machine>();
        Checklists = new List<Checklist>();
    }

    public MachineType(string code, string description, ELicence? licenceRequired, EAccessLevel accessLevel, List<Machine> machines, List<Checklist> checklists)
    {
        Code = code;
        Description = description;
        LicenceRequired = licenceRequired;
        AccessLevel = accessLevel;
        Machines = machines;
        Checklists = checklists;
    }
}