using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class HydraDataSet
{
    public Dictionary<int, NAVItem> Items { get; set; }
    public Dictionary<string, Site> Sites { get; set; }
    public Dictionary<string, NAVZone> Zones { get; set; }
    public Dictionary<string, NAVBin> Bins { get; set; }
    public Dictionary<(string, int), SiteItemLevel> SiteItemLevels { get; set; }
    public List<NAVStock> NAVStock { get; set; }
    public List<NAVUoM> UoMs { get; set; }
    public List<Stock> Stock { get; set; }

    public HydraDataSet()
    {
        Items = new Dictionary<int, NAVItem>();
        Sites = new Dictionary<string, Site>();
        Zones = new Dictionary<string, NAVZone>();
        Bins = new Dictionary<string, NAVBin>();
        SiteItemLevels = new Dictionary<(string, int), SiteItemLevel>();
        NAVStock = new List<NAVStock>();
        UoMs = new List<NAVUoM>();
        Stock = new List<Stock>();
    }

    public HydraDataSet(IEnumerable<NAVItem> items, IEnumerable<Site> sites, IEnumerable<NAVZone> zones,
        IEnumerable<SiteItemLevel> siteItemLevels, IEnumerable<NAVBin> bins, IEnumerable<NAVStock> stock,
        IEnumerable<NAVUoM> uomList)
    {
        Items = items.ToDictionary(i => i.Number, i => i);
        Sites = sites.ToDictionary(s => s.Name, s => s);
        Zones = zones.ToDictionary(z => z.ID, z => z);
        SiteItemLevels = siteItemLevels.ToDictionary(l => (l.SiteName, l.ItemNumber), l => l);
        NAVStock = stock.ToList();
        Bins = bins.ToDictionary(b => b.Code, b => b);
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

    public void SetFromUoMs()
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

    public void SetFromBins()
    {
        foreach (var (_, bin) in Bins)
        {
            if (Zones.TryGetValue(bin.ZoneCode, out var zone))
            {
                zone.Bins.Add(bin);
                bin.Zone = zone;
            }
        }
    }

    public void SetFromStock()
    {
        foreach (var stock in NAVStock)
        {
            if (Items.TryGetValue(stock.ItemNumber, out var item))
            {
                item.NAVStock.Add(stock);
                stock.Item = item;
            }

            if (Zones.TryGetValue(stock.ZoneID, out var zone))
            {
                zone.Stock.Add(stock);
                stock.Zone = zone;
            }

            if (Bins.TryGetValue(stock.BinCode, out var bin))
            {
                bin.NAVStock.Add(stock);
                stock.Bin = bin;
            }

            Stock.Add(new Stock(stock));
        }
    }
}