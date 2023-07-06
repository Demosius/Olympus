using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public enum EErrorMethod
{
    ErrorDiscovered,
    ErrorMade
}

public class Mispick : IEquatable<Mispick>
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
    public bool QAAllocated { get; set; }

    public string Comments { get; set; }
    public DateTime ErrorDate { get; set; } // This should represent the date of the actual error. 99+% of the time this will match ShipmentDate, and will default to that.

    public bool Checked { get; set; }
    public bool NoCarton { get; set; }  // No appropriate carton found when checking pick events.
    public bool NoItem { get; set; }    // No item found to be picking amongst relevant pick events.
    public bool NoMatch { get; set; }   // Has Item and Carton, but could not be adequately assigned with available information.
    public bool MultiMatch { get; set; }// Has multiple potential culprits.

    // Mispick Assignment Data
    [ForeignKey(typeof(PickEvent))] public string PickEventID { get; set; }
    [ForeignKey(typeof(PickSession))] public string PickSessionID { get; set; }
    [ForeignKey(typeof(PickDailyStats))] public string PickStatsID { get; set; }

    public string AssignedRF_ID { get; set; }
    public string AssignedDematicID { get; set; }
    public ETechType TechType { get; set; }

    [Ignore]
    public string AssignmentString => AssignedRF_ID != string.Empty ? AssignedRF_ID :
        NoCarton ? "No Carton" :
        NoItem ? "No Item" :
        NoMatch ? "No Match" : "Unassigned";

    [OneToOne(nameof(PickEventID), nameof(Models.PickEvent.Mispick), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickEvent? PickEvent { get; set; }
    [ManyToOne(nameof(PickSessionID), nameof(Models.PickSession.Mispicks), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickSession? PickSession { get; set; }
    [ManyToOne(nameof(PickStatsID), nameof(PickDailyStats.Mispicks), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickDailyStats? PickStats { get; set; }

    [Ignore] public Employee? Employee { get; set; }

    [Ignore]
    public bool IsAssigned => Checked && !NoCarton && !NoItem &&
                              (AssignedDematicID != string.Empty || AssignedRF_ID != string.Empty);
    [Ignore] public bool CanAllocateRF => !QAAllocated || AssignedRF_ID == string.Empty;
    public Mispick()
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

    // Use when you can pinpoint a specific pick event to be tied to this mispick.
    public void AssignPickEvent(PickEvent pickEvent)
    {
        PickEventID = pickEvent.ID;
        PickEvent = pickEvent;
        PickSessionID = pickEvent.SessionID;
        PickSession = pickEvent.Session;
        PickStatsID = pickEvent.StatsID;
        PickStats = pickEvent.PickStats;

        if (CanAllocateRF) AssignedRF_ID = pickEvent.OperatorRF_ID;
        AssignedDematicID = pickEvent.OperatorDematicID;
        TechType = pickEvent.TechType;

        ErrorDate = pickEvent.Date;

        PickEvent.MispickID = ID;
        PickEvent.Mispick = this;
        PickSession?.Mispicks.Add(this);
        PickStats?.Mispicks.Add(this);
    }
    // Use when it can be determined that the mispick happened in a specific session (by a specific operator) - but cannot be tied to a specific pick event.
    public void AssignPickSession(PickSession pickSession)
    {
        PickSessionID = pickSession.ID;
        PickSession = pickSession;
        PickStatsID = pickSession.StatsID;
        PickStats = pickSession.PickStats;

        if (CanAllocateRF) AssignedRF_ID = pickSession.OperatorRF_ID;
        AssignedDematicID = pickSession.OperatorDematicID;
        TechType = pickSession.TechType;

        ErrorDate = pickSession.Date;

        PickSession.Mispicks.Add(this);
        PickStats?.Mispicks.Add(this);
    }
    // Use when it can only be determined that the mispick was made by a specific operator on a given date.
    public void AssignPickStats(PickDailyStats pickStats, ETechType tech)
    {
        PickStatsID = pickStats.ID;
        PickStats = pickStats;

        if (CanAllocateRF) AssignedRF_ID = pickStats.OperatorRF_ID;
        AssignedDematicID = pickStats.OperatorDematicID;
        TechType = tech;

        ErrorDate = pickStats.Date;

        PickStats.Mispicks.Add(this);
    }

    /// <summary>
    /// Use when assigning a cross-matching mispick.
    /// </summary>
    /// <param name="assignedMispick">Another mispick that has already been assigned.</param>
    public void AssignMatchingMispick(Mispick assignedMispick)
    {
        if (assignedMispick.PickEvent is not null)
            AssignPickEvent(assignedMispick.PickEvent);
        else if (assignedMispick.PickSession is not null)
            AssignPickSession(assignedMispick.PickSession);
        else if (assignedMispick.PickStats is not null)
            AssignPickStats(assignedMispick.PickStats, TechType);
    }

    // Assumes no relevant picker/event found. Makes sure data is clear.
    public void ClearAssigned()
    {
        PickEventID = string.Empty;
        PickSessionID = string.Empty;
        PickStatsID = string.Empty;
        PickEvent = null;
        PickSession = null;
        PickStats = null;

        if (QAAllocated) return;

        AssignedDematicID = string.Empty;
        AssignedRF_ID = string.Empty;
        Employee = null;
    }

    public void ClearMultiAssignComments()
    {
        // Clear old MA comments.
        Comments = Regex.Replace(Comments, "\\(MA:\\(.*\\)\\) ", "");
    }

    public void MultiAssign(IEnumerable<string> potentialIDMatches)
    {
        MultiMatch = true;

        ClearMultiAssignComments();

        Comments = $"(MA:({string.Join("|", potentialIDMatches)})) {Comments}";
    }

    public static string GetMispickID(string cartonID, int itemNo) => $"{cartonID}:{itemNo}";

    public static void HandleDuplicateValues(ref List<Mispick> mispicks)
    {
        // Remove duplicates.
        mispicks = mispicks.DistinctBy(e => e.ID).ToList();
    }
    public static bool Equals(Mispick? x, Mispick? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.ID == y.ID;
    }

    public static int GetHashCode(Mispick obj)
    {
        return obj.ID.GetHashCode();
    }

    public bool Equals(Mispick? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Mispick)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return ID.GetHashCode();
    }
}