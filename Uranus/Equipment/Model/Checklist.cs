using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

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

    [ManyToOne(nameof(TypeCode), nameof(Model.MachineType.Checklists), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public MachineType? MachineType { get; set; }

    [Ignore] public List<Check> Checks { get; set; }

    public Checklist()
    {
        Name = string.Empty;
        TypeCode = string.Empty;
        CheckCode = string.Empty;
        Checks = new List<Check>();
        Machines = new List<Machine>();
        CompletedChecklists = new List<CompletedChecklist>();
    }

    public Checklist(string name, string typeCode, List<Check> checks)
    {
        Name = name;
        TypeCode = typeCode;
        Checks = checks;
        CheckCode = string.Empty;
        Machines = new List<Machine>();
        CompletedChecklists = new List<CompletedChecklist>();
    }
}

public class Check
{
    public string Description { get; set; }
    public bool DesiredResponse { get; set; }
    public bool? Response { get; set; }
    public bool FaultFails { get; set; }

    public Check()
    {
        Description = string.Empty;
    }

    public Check(string description, bool desiredResponse, bool? response, bool faultFails)
    {
        Description = description;
        DesiredResponse = desiredResponse;
        Response = response;
        FaultFails = faultFails;
    }

    public bool? IsFault()
    {
        return Response is null ? Response : Response != DesiredResponse;
    }

    public string FaultString()
    {
        return $"{Description}: {Response}";
    }
}