using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

public class Batch
{
    [PrimaryKey] public string ID { get; set; }
    public int Priority { get; set; }

    [OneToMany(nameof(Move.BatchID), nameof(Move.Batch), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Move> Moves { get; set; }

    public Batch()
    {
        ID = "";
        Moves = new List<Move>();
    }

    public Batch(string id, int priority, List<Move> moves)
    {
        ID = id;
        Priority = priority;
        Moves = moves;
    }

}