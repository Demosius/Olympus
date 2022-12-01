using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class BasicStockDataSet
{
    public Dictionary<int, NAVItem> Items { get; set; }
    public Dictionary<string, NAVZone> Zones { get; set; }
    public Dictionary<string, NAVBin> Bins { get; set; }
    public List<NAVStock> NAVStock { get; set; }
    public List<NAVUoM> UoMs { get; set; }
    public List<Stock> Stock { get; set; }


    public BasicStockDataSet(IEnumerable<NAVItem> items, IEnumerable<NAVZone> zones, IEnumerable<NAVBin> bins,
        IEnumerable<NAVStock> stock, IEnumerable<NAVUoM> uomList)
    {
        Items = items.ToDictionary(i => i.Number, i => i);
        Zones = zones.ToDictionary(z => z.ID, z => z);
        NAVStock = stock.ToList();
        Bins = bins.ToDictionary(b => b.ID, b => b);
        UoMs = uomList.ToList();
        Stock = new List<Stock>();

        SetRelationships();
    }

    public void SetRelationships()
    {
        SetFromUoMs();
        SetFromBins();
        SetFromStock();
    }

    private void SetFromUoMs()
    {
        foreach (var navUoM in UoMs)
        {
            if (Items.TryGetValue(navUoM.ItemNumber, out var item))
            {
                navUoM.Item = item;
                switch (navUoM.UoM)
                {
                    case EUoM.CASE:
                        item.Case = navUoM;
                        break;
                    case EUoM.EACH:
                        item.Each = navUoM;
                        break;
                    case EUoM.PACK:
                        item.Pack = navUoM;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(navUoM.UoM));
                }
            }
        }
    }

    private void SetFromBins()
    {
        foreach (var (_, bin) in Bins)
        {
            if (!Zones.TryGetValue(bin.ZoneID, out var zone)) continue;
            zone.Bins.Add(bin);
            bin.Zone = zone;
        }
    }

    private void SetFromStock()
    {
        foreach (var navStock in NAVStock)
        {
            if (Items.TryGetValue(navStock.ItemNumber, out var item))
            {
                item.NAVStock.Add(navStock);
                navStock.Item = item;
            }

            if (Zones.TryGetValue(navStock.ZoneID, out var zone))
            {
                zone.NAVStock.Add(navStock);
                navStock.Zone = zone;
            }

            if (Bins.TryGetValue(navStock.BinID, out var bin))
            {
                bin.NAVStock.Add(navStock);
                navStock.Bin = bin;
            }

            var stock = new Stock(navStock);
            stock.AddStock();

            if (!stock.Merged) Stock.Add(stock);
        }
    }
}