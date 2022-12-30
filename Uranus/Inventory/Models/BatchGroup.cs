using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class BatchGroup
{
    [PrimaryKey] public string Name { get; set; }

    [ManyToMany(typeof(BatchGroupLink), nameof(BatchGroupLink.GroupName), nameof(Batch.Groups), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<Batch> Batches { get; set; }

    public BatchGroup(string name)
    {
        Name = name;
        Batches = new List<Batch>();
    }
}