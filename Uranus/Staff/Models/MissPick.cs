﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Staff.Models;

public class MissPick : IEquatable<MissPick>
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

    public bool Checked { get; set; }
    public bool NoCarton { get; set; }  // No appropriate carton found when checking pick events.
    public bool NoItem { get; set; }    // No item found to be picking amongst relevant pick events.
    public bool NoMatch { get; set; }   // Has Item and Carton, but could not be adequately assigned with available information.
    public bool MultiMatch { get; set; }// Has multiple potential culprits.

    // MissPick Assignment Data
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

    [OneToOne(nameof(PickEventID), nameof(Models.PickEvent.MissPick), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickEvent? PickEvent { get; set; }
    [ManyToOne(nameof(PickSessionID), nameof(Models.PickSession.MissPicks), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickSession? PickSession { get; set; }
    [ManyToOne(nameof(PickStatsID), nameof(PickDailyStats.MissPicks), CascadeOperations = CascadeOperation.CascadeRead)]
    public PickDailyStats? PickStats { get; set; }

    [Ignore] public Employee? Employee { get; set; }

    [Ignore]
    public bool IsAssigned => Checked && !NoCarton && !NoItem &&
                              (AssignedDematicID != string.Empty || AssignedRF_ID != string.Empty);

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
        TechType = pickEvent.TechType;

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
        TechType = pickSession.TechType;

        PickSession.MissPicks.Add(this);
        PickStats?.MissPicks.Add(this);
    }
    // Use when it can only be determined that the miss pick was made by a specific operator on a given date.
    public void AssignPickStats(PickDailyStats pickStats, ETechType tech)
    {
        PickStatsID = pickStats.ID;
        PickStats = pickStats;

        AssignedRF_ID = pickStats.OperatorRF_ID;
        AssignedDematicID = pickStats.OperatorDematicID;
        TechType = tech;

        PickStats.MissPicks.Add(this);
    }

    /// <summary>
    /// Use when assigning a cross-matching miss pick.
    /// </summary>
    /// <param name="assignedMissPick">Another miss pick that has already been assigned.</param>
    public void AssignMatchingMissPick(MissPick assignedMissPick)
    {
        if (assignedMissPick.PickEvent is not null) 
            AssignPickEvent(assignedMissPick.PickEvent);
        else if (assignedMissPick.PickSession is not null)
            AssignPickSession(assignedMissPick.PickSession);
        else if (assignedMissPick.PickStats is not null)
            AssignPickStats(assignedMissPick.PickStats, TechType);
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

        AssignedDematicID = string.Empty;
        AssignedRF_ID = string.Empty;
        Employee = null;
    }

    public void MultiAssign(IEnumerable<string> potentialIDMatches)
    {
        MultiMatch = true;
        // Clear old MA comments.
        Regex.Replace(Comments, "\\(MA:\\(.*\\)\\) ", "");

        Comments = $"(MA:({string.Join("|", potentialIDMatches)})) {Comments}";
    }

    public static string GetMissPickID(string cartonID, int itemNo) => $"{cartonID}:{itemNo}";

    public bool Equals(MissPick? x, MissPick? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.ID == y.ID;
    }

    public int GetHashCode(MissPick obj)
    {
        return obj.ID.GetHashCode();
    }

    public bool Equals(MissPick? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MissPick) obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return ID.GetHashCode();
    }
}