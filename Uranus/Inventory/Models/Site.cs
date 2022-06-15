using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class Site
{
    [PrimaryKey] public string Name { get; set; }
    public int? MinEaches { get; set; }
    public int? MaxEaches { get; set; }
    public int? MinCases { get; set; }
    public int? MaxCases { get; set; }
    public float? MinPct { get; set; }
    public float? MaxPct { get; set; }

    [OneToMany(nameof(ZoneExtension.SiteName), nameof(ZoneExtension.Site), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<ZoneExtension> ZoneExtensions { get; set; }

    [OneToMany(nameof(SiteItemLevel.SiteName), nameof(SiteItemLevel.Site), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<SiteItemLevel> ItemLevels { get; set; }

    private List<NAVZone>? zones;
    [Ignore]
    public List<NAVZone> Zones
    {
        get => zones ??= ZoneExtensions.Select(e => e.Zone ??= new NAVZone(e.ZoneID)).ToList();
        set => zones = value;
    }

    [Ignore] public Dictionary<int, Stock> Stock { get; set; }

    // For site specific loc/zone/bin.
    [Ignore] public NAVLocation Location { get; set; }
    [Ignore] public NAVZone Zone { get; set; }
    [Ignore] public NAVBin Bin { get; set; }

    public Site()
    {
        Name = string.Empty;
        ZoneExtensions = new List<ZoneExtension>();
        ItemLevels = new List<SiteItemLevel>();
        Stock = new Dictionary<int, Stock>();
    }

    public Site(string name)
    {
        Name = name;
        ZoneExtensions = new List<ZoneExtension>();
        ItemLevels = new List<SiteItemLevel>();
        Stock = new Dictionary<int, Stock>();
    }

    public void AddZone(NAVZone newZone)
    {
        (zones ??= new List<NAVZone>()).Add(newZone);
        ZoneExtensions.Add(newZone.Extension ??= new ZoneExtension(newZone));
        newZone.Site = this;
    }

    public void RemoveZone(NAVZone oldZone)
    {
        zones?.Remove(oldZone);
        ZoneExtensions.Remove(oldZone.Extension ??= new ZoneExtension(oldZone));
        oldZone.Site = null;
    }

    public override string ToString()
    {
        return Name;
    }

    public void AddStock(Stock newStock)
    {
        if (Stock.TryGetValue(newStock.ItemNumber, out var oldStock))
            oldStock.Add(newStock);
        else
            Stock.Add(newStock.ItemNumber, newStock.Copy());
    }

    public void RemoveStock(Stock stock)
    {
        if (!Stock.TryGetValue(stock.ItemNumber, out var currentStock)) return;

        currentStock.Sub(stock);
        if (currentStock.IsEmpty()) Stock.Remove(stock.ItemNumber);
    }
}