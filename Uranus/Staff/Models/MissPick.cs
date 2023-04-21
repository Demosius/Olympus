using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class MissPick
{
    [PrimaryKey] public string ID { get; set; } // e.g. 8292418:215854 => [CartonID]:[ItemNumber]
    public DateTime ShipmentDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public DateTime PostedDate { get; set; }
    public string CartonID { get; set; }
    public int ItemNumber { get; set; }
    public string ItemDescription { get; set; }
    public string ActionNotes { get; set; }
    public int OriginalQty { get; set; }
    public int ReceivedQty { get; set; }
    public int VarianceQty { get; set; }

    public string Comments { get; set; }

    // MissPick Assignment Data
    [ForeignKey(typeof(PickEvent))] public string PickEventID { get; set; }
    [ForeignKey(typeof(PickSession))] public string PickSessionID { get; set; }
    [ForeignKey(typeof(PickStatisticsByDay))] public string PickStatsID { get; set; }

    public string AssignedRF_ID { get; set; }
    public string AssignedDematicID { get; set; }

    [OneToOne(nameof(PickEventID), nameof(Models.PickEvent.MissPick), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickEvent? PickEvent { get; set; }
    [ManyToOne(nameof(PickSessionID), nameof(Models.PickSession.MissPicks), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickSession? PickSession { get; set; }
    [ManyToOne(nameof(PickStatsID), nameof(PickStatisticsByDay.MissPicks), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickStatisticsByDay? PickStats { get; set; }

    [Ignore] public Employee? Employee { get; set; }

    public MissPick()
    {
        // Use temporary unique value for ID before appropriate assignment.
        ID = Guid.NewGuid().ToString();
        CartonID = string.Empty;
        ItemDescription = string.Empty;
        ActionNotes = string.Empty;
        Comments = string.Empty;

        PickEventID = string.Empty;
        PickSessionID = string.Empty;
        PickStatsID = string.Empty;
        AssignedRF_ID = string.Empty;
        AssignedDematicID = string.Empty;
    }

    // Use when you can pinpoint a specific pick event to be tied to this miss pick.
    public void AssignPickEvent(PickEvent pickEvent)
    {
        PickEventID = pickEvent.ID;
        PickEvent = pickEvent;
        PickSessionID = pickEvent.SessionID;
        PickSession = pickEvent.Session;
        PickStatsID = pickEvent.StatsID;
        PickStats = pickEvent.PickStats;

        AssignedRF_ID = pickEvent.OperatorRF_ID;
        AssignedDematicID = pickEvent.OperatorDematicID;

        PickEvent.MissPickID = ID;
        PickEvent.MissPick = this;
        PickSession?.MissPicks.Add(this);
        PickStats?.MissPicks.Add(this);
    }
    // Use when it can be determined that the miss pick happened in a specific session (by a specific operator) - but cannot be tied to a specific pick event.
    public void AssignPickSession(PickSession pickSession)
    {
        PickSessionID = pickSession.ID;
        PickSession = pickSession;
        PickStatsID = pickSession.StatsID;
        PickStats = pickSession.PickStats;

        AssignedRF_ID = pickSession.OperatorRF_ID;
        AssignedDematicID = pickSession.OperatorDematicID;

        PickSession.MissPicks.Add(this);
        PickStats?.MissPicks.Add(this);
    }
    // Use when it can only be determined that the miss pick was made by a specific operator on a given date.
    public void AssignPickStats(PickStatisticsByDay pickStats)
    {
        PickStatsID = pickStats.ID;
        PickStats = pickStats;

        AssignedRF_ID = pickStats.OperatorRF_ID;
        AssignedDematicID = pickStats.OperatorDematicID;

        PickStats.MissPicks.Add(this);
    }

    public static string GetMissPickID(string cartonID, int itemNo) => $"{cartonID}:{itemNo}";
}