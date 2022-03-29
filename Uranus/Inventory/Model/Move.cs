using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Model;

public class Move
{
    [PrimaryKey] public Guid ID { get; set; }
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }
    [ForeignKey(typeof(NAVBin))] public string TakeBinID { get; set; }
    [ForeignKey(typeof(NAVBin))] public string PlaceBinID { get; set; }
    [ForeignKey(typeof(Batch))] public string BatchID { get; set; }

    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.Moves), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }
    [ManyToOne(nameof(TakeBinID), nameof(NAVBin.FromMoves), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin? TakeBin { get; set; }
    [ManyToOne(nameof(PlaceBinID), nameof(NAVBin.ToMoves), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin? PlaceBin { get; set; }
    [ManyToOne(nameof(BatchID), nameof(Model.Batch.Moves), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Batch? Batch { get; set; }

    public int TakeCases { get; set; }
    public int TakePacks { get; set; }
    public int TakeEaches { get; set; }
    public int PlaceCases { get; set; }
    public int PlacePacks { get; set; }
    public int PlaceEaches { get; set; }

    [Ignore] public bool FullPallet => TakeBin != null && (TakeBin.IsFullQty(this) ?? false) && AccessLevel != EAccessLevel.Ground;

    [Ignore] public EAccessLevel AccessLevel => (TakeBin ?? new NAVBin()).Zone?.AccessLevel ?? EAccessLevel.Ground;

    [Ignore] public int AssignedOperator { get; set; }
    public float TimeEstimate { get; set; }

    public Move()
    {
        ID = Guid.NewGuid();
        TakeBinID = string.Empty;
        PlaceBinID = string.Empty;
        BatchID = string.Empty;
    }

    public Move(string takeBinID, string placeBinID, string batchID) : this()
    {
        TakeBinID = takeBinID;
        PlaceBinID = placeBinID;
        BatchID = batchID;
    }

    public Move(string takeBinID, string placeBinID, string batchID, NAVItem item, NAVBin takeBin, NAVBin placeBin, Batch batch) : this(takeBinID, placeBinID, batchID)
    {
        Item = item;
        TakeBin = takeBin;
        PlaceBin = placeBin;
        Batch = batch;
    }

    public Move(NAVMoveLine moveLine, string placeBinID, string batchID) : this()
    {
        PlaceBinID = placeBinID;
        BatchID = batchID;
        ItemNumber = moveLine.ItemNumber;
        if (moveLine.ActionType == EAction.Place)
        {
            PlaceBin = moveLine.Bin;
            PlaceBinID = moveLine.BinID;
        }
        else // MUST be take.
        {
            TakeBin = moveLine.Bin;
            PlaceBinID = moveLine.BinID;
        }
    }

    public bool LineMatch(NAVMoveLine moveLine)
    {
        //TODO: PartialMove line matching.
        throw new NotImplementedException();
    }

    public bool MergeLine(NAVMoveLine moveLine)
    {
        //TODO: PartialMove line merging.
        throw new NotImplementedException();
    }
}