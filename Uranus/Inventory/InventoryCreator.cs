using System;
using System.Collections.Generic;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

public class InventoryCreator
{
    public InventoryChariot Chariot { get; set; }

    public InventoryCreator(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    public bool NAVBins(List<NAVBin> bins)
    {
        if (Chariot.ReplaceFullTable(bins) <= 0) return false;
        _ = Chariot.SetTableUpdateTime(typeof(NAVBin));
        return true;
    }

    public bool NAVItems(List<NAVItem> items, DateTime dateTime)
    {
        if (Chariot.ReplaceFullTable(items) <= 0) return false;
        _ = Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
        return true;
    }

    public bool NAVUoMs(List<NAVUoM> uomList)
    {
        if (Chariot.ReplaceFullTable(uomList) <= 0) return false;
        _ = Chariot.SetTableUpdateTime(typeof(NAVUoM));
        return true;
    }

    public bool NAVStock(List<NAVStock> stock)
    {
        if (Chariot.ReplaceFullTable(stock) <= 0) return false;
        _ = Chariot.EmptyTable<BinContentsUpdate>();
        _ = Chariot.SetTableUpdateTime(typeof(NAVStock));
        _ = Chariot.SetStockUpdateTimes(stock);
        return true;
    }

    public int NAVTransferOrders(IEnumerable<NAVTransferOrder> transferOrders) => Chariot.ReplaceFullTable(transferOrders);

    public int NAVZone(List<NAVZone> zones) => Chariot.ReplaceFullTable(zones);

    public int NAVLocation(List<NAVLocation> locations) => Chariot.ReplaceFullTable(locations);

    public int NAVDivision(List<NAVDivision> divs) => Chariot.ReplaceFullTable(divs);

    public int NAVCategory(List<NAVCategory> cats) => Chariot.ReplaceFullTable(cats);

    public int NAVPlatform(List<NAVPlatform> pfs) => Chariot.ReplaceFullTable(pfs);

    public int NAVGenre(List<NAVGenre> gens) => Chariot.ReplaceFullTable(gens);

    public int Site(Site site) => Chariot.InsertOrUpdate(site);
}