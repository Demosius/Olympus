﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Model;

public class Site
{
    [PrimaryKey] public string Name { get; set; }
    public int? MinUnits { get; set; }
    public int? MaxUnits { get; set; }
    public int? MinCases { get; set; }
    public int? MaxCases { get; set; }
    public float? MinPct { get; set; }
    public float? MaxPct { get; set; }

    [OneToMany(nameof(ZoneExtension.SiteName), nameof(ZoneExtension.Site),CascadeOperations = CascadeOperation.CascadeRead)]
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
    
    public Site()
    {
        Name = string.Empty;
        ZoneExtensions = new List<ZoneExtension>();
        ItemLevels = new List<SiteItemLevel>();
    }

    public override string ToString()
    {
        return Name;
    }
}