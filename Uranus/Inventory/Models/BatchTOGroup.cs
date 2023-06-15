using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

/// <summary>
/// Sorted group of BatchTOLines
/// </summary>
public class BatchTOGroup
{
    [PrimaryKey] public Guid ID { get; set; }
    // These determine what lines are grouped. They are not determined by what is grouped.
    public string CartonSizes { get; set; } // e.g. "A" or "A,C,D"
    public string StartBays { get; set; }  // e.g. "B-F"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BatchString { get; set; }
    public string ZoneString { get; set; }
    public string LabelFileName { get; set; }
    public string LabelFileDirectory { get; set; }
    public bool IsFinalised { get; set; }

    [OneToMany(nameof(BatchTOLine.GroupID), nameof(BatchTOLine.Group), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<BatchTOLine> Lines { get; set; }

    // These are determined by the actual BatchLines.
    [Ignore] public string ActualCartonSizes => string.Join(',', Lines.Select(l => l.CartonType).Distinct().OrderBy(s => s));
    [Ignore]
    public string ActualStartBays =>
        $"{Lines.Select(l => l.StartBay).Distinct().OrderBy(s => s).Min()}-{Lines.Select(l => l.StartBay).Distinct().OrderBy(s => s).Max()}";

    public BatchTOGroup()
    {
        ID = Guid.NewGuid();
        Lines = new List<BatchTOLine>();
        CartonSizes = string.Empty;
        StartBays = string.Empty;
        BatchString = string.Empty;
        ZoneString = string.Empty;
        LabelFileName = string.Empty;
        LabelFileDirectory = string.Empty;
    }

    public BatchTOGroup(List<BatchTOLine> lines) : this()
    {
        foreach (var line in lines)
            AddLine(line);
        SetData();
    }

    public BatchTOGroup(ref List<BatchTOLine> lines, List<string>? cartonSizes = null, string? startBays = null, List<string>? startZones = null, int? lineCount = null) : this()
    {
        var sBay = Regex.Match(startBays ?? string.Empty, @"^\w+").Value;
        var eBay = Regex.Match(startBays ?? string.Empty, @"\w+$").Value;
        lineCount ??= lines.Count;
        lines = lines.OrderBy(l => l.StartBin).ThenBy(l => l.EndBin).ToList();
        // Gather lines first.
        var relevantLines = lines.Where(line =>
            (cartonSizes is null || cartonSizes.Contains(line.CartonType.ToUpper())) &&
            (startZones is null || startZones.Contains(line.StartZone)) &&
            (startBays is null ||
              (string.Compare(line.StartBay, sBay, StringComparison.OrdinalIgnoreCase) >= 0 &&
              string.Compare(line.StartBay, eBay, StringComparison.OrdinalIgnoreCase) <= 0)));

        foreach (var line in relevantLines)
        {
            AddLine(line);
            if (Lines.Count >= lineCount) break;
        }

        foreach (var line in Lines)
            lines.Remove(line);
        
        SetData();
    }

    public void Finalise(string filePath)
    {
        LabelFileName = Path.GetFileName(filePath);
        LabelFileDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;

        foreach (var line in Lines) line.Finalise(LabelFileDirectory, LabelFileName);

        IsFinalised = true;
    }

    /// <summary>
    /// Set carton/zone/bay/date data from contained lines.
    /// </summary>
    public void SetData()
    {
        CartonSizes = ActualCartonSizes;
        StartBays = ActualStartBays;
        SetAutoData();
    }

    private void SetAutoData()
    {
        if (!Lines.Any()) return;
        StartDate = Lines.Min(l => l.Date);
        EndDate = Lines.Max(l => l.Date);
        BatchString = string.Join(',', Lines.Select(l => l.BatchID).Distinct().OrderBy(s => s));
        ZoneString = string.Join(',', Lines.Select(l => l.StartZone).Distinct().OrderBy(s => s));
    }

    public void AddLine(BatchTOLine line)
    {
        line.Group?.Lines.Remove(line);
        Lines.Add(line);
        line.Group = this;
        line.GroupID = ID;
    }

    public void Merge(BatchTOGroup otherGroup)
    {
        foreach (var line in otherGroup.Lines)
            AddLine(line);

        SetData();
    }

    public List<BatchTOGroup> SplitByStartBay(List<string> bayRanges)
    {
        var lines = Lines;

        var groups = bayRanges.Select(range => new BatchTOGroup(ref lines, startBays: range)).ToList();

        Lines = lines;
        if (Lines.Count <= 0) return groups;

        SetData();
        return groups;
    }

    private static List<string> GetBayRangeStringList(string bayRangeString)
    {
        // Convert all to upper case.
        bayRangeString = bayRangeString.Trim().ToUpper();

        // Separate first by comma.
        var init = bayRangeString.Split(',');

        // Order.
        var list = init.OrderBy(s => s).ToList();

        // Iterate through and convert if needs be. (e.g. B,F,H should become B-E,F-G,H-N)
        for (var i = 0; i < init.Length; i++)
        {
            var s = list[i];

            if (Regex.IsMatch(s, "\\w+-\\w+")) continue;

            var next = i < init.Length -1 ? init[i +1][0]-1 : 'Z';
            s = $"{s}-{(char)next}";
            list[i] = s;
        }

        return list;
    }

    public List<BatchTOGroup> SplitByStartBay(string bayRangeString) => SplitByStartBay(GetBayRangeStringList(bayRangeString));

    public List<BatchTOGroup> SplitByCartonSize(List<string> cartonsSizeStringList)
    {
        var lines = Lines;

        var groups = cartonsSizeStringList.Select(c => new BatchTOGroup(ref lines, cartonSizes: c.ToUpper().Split(',').ToList())).ToList();

        Lines = lines;
        if (Lines.Count <= 0) return groups;

        SetData();
        return groups;
    }

    public List<BatchTOGroup> SplitByCartonSize() => SplitByCartonSize(CartonSizes.Split(',').ToList());

    public List<BatchTOGroup> SplitByZone()
    {
        var lines = Lines;

        var zones = lines.Select(l => l.StartZone).Distinct().ToList();

        var groups = zones.Select(zone => new List<string> { zone }).Select(zoneList => new BatchTOGroup(ref lines, startZones: zoneList)).ToList();

        Lines = lines;
        if (Lines.Count <= 0) return groups;

        SetData();
        return groups;
    }

    public BatchTOGroup PullByCount(int count)
    {
        var lines = Lines;
        // Assume that the group is in the desired order.
        var newGroup = new BatchTOGroup(ref lines, lineCount: count);

        Lines = lines;
        if (lines.Count <= 0) return newGroup;

        SetData();
        return newGroup;
    }

    public List<BatchTOGroup> SplitByCount(List<int> countList)
    {
        var lines = Lines;

        var groups = countList.Select(c => new BatchTOGroup(ref lines, lineCount: c)).ToList();

        Lines = lines;
        if (Lines.Count <= 0) return groups;

        SetData();
        return groups;
    }

    public List<BatchTOGroup> SplitByRatio(List<int> ratioList) => SplitByCount(GetCountFromRatio(ratioList));

    private List<int> GetCountFromRatio(List<int> ratioList)
    {
        var ratSum = (double) ratioList.Sum();
        var countList = ratioList.Select(rat => (int) Math.Round(rat / ratSum * Lines.Count)).ToList();

        // Make sure that all lines are accounted for.
        var rand = new Random();
        while (countList.Sum() < Lines.Count)
        {
            var i = rand.Next(0, countList.Count - 1);
            countList[i]++;
        }

        return countList;
    }

    public List<BatchTOGroup> SplitByCount(int count)
    {
        var lines = Lines;
        var groups = new List<BatchTOGroup>();
        while (lines.Count > count)
        {
            groups.Add(new BatchTOGroup(ref lines, lineCount: count));
        }

        Lines = lines;

        SetData();
        return groups;
    }

    private List<int> EvenGroupList(int groupCount)
    {
        if (groupCount <= 1) return new List<int> { Lines.Count };
        var baseSize = Lines.Count / groupCount;
        var remainder = Lines.Count % groupCount;

        var lineGroups = new List<int>();

        for (var i = 0; i < groupCount; i++)
            lineGroups.Add(baseSize + (i < remainder ? 1 : 0));

        SetData();
        return lineGroups;
    }

    public List<BatchTOGroup> SplitIntoEvenGroups(int groupCount) => SplitByCount(EvenGroupList(groupCount));

    public List<BatchTOGroup> SplitEvenlyByCount(int count) =>
        SplitByCount(EvenGroupList((int)Math.Ceiling((float)Lines.Count / (count <= 0 ? 1 : count))));

    public List<BatchTOGroup> SplitByBatch()
    {
        var batches = Lines.Select(l => l.BatchID).Distinct().ToList();
        if (batches.Count <= 1) return new List<BatchTOGroup>();

        var lines = Lines;

        // Keep first batch lines within this group to save on additional creation/removal.
        var firstBatch = batches.First();
        Lines = lines.Where(l => l.BatchID == firstBatch).ToList();
        batches.Remove(firstBatch);

        // Create additional groups based on batches.
        var lineGroups = batches.Select(batch => lines.Where(l => l.BatchID == batch).ToList()).ToList();
        var groups = lineGroups.Select(g => new BatchTOGroup(ref g)).ToList();

        SetData();

        return groups;
    }
}