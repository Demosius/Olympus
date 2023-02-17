using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using Uranus.Staff.Models;

namespace Uranus.Inventory.Models;

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
    [ManyToOne(nameof(BatchID), nameof(Models.Batch.Moves), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Batch? Batch { get; set; }

    public int TakeCases { get; set; }
    public int TakePacks { get; set; }
    public int TakeEaches { get; set; }
    public int PlaceCases { get; set; }
    public int PlacePacks { get; set; }
    public int PlaceEaches { get; set; }

    public int? GroupPriority { get; set; }
    public int? IndividualPriority { get; set; }

    public bool HasExecuted { get; set; }

    public int AssignedOperatorID { get; set; }
    public float TimeEstimate { get; set; }

    [Ignore] public bool FullPallet => TakeBin != null && (TakeBin.IsFullQty(this) ?? false) && AccessLevel != EAccessLevel.Ground;

    [Ignore] public EAccessLevel AccessLevel => (TakeBin ?? new NAVBin()).Zone?.AccessLevel ?? EAccessLevel.Ground;

    [Ignore] public NAVZone? TakeZone => TakeBin?.Zone;
    [Ignore] public Site? TakeSite => TakeZone?.Site;
    [Ignore] public Bay? TakeBay => TakeBin?.Bay;
    [Ignore] public string? TakeLocation => TakeBin?.LocationCode;

    [Ignore] public NAVZone? PlaceZone => PlaceBin?.Zone;
    [Ignore] public Site? PlaceSite => PlaceZone?.Site;
    [Ignore] public Bay? PlaceBay => PlaceBin?.Bay;
    [Ignore] public string? PlaceLocation => PlaceBin?.LocationCode;

    [Ignore] public int? BatchPriority => Batch?.Priority;

    [Ignore] public Employee? AssignedOperator { get; set; }
    [Ignore] public string OperatorName => AssignedOperator?.DisplayName ?? AssignedOperatorID.ToString();

    [Ignore]
    public int Priority
    {
        get => IndividualPriority ?? GroupPriority ?? BatchPriority ?? 1;
        set => IndividualPriority = value;
    }

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

    public Move(NAVBin takeBin, NAVBin placeBin, NAVItem item, int caseQty, int packQty, int eachQty)
    {
        ID = Guid.NewGuid();
        Item = item;
        ItemNumber = item.Number;
        TakeBin = takeBin;
        TakeBinID = takeBin.ID;
        PlaceBin = placeBin;
        PlaceBinID = placeBin.ID;
        TakeCases = caseQty;
        TakePacks = packQty;
        TakeEaches = eachQty;
        PlaceCases = caseQty;
        PlacePacks = packQty;
        PlaceEaches = eachQty;

        Batch = null;
        BatchID = string.Empty;
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

    public void Execute()
    {
        // TODO: Add appropriate error generation here.
        if (TakeBin is null || PlaceBin is null || Item is null || HasExecuted) return;
        if (!TakeBin.Stock.TryGetValue(Item.Number, out var stock)) return;

        var moveStock = stock.Split(TakeEaches, TakePacks, TakeCases);

        moveStock.RemoveStock();
        moveStock.Convert(PlaceEaches, PlacePacks, PlaceCases);
        moveStock.ChangeBin(PlaceBin);
        moveStock.AddStock();

        HasExecuted = true;
    }

    public void Undo()
    {
        // TODO: Add appropriate error generation here.
        if (TakeBin is null || PlaceBin is null || Item is null || !HasExecuted) return;
        if (!PlaceBin.Stock.TryGetValue(Item.Number, out var stock)) return;

        var moveStock = stock.Split(PlaceEaches, PlacePacks, PlaceCases);

        moveStock.RemoveStock();
        moveStock.Convert(TakeEaches, TakePacks, TakeCases);
        moveStock.ChangeBin(TakeBin);
        moveStock.AddStock();

        HasExecuted = false;
    }

    public bool IsTakeFull()
    {
        if (TakeBin is null || PlaceBin is null || Item is null) return false;
        return TakeBin.Stock.TryGetValue(Item.Number, out var stock) &&
               stock.IsFull(TakeEaches, TakePacks, TakeCases);
    }

    public bool IsPlaceFull()
    {
        if (TakeBin is null || PlaceBin is null || Item is null) return false;
        return PlaceBin.Stock.TryGetValue(Item.Number, out var stock) &&
               stock.IsFull(PlaceEaches, PlacePacks, PlaceCases);
    }

    /// <summary>
    /// Static method used to generate move from two (presumably matching) move lines.
    /// </summary>
    /// <param name="takeLine">A move line with take action to match the place line.</param>
    /// <param name="placeLine">A move line with place action to match the take line.</param>
    /// <returns>Null if lines don't match, otherwise a suitable Move object.</returns>
    public static Move? GetMove(NAVMoveLine takeLine, NAVMoveLine placeLine)
    {
        if takeLine
        return null;
    }
}