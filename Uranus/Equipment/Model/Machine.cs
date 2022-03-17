using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

namespace Uranus.Equipment.Model;

public class Machine
{
    [PrimaryKey] public int SerialNumber { get; set; }
    [ForeignKey(typeof(MachineType))] public string TypeCode { get; set; }
    public DateTime ServiceDueDate { get; set; }
    public DateTime LastServiceDate { get; set; }
    public DateTime LastPreOpCheck { get; set; }
    public string Ownership { get; set; }
    public string LicenceCode { get; set; }
    [ForeignKey(typeof(Checklist))] public string ChecklistName { get; set; }

    [ManyToOne(nameof(ChecklistName), nameof(Model.Checklist.Machines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Checklist? Checklist { get; set; }
    [ManyToOne(nameof(TypeCode), nameof(MachineType.Machines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public MachineType? Type { get; set; }

    [OneToMany(nameof(CompletedChecklist.MachineSerialNumber), nameof(CompletedChecklist.Machine), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<CompletedChecklist> CompletedChecklists { get; set; }

    public Machine()
    {
        TypeCode = string.Empty;
        ServiceDueDate = DateTime.MinValue;
        LastServiceDate = DateTime.MinValue;
        LastPreOpCheck = DateTime.MinValue;
        Ownership = string.Empty;
        LicenceCode = string.Empty;
        ChecklistName = string.Empty;
        CompletedChecklists = new List<CompletedChecklist>();
    }

    public Machine(int serialNumber, string typeCode, DateTime serviceDueDate, DateTime lastServiceDate, DateTime lastPreOpCheck, string ownership, string licenceCode, string checklistName, Checklist checklist, MachineType type, List<CompletedChecklist> completedChecklists)
    {
        SerialNumber = serialNumber;
        TypeCode = typeCode;
        ServiceDueDate = serviceDueDate;
        LastServiceDate = lastServiceDate;
        LastPreOpCheck = lastPreOpCheck;
        Ownership = ownership;
        LicenceCode = licenceCode;
        ChecklistName = checklistName;
        Checklist = checklist;
        Type = type;
        CompletedChecklists = completedChecklists;
    }

    public bool HighReach() => Type?.AccessLevel == Inventory.EAccessLevel.HighReach;
}