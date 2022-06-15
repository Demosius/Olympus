using System.Collections.Generic;
using System.Linq;
using Uranus.Inventory.Models;

namespace Hydra.Helpers;

public struct Levels
{
    public int MinEaches { get; set; }
    public int MaxEaches { get; set; }
    public int MinCases { get; set; }
    public int MaxCases { get; set; }
    public float MinPct { get; set; }
    public float MaxPct { get; set; }
    public int MinUnits { get; set; }
    public int MaxUnits { get; set; }
}

public struct SIL
{
    public int Value { get; set; }  // Context determines value for the user: e.g. value as a place site.
    public Site Site { get; set; }
    public NAVItem Item { get; set; }
    public Levels Levels { get; set; }
}

public struct PotentialMove
{
    public int Potential { get; set; }
    public Move Move { get; set; }
    public SIL TakeSIL { get; set; }
    public SIL PlaceSIL { get; set; }
}

public static class MoveGenerator
{
    public static IEnumerable<Move> GenerateSiteMoves(HydraDataSet dataSet, IEnumerable<string> fromSites, IEnumerable<string> toSites)
    {
        var returnList = new List<Move>();

        // Set Site Objects
        var takeSites = new List<Site>();
        var placeSites = new List<Site>();

        var allSILList = new List<SIL>();
        var takeSILList = new List<SIL>();
        var placeSILList = new List<SIL>();

        foreach (var siteName in fromSites)
            if (dataSet.Sites.TryGetValue(siteName, out var site)) takeSites.Add(site);
        foreach (var siteName in toSites)
            if (dataSet.Sites.TryGetValue(siteName, out var site)) placeSites.Add(site);

        // TODO: Break down into a function.

        // Go through and set levels for each relevant item.
        foreach (var site in takeSites.Concat(placeSites).Distinct())
        {
            foreach (var (itemNumber, stock) in site.Stock)
            {
                // Only stop to check if item is set up for hydra.
                if (!(stock.Item?.SiteLevelTarget ?? false)) continue;

                // Get siteItemLevels. Skip if not available.
                if (!dataSet.SiteItemLevels.TryGetValue((site.Name, itemNumber), out var siteItemLevel)) continue;

                // Skip if item is not set up for site.
                if (!siteItemLevel.Active) continue;

                // Set levels.
                var levels = GetLevels(siteItemLevel, site);
                SetPctLevels(levels, stock);

                // Check if movements are potentially required.
                var take = takeSites.Contains(site);
                var place = placeSites.Contains(site);

                var baseQty = stock.BaseQty;
                var cases = stock.Cases?.Qty ?? 0;
                var eaches = stock.Eaches?.Qty ?? 0;

                var sil = new SIL
                {
                    Site = site,
                    Item = stock.Item,
                    Levels = levels
                };

                allSILList.Add(sil);

                if (take && (baseQty > levels.MaxUnits || cases > levels.MaxCases || eaches > levels.MaxEaches))
                    takeSILList.Add(sil);

                if (place && (baseQty < levels.MinUnits || cases < levels.MinCases || eaches < levels.MinEaches))
                    placeSILList.Add(sil);
            }
        }

        // TODO: Break down into a function.
        // Go through site/item combinations that are over targets and can be moved out.
        foreach (var takeSIL in takeSILList)
        {
            var takeSite = takeSIL.Site;
            var takeItem = takeSIL.Item;
            var takeLevels = takeSIL.Levels;

            var takeStock = takeSite.Stock[takeItem.Number];

            // Make sure that the quantity is over by at least one value.
            if (takeStock.EachQty <= takeLevels.MaxEaches &&
                takeStock.CaseQty <= takeLevels.MaxCases &&
                takeStock.BaseQty <= takeLevels.MaxUnits) continue;

            // Set and order Site-Item-Levels list.
            var silList = allSILList.Where(sil => sil.Item == takeItem && sil.Site != takeSite && placeSites.Contains(sil.Site)).ToList();

            // Ordered by which site could potentially benefit most from more of the item,
            // as determined by levels.
            OrderSILList(ref silList);

            var potentialMoves = GetPotentialMovesToPlace(takeSIL, takeStock, silList);

            var newMoves = ConvertToActualMoves(potentialMoves);
            var moves = newMoves.ToList();
            if (moves.Any()) returnList.AddRange(moves);
        }

        // TODO: Break down into a function.
        // Go through site/item combinations that are under targets and require more moved in.
        foreach (var placeSil in placeSILList)
        {
            var placeSite = placeSil.Site;
            var placeItem = placeSil.Item;
            var placeLevels = placeSil.Levels;

            var placeStock = placeSite.Stock[placeItem.Number];

            // Make sure that the quantity is under by at least one value.
            if (placeStock.EachQty >= placeLevels.MinEaches &&
                placeStock.CaseQty >= placeLevels.MinCases &&
                placeStock.BaseQty >= placeLevels.MinUnits) continue;

            // Set and order Site-Item-Levels list.
            var silList = allSILList.Where(sil => sil.Item == placeItem && sil.Site != placeSite && takeSites.Contains(sil.Site)).ToList();

            // Ordered by which site could potentially benefit most from more of the item,
            // as determined by levels.
            OrderSILList(ref silList);

            var potentialMoves = GetPotentialMovesToTake(placeSil, placeStock, silList);

            var newMoves = ConvertToActualMoves(potentialMoves);
            var moves = newMoves.ToList();
            if (moves.Any()) returnList.AddRange(moves);
        }

        return returnList;
    }

    private static IEnumerable<Move> ConvertToActualMoves(IEnumerable<PotentialMove> potentialMoves)
    {
        var returnList = new List<Move>();
        // Go through potential moves while there are any,
        // activating the best option, and re-organizing after.
        var pmList = potentialMoves.ToList();
        var maxCount = pmList.Count + 1;
        var count = 0;
        while (pmList.Any() && count <= maxCount)
        {
            ++count;
            var maxPotential = pmList.Max(pm => pm.Potential);
            var move = pmList.First(pm => pm.Potential == maxPotential).Move;

            returnList.Add(move);

            move.Execute();

            foreach (var potentialMove in pmList) SetMoveValue(potentialMove);

            pmList = pmList.Where(pm => pm.Potential >= 0).ToList();
        }

        return returnList;
    }

    private static IEnumerable<PotentialMove> GetPotentialMovesToPlace(SIL takeSIL, Stock takeStock, IEnumerable<SIL> placeSILList)
    {
        var takeSite = takeSIL.Site;
        var takeItem = takeSIL.Item;

        var potentialMoves = new List<PotentialMove>();

        // Go through each site (with levels) to gather potential moves.
        foreach (var placeSIL in placeSILList)
        {
            var placeSite = placeSIL.Site;

            if (!placeSite.Stock.TryGetValue(takeItem.Number, out var placeStock)) placeStock = new Stock();

            potentialMoves.AddRange(from moveStock in takeItem.StockDict.Values.Where(s => s.Site == takeSite)
                select GetPotentialMove(takeSIL, takeStock, placeSIL, placeStock, moveStock)
                into newMove
                where newMove is not null
                select (PotentialMove) newMove);
        }
        return potentialMoves;
    }

    private static IEnumerable<PotentialMove> GetPotentialMovesToTake(SIL placeSIL, Stock placeStock, IEnumerable<SIL> takeSILList)
    {
        var potentialMoves = new List<PotentialMove>();

        // Go through each site (with levels) to gather potential moves.
        foreach (var takeSIL in takeSILList)
        {
            var takeSite = takeSIL.Site;
            var takeItem = takeSIL.Item;

            if (!takeSite.Stock.TryGetValue(takeItem.Number, out var takeStock)) takeStock = new Stock();

            potentialMoves.AddRange(from moveStock in takeItem.StockDict.Values.Where(s => s.Site == takeSite)
                select GetPotentialMove(takeSIL, takeStock, placeSIL, placeStock, moveStock)
                into newMove
                where newMove is not null
                select (PotentialMove) newMove);
        }
        return potentialMoves;
    }

    private static PotentialMove? GetPotentialMove(SIL takeSIL, Stock takeStock, SIL placeSIL, Stock placeStock, Stock moveStock)
    {
        var takeLevels = takeSIL.Levels;
        var placeLevels = placeSIL.Levels;
        var takeSite = takeSIL.Site;
        var item = placeSIL.Item;

        var val = MoveCheck(takeStock, takeLevels, placeStock, placeLevels, moveStock);

        if (val <= 0 || moveStock.Bin is null) return null;

        var newMove = GenerateMove(moveStock.Bin, takeSite.Bin, item);

        if (newMove is null) return null;

        return new PotentialMove
        {
            Move = newMove,
            Potential = val,
            TakeSIL = placeSIL,
            PlaceSIL = takeSIL
        };
    }

    /// <summary>
    /// Generate a move from the given parameters.
    /// Will fail (and return null) if the stock does not exist, or if there are pending values.
    /// </summary>
    /// <param name="fromBin"></param>
    /// <param name="toBin"></param>
    /// <param name="item"></param>
    /// <param name="caseQty"></param>
    /// <param name="packQty"></param>
    /// <param name="eachQty"></param>
    /// <returns></returns>
    public static Move? GenerateMove(NAVBin fromBin, NAVBin toBin, NAVItem item,
        int? caseQty = null, int? packQty = null, int? eachQty = null)
    {
        // Set qty as appropriate, based on available qty.
        if (!fromBin.Stock.TryGetValue(item.Number, out var stock)) return null;

        if (stock.Pending()) return null;

        if (caseQty is null || caseQty > stock.Cases?.AvailableQty) caseQty = stock.Cases?.AvailableQty ?? 0;
        if (packQty is null || packQty > stock.Packs?.AvailableQty) packQty = stock.Packs?.AvailableQty ?? 0;
        if (eachQty is null || eachQty > stock.Eaches?.AvailableQty) eachQty = stock.Eaches?.AvailableQty ?? 0;

        return new Move(fromBin, toBin, item, (int)caseQty, (int)packQty, (int)eachQty);
    }

    public static Levels GetLevels(SiteItemLevel sil, Site site)
    {
        var useSil = sil.OverrideDefaults;
        var item = sil.Item;
        var levels = new Levels
        {
            MinEaches = useSil ? sil.MinEaches ?? 0 : site.MinEaches ?? 0,
            MaxEaches = useSil
                ? sil.MaxEaches ?? (item?.Stock?.BaseQty ?? 0)
                : site.MaxEaches ?? (item?.Stock?.BaseQty ?? 0),
            MinCases = useSil ? sil.MinCases ?? 0 : site.MinCases ?? 0,
            MaxCases = useSil
                ? sil.MaxCases ?? (item?.Stock?.Cases?.Qty ?? 0)
                : site.MaxCases ?? (item?.Stock?.Cases?.Qty ?? 0),
            MinPct = useSil ? sil.MinPct ?? 0 : site.MinPct ?? 0,
            MaxPct = useSil ? sil.MaxPct ?? 1 : site.MaxPct ?? 1,
        };

        return levels;
    }

    /// <summary>
    /// Based on stock levels, convert the percentage to unit targets, and override the min/max units where appropriate.
    /// </summary>
    /// <param name="levels"></param>
    /// <param name="stock"></param>
    public static void SetPctLevels(Levels levels, Stock stock)
    {
        var minUnits = levels.MinEaches + levels.MinCases * stock.Item?.QtyPerCase ?? 0;
        var maxUnits = levels.MaxEaches + levels.MaxCases * stock.Item?.QtyPerCase ?? 0;

        var baseQty = stock.Item?.Stock?.BaseQty ?? 0;

        var minPct = (int)(levels.MinPct * baseQty);
        var maxPct = (int)(levels.MaxPct * baseQty);

        if (minPct > minUnits) minUnits = minPct;
        if (maxPct < maxUnits) maxUnits = maxPct;

        levels.MinUnits = minUnits;
        levels.MaxUnits = maxUnits;
    }

    /// <summary>
    /// Checks a potential move between two sites and quantifies the move based on given target levels.
    /// </summary>
    /// <returns>int - Higher value means greater positive overall move. Negative values given for bad moves. int.Min for untenable moves.</returns>
    private static int MoveCheck(Stock fromStock, Levels fromLevels, Stock toStock, Levels toLevels, Stock moveStock)
    {
        var toVal = ToCheck(toStock, toLevels, moveStock);
        var fromVal = FromCheck(fromStock, fromLevels, moveStock);

        if (toVal == int.MinValue || fromVal == int.MinValue) return int.MinValue;
        return toVal + fromVal;
    }

    private static void SetMoveValue(PotentialMove potentialMove)
    {
        var item = potentialMove.TakeSIL.Item;
        var move = potentialMove.Move;

        if (!potentialMove.TakeSIL.Site.Stock.TryGetValue(item.Number, out var fromStock) ||
            !potentialMove.PlaceSIL.Site.Stock.TryGetValue(item.Number, out var toStock) ||
            move.TakeBin is null ||
            !move.TakeBin.Stock.TryGetValue(item.Number, out var moveStock))
        {
            potentialMove.Potential = int.MinValue;
            return;
        }

        potentialMove.Potential = MoveCheck(fromStock, potentialMove.TakeSIL.Levels,
            toStock, potentialMove.PlaceSIL.Levels, moveStock);
    }

    /// <summary>
    /// Checks a potential movement of stock from a greater stock, and compares it to target levels to quantify the quality of the move.
    /// </summary>
    /// <param name="fromStock"></param>
    /// <param name="levels"></param>
    /// <param name="moveStock"></param>
    /// <returns>int - Higher = better move, negative is worse but acceptable, min means that the move is untenable.</returns>
    private static int FromCheck(Stock fromStock, Levels levels, Stock moveStock)
    {
        var eachesBefore = fromStock.EachQty;
        var eachesAfter = eachesBefore - moveStock.EachQty;
        var casesBefore = fromStock.CaseQty;
        var casesAfter = casesBefore - moveStock.CaseQty;
        var unitsBefore = fromStock.BaseQty;
        var unitsAfter = unitsBefore - moveStock.BaseQty;

        // Check for changes from with limits to outside limits.
        // These automatically disqualify the move.
        if ((eachesBefore <= levels.MaxEaches && eachesBefore >= levels.MinEaches &&
             (eachesAfter > levels.MaxEaches || eachesAfter < levels.MinEaches)) ||
            (casesBefore <= levels.MaxCases && casesBefore >= levels.MinCases &&
             (casesAfter > levels.MaxCases || casesAfter < levels.MinCases)) ||
            (unitsBefore <= levels.MaxUnits && unitsBefore >= levels.MinUnits &&
             (unitsAfter > levels.MaxUnits || unitsAfter < levels.MinUnits)))
            return int.MinValue;

        var eachLevel = CheckLevel(eachesBefore, eachesAfter, levels.MinEaches, levels.MaxEaches);
        var caseLevel = CheckLevel(casesBefore, casesAfter, levels.MinCases, levels.MaxCases) * fromStock.Item?.QtyPerCase ?? 1;
        var unitLevel = CheckLevel(unitsBefore, unitsAfter, levels.MinUnits, levels.MaxUnits);

        return eachLevel + caseLevel + unitLevel;
    }


    /// <summary>
    /// Checks a potential movement of stock to a greater stock, and compares it to target levels to quantify the quality of the move.
    /// </summary>
    /// <param name="toStock"></param>
    /// <param name="levels"></param>
    /// <param name="moveStock"></param>
    /// <returns>int - Higher = better move, negative is worse but acceptable, min means that the move is untenable.</returns>
    private static int ToCheck(Stock toStock, Levels levels, Stock moveStock)
    {
        var eachesBefore = toStock.EachQty;
        var eachesAfter = eachesBefore + moveStock.EachQty;
        var casesBefore = toStock.CaseQty;
        var casesAfter = casesBefore + moveStock.CaseQty;
        var unitsBefore = toStock.BaseQty;
        var unitsAfter = unitsBefore + moveStock.BaseQty;

        // Check for changes from with limits to outside limits.
        // These automatically disqualify the move.
        if ((eachesBefore <= levels.MaxEaches && eachesBefore >= levels.MinEaches &&
             (eachesAfter > levels.MaxEaches || eachesAfter < levels.MinEaches)) ||
            (casesBefore <= levels.MaxCases && casesBefore >= levels.MinCases &&
             (casesAfter > levels.MaxCases || casesAfter < levels.MinCases)) ||
            (unitsBefore <= levels.MaxUnits && unitsBefore >= levels.MinUnits &&
             (unitsAfter > levels.MaxUnits || unitsAfter < levels.MinUnits)))
            return int.MinValue;

        var eachLevel = CheckLevel(eachesBefore, eachesAfter, levels.MinEaches, levels.MaxEaches);
        var caseLevel = CheckLevel(casesBefore, casesAfter, levels.MinCases, levels.MaxCases) * toStock.Item?.QtyPerCase ?? 1;
        var unitLevel = CheckLevel(unitsBefore, unitsAfter, levels.MinUnits, levels.MaxUnits);

        return eachLevel + caseLevel + unitLevel;

    }

    private static int CheckLevel(int before, int after, int min, int max)
    {
        var beforeLevel = 0;
        var afterLevel = 0;

        if (before < min)
            beforeLevel = before - min;
        else if (before > max)
            beforeLevel = max - before;

        if (after < min)
            afterLevel = before - min;
        else if (after > max)
            afterLevel = max - before;

        return afterLevel - beforeLevel;
    }

    private static void OrderSILList(ref List<SIL> silList)
    {
        foreach (var sil in silList) SetPlacePotential(sil);

        silList = silList.OrderBy(sil => sil.Value).ToList();
    }

    /// <summary>
    /// Compare stock values to minimum levels to quantify a numeric value according to whether stock should go TO the given site.
    /// </summary>
    /// <param name="sil"></param>
    /// <returns>int value representing the potential of the site to receive the item. Higher = better.</returns>
    private static void SetPlacePotential(SIL sil)
    {
        if (!sil.Site.Stock.TryGetValue(sil.Item.Number, out var stock)) stock = new Stock();

        var eaches = sil.Levels.MinEaches - stock.EachQty;
        var cases = (sil.Levels.MinCases - stock.CaseQty) * sil.Item.QtyPerCase;
        var units = sil.Levels.MinUnits - stock.BaseQty;

        sil.Value = eaches + units + cases;
    }
}