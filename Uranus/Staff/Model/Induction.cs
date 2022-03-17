using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Staff.Model;

public class Induction
{
    [PrimaryKey] public string Type { get; set; }
    public string Description { get; set; }
    public string Period { get; set; }

    [OneToMany(nameof(EmployeeInductionReference.InductionType), nameof(EmployeeInductionReference.Induction), CascadeOperations = CascadeOperation.All)]
    public List<EmployeeInductionReference> EmployeeReferences { get; set; }

    public Induction()
    {
        Type = string.Empty;
        Description = string.Empty;
        Period = string.Empty;
        EmployeeReferences = new List<EmployeeInductionReference>();
    }

    public Induction(string type, string description, string period, List<EmployeeInductionReference> employeeReferences)
    {
        Type = type;
        Description = description;
        Period = period;
        EmployeeReferences = employeeReferences;
    }
}