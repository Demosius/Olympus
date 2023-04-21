using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class PickStatisticsByDay
{
    [PrimaryKey] public string ID { get; set; } // Example: 6118.2022.11.23 ([OperatorID].[Year].[Month].[Day]) 
    public DateTime Date { get; set; }
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

    // Must be set after original creation.
    [ForeignKey(typeof(Employee))] public int OperatorID { get; set; }

    [ManyToOne(nameof(OperatorID), nameof(Employee.PickStatistics), CascadeOperations = CascadeOperation.CascadeRead)]
    public Employee? Operator { get; set; }

    [OneToMany(nameof(PickSession.StatsID), nameof(PickSession.PickStats), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickSession> PickSessions { get; set; }
    [OneToMany(nameof(PickEvent.StatsID), nameof(PickEvent.PickStats), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickEvent> PickEvents { get; set; }
    [OneToMany(nameof(MissPick.PickStatsID), nameof(MissPick.PickStats), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<MissPick> MissPicks { get; set; }

    public PickStatisticsByDay()
    {
        ID = Guid.NewGuid().ToString(); // This should be immediately overwritten, but automatically should be unique so as not to cause potential issues.
        OperatorDematicID = string.Empty;
        OperatorRF_ID = string.Empty;
        StartTimeStamp = string.Empty;
        EndTimeStamp = string.Empty;

        PickSessions = new List<PickSession>();
        PickEvents = new List<PickEvent>();
        MissPicks = new List<MissPick>();
    }
    
    public PickStatisticsByDay(string dematicID, DateTime date, List<PickSession> sessions)
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
        PickDuration = last.EndDateTime.Subtract(first.StartDateTime);
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

        MissPicks = new List<MissPick>();
        foreach (var missPick in PickEvents.Select(e => e.MissPick))
        {
            if (missPick is null) continue;
            AssignMissPick(missPick);
        }
    }

    public static string GetStatsID(string dematicID, DateTime date) => $"{dematicID}.{date:yyyy.MM.dd}";

    public void AddSession(PickSession session)
    {
        PickSessions.Add(session);
        session.PickStats = this;
        PickEvents.AddRange(session.PickEvents);
    }

    public void AssignMissPick(MissPick missPick)
    {
        missPick.PickStatsID = ID;
        missPick.PickStats = this;

        MissPicks.Add(missPick);
    }
}