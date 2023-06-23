using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Uranus.Inventory.Models;

public enum EBatchProgress
{
    Created,
    Cartonized,
    AutoRun,
    FileExported,
    DataUploaded,
    LabelFilesCreated,
    LabelsPrinted,
    SentToPick,
    Completed
}

public enum EBatchFillProgress
{
    None,
    DataUploaded,
    LabelsPrinted,
    WorkDistributed,
    Replenished
}

public class Batch
{
    [PrimaryKey] public string ID { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime LastTimeCartonizedDate { get; set; }
    public DateTime LastTimeCartonizedTime { get; set; }
    public int Cartons { get; set; }
    public int Units { get; set; }
    public int Hits { get; set; }
    public int BulkHits { get; set; }
    public int PKHits { get; set; }
    public int SP01Hits { get; set; }
    public int Priority { get; set; }
    public string TagString { get; set; }
    public EBatchProgress Progress { get; set; }
    public EBatchFillProgress FillProgress { get; set; }
    public bool Persist { get; set; }
    public int LineCount { get; set; }
    public int CalculatedUnits { get; set; }

    [OneToMany(nameof(Move.BatchID), nameof(Move.Batch), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Move> Moves { get; set; }
    [OneToMany(nameof(BatchTOLine.BatchID), nameof(BatchTOLine.Batch), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<BatchTOLine> TOLines { get; set; }
    [OneToMany(nameof(PickLine.BatchID), nameof(PickLine.Batch), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickLine> PickLines { get; set; }

    [ManyToMany(typeof(BatchGroupLink), nameof(BatchGroupLink.BatchID), nameof(BatchGroup.Batches), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<BatchGroup> Groups { get; set; }

    [Ignore] public List<string> Tags { get; set; }

    public Batch()
    {
        ID = string.Empty;
        Description = string.Empty;
        CreatedBy = string.Empty;
        TagString = string.Empty;
        Groups = new List<BatchGroup>();
        Moves = new List<Move>();
        Tags = new List<string>();
        TOLines = new List<BatchTOLine>();
        PickLines = new List<PickLine>();
    }

    public Batch(string id) : this()
    {
        ID = id;
        if (ID.StartsWith('E')) SetTagString("Web");
    }

    public Batch(string id, int priority, List<Move> moves) : this(id)
    {
        Priority = priority;
        Moves = moves;
    }

    public override string ToString()
    {
        return $"{ID}{(Description == "" ? "" : " - ")}{Description}";
    }

    /// <summary>
    /// Merge raw batch data (fresh from NAV) into this batch, updating only the required fields.
    /// </summary>
    /// <param name="rawBatch"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void MergeRaw(Batch rawBatch)
    {
        CreatedOn = rawBatch.CreatedOn;
        CreatedBy = rawBatch.CreatedBy;

        var newTags = DetectTags(rawBatch.Description, rawBatch.ID);
        var tags = TagString.Split(',').ToList();
        tags.AddRange(newTags);
        TagString = string.Join(',', tags.Distinct().OrderBy(s => s));

        Description = rawBatch.Description;
        LastTimeCartonizedDate = rawBatch.LastTimeCartonizedDate;
        LastTimeCartonizedTime = rawBatch.LastTimeCartonizedTime;

        Priority = ComparePriority(Priority, DetectPriority(Description));

        Cartons = rawBatch.Cartons;
        Units = rawBatch.Units;

        UpdateBatchProgress(rawBatch.Progress);
    }

    public bool UpdateBatchProgress(EBatchProgress progress, bool force = false)
    {
        var initialProgress = Progress;
        if (force || progress > Progress) Progress = progress;
        return Progress == progress && Progress != initialProgress;
    }

    public static List<string> DetectTags(string description, string id)
    {
        // Break up the string
        var parts = description.Split(' ').ToList();
        if (id.StartsWith('E')) parts.Add("Web");
        return parts.Count <= 1 ? 
            new List<string>() :
            parts.Where(p => !double.TryParse(p, out _)).ToList();
    }

    public static int DetectPriority(string description)
    {
        var parts = description.Split(' ');

        if (parts.Length > 1 && int.TryParse(parts.Last(), out var i))
            return i;
        if (parts.Length > 2 && int.TryParse(parts[^2], out i))
            return i;
        return 0;
    }

    public static int ComparePriority(int a, int b)
    {
        var c = Math.Min(a == 0 ? int.MaxValue : a, b == 0 ? int.MaxValue : b);
        return c == int.MaxValue ? 0 : c;
    }

    public void CalculateHits()
    {
        Hits = PickLines.Count(l => l.ActionType == EAction.Take);
        BulkHits = PickLines.Count(l => l.ActionType == EAction.Take && Regex.IsMatch(l.ZoneCode, "^BLK", RegexOptions.IgnoreCase));
        PKHits = PickLines.Count(l => l.ActionType == EAction.Take && l.ZoneCode == "PK");
        SP01Hits = PickLines.Count(l => l.ActionType == EAction.Take && l.ZoneCode is "PO PK" or "SP PK" or "SUP PK");
        LineCount = PickLines.Count;
        CalculatedUnits = PickLines.Where(l => l.ActionType == EAction.Take).Sum(l => l.BaseQty);
    }

    public void CalculateHits(List<PickLine> pickLines)
    {
        Hits = pickLines.Count(l => l.ActionType == EAction.Take);
        BulkHits = pickLines.Count(l => l.ActionType == EAction.Take && Regex.IsMatch(l.ZoneCode, "^BLK", RegexOptions.IgnoreCase));
        PKHits = pickLines.Count(l => l.ActionType == EAction.Take && l.ZoneCode == "PK");
        SP01Hits = pickLines.Count(l => l.ActionType == EAction.Take && l.ZoneCode is "PO PK" or "SP PK" or "SUP PK");
        LineCount = pickLines.Count;
        CalculatedUnits = pickLines.Where(l => l.ActionType == EAction.Take).Sum(l => l.BaseQty);
    }

    public void SetTags()
    {
        Tags = TagString == string.Empty ?
            new List<string>() :
            TagString.Split(',').ToList();
    }

    public void SetTagString(string tagString)
    {
        TagString = tagString;
        SetTags();
    }
}