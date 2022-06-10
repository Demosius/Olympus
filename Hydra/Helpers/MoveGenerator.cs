using System;
using Hydra.ViewModels.Controls;
using System.Collections.Generic;
using System.Linq;
using Uranus.Inventory.Models;

namespace Hydra.Helpers;

public struct Levels
{
    public int MinUnits { get; set; }
    public int MaxUnits { get; set; }
    public int MinCases { get; set; }
    public int MaxCases { get; set; }
    public float MinPct { get; set; }
    public float MaxPct { get; set; }
}

public static class MoveGenerator
{
    public static IEnumerable<Move> GenerateSiteMoves(HydraDataSet dataSet, IEnumerable<string> fromSites, IEnumerable<string> toSites)
    {
        var returnList = new List<Move>();

        // Set Site Objects
        var takeSites = new List<Site>();
        var placeSites = new List<Site>();

        var levelDict = new Dictionary<(Site, NAVItem), Levels>();

        foreach (var siteName in fromSites)
            if (dataSet.Sites.TryGetValue(siteName, out var site)) takeSites.Add(site);
        foreach (var siteName in toSites)
            if (dataSet.Sites.TryGetValue(siteName, out var site)) placeSites.Add(site);

        // Go through and set levels for each relevant item.
        foreach (var site in takeSites.Concat(placeSites).Distinct())
        {
            foreach (var (itemNumber, stock) in site.Stock)
            {
                // Only stop to check if item is set up for hydra.
                if (!(stock.Item?.SiteLevelTarget ?? false)) continue;

                // Get siteItemLevels. Skip if not available.
                if (!dataSet.SiteItemLevels.TryGetValue((site.Name, itemNumber), out var sil)) continue;

                // Skip if item is not set up for 
                if (!sil.Active) continue;

                // Set levels.
                var levels = GetLevels(sil, site);
                SetPctLevels(levels, stock);

                // Check if movements are potentially required.
                var take = takeSites.Contains(site);
                var place = placeSites.Contains(site);

                var baseQty = stock.BaseQty;
                var cases = stock.Cases?.Qty ?? 0;

                if ((take && (baseQty > levels.MaxUnits || cases > levels.MaxCases)) ||
                    (place && (baseQty < levels.MinUnits || cases < levels.MinCases)))
                    levelDict.Add((site, stock.Item), levels);


            }
        }

        return returnList;
    }

    public static Move? GenerateMove(NAVBin fromBin, NAVBin toBin, NAVItem item,
        int? caseQty = null, int? packQty = null, int? eachQty = null)
    {
        // Set qty as appropriate, based on available qty.
        if (!fromBin.Stock.TryGetValue(item.Number, out var stock)) return null;

        if (caseQty is null || caseQty > stock.Cases?.AvailableQty) caseQty = stock.Cases?.AvailableQty ?? 0;
        if (packQty is null || packQty > stock.Packs?.AvailableQty) packQty = stock.Packs?.AvailableQty ?? 0;
        if (eachQty is null || eachQty > stock.Eaches?.AvailableQty) eachQty = stock.Eaches?.AvailableQty ?? 0;

        return new Move(fromBin, toBin, item, (int)caseQty, (int)packQty, (int)eachQty);
    }

    public static Levels GetLevels(SiteItemLevel sil, Site site)
    {
        var useSil = sil.OverrideDefaults;
        var levels = new Levels
        {
            MinUnits = useSil ? sil.MinUnits ?? 0 : site.MinUnits ?? 0,
            MaxUnits = useSil ? sil.MaxUnits ?? int.MaxValue : site.MaxUnits ?? int.MaxValue,
            MinCases = useSil ? sil.MinCases ?? 0 : site.MinCases ?? 0,
            MaxCases = useSil ? sil.MaxCases ?? int.MaxValue : site.MaxCases ?? int.MaxValue,
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
        var minUnits = levels.MinUnits;
        var maxUnits = levels.MaxUnits;

        var baseQty = stock.Item?.Stock?.BaseQty ?? 0;

        var minPct = (int)(levels.MinPct * baseQty);
        var maxPct = (int)(levels.MaxPct * baseQty);

        if (minPct > minUnits) minUnits = minPct;
        if (maxPct < maxUnits) maxUnits = maxPct;

        levels.MinUnits = minUnits;
        levels.MaxUnits = maxUnits;
    }
}