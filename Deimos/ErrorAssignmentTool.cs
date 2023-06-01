using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Morpheus.ViewModels.Controls;
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

    public List<Mispick> Mispicks { get; set; }
    public List<PickDailyStats> Stats { get; set; }
    public List<PickSession> PickSessions { get; set; }
    public List<PickEvent> PickEvents { get; set; }
    public Dictionary<string, Dictionary<int, List<PickEvent>>> PickEventDictionary { get; set; }   // Container => Item
    public Dictionary<string, List<PickEvent>> CartonDictionary { get; set; }
    public Dictionary<(int, DateTime), List<PickEvent>> ItemDictionary { get; set; }
    public Dictionary<string, List<PickEvent>> ClusterDictionary { get; set; }

    // Track assigned and unassigned mispicks that have items and cartons. 
    // These can be compared after initial checks for up-down error matching.
    // They can also be used tp track assignment success rates.
    public List<Mispick> UnassignedMispicks { get; set; }
    public List<Mispick> AssignedMispicks { get; set; }

    public int AssignedCount { get; set; }
    public int AssignedUnits { get; set; }
    public int UnassignedCount { get; set; }
    public int UnassignedUnits { get; set; }

    public ErrorAssignmentTool(Helios helios, DateTime startDate, DateTime endDate, bool overwrite)
    {
        Helios = helios;
        StartDate = startDate;
        EndDate = endDate;
        Overwrite = overwrite;

        Mispicks = new List<Mispick>();
        Stats = new List<PickDailyStats>();
        PickSessions = new List<PickSession>();
        PickEvents = new List<PickEvent>();
        PickEventDictionary = new Dictionary<string, Dictionary<int, List<PickEvent>>>();
        CartonDictionary = new Dictionary<string, List<PickEvent>>();
        ItemDictionary = new Dictionary<(int, DateTime), List<PickEvent>>();
        ClusterDictionary = new Dictionary<string, List<PickEvent>>();

        UnassignedMispicks = new List<Mispick>();
        AssignedMispicks = new List<Mispick>();
    }

    private async Task GetData(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate) return;

        var dataTask = Helios.StaffReader.ErrorAssignmentDataAsync(fromDate, toDate);

        (Stats, Mispicks) = await dataTask;

        PickSessions = Stats.SelectMany(stats => stats.PickSessions).ToList();
        PickEvents = PickSessions.SelectMany(session => session.PickEvents).ToList();
        PickEventDictionary = PickEvents.GroupBy(e => e.ContainerID).ToDictionary(cartonGroup => cartonGroup.Key, cartonGroup => cartonGroup
                    .GroupBy(e => e.ItemNumber).ToDictionary(itemGroup => itemGroup.Key, itemGroup => itemGroup.ToList()));
        CartonDictionary = PickEvents.GroupBy(e => e.ContainerID).ToDictionary(g => g.Key, g => g.ToList());
        ItemDictionary = PickEvents.GroupBy(e => (e.ItemNumber, e.Date)).ToDictionary(g => g.Key, g => g.ToList());
        ClusterDictionary = PickEvents.GroupBy(e => e.ClusterReference).ToDictionary(g => g.Key, g => g.ToList());

        UnassignedMispicks = new List<Mispick>();
        AssignedMispicks = new List<Mispick>();
    }

    public async Task<bool> AssignErrorsAsync(DateTime fromDate, DateTime toDate, IProgress<ProgressTaskVM>? progress)
    {
        if (fromDate > toDate) return false;

        progress?.Report(new ProgressTaskVM("Gathering data...", $"{fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy}", 0, 0, 0));
        await GetData(fromDate, toDate);

        var mispicks = Mispicks.Where(mispick => !mispick.IsAssigned || Overwrite).ToList();

        var total = mispicks.Count;
        var count = 0;

        foreach (var mispick in mispicks)
        {
            progress?.Report(new ProgressTaskVM($"Assigning mispicks ({fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy})...", $"{count} / {total}", 0, total, count));
            
            // At this point, whatever the result, the mispick has been checked.
            mispick.Checked = true;
            count++;

            mispick.ClearMultiAssignComments();
            
            // If there is no carton or no item to be found, no reliable method remains to assign fault for the error.
            mispick.NoCarton = !CartonDictionary.ContainsKey(mispick.CartonID);
            mispick.NoItem = !ItemDictionary.ContainsKey((mispick.ItemNumber, mispick.ShipmentDate));
            if (mispick.NoCarton || mispick.NoItem)
            {
                mispick.ClearAssigned();
                UnassignedMispicks.Add(mispick);
                continue;
            }

            // Check the obvious: Single event matches date, carton, and item.
            if (CheckBasic(mispick))
            {
                AssignedMispicks.Add(mispick);
                continue;
            }

            // Check cluster for potential errors created between cartons.
            if (CheckCluster(mispick))
            {
                AssignedMispicks.Add(mispick);
                continue;
            }

            // Check zones for item and carton picking.
            if (CheckZoneMatching(mispick))
            {
                AssignedMispicks.Add(mispick);
                continue;
            }

            UnassignedMispicks.Add(mispick);
        }

        MatchUnassigned(progress);

        AssignedCount += AssignedMispicks.Count;
        AssignedUnits += AssignedMispicks.Sum(m => Math.Abs(m.VarianceQty));
        UnassignedCount += UnassignedMispicks.Count;
        UnassignedUnits += UnassignedMispicks.Sum(m => Math.Abs(m.VarianceQty));

        progress?.Report(new ProgressTaskVM("Saving data...", $"{fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy}", 0, 0, 0));
        return await SaveData();
    }

    public async Task<bool> AssignErrorsAsync(IProgress<ProgressTaskVM>? progress)
    {
        if (StartDate > EndDate) return false;

        AssignedCount = 0;
        AssignedUnits = 0;
        UnassignedCount = 0;
        UnassignedUnits = 0;

        var anySuccess = false;

        var diff = (EndDate - StartDate).Days;
        if (diff < 1) diff = 1;
        var days = 28;  // Denotes number of dates of mispicks to look up. 7 more days of Pick events will be included.
        var intervals = (diff + days - 1) / days;
        days = (diff + intervals - 1) / intervals;

        var toDate = StartDate.AddDays(-1);
        for (var i = 0; i < intervals; i++)
        {
            var fromDate = toDate.AddDays(1);
            toDate = i == intervals - 1 ? EndDate : fromDate.AddDays(days);

            progress?.Report(new ProgressTaskVM("Error assigning in chunks...", $"{fromDate:dd/MM/yyyy} to {toDate:dd/MM/yyyy}", 0, intervals, i));

            anySuccess |= await AssignErrorsAsync(fromDate, toDate, progress);
        }

        return anySuccess;
    }

    private async Task<bool> SaveData()
    {
        bool success;
        try
        {
            success = await Helios.StaffUpdater.ErrorAssignmentAsync(Mispicks, PickEvents, PickSessions, Stats) > 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when saving mispick assignment data.");
            MessageBox.Show($"Error occurred when saving data:\n\n{ex}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            success = false;
        }

        return success;
    }

    private bool CheckBasic(Mispick mispick)
    {
        if (!PickEventDictionary.TryGetValue(mispick.CartonID, out var cartonDictionary)) return false;

        if (!cartonDictionary.TryGetValue(mispick.ItemNumber, out var itemEvents)) return false;

        if (itemEvents.Count == 0) return false;

        itemEvents.First().AssignMispick(mispick);

        return true;
    }

    /// <summary>
    /// Check the zone of the item for the day compared to picks of the carton.
    /// Pick events in the same zone for that carton suggest the error.
    ///
    /// Assign error to session or stats as appropriate
    /// </summary>
    /// <param name="mispick"></param>
    /// <returns>True if successfully found responsible session/stats. Otherwise, false</returns>
    private bool CheckZoneMatching(Mispick mispick)
    {
        // Get list of zones relevant to both the carton and the item.
        var cartonZones = GetCartonZones(mispick.CartonID);
        var itemZones = GetItemZones(mispick.ShipmentDate, mispick.ItemNumber)?.ToList();
        if (cartonZones is null || itemZones is null) return false;

        // Find intersecting zones.
        var sharedZones = cartonZones.Intersect(itemZones).ToList();

        if (!sharedZones.Any()) return CheckItemZone(mispick, itemZones);

        // Figure out potential event culprits based on the carton and zone.
        if (!CartonDictionary.TryGetValue(mispick.CartonID, out var cartonEvents)) return false;
        var potentialEvents = cartonEvents.Where(e => sharedZones.Contains(e.ZoneID)).ToList();

        var dematicIDs = potentialEvents.Select(e => e.OperatorDematicID).Distinct().ToList();

        if (!dematicIDs.Any()) return false;

        if (dematicIDs.Count > 1)
        {
            mispick.MultiAssign(dematicIDs);
            return false;
        }

        var id = dematicIDs.First();

        return AssignToSession(mispick, id, potentialEvents);
    }

    private bool CheckCluster(Mispick mispick)
    {
        // Gather pick events of carton.
        if (!CartonDictionary.TryGetValue(mispick.CartonID, out var cartonEvents)) return false;

        // Check if any clusters are applicable to this carton.
        var clusters = cartonEvents.Where(e => e.ClusterReference != string.Empty).Select(e => e.ClusterReference).Distinct().ToList();
        if (!clusters.Any() || clusters.Count == 1 && clusters.First() == string.Empty) return false;

        // Gather pick events of clusters.
        var clusterEvents = new List<PickEvent>();
        foreach (var cluster in clusters)
            if (ClusterDictionary.TryGetValue(cluster, out var events))
                clusterEvents.AddRange(events);

        // Check if item was picked in one (or more) of these clusters.
        clusterEvents = clusterEvents.Where(e => e.ItemNumber == mispick.ItemNumber).ToList();
        if (!clusterEvents.Any()) return false;

        // If there is only one eligible event:
        if (clusterEvents.Count == 1)
        {
            mispick.AssignPickEvent(clusterEvents.First());
            return true;
        }

        // Multi/Assign as appropriate.
        var dematicIDs = clusterEvents.Select(e => e.OperatorDematicID).Distinct().ToList();

        if (!dematicIDs.Any()) return false;

        if (dematicIDs.Count > 1)
        {
            mispick.MultiAssign(dematicIDs);
            return false;
        }

        var id = dematicIDs.First();

        return AssignToSession(mispick, id, clusterEvents);
    }

    /// <summary>
    /// Assuming the carton for the mispick does not hit the same zone(s) as the item,
    /// check zones adjacent to the item zone among the zones visited by the carton.
    ///
    /// If found, assigns mispick to the appropriate session/stats.
    /// </summary>
    /// <returns>True if error assigned.</returns>
    private bool CheckItemZone(Mispick mispick, IEnumerable<string> itemZones)
    {
        // Get carton events.
        if (!CartonDictionary.TryGetValue(mispick.CartonID, out var cartonEvents)) return false;

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

        return AssignToSession(mispick, id, potentialEvents);
    }

    private void MatchUnassigned(IProgress<ProgressTaskVM>? progress)
    {
        // Iterate through each unassigned.
        var newlyAssigned = new List<Mispick>();

        var ump = UnassignedMispicks.Where(mp => !mp.NoCarton).ToList();
        var total = ump.Count;
        var count = 0;

        foreach (var unassignedMispick in ump)
        {
            progress?.Report(new ProgressTaskVM("Assigning unmatched..", $"{count} / {total}", 0, total, count));
            count++;

            // Get assigned mispicks with the same ccn.
            var assigned = AssignedMispicks.Where(m => m.CartonID == unassignedMispick.CartonID);

            foreach (var mispick in assigned)
            {
                var uVar = unassignedMispick.VarianceQty;
                var aVar = mispick.VarianceQty;
                // Check if one is up and the other down.
                // Don't check for absolute variance similarity, as that may hide double errors (pick wrong item AND wrong qty).
                if (!(uVar < 0 ^ aVar < 0)) continue;

                unassignedMispick.AssignMatchingMispick(mispick);
                newlyAssigned.Add(mispick);
                break;
            }

            if (!unassignedMispick.IsAssigned) unassignedMispick.NoMatch = true;
        }

        // Move newly assigned to correct list.
        foreach (var mispick in newlyAssigned)
        {
            UnassignedMispicks.Remove(mispick);
            AssignedMispicks.Add(mispick);
        }
    }

    private bool AssignToSession(Mispick mispick, string id, IReadOnlyCollection<PickEvent> potentialEvents)
    {
        var tech = potentialEvents
            .Select(e => e.TechType)
            .GroupBy(techType => techType)
            .OrderByDescending(grp => grp.Count())
            .Select(grp => grp.Key).First();

        var session = GetSession(id, potentialEvents.Min(e => e.DateTime), potentialEvents.Max(e => e.DateTime), tech);

        if (session is null) return AssignToStats(mispick, id, tech);

        mispick.AssignPickSession(session);
        return true;

    }

    private bool AssignToStats(Mispick mispick, string id, ETechType tech)
    {
        var stats = GetStats(id, mispick.ShipmentDate);

        if (stats is null) return false;

        mispick.AssignPickStats(stats, tech);
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