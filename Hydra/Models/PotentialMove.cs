using Uranus.Inventory.Models;

namespace Hydra.Models;

internal class PotentialMove
{
    public Move Move { get; set; }
    public SiteItemLevel TakeSIL { get; set; }
    public SiteItemLevel PlaceSIL { get; set; }

    public Site? TakeSite => TakeSIL.Site;
    public Site? PlaceSite => PlaceSIL.Site;

    public Levels TakeLevels => TakeSIL.Levels;
    public Levels PlaceLevels => PlaceSIL.Levels;

    public Stock? TakeStock => !(TakeSite?.Stock.TryGetValue(Item?.Number ?? 0, out var fromStock) ?? false) ? null : fromStock;
    public Stock? PlaceStock => !(PlaceSite?.Stock.TryGetValue(Item?.Number ?? 0, out var placeStock) ?? false) ? null : placeStock;

    public Stock? MoveStock => !(TakeBin?.Stock.TryGetValue(Item?.Number ?? 0, out var moveStock) ?? false) ? null : moveStock;

    public NAVBin? TakeBin => Move.TakeBin;

    public NAVItem? Item { get; set; }

    public int Potential => TakeStock is null || PlaceStock is null || TakeBin is null || MoveStock is null ? int.MinValue : MoveCheck();

    public PotentialMove(Move move, SiteItemLevel takeSIL, SiteItemLevel placeSIL)
    {
        Move = move;
        TakeSIL = takeSIL;
        PlaceSIL = placeSIL;
        Item = TakeSIL.Item;
    }

    /// <summary>
    /// Checks a potential move between two sites and quantifies the move based on given target levels.
    /// </summary>
    /// <returns>int - Higher value means greater positive overall move. Negative values given for bad moves. int.Min for untenable moves.</returns>
    private int MoveCheck()
    {
        if (PlaceStock is null || TakeStock is null || MoveStock is null) return int.MinValue;

        var toVal = MoveCheck(PlaceStock, MoveStock.EachQty, MoveStock.CaseQty, MoveStock.BaseQty, false);
        var fromVal = MoveCheck(TakeStock, -MoveStock.EachQty, -MoveStock.CaseQty, -MoveStock.BaseQty, true);

        if (toVal == int.MinValue || fromVal == int.MinValue) return int.MinValue;
        return toVal + fromVal;
    }

    /// <summary>
    /// Checks a potential movement of stock to or from a greater stock, and compares it to target levels to quantify the quality of the move.
    /// </summary>
    /// <returns>int - Higher = better move, negative is worse but acceptable, min means that the move is untenable.</returns>
    private int MoveCheck(Stock stock, int moveEaches, int moveCases, int moveUnits, bool take)
    {
        if ((take ? TakeSIL : PlaceSIL).BrokenByMove(moveEaches, moveCases, moveUnits))
            return int.MinValue;

        var levels = take ? TakeLevels : PlaceLevels;

        var eachLevel = CheckLevel(stock.EachQty, moveEaches, levels.MinEaches, levels.MaxEaches);
        var caseLevel = CheckLevel(stock.CaseQty, moveCases, levels.MinCases, levels.MaxCases) * (stock.Item?.QtyPerCase ?? 1);
        var unitLevel = CheckLevel(stock.BaseQty, moveUnits, levels.MinUnits, levels.MaxUnits);

        return eachLevel + caseLevel + unitLevel;
    }

    private static int CheckLevel(int before, int diff, int min, int max)
    {
        var after = before + diff;

        var beforeLevel = 0;
        var afterLevel = 0;

        if (before < min)
            beforeLevel = before - min;
        else if (before > max)
            beforeLevel = max - before;

        if (after < min)
            afterLevel = after - min;
        else if (after > max)
            afterLevel = max - after;

        return afterLevel - beforeLevel;
    }

    public void Execute() => Move.Execute();

}