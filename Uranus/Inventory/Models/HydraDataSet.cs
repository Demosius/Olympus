using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Uranus.Inventory.Models;

public class HydraDataSet
{
    public Dictionary<int, NAVItem> Items { get; set; }
    public Dictionary<string, Site> Sites { get; set; }
    public Dictionary<string, NAVLocation> Locations { get; set; }
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
        Locations = new Dictionary<string, NAVLocation>();
        Zones = new Dictionary<string, NAVZone>();
        Bins = new Dictionary<string, NAVBin>();
        SiteItemLevels = new Dictionary<(string, int), SiteItemLevel>();
        NAVStock = new List<NAVStock>();
        UoMs = new List<NAVUoM>();
        Stock = new List<Stock>();
    }

    public HydraDataSet(IEnumerable<NAVItem> items, IEnumerable<Site> sites, IEnumerable<NAVZone> zones,
        IEnumerable<SiteItemLevel> siteItemLevels, IEnumerable<NAVBin> bins, IEnumerable<NAVStock> stock,
        IEnumerable<NAVUoM> uomList, IEnumerable<NAVLocation> locations)
    {
        Items = items.ToDictionary(i => i.Number, i => i);
        Sites = sites.ToDictionary(s => s.Name, s => s);
        Locations = locations.ToDictionary(l => l.Code, l => l);
        Zones = zones.ToDictionary(z => z.ID, z => z);
        SiteItemLevels = siteItemLevels.ToDictionary(l => (l.SiteName, l.ItemNumber), l => l);
        NAVStock = stock.ToList();
        Bins = bins.ToDictionary(b => b.ID, b => b);
        UoMs = uomList.ToList();
        Stock = new List<Stock>();

        FillSites();

        SetRelationships();
    }

    public void SetRelationships()
    {
        SetFromZones();
        SetFromUoMs();
        SetFromBins();
        SetFromStock();
    }

    private void FillSites()
    {
        // Generate Site Specific Zones/Bays/Bins
        foreach (var (name, site) in Sites)
        {
            var locationCode = $"{name}SiteLocation";
            var zoneCode = $"{name}SiteZone";
            var binCode = $"{name}SiteBin";

            var location = new NAVLocation(locationCode, locationCode);
            var zone = new NAVZone(zoneCode, location);
            var bin = new NAVBin(binCode, zone);

            site.Zones.Add(zone);
            zone.Site = site;
            zone.SiteName = name;

            Zones.Add(zone.ID, zone);
            Locations.Add(location.Code, location);
            Bins.Add(bin.ID, bin);

            site.Location = location;
            site.Zone = zone;
            site.Bin = bin;
        }
    }

    private void SetFromZones()
    {
        foreach (var (_, zone) in Zones)
        {
            if (Locations.TryGetValue(zone.LocationCode, out var location))
            {
                location.Zones.Add(zone);
                zone.Location = location;
            }
        }
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
            if (Zones.TryGetValue(bin.ZoneID, out var zone))
            {
                zone.Bins.Add(bin);
                bin.Zone = zone;
            }


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
            Stock.Add(stock);

            stock.AddStock();

        }
    }
}