﻿using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model;

public class SiteItemLevel
{
    [ForeignKey(typeof(ItemExtension))] public int ItemNumber { get; set; }
    [ForeignKey(typeof(Site))] public string SiteName { get; set; }
    public bool Active { get; set; }
    public bool OverrideDefaults { get; set; }
    public int? MinUnits { get; set; }
    public int? MaxUnits { get; set; }
    public int? MinCases { get; set; }
    public int? MaxCases { get; set; }
    public float? MinPct { get; set; }
    public float? MaxPct { get; set; }

    [ManyToOne(nameof(SiteName), nameof(Model.Site.ItemLevels), CascadeOperations = CascadeOperation.CascadeRead)]
    public Site? Site { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.SiteLevels), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVItem? Item { get; set; }
    
    public SiteItemLevel()
    {
        SiteName = string.Empty;
    }


}