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
    public Checklist Checklist { get; set; }
    [ManyToOne(nameof(TypeCode), nameof(MachineType.Machines), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public MachineType Type { get; set; }

    [OneToMany(nameof(CompletedChecklist.MachineSerialNumber), nameof(CompletedChecklist.Machine), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<CompletedChecklist> CompletedChecklists { get; set; }
}