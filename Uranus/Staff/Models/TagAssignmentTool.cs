using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Staff.Models;

/// <summary>
/// Too for assigning Tags to Employees based on dates and potential temp tag use.
/// </summary>
public class TagAssignmentTool
{
    public List<TempTag> TempTags { get; set; }
    public List<Employee> Employees { get; set; }
    public List<TagUse> TagHistory { get; set; }

    public Dictionary<int, Employee> EmployeeDict { get; set; }
    public Dictionary<string, Employee> EmployeeRFDict { get; set; }
    public Dictionary<string, TempTag> TagDict { get; set; }

    public TagAssignmentTool()
    {
        TempTags = new List<TempTag>();
        Employees = new List<Employee>();
        TagHistory = new List<TagUse>();
        EmployeeDict = new Dictionary<int, Employee>();
        EmployeeRFDict = new Dictionary<string, Employee>();
        TagDict = new Dictionary<string, TempTag>();
    }

    public TagAssignmentTool(List<TempTag> tempTags, List<Employee> employees, List<TagUse> tagHistory)
    {
        TempTags = tempTags;
        Employees = employees;
        TagHistory = tagHistory;

        EmployeeDict = Employees.ToDictionary(e => e.ID, e => e);
        EmployeeRFDict = Employees
            .Where(e => e.RF_ID != string.Empty)
            .GroupBy(e => e.RF_ID)
            .ToDictionary(g => g.Key, e => e.First());
        TagDict = TempTags.ToDictionary(t => t.RF_ID, t => t);

        // Assign active use tags.
        foreach (var tempTag in TempTags)
            if (tempTag.EmployeeID != 0 && EmployeeDict.TryGetValue(tempTag.EmployeeID, out var employee))
            {
                tempTag.Employee = employee;
                employee.TempTag = tempTag;
            }

        foreach (var tagUse in TagHistory)
        {
            if (TagDict.TryGetValue(tagUse.TempTagRF_ID, out var tag))
            {
                tag.TagUse.Add(tagUse);
                tagUse.TempTag = tag;
            }

            if (!EmployeeDict.TryGetValue(tagUse.EmployeeID, out var employee)) continue;

            employee.TagUse.Add(tagUse);
            tagUse.Employee = employee;
        }
    }

    public Employee? Employee(DateTime date, string rfID)
    {
        if (!TagDict.TryGetValue(rfID, out var tag))
            return EmployeeByRF(rfID);

        var tagUse = tag.TagUse.FirstOrDefault(u => u.StartDate <= date && (u.EndDate is null || u.EndDate >= date));

        return tagUse?.Employee ?? EmployeeByRF(rfID);
    }

    public Employee? EmployeeByRF(string rfID)
    {
        return !EmployeeRFDict.TryGetValue(rfID, out var employee) ? null : employee;
    }

    public void SetSessionPicker(PickSession session)
    {
        var picker = Employee(session.Date, session.OperatorRF_ID);
        
        session.Operator = picker;

        if (picker is null) return;

        session.OperatorRF_ID = picker.RF_ID;
        session.OperatorID = picker.ID;
    }

    public void SetMispickOperator(Mispick mispick)
    {
        var picker = Employee(mispick.ErrorDate, mispick.AssignedRF_ID);

        mispick.Employee = picker;
        mispick.AssignedRF_ID = picker?.RF_ID ?? mispick.AssignedRF_ID;
    }
}