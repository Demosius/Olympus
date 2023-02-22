using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [Ignore] public string OperatorName => AssignedOperator?.DisplayName ?? (AssignedOperatorID == 0 ? string.Empty : AssignedOperatorID.ToString());

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

    public Move(NAVMoveLine takeLine, NAVMoveLine placeLine)
    {
        ID = Guid.NewGuid();
        BatchID = string.Empty;
        Item = takeLine.Item ?? placeLine.Item;
        ItemNumber = takeLine.ItemNumber;
        TakeBin = takeLine.Bin;
        TakeBinID = takeLine.BinID;
        PlaceBin = placeLine.Bin;
        PlaceBinID = placeLine.BinID;
        switch (takeLine.UoM)
        {
            case EUoM.EACH:
                TakeEaches = takeLine.Qty;
                break;
            case EUoM.PACK:
                TakePacks = takeLine.Qty;
                break;
            case EUoM.CASE:
                TakeCases = takeLine.Qty;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        switch (placeLine.UoM)
        {
            case EUoM.EACH:
                PlaceEaches = placeLine.Qty;
                break;
            case EUoM.PACK:
                PlacePacks = placeLine.Qty;
                break;
            case EUoM.CASE:
                PlaceCases = placeLine.Qty;
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        return !takeLine.IsMatch(placeLine) ? null : new Move(takeLine, placeLine);
    }

    public static List<Move> GenerateMoveList(List<NAVMoveLine> moveLines)
    {
        var moveList = new List<Move>();
        
        // Sort into items.
        var lineDict = moveLines.GroupBy(l => l.ItemNumber)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Iterate through item groupings to match and generate moves.
        foreach (var (itemNo, lines) in lineDict)
        {
            // Separate take and place lines.
            var takeLines = lines.Where(l => l.ActionType == EAction.Take).ToList();
            var placeLines = lines.Where(l => l.ActionType == EAction.Place).ToList();

            // Make sure total qty matches.
            var takeQty = takeLines.Sum(l => l.BaseQty);
            var placeQty = placeLines.Sum(l => l.BaseQty);
            if (takeQty != placeQty) throw new Exception($"Uneven movement data. Item {itemNo} has take qty of {takeQty} and place qty of {placeQty}.");

            // Prep for potential uneven lines.
            var unevenTakes = new List<NAVMoveLine>();

            // Iterate through take lines.
            foreach (var takeLine in takeLines)
            {
                // Find place line match.
                var placeLine = placeLines.FirstOrDefault(p => p.BaseQty == takeLine.BaseQty);
                if (placeLine is null)
                {
                    unevenTakes.Add(takeLine);
                    continue;
                }

                // Generate move.
                var move = GetMove(takeLine, placeLine);

                if (move is null)
                {
                    unevenTakes.Add(takeLine);
                    continue;
                }

                moveList.Add(move);
                placeLines.Remove(placeLine);
            }

            if (unevenTakes.Count > 0 && placeLines.Count > 0) moveList.AddRange(GenerateMovesFromUnevenLines(unevenTakes, placeLines));
        }

        return moveList;
    }

    private static List<Move> GenerateMovesFromUnevenLines(List<NAVMoveLine> takeLines, List<NAVMoveLine> placeLines)
    {
        var moveList = new List<Move>();

        takeLines = takeLines.OrderByDescending(l => l.BaseQty).ToList();
        placeLines = placeLines.OrderByDescending(l => l.BaseQty).ToList();

        var attempts = 0;

        while (takeLines.Count > 0 && placeLines.Count > 0 && attempts < 3)
        {
            attempts++;
            // TODO: Find matching move quantity groups.
        }

        return moveList;
    }

    /// <summary>
    /// Given a single move line and a list of (assumed) opposite lines, find a matching group.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="moveLines"></param>
    /// <returns></returns>
    private static List<NAVMoveLine> GetMatchingLines(int targetQty, List<NAVMoveLine> moveLines)
    {
        var matchQty = 0;

        int[] lines = { };

        int index = 0;
        // TODO: Finish this.
        return moveLines;
    }
}