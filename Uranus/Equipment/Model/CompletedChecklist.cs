using Uranus.Staff.Model;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Equipment.Model;

public class CompletedChecklist
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(Checklist))] public string ChecklistName { get; set; }
    [ForeignKey(typeof(Machine))] public int MachineSerialNumber { get; set; }
    public int EmployeeID { get; set; }
    public DateTime CompletedTime { get; set; }

    public string Comment { get; set; }

    public bool Pass { get; set; }

    [ManyToOne(nameof(ChecklistName), nameof(Checklist.CompletedChecklists), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Checklist OriginalChecklist { get; set; }
    [ManyToOne(nameof(MachineSerialNumber), nameof(Model.Machine.CompletedChecklists), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Machine Machine { get; set; }

    [Ignore] public List<Check> Checks { get; set; }

    public CompletedChecklist() { }

    public CompletedChecklist(Checklist checklist, int machineSerialNumber, int employeeID)
    {
        OriginalChecklist = checklist;
        Checks = checklist.Checks;
        ChecklistName = checklist.Name;
        MachineSerialNumber = machineSerialNumber;
        EmployeeID = employeeID;
    }

    public CompletedChecklist(Checklist checklist, Machine machine, Employee employee)
        : this(checklist, machine.SerialNumber, employee.ID)
    {
        Machine = machine;
    }

    public int Faults()
    {
        Pass = true;
        var count = 0;
        foreach (var check in Checks.Where(check => check.IsFault() ?? false))
        {
            count++;
            if (Pass) Pass = !check.FaultFails;
        }
        return count;
    }
}