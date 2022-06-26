using Hydra.Models;
using System.Collections.Generic;
using System.Linq;
using Uranus.Inventory.Models;

namespace Hydra.Helpers;

public static class MoveGenerator
{
    public static IEnumerable<Move> GenerateSiteMoves(HydraDataSet? dataSet, IEnumerable<string> fromSites, IEnumerable<string> toSites)
    {
        var returnList = new List<Move>();

        // Set Site Objects
        var takeSites = dataSet.Sites.Values.Where(s => fromSites.Contains(s.Name)).ToList();
        var placeSites = dataSet.Sites.Values.Where(s => toSites.Contains(s.Name)).ToList();

        // Go through and set levels for each relevant item.
        GetSILLists(dataSet, takeSites, placeSites, out var takeableSIL, out var placeableSIL,
            out var targetTakeSIL, out var targetPlaceSIL);
        
        // Go through site/item combinations that are over targets and can be moved out.
        var takeMoves = GetMoves(targetTakeSIL, placeableSIL, true).ToList();
        // Go through site/item combinations that are under targets and require more moved in.
        var placeMoves = GetMoves(targetPlaceSIL, takeableSIL, false).ToList();

        if (takeMoves.Any()) returnList.AddRange(takeMoves);
        if (placeMoves.Any()) returnList.AddRange(placeMoves);

        return returnList;
    }

    private static void GetSILLists(HydraDataSet? dataSet, ICollection<Site> takeSites, ICollection<Site> placeSites,
        out List<SiteItemLevel> takeableSIL, out List<SiteItemLevel> placeableSIL,
        out List<SiteItemLevel> targetTakeSIL, out List<SiteItemLevel> targetPlaceSIL)
    {
        takeableSIL = new List<SiteItemLevel>();
        placeableSIL = new List<SiteItemLevel>();
        targetTakeSIL = new List<SiteItemLevel>();
        targetPlaceSIL = new List<SiteItemLevel>();

        var items = dataSet.Items.Values.Where(i => i.SiteLevelTarget).ToList();

        foreach (var site in takeSites.Concat(placeSites).Distinct())
        {
            foreach (var item in items)
            {
                if (!site.Stock.TryGetValue(item.Number, out var stock))
                {
                    stock = new Stock(item, site.Bin!);
                    site.AddStock(stock);
                }

                // Get siteItemLevels. Skip if not available.
                if (!dataSet.SiteItemLevels.TryGetValue((site.Name, item.Number), out var sil)) continue;

                // Skip if item is not set up for site.
                if (!sil.Active) continue;

                // Check if movements are potentially required.
                var take = takeSites.Contains(site);
                var place = placeSites.Contains(site);

                if (take)
                {
                    takeableSIL.Add(sil);
                    if (sil.TargetToTake)
                        targetTakeSIL.Add(sil);
                }

                if (!place) continue;

                placeableSIL.Add(sil);
                if (sil.TargetToPlace)
                    targetPlaceSIL.Add(sil);
            }
        }
    }

    private static IEnumerable<Move> GetMoves(List<SiteItemLevel> targetSIL, IReadOnlyCollection<SiteItemLevel> otherSIL, bool take)
    {
        var returnList = new List<Move>();

        // Go through site/item combinations that are over targets and can be moved out.
        foreach (var sil in targetSIL)
        {
            if (sil.Site is null || sil.Item is null) continue;

            var site = sil.Site;
            var item = sil.Item;

            // Make sure that the quantity is over by at least one value.
            if ((take && !sil.Takeable) || (!take && !sil.Placeable)) continue;

            // Set and order Site-Item-Levels list.
            var silList = otherSIL.Where(s => s.Item == item && s.Site != site).ToList();

            // Ordered by which site has the greatest move potential.
            silList = silList.OrderBy(s => take ? s.TakePotential : s.PlacePotential).ToList();

            var potentialMoves = GetPotentialMoves(sil, silList, take);

            var newMoves = ConvertToActualMoves(potentialMoves).ToList();

            if (newMoves.Any()) returnList.AddRange(newMoves);
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
            var maxPotential = 0;
            PotentialMove? potentialMove = null;

            foreach (var move in pmList)
            {
                if (move.Potential <= maxPotential) continue;
                maxPotential = move.Potential;
                potentialMove = move;
            }

            if (potentialMove is null) break;

            returnList.Add(potentialMove.Move);
            pmList.Remove(potentialMove);
            
            potentialMove.Execute();

            pmList = pmList.Where(pm => pm.Potential >= 0).ToList();
        }

        return returnList;
    }
    
    private static IEnumerable<PotentialMove> GetPotentialMoves(SiteItemLevel sil, IEnumerable<SiteItemLevel> otherSIL, bool take)
    {
        if (sil.Site is null || sil.Item is null) return new List<PotentialMove>();

        var potentialMoves = new List<PotentialMove>();

        Site? takeSite = null;
        var item = sil.Item;

        if (take) takeSite = sil.Site;

        foreach (var siteItemLevel in otherSIL)
        {
            if (siteItemLevel.Site is null) continue;

            if (!take) takeSite = siteItemLevel.Site;
            var site = takeSite;

            potentialMoves.AddRange(from moveStock in item.StockDict.Values.Where(s => s.Site == site)
                select GetPotentialMove(take ? sil : siteItemLevel , take ? siteItemLevel : sil, moveStock)
                into newMove
                where newMove is not null
                select (PotentialMove)newMove);
        }

        return potentialMoves;
    }

    private static PotentialMove? GetPotentialMove(SiteItemLevel takeSIL, SiteItemLevel placeSIL, Stock moveStock)
    {
        if (placeSIL.Item is null) return null;

        var placeSite = placeSIL.Site;
        var item = placeSIL.Item;

        if (moveStock.Bin is null || placeSite?.Bin is null) return null;

        var newMove = GenerateMove(moveStock.Bin, placeSite.Bin, item);

        if (newMove is null) return null;
        var pm = new PotentialMove(newMove, takeSIL, placeSIL);

        return pm.Potential > 0 ? pm : null;
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
}