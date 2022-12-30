using System;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

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

    [OneToMany(nameof(Move.BatchID), nameof(Move.Batch), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Move> Moves { get; set; }

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
    }

    public Batch(string id, int priority, List<Move> moves)
    {
        ID = id;
        Description = string.Empty;
        CreatedBy = string.Empty;
        TagString = string.Empty;
        Priority = priority;
        Moves = moves;
        Groups = new List<BatchGroup>();
        Tags = new List<string>();
    }

}