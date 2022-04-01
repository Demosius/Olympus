using System;
using System.Collections.Generic;
using System.Data;

namespace Uranus.Staff.Model;

public class RosterDataSet
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Department Department { get; set; }
    public Shift? DefaultShift { get; set; }

    public Dictionary<int, Employee> Employees { get; set; }
    public Dictionary<string, Shift> Shifts { get; set; }
    public Dictionary<(int, DateTime), Roster> Rosters { get; set; }
    public IEnumerable<EmployeeShift> EmpShiftConnections { get; set; }

    public DataTable ViewTable { get; set; }

    public void SetDataTable()
    {
        // TODO: Set Roster Data Table
        // Use Name, then dates as data headers.
        // Enter employees in Name column, then the relevant rosters at each cross point.
    }
}