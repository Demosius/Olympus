using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class PickDailyStats
{
    [PrimaryKey] public string ID { get; set; } // Example: 6118.2022.11.23 ([OperatorID].[Year].[Month].[Day]) 
    [Indexed] public DateTime Date { get; set; }
    public TimeSpan PickDuration { get; set; }
    public int Qty { get; set; }
    public int SessionCount { get; set; }
    public int EventCount { get; set; }
    [StringLength(4)] public string OperatorDematicID { get; set; }
    public string OperatorRF_ID { get; set; }
    public string StartTimeStamp { get; set; }
    public string EndTimeStamp { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    [Ignore] public TimeSpan StartTime => StartDateTime.TimeOfDay;
    [Ignore] public TimeSpan EndTime => EndDateTime.TimeOfDay;
    [Ignore] public TimeSpan TrackingDuration => EndTime.Subtract(StartTime);
    [Ignore] public TimeSpan DownTime => TrackingDuration.Subtract(PickDuration);
    [Ignore] public int BreakCount => SessionCount - 1;

    // Must be set after original creation.
    [ForeignKey(typeof(Employee))] public int OperatorID { get; set; }

    [ManyToOne(nameof(OperatorID), nameof(Employee.PickStatistics), CascadeOperations = CascadeOperation.CascadeRead)]
    public Employee? Operator { get; set; }

    [OneToMany(nameof(PickSession.StatsID), nameof(PickSession.PickStats), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickSession> PickSessions { get; set; }
    [OneToMany(nameof(PickEvent.StatsID), nameof(PickEvent.PickStats), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickEvent> PickEvents { get; set; }
    [OneToMany(nameof(Mispick.PickStatsID), nameof(Mispick.PickStats), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Mispick> Mispicks { get; set; }

    [Ignore] public int Hits => EventCount;
    [Ignore] public int Units => Qty;
    [Ignore] public double HitsPerMinute => Hits / (PickDuration.Seconds / 60.0);
    [Ignore] public double UnitsPerMinute => Units / (PickDuration.Seconds / 60.0);

    [Ignore] public List<Mispick> ShipDateMispicks => Mispicks;
    [Ignore] public List<Mispick> ReceiveDateMispicks { get; set; }
    [Ignore] public List<Mispick> PostedDateMispicks { get; set; }

    public PickDailyStats()
    {
        ID = Guid.NewGuid().ToString(); // This should be immediately overwritten, but automatically should be unique so as not to cause potential issues.
        OperatorDematicID = string.Empty;
        OperatorRF_ID = string.Empty;
        StartTimeStamp = string.Empty;
        EndTimeStamp = string.Empty;

        PickSessions = new List<PickSession>();
        PickEvents = new List<PickEvent>();
        Mispicks = new List<Mispick>();

        ReceiveDateMispicks = new List<Mispick>();
        PostedDateMispicks = new List<Mispick>();
    }
    
    public PickDailyStats(string dematicID, DateTime date, List<PickSession> sessions)
    {
        ID = GetStatsID(dematicID, date);

        // Make sure that the sessions are correct and in order.
        sessions = sessions
            .Where(s => s.OperatorDematicID == dematicID && s.Date == date)
            .OrderBy(s => s.StartDateTime)
            .ToList();
        var first = sessions.First();
        var last = sessions.Last();

        PickSessions = sessions;
        PickEvents = sessions.SelectMany(s => s.PickEvents).ToList();

        Date = date;
        PickDuration = new TimeSpan(sessions.Sum(s => s.Duration.Ticks));
        Qty = sessions.Sum(s => s.Qty);
        SessionCount = sessions.Count;
        EventCount = sessions.Sum(s => s.EventCount);
        OperatorDematicID = dematicID;
        OperatorRF_ID = first.OperatorRF_ID;
        StartTimeStamp = first.StartTimeStamp;
        EndTimeStamp = last.EndTimeStamp;
        StartDateTime = first.StartDateTime;
        EndDateTime = last.EndDateTime;

        OperatorID = first.OperatorID;
        Operator = first.Operator;

        foreach (var pickSession in sessions) pickSession.PickStats = this;
        foreach (var pickEvent in PickEvents) pickEvent.PickStats = this;

        Mispicks = new List<Mispick>();
        foreach (var mispick in PickEvents.Select(e => e.Mispick))
        {
            if (mispick is null) continue;
            AssignMispick(mispick);
        }

        ReceiveDateMispicks = new List<Mispick>();
        PostedDateMispicks = new List<Mispick>();
    }

    public static string GetStatsID(string dematicID, DateTime date) => $"{dematicID}.{date:yyyy.MM.dd}";

    public void AddSession(PickSession session)
    {
        PickSessions.Add(session);
        session.PickStats = this;
        PickEvents.AddRange(session.PickEvents);
    }

    public void AssignMispick(Mispick mispick)
    {
        mispick.PickStatsID = ID;
        mispick.PickStats = this;
        mispick.ErrorDate = Date;

        Mispicks.Add(mispick);
    }
}