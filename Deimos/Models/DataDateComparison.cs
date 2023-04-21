using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Models;

namespace Deimos.Models;

/// <summary>
/// Use to compare PickEvents to MissPicks on a given date.
/// Only holds general meta data, not the data itself.
/// </summary>
public class DataDateComparison
{
    public DateTime Date { get; set; }
    public int PickEventLines { get; set; }
    public int MissPickLines { get; set; }

    public bool HasPickEvents => PickEventLines > 0;
    public bool HasMissPicks => MissPickLines > 0;
    public bool HasBoth => HasPickEvents && HasMissPicks;

    public DataDateComparison(DateTime date, ref List<PickEvent> pickEvents, ref List<MissPick> missPicks)
    {
        Date = date;
        PickEventLines = pickEvents.RemoveAll(e => e.Date == date);
        MissPickLines = missPicks.RemoveAll(mp => mp.ShipmentDate == date);
    }

    public static IEnumerable<DataDateComparison> GetDateComparisons(List<PickEvent> pickEvents, List<MissPick> missPicks)
    {
        var dates = pickEvents.Select(e => e.Date).Concat(missPicks.Select(mp => mp.ShipmentDate)).Distinct().ToList();
        dates.Sort();

        var comps = dates.Select(date => new DataDateComparison(date, ref pickEvents, ref missPicks)).ToList();

        return comps;
    }
}