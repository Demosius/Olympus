using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;
using Uranus.Inventory.Models;

namespace Uranus.Staff.Models;

public enum ETechType
{
    PTL,
    RFT
}

public class PickEvent : IEquatable<PickEvent>
{
    [PrimaryKey] public string ID { get; set; } // Example: 6118.2022.11.23.23.09.03 ([OperatorID].[Year].[Month].[Day].[Hour].[Minute].[Second])
    public string TimeStamp { get; set; }
    public DateTime DateTime { get; set; }
    [Indexed] public DateTime Date { get; set; }
    [StringLength(4)] public string OperatorDematicID { get; set; }
    public string OperatorRF_ID { get; set; }
    public int Qty { get; set; }
    public string ContainerID { get; set; }
    public string TechString { get; set; }
    public ETechType TechType { get; set; }
    public string ZoneID { get; set; }
    public string WaveID { get; set; }
    public string WorkAssignment { get; set; }
    public string StoreNumber { get; set; }
    public string DeviceID { get; set; }
    public int ItemNumber { get; set; }
    public string ItemDescription { get; set; }
    public string ClusterReference { get; set; }

    public string MispickID { get; set; }

    [ForeignKey(typeof(PickSession))] public string SessionID { get; set; }
    [ForeignKey(typeof(PickDailyStats))]
    public string StatsID { get; set; }

    // Must be set after original creation.
    [ForeignKey(typeof(Employee))] public int OperatorID { get; set; }

    [Ignore] public TimeSpan Time => DateTime.TimeOfDay;

    [ManyToOne(nameof(OperatorID), nameof(Employee.PickEvents), CascadeOperations = CascadeOperation.CascadeRead)]
    public Employee? Operator { get; set; }
    [ManyToOne(nameof(SessionID), nameof(PickSession.PickEvents), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickSession? Session { get; set; }
    [ManyToOne(nameof(StatsID), nameof(PickDailyStats.PickEvents), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickDailyStats? PickStats { get; set; }

    [OneToOne(nameof(MispickID), nameof(Models.Mispick.PickEvent), CascadeOperations = CascadeOperation.CascadeRead)]
    public Mispick? Mispick { get; set; }

    // Cannot be tied to items directly, as they belong to a separate database.
    [Ignore] public NAVItem? Item { get; set; }

    public PickEvent()
    {
        ID = Guid.NewGuid().ToString(); // This should be immediately overwritten, but automatically should be unique so as not to cause potential issues.
        TimeStamp = string.Empty;
        OperatorDematicID = "0000";
        OperatorRF_ID = string.Empty;
        ContainerID = string.Empty;
        TechString = string.Empty;
        TechType = ETechType.PTL;
        ZoneID = string.Empty;
        WaveID = string.Empty;
        WorkAssignment = string.Empty;
        StoreNumber = string.Empty;
        DeviceID = string.Empty;
        ItemDescription = string.Empty;
        ClusterReference = string.Empty;

        SessionID = string.Empty;
        StatsID = string.Empty;

        MispickID = string.Empty;
    }

    public static string GetEventID(string dematicID, DateTime dateTime) => $"{dematicID}.{dateTime:yyyy.MM.dd.hh.mm.ss}";

    private void ResetID()
    {
        ID = GetEventID(OperatorDematicID, DateTime);
    }

    public static ETechType GetTechType(string techString)
    {
        _ = Enum.TryParse<ETechType>(techString, true, out var tt);
        return tt;
    }

    public void AssignMispick(Mispick mispick)
    {
        MispickID = mispick.ID;
        Mispick = mispick;

        Mispick.PickEventID = ID;
        Mispick.PickEvent = this;

        Mispick.AssignedRF_ID = OperatorRF_ID;
        Mispick.AssignedDematicID = OperatorDematicID;

        Session?.AssignMispick(mispick);
    }

    /// <summary>
    /// Checks given list for potential duplicates, and adjusts timestamps to shift values apart.
    /// </summary>
    /// <param name="sampleEvents"></param>
    /// <returns>Number of values that had to be changed.</returns>
    public static int HandleDuplicateValues(ref List<PickEvent> sampleEvents)
    {
        // Set pure ID.
        foreach (var pickEvent in sampleEvents)
            pickEvent.ResetID();

        // Remove genuine duplicates.
        sampleEvents = sampleEvents.DistinctBy(e => $"{e.ID}{e.ItemNumber}{e.Qty}").ToList();

        var dict = sampleEvents
            .GroupBy(e => e.ID)
            .Where(g => g.Count() > 1)
            .ToDictionary(g => g.Key, g => g.ToList());

        var count = 0;

        foreach (var (id, events) in dict)
        {
            for (var i = 0; i < events.Count; i++)
            {
                events[i].ID = $"{id}[{i}]";
                count++;
            }
        }

        return count;
    }

    public static List<PickEvent> GenerateFromRawData(string raw, out List<PickSession> sessions, out List<PickDailyStats> stats, TimeSpan? ptlBreak = null, TimeSpan? rftBreak = null)
    {
        var events = DataConversion.RawStringToPickEvents(raw);

        sessions = GenerateStatisticsFromEvents(ref events, out stats, ptlBreak, rftBreak);

        return events;
    }

    public static List<PickSession> GenerateStatisticsFromEvents(ref List<PickEvent> events, out List<PickDailyStats> stats,
        TimeSpan? ptlBreak = null, TimeSpan? rftBreak = null)
    {
        HandleDuplicateValues(ref events);

        var sessions = PickSession.GeneratePickSessions(events, out stats, ptlBreak, rftBreak);

        return sessions;
    }

    /* Async Methods */

    public static async Task<(List<PickEvent> events, List<PickSession> sessions, List<PickDailyStats> stats)> GenerateStatisticsFromEventsAsync(List<PickEvent> events, TimeSpan? ptlBreak = null, TimeSpan? rftBreak = null)
    {
        var sessions = new List<PickSession>();
        var stats = new List<PickDailyStats>();

        await Task.Run(() =>
        {
            HandleDuplicateValues(ref events);
            sessions = PickSession.GeneratePickSessions(events, out stats, ptlBreak, rftBreak);
        });

        return (events, sessions, stats);
    }

    /* Equality Members */

    public bool Equals(PickEvent? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((PickEvent)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return ID.GetHashCode();
    }
}