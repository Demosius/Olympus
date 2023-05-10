using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using Uranus;
using Uranus.Staff.Models;

namespace Deimos;

public class ErrorAssignmentTool
{
    public Helios Helios { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Overwrite { get; set; } // If true, check and assign errors that already have assignments.

    public List<MissPick> MissPicks { get; set; }
    public List<PickDailyStats> Stats { get; set; }
    public List<PickSession> PickSessions { get; set; }
    public List<PickEvent> PickEvents { get; set; }
    public Dictionary<DateTime, Dictionary<string, Dictionary<int, List<PickEvent>>>> PickEventDictionary { get; set; }   // Date => Container => Item
    public Dictionary<string, List<PickEvent>> CartonDictionary { get; set; }
    public Dictionary<(int, DateTime), List<PickEvent>> ItemDictionary { get; set; }

    // Track assigned and unassigned missPicks that have items and cartons. 
    // These can be compared after initial checks for up-down error matching.
    // They can also be used tp track assignment success rates.
    public List<MissPick> UnassignedMissPicks { get; set; }
    public List<MissPick> AssignedMissPicks { get; set; }

    public int AssignedCount => AssignedMissPicks.Count;
    public int AssignedUnits => AssignedMissPicks.Sum(m => Math.Abs(m.VarianceQty));
    public int UnassignedCount => UnassignedMissPicks.Count;
    public int UnassignedUnits => UnassignedMissPicks.Sum(m => Math.Abs(m.VarianceQty));

    public ErrorAssignmentTool(Helios helios, DateTime startDate, DateTime endDate, bool overwrite)
    {
        Helios = helios;
        StartDate = startDate;
        EndDate = endDate;
        Overwrite = overwrite;

        MissPicks = new List<MissPick>();
        Stats = new List<PickDailyStats>();
        PickSessions = new List<PickSession>();
        PickEvents = new List<PickEvent>();
        PickEventDictionary = new Dictionary<DateTime, Dictionary<string, Dictionary<int, List<PickEvent>>>>();
        CartonDictionary = new Dictionary<string, List<PickEvent>>();
        ItemDictionary = new Dictionary<(int, DateTime), List<PickEvent>>();
        
        UnassignedMissPicks = new List<MissPick>();
        AssignedMissPicks = new List<MissPick>();

        Task.Run(SetData);
    }

    private async Task SetData()
    {
        var missPickTask = Helios.StaffReader.RawMissPicksAsync(StartDate, EndDate);
        var statsTask = Helios.StaffReader.PickStatsAsync(StartDate, EndDate);

        Stats = (await statsTask).ToList();
        PickSessions = Stats.SelectMany(stats => stats.PickSessions).ToList();
        PickEvents = PickSessions.SelectMany(session => session.PickEvents).ToList();
        PickEventDictionary = PickEvents
            .GroupBy(e => e.Date).ToDictionary(dateGroup => dateGroup.Key, dateGroup => dateGroup
                .GroupBy(e => e.ContainerID).ToDictionary(cartonGroup => cartonGroup.Key, cartonGroup => cartonGroup
                    .GroupBy(e => e.ItemNumber).ToDictionary(itemGroup => itemGroup.Key, itemGroup => itemGroup.ToList())));
        CartonDictionary = PickEvents.GroupBy(e => e.ContainerID).ToDictionary(g => g.Key, g => g.ToList());
        ItemDictionary = PickEvents.GroupBy(e => (e.ItemNumber, e.Date)).ToDictionary(g => g.Key, g => g.ToList());
        
        MissPicks = (await missPickTask).ToList();
    }

    public async Task<bool> AssignErrors()
    {
        if (StartDate > EndDate) return false;

        foreach (var missPick in MissPicks.Where(missPick => !missPick.IsAssigned || Overwrite))
        {
            // At this point, whatever the result, the miss pick has been checked.
            missPick.Checked = true;

            // If there is no carton or no item to be found, no reliable method remains to assign fault for the error.
            missPick.NoCarton = !CartonDictionary.ContainsKey(missPick.CartonID);
            missPick.NoItem = !ItemDictionary.ContainsKey((missPick.ItemNumber, missPick.ShipmentDate));
            if (missPick.NoCarton || missPick.NoItem)
            {
                missPick.ClearAssigned();
                UnassignedMissPicks.Add(missPick);
                continue;
            }

            // Check the obvious: Single event matches date, carton, and item.
            if (CheckBasic(missPick))
            {
                AssignedMissPicks.Add(missPick);
                continue;
            }

            // Check zones for item and carton picking.
            if (CheckZoneMatching(missPick))
            {
                AssignedMissPicks.Add(missPick);
                continue;
            }

            UnassignedMissPicks.Add(missPick);
        }

        MatchUnassigned();

        return await SaveData();
    }

    private async Task<bool> SaveData()
    {
        bool success;
        try
        {
            success = await Helios.StaffUpdater.ErrorAssignmentAsync(MissPicks, PickEvents, PickSessions, Stats) > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when saving missPick assignment data.");
            MessageBox.Show($"Error occurred when saving data:\n\n{ex}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            success = false;
        }

        return success;
    }

    private bool CheckBasic(MissPick missPick)
    {
        if (!PickEventDictionary.TryGetValue(missPick.ShipmentDate, out var cartonDictionary)) return false;

        if (!cartonDictionary.TryGetValue(missPick.CartonID, out var itemDictionary)) return false;

        if (!itemDictionary.TryGetValue(missPick.ItemNumber, out var events)) return false;

        if (events.Count == 0) return false;

        events.First().AssignMissPick(missPick);

        return true;
    }

    /// <summary>
    /// Check the zone of the item for the day compared to picks of the carton.
    /// Pick events in the same zone for that carton suggest the error.
    ///
    /// Assign error to session or stats as appropriate
    /// </summary>
    /// <param name="missPick"></param>
    /// <returns>True if successfully found responsible session/stats. Otherwise, false</returns>
    private bool CheckZoneMatching(MissPick missPick)
    {
        var cartonZones = GetCartonZones(missPick.CartonID);
        var itemZones = GetItemZones(missPick.ShipmentDate, missPick.ItemNumber)?.ToList();
        if (cartonZones is null || itemZones is null) return false;

        var sharedZones = cartonZones.Intersect(itemZones).ToList();

        if (!sharedZones.Any()) return CheckItemZone(missPick, itemZones);

        if (!CartonDictionary.TryGetValue(missPick.CartonID, out var cartonEvents)) return false;

        var potentialEvents = cartonEvents.Where(e => sharedZones.Contains(e.ZoneID)).ToList();

        var dematicIDs = potentialEvents.Select(e => e.OperatorDematicID).Distinct().ToList();

        if (!dematicIDs.Any()) return false;

        if (dematicIDs.Count > 1)
        {
            missPick.MultiAssign(dematicIDs);
            return false;
        }

        var id = dematicIDs.First();

        return AssignToSession(missPick, id, potentialEvents);
    }

    /// <summary>
    /// Assuming the carton for the miss pick does not hit the same zone(s) as the item,
    /// check zones adjacent to the item zone among the zones visited by the carton.
    ///
    /// If found, assigns miss pick to the appropriate session/stats.
    /// </summary>
    /// <returns>True if error assigned.</returns>
    private bool CheckItemZone(MissPick missPick, IEnumerable<string> itemZones)
    {
        // Get carton events.
        if (!CartonDictionary.TryGetValue(missPick.CartonID, out var cartonEvents)) return false;

        var idEvents = new Dictionary<string, List<PickEvent>>();

        foreach (var zone in itemZones)
        {
            // Check if carton picking does straddle item zone.
            var earlier = cartonEvents.Where(e => string.Compare(e.ZoneID, zone, StringComparison.OrdinalIgnoreCase) < 0).ToList();
            var later = cartonEvents.Where(e => string.Compare(e.ZoneID, zone, StringComparison.OrdinalIgnoreCase) < 0).ToList();

            if (!earlier.Any() || !later.Any()) continue;

            // Get estimated time-frame that the carton would be in zone.
            var startTime = earlier.Max(e => e.DateTime);
            var endTime = later.Min(e => e.DateTime);

            // Find id of picker(s) and associated events in zone in that time-frame.}
            var zoneEvents = PickEvents.Where(e => e.ZoneID == zone && e.DateTime > startTime && e.DateTime < endTime).GroupBy(e => e.OperatorDematicID);
            foreach (var zoneEventGroup in zoneEvents)
            {
                var key = zoneEventGroup.Key;
                var list = zoneEventGroup.ToList();
                if (idEvents.ContainsKey(key))
                    idEvents[key].AddRange(list);
                else
                    idEvents[key] = list;
            }
        }

        if (idEvents.Count != 1) return false;

        var (id, potentialEvents) = idEvents.First();

        return AssignToSession(missPick, id, potentialEvents);
    }

    private void MatchUnassigned()
    {
        // Iterate through each unassigned.
        var newlyAssigned = new List<MissPick>();
        foreach (var unassignedMissPick in UnassignedMissPicks.Where(mp => !mp.NoCarton))
        {
            // Get assigned miss picks with the same ccn.
            var assigned = AssignedMissPicks.Where(m => m.CartonID == unassignedMissPick.CartonID);

            foreach (var missPick in assigned)
            {
                var uVar = unassignedMissPick.VarianceQty;
                var aVar = missPick.VarianceQty;
                // Check if one is up and the other down.
                // Don't check for absolute variance similarity, as that may hide double errors (pick wrong item AND wrong qty).
                if (!(uVar < 0 ^ aVar < 0)) continue;

                unassignedMissPick.AssignMatchingMissPick(missPick);
                newlyAssigned.Add(missPick);
                break;
            }

            if (!unassignedMissPick.IsAssigned) unassignedMissPick.NoMatch = true;
        }

        // Move newly assigned to correct list.
        foreach (var missPick in newlyAssigned)
        {
            UnassignedMissPicks.Remove(missPick);
            AssignedMissPicks.Add(missPick);
        }
    }

    private bool AssignToSession(MissPick missPick, string id, IReadOnlyCollection<PickEvent> potentialEvents)
    {
        var tech = potentialEvents
            .Select(e => e.TechType)
            .GroupBy(techType => techType)
            .OrderByDescending(grp => grp.Count())
            .Select(grp => grp.Key).First();

        var session = GetSession(id, potentialEvents.Min(e => e.DateTime), potentialEvents.Max(e => e.DateTime), tech);

        if (session is null) return AssignToStats(missPick, id, tech);

        missPick.AssignPickSession(session);
        return true;

    }

    private bool AssignToStats(MissPick missPick, string id, ETechType tech)
    {
        var stats = GetStats(id, missPick.ShipmentDate);

        if (stats is null) return false;

        missPick.AssignPickStats(stats, tech);
        return true;
    }

    private PickSession? GetSession(string dematicID, DateTime startTime, DateTime endTime, ETechType? tech = null)
    {
        var sessions = PickSessions
            .Where(s =>
                (tech is null || s.TechType == tech) &&
                s.OperatorDematicID == dematicID &&
                s.EndDateTime >= startTime &&
                s.StartDateTime <= endTime)
            .ToList();

        return sessions.Count != 1 ? null : sessions.First();
    }

    private PickDailyStats? GetStats(string dematicID, DateTime date)
    {
        return Stats.FirstOrDefault(s => s.OperatorDematicID == dematicID && s.Date == date);
    }

    private IEnumerable<string>? GetCartonZones(string cartonID)
    {
        return !CartonDictionary.TryGetValue(cartonID, out var cartonEvents) ? null : cartonEvents.Select(e => e.ZoneID).Distinct().OrderBy(s => s);
    }

    private IEnumerable<string>? GetItemZones(DateTime date, int itemNumber)
    {
        return !ItemDictionary.TryGetValue((itemNumber, date), out var cartonEvents) ? null : cartonEvents.Select(e => e.ZoneID).Distinct().OrderBy(s => s);
    }
}