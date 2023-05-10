using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

/// <summary>
/// A string of pick events that run between breaks, assumed to be the result of constant picking by the operator/Employee.
/// </summary>
public class PickSession
{
    public static TimeSpan PTLBreakSpan => new(0, 2, 0);
    public static TimeSpan RFTBreakSpan => new(0, 5, 0);

    [PrimaryKey] public string ID { get; set; } // Example: 6118.2022.11.23.23.09.03 ([OperatorID].[Year].[Month].[Day].[Hour].[Minute].[Second] - Where time based on initial pick even in session.) 
    public string StartTimeStamp { get; set; }
    public string EndTimeStamp { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    [Indexed] public DateTime Date { get; set; }
    [StringLength(4)] public string OperatorDematicID { get; set; }
    public string OperatorRF_ID { get; set; }
    public int Qty { get; set; }
    public int EventCount { get; set; }
    public string TechString { get; set; }
    public ETechType TechType { get; set; }

    [ForeignKey(typeof(PickDailyStats))]
    public string StatsID { get; set; }

    [Ignore] public TimeSpan StartTime => StartDateTime.TimeOfDay;
    [Ignore] public TimeSpan EndTime => EndDateTime.TimeOfDay;
    [Ignore] public TimeSpan Duration => EndTime.Subtract(StartTime);

    // Must be set after original creation.
    [ForeignKey(typeof(Employee))] public int OperatorID { get; set; }

    [ManyToOne(nameof(OperatorID), nameof(Employee.PickSessions), CascadeOperations = CascadeOperation.CascadeRead)]
    public Employee? Operator { get; set; }
    [ManyToOne(nameof(StatsID), nameof(PickDailyStats.PickSessions), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickDailyStats? PickStats { get; set; }

    [OneToMany(nameof(PickEvent.SessionID), nameof(PickEvent.Session), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickEvent> PickEvents { get; set; }
    [OneToMany(nameof(MissPick.PickSessionID), nameof(MissPick.PickSession), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<MissPick> MissPicks { get; set; }

    [Ignore] public int Hits => EventCount;
    [Ignore] public int Units => Qty;
    [Ignore] public double HitsPerMinute => Hits / (Duration.Seconds / 60.0);
    [Ignore] public double UnitsPerMinute => Units / (Duration.Seconds / 60.0);

    public PickSession()
    {
        ID = Guid.NewGuid().ToString(); // This should be immediately overwritten, but automatically should be unique so as not to cause potential issues.
        StartTimeStamp = string.Empty;
        EndTimeStamp = string.Empty;
        OperatorDematicID = "0000";
        OperatorRF_ID = string.Empty;
        TechString = string.Empty;
        TechType = ETechType.PTL;

        StatsID = string.Empty;

        PickEvents = new List<PickEvent>();
        MissPicks = new List<MissPick>();
    }

    // Assume all given events do indeed form a valid pick session, and are in the correct order.
    public PickSession(List<PickEvent> pickEvents)
    {
        var first = pickEvents.First();
        var last = pickEvents.Last();

        // Check that the events aren't obviously wrong.
        if (last.DateTime < first.DateTime)
        {
            pickEvents = pickEvents.OrderBy(e => e.DateTime).ToList();
            first = pickEvents.First();
            last = pickEvents.Last();
        }

        StartTimeStamp = first.TimeStamp;
        EndTimeStamp = last.TimeStamp;
        StartDateTime = first.DateTime;
        EndDateTime = last.DateTime;
        Date = first.Date;
        OperatorDematicID = first.OperatorDematicID;
        
        ID = GetSessionID(OperatorDematicID, StartDateTime);

        OperatorRF_ID = first.OperatorRF_ID;
        Qty = pickEvents.Sum(e => e.Qty);
        EventCount = pickEvents.Count;
        TechString = first.TechString;
        TechType = first.TechType;

        OperatorID = first.OperatorID;
        Operator = first.Operator;

        StatsID = PickDailyStats.GetStatsID(OperatorDematicID, Date);

        PickEvents = pickEvents;
        foreach (var pickEvent in pickEvents)
        {
            pickEvent.SessionID = ID;
            pickEvent.Session = this;
            pickEvent.StatsID = StatsID;
        }

        MissPicks = new List<MissPick>();
        foreach (var missPick in PickEvents.Select(e => e.MissPick))
        {
            if (missPick is null) continue;

            MissPicks.Add(missPick);
            missPick.PickSessionID = ID;
            missPick.PickSession = this;
        }
    }

    public void AssignMissPick(MissPick missPick)
    {
        MissPicks.Add(missPick);

        missPick.PickSessionID = ID;
        missPick.PickSession = this;

        PickStats?.AssignMissPick(missPick);
    }

    public static string GetSessionID(string dematicID, DateTime dateTime) => $"{dematicID}.{dateTime:yyyy.MM.dd.hh.mm.ss}";

    public static List<PickSession> GeneratePickSessions(
        IEnumerable<PickEvent> pickEvents, out List<PickDailyStats> stats, TimeSpan? ptlBreak = null, TimeSpan? rftBreak = null)
    {
        ptlBreak ??= PTLBreakSpan;
        rftBreak ??= RFTBreakSpan;

        var orderedEvents = pickEvents.OrderBy(e => e.DateTime);
        var eventDict = orderedEvents.GroupBy(e => (e.OperatorDematicID, e.Date))
            .ToDictionary(g => g.Key, g => g.ToList());

        var sessionDict = new Dictionary<(string, DateTime), List<PickSession>>();
        var allSessions = new List<PickSession>();

        foreach (var (key, events) in eventDict)
        {
            sessionDict.Add(key, new List<PickSession>());

            var i = 0;
            var sessionEvents = new List<PickEvent>();
            while (i < events.Count)
            {
                var pickEvent = events[i];
                sessionEvents.Add(pickEvent);

                var nextEvent = i + 1 == events.Count ? null : events[i + 1];

                // Session over if: There is no more events; Tech Type changes; time between events passes break span.
                if (nextEvent is null ||
                    pickEvent.TechType != nextEvent.TechType ||
                    (pickEvent.TechType == ETechType.PTL && nextEvent.DateTime.Subtract(pickEvent.DateTime) >= ptlBreak) ||
                    (pickEvent.TechType == ETechType.RFT && nextEvent.DateTime.Subtract(pickEvent.DateTime) >= rftBreak))
                {
                    var session = new PickSession(sessionEvents);
                    allSessions.Add(session);
                    sessionEvents.Clear();
                    sessionDict[key].Add(session);
                }

                i++;
            }
        }

        stats = sessionDict.Select(dictItem =>
            new PickDailyStats(dictItem.Key.Item1, dictItem.Key.Item2, dictItem.Value)).ToList();

        return allSessions;
    }
}