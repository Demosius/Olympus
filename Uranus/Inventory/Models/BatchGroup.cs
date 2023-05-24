using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class BatchGroup
{
    [PrimaryKey] public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Continuous { get; set; }

    [Ignore] public bool Hide { get; set; }

    [ManyToMany(typeof(BatchGroupLink), nameof(BatchGroupLink.GroupName), nameof(Batch.Groups), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Batch> Batches { get; set; }

    public BatchGroup()
    {
        Name = string.Empty;
        Batches = new List<Batch>();
    }

    public BatchGroup(string name)
    {
        Name = name;
        Batches = new List<Batch>();
    }

    public BatchGroup(string name, List<Batch> batches) : this(name)
    {
        SetBatches(batches);
    }

    public void AddBatch(Batch batch)
    {
        Batches.Add(batch);
        batch.Groups.Add(this);
        if (batch.CreatedOn < StartDate) StartDate = batch.CreatedOn;
        if (batch.CreatedOn > EndDate) EndDate = batch.CreatedOn;
        if (batch.LastTimeCartonizedDate < StartDate) StartDate = batch.LastTimeCartonizedDate;
        if (batch.LastTimeCartonizedDate > EndDate) EndDate = batch.LastTimeCartonizedDate;
    }

    public void SetBatches(List<Batch> batches)
    {
        Batches = batches;
        var minCreated = Batches.Min(b => b.CreatedOn);
        var maxCreated = Batches.Max(b => b.CreatedOn);
        var minCartonized = Batches.Min(b => b.LastTimeCartonizedDate);
        var maxCartonized = Batches.Max(b => b.LastTimeCartonizedDate);
        StartDate = minCreated < minCartonized ? minCreated : minCartonized;
        EndDate = maxCreated > maxCartonized ? maxCreated : maxCartonized;
        foreach (var batch in batches)
            batch.Groups.Add(this);
    }
}