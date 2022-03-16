using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Dynamic;

namespace Uranus.Equipment.Model;

public class Checklist
{
    [PrimaryKey] public string Name { get; set; }
    [ForeignKey(typeof(MachineType))] public string TypeCode { get; set; }
    public string CheckCode { get; set; }

    [OneToMany(nameof(Machine.ChecklistName), nameof(Machine.Checklist), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Machine> Machines { get; set; }
    [OneToMany(nameof(CompletedChecklist.ChecklistName), nameof(CompletedChecklist.OriginalChecklist), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<CompletedChecklist> CompletedChecklists { get; set; }

    [ManyToOne(nameof(TypeCode), nameof(Model.MachineType.Checklists),CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public MachineType MachineType { get; set; }

    [Ignore] public List<Check> Checks { get; set; }

    public Checklist() { }

    public Checklist(string name, string typeCode, List<Check> checks)
    {
        Name = name;
        TypeCode = typeCode;
        Checks = checks;
    }
}

public class Check
{
    public string Description { get; set; }
    public bool DesiredResponse { get; set; }
    public bool? Response { get; set; }
    public bool FaultFails { get; set; }

    public bool? IsFault()
    {
        return Response is null ? Response : Response != DesiredResponse;
    }

    public string FaultString()
    {
        return $"{Description}: {Response}";
    }
}