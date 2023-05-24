using System;
using System.Collections.Generic;
using System.Linq;
using Uranus.Staff.Models;

namespace Deimos.Models;

/// <summary>
/// Use to compare PickEvents to Mispicks on a given date.
/// Only holds general meta data, not the data itself.
/// </summary>
public class DataDateComparison
{
    public DateTime Date { get; set; }
    public int PickEventLines { get; set; }
    public int MispickLines { get; set; }

    public bool HasPickEvents => PickEventLines > 0;
    public bool HasMispicks => MispickLines > 0;
    public bool HasBoth => HasPickEvents && HasMispicks;

    public DataDateComparison(DateTime date, ref List<PickEvent> pickEvents, ref List<Mispick> mispicks)
    {
        Date = date;
        PickEventLines = pickEvents.RemoveAll(e => e.Date == date);
        MispickLines = mispicks.RemoveAll(mp => mp.ShipmentDate == date);
    }

    public DataDateComparison(DateTime date, int pickEventLines, int mispickLines)
    {
        Date = date;
        PickEventLines = pickEventLines;
        MispickLines = mispickLines;
    }

    public DataDateComparison(DateTime date, Dictionary<DateTime, int> pickEventCount, ref List<Mispick> mispicks)
    {
        Date =date;
        if (!pickEventCount.TryGetValue(Date, out var peCount)) peCount = 0;
        PickEventLines = peCount;
        MispickLines = mispicks.RemoveAll(mp => mp.ShipmentDate == date);
    }

    public static IEnumerable<DataDateComparison> GetDateComparisons(List<PickEvent> pickEvents, List<Mispick> mispicks)
    {
        var dates = pickEvents.Select(e => e.Date).Concat(mispicks.Select(mp => mp.ShipmentDate)).Distinct().ToList();
        dates.Sort();

        var comps = dates.Select(date => new DataDateComparison(date, ref pickEvents, ref mispicks)).ToList();

        return comps;
    }

    public static IEnumerable<DataDateComparison> GetDateComparisons(Dictionary<DateTime, int> pickEventCount, List<Mispick> mispicks)
    {
        var dates = pickEventCount.Keys.Concat(mispicks.Select(mp => mp.ShipmentDate)).Distinct().ToList();
        dates.Sort();

        var comps = dates.Select(date => new DataDateComparison(date, pickEventCount, ref mispicks)).ToList();

        return comps;
    }

    public static IEnumerable<DataDateComparison> GetDateComparisons(Dictionary<DateTime, int> pickEventCount, Dictionary<DateTime, int> mispickCount)
    {
        var dates = pickEventCount.Keys
            .Concat(mispickCount.Keys)
            .Distinct()
            .ToList();

        dates.Sort();

        var comps = new List<DataDateComparison>();

        foreach (var date in dates)
        {
            if (!pickEventCount.TryGetValue(date, out var pickLines)) pickLines = 0;
            if (!mispickCount.TryGetValue(date, out var mispickLines)) mispickLines = 0;
            comps.Add(new DataDateComparison(date, pickLines, mispickLines));
        }

        return comps;
    }
}