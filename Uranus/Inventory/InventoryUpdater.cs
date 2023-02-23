using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

public class InventoryUpdater
{
    public InventoryChariot Chariot { get; set; }

    public InventoryUpdater(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    public int NAVBins(List<NAVBin> bins)
    {
        var count = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            count = Chariot.UpdateTable(bins);
            if (count > 0) Chariot.SetTableUpdateTime(typeof(NAVBin));
        });
        return count;
    }

    public int NAVItems(IEnumerable<NAVItem> items, DateTime dateTime)
    {
        var count = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            var navItems = items as NAVItem[] ?? items.ToArray();
            count = Chariot.UpdateTable(navItems);
            count += Chariot.UpdateTable(navItems.Select(i => i.Extension));
            if (count > 0) Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
        });
        return count;
    }

    public bool NAVUoMs(List<NAVUoM> uomList)
    {
        if (uomList.Count == 0) return false;
        // Remove previous data with relevant UoMCode 
        // (Expect for ease/speed updates will happen one UoMCode at a time)
        Chariot.UoMCodeDelete(uomList.Select(s => s.Code).Distinct().ToList());

        if (Chariot.InsertIntoTable(uomList) == 0) return false;

        _ = Chariot.SetTableUpdateTime(typeof(NAVUoM));
        return true;
    }

    public bool NAVStock(List<NAVStock> stock)
    {
        if (stock.Count == 0) return false;
        // Remove from stock table anything with zones equal to what is being put in.
        Chariot.StockZoneDeletes(stock.Select(s => s.ZoneID).Distinct().ToList());
        if (Chariot.InsertIntoTable(stock) <= 0) return false;

        _ = Chariot.SetTableUpdateTime(typeof(NAVStock));
        _ = Chariot.SetStockUpdateTimes(stock);
        return true;
    }

    public int NAVZones(List<NAVZone> zones) => Chariot.UpdateTable(zones);

    public int Zones(IEnumerable<NAVZone> zones)
    {
        var count = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            var list = zones as NAVZone[] ?? zones.ToArray();
            count += Chariot.UpdateTable(list);
            count += Chariot.UpdateTable(list.Select(z => z.Extension));
        });
        return count;
    }

    public int NAVLocation(List<NAVLocation> locations) => Chariot.UpdateTable(locations);

    public int NAVDivision(List<NAVDivision> divs) => Chariot.UpdateTable(divs);

    public int NAVCategory(List<NAVCategory> cats) => Chariot.UpdateTable(cats);

    public int NAVPlatform(List<NAVPlatform> pfs) => Chariot.UpdateTable(pfs);

    public int NAVGenre(List<NAVGenre> gens) => Chariot.UpdateTable(gens);

    /// <summary>
    /// Replaces the current data with all new data, and updates connected zoneExtensions as applicable.
    /// </summary>
    /// <param name="newZones"></param>
    /// <returns></returns>
    public int ReplaceZones(IEnumerable<NAVZone> newZones)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            var list = newZones as NAVZone[] ?? newZones.ToArray();
            lines += Chariot.UpdateTable(list.Select(z => z.Extension));
            lines += Chariot.ReplaceFullTable(list);
        });
        return lines;
    }

    public int ReplaceMixedCartons(IEnumerable<MixedCarton> mixedCartons)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            var mcList = mixedCartons.ToList();
            var mcItems = mcList.SelectMany(mc => mc.Items);
            lines += Chariot.ReplaceFullTable(mcItems);
            lines += Chariot.ReplaceFullTable(mcList);
        });
        return lines;
    }

    public int Sites(IEnumerable<Site> sites)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            var list = sites as Site[] ?? sites.ToArray();
            lines += Chariot.UpdateTable(list);
            lines += Chariot.UpdateTable(list.SelectMany(s => s.Zones));
            lines += Chariot.UpdateTable(list.SelectMany(s => s.Zones.Select(z => z.Extension)));
        });
        return lines;
    }

    public int Sites(IEnumerable<Site> sites, IEnumerable<NAVZone> zones)
    {
        var lines = 0;
        Chariot.Database?.RunInTransaction(() =>
        {
            var zoneArray = zones as NAVZone[] ?? zones.ToArray();
            lines += Chariot.UpdateTable(sites);
            lines += Chariot.UpdateTable(zoneArray);
            lines += Chariot.UpdateTable(zoneArray.Select(z => z.Extension));
        });
        return lines;
    }

    public int SiteItemLevels(IEnumerable<SiteItemLevel> siteItemLevels) => Chariot.UpdateTable(siteItemLevels);

    public int Site(Site site) => Chariot.Update(site);

}