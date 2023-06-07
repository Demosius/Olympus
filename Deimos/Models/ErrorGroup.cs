using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Models;

namespace Deimos.Models;

/// <summary>
/// Errors of a user (or unassigned) for a specific date.
/// </summary>
public class ErrorGroup
{
    public DateTime Date { get; }
    public string AssignedRF_ID { get; }
    public int ErrorCount { get; set; }
    public int ErrorSum { get; set; }
    public List<Mispick> Mispicks { get; set; }

    public Employee? Employee { get; set; }

    public ErrorGroup(DateTime date, string assignedRF_ID, List<Mispick> mispicks)
    {
        Date = date;
        AssignedRF_ID = assignedRF_ID == string.Empty ? "Unassigned" : assignedRF_ID;
        
        Mispicks = mispicks;

        ErrorCount = Mispicks.Count;
        ErrorSum = Mispicks.Sum(mp => Math.Abs(mp.VarianceQty));
    }

    public static List<ErrorGroup> GenerateErrorGroups(IEnumerable<Mispick> mispicks)
    {
        var dict = mispicks
            .GroupBy(mp => (mp.ShipmentDate, mp.AssignmentString))
            .ToDictionary(g => g.Key, g => g.ToList());

        var groups = new List<ErrorGroup>();

        foreach (var (key, value) in dict)
            if (value.Any()) groups.Add(new ErrorGroup(key.ShipmentDate, key.AssignmentString, value));
        
        return groups;
    }

    public static List<ErrorGroup> GenerateErrorGroupsPosted(IEnumerable<Mispick> mispicks)
    {
        var dict = mispicks
            .GroupBy(mp => (mp.PostedDate, mp.AssignmentString))
            .ToDictionary(g => g.Key, g => g.ToList());

        var groups = new List<ErrorGroup>();

        foreach (var (key, value) in dict)
            if (value.Any()) groups.Add(new ErrorGroup(key.PostedDate, key.AssignmentString, value));

        return groups;
    }
}