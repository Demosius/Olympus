using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public struct Levels
{
    public int MinEaches { get; set; }
    public int MaxEaches { get; set; }
    public int MinCases { get; set; }
    public int MaxCases { get; set; }
    public float MinPct { get; set; }
    public float MaxPct { get; set; }
    public int MinUnits { get; set; }
    public int MaxUnits { get; set; }
}

public class SiteItemLevel
{
    [PrimaryKey] public string ID { get; set; } // ItemNumber:SiteName - e.g. "145556:DC"
    [ForeignKey(typeof(ItemExtension))] public int ItemNumber { get; set; }
    [ForeignKey(typeof(Site))] public string SiteName { get; set; }
    public bool Active { get; set; }
    public bool OverrideDefaults { get; set; }
    public int? MinEaches { get; set; }
    public int? MaxEaches { get; set; }
    public int? MinCases { get; set; }
    public int? MaxCases { get; set; }
    public float? MinPct { get; set; }
    public float? MaxPct { get; set; }

    [ManyToOne(nameof(SiteName), nameof(Models.Site.ItemLevels), CascadeOperations = CascadeOperation.CascadeRead)]
    public Site? Site { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.SiteLevels), CascadeOperations = CascadeOperation.CascadeRead)]
    public NAVItem? Item { get; set; }

    private Levels? levels;
    [Ignore]
    public Levels Levels
    {
        get
        {
            if (levels == null) SetLiveLevels();
            return (Levels)levels!;
        }
    }

    [Ignore] public Stock Stock => Site?.Stock.TryGetValue(ItemNumber, out var stock) ?? false ? stock : new Stock();

    [Ignore] public int BaseQty => Stock.BaseQty;
    [Ignore] public int CaseQty => Stock.CaseQty;
    [Ignore] public int EachQty => Stock.EachQty;

    [Ignore] public int TotalEachQty => Item?.Stock?.EachQty ?? 0;
    [Ignore] public int TotalCaseQty => Item?.Stock?.CaseQty ?? 0;
    [Ignore] public int TotalBaseQty => Item?.Stock?.BaseQty ?? 0;

    [Ignore] public int EachPlacePotential => Levels.MinEaches - Stock.EachQty;
    [Ignore] public int CasePlacePotential => (Levels.MinCases - Stock.CaseQty) * QtyPerCase;
    [Ignore] public int UnitPlacePotential => Levels.MinUnits - Stock.BaseQty;
    [Ignore] public int PlacePotential => EachPlacePotential + CasePlacePotential + UnitPlacePotential;

    [Ignore] public int EachTakePotential => Levels.MinEaches + Stock.EachQty;
    [Ignore] public int CaseTakePotential => (Levels.MinCases + Stock.CaseQty) * QtyPerCase;
    [Ignore] public int UnitTakePotential => Levels.MinUnits + Stock.BaseQty;
    [Ignore] public int TakePotential => EachTakePotential + CaseTakePotential + UnitTakePotential;

    [Ignore]
    public bool TargetToTake => BaseQty > Levels.MaxUnits || CaseQty > Levels.MaxCases || EachQty > Levels.MaxEaches;
    [Ignore]
    public bool TargetToPlace => BaseQty < Levels.MinUnits || CaseQty < Levels.MinCases || EachQty < Levels.MinEaches;

    [Ignore] public bool Takeable => EachQty > Levels.MaxEaches || CaseQty > Levels.MaxCases || BaseQty > Levels.MaxUnits;
    [Ignore] public bool Placeable => EachQty < Levels.MinEaches || CaseQty < Levels.MinCases || BaseQty < Levels.MinUnits;

    [Ignore] public bool EachesWithinLimits => EachQty >= Levels.MinEaches && EachQty <= Levels.MaxEaches;
    [Ignore] public bool CasesWithinLimits => CaseQty >= Levels.MinCases && CaseQty <= Levels.MaxCases;
    [Ignore] public bool UnitsWithinLimits => BaseQty >= Levels.MinUnits && BaseQty <= Levels.MaxUnits;

    private int QtyPerCase => Item?.QtyPerCase ?? 0;

    public SiteItemLevel()
    {
        SiteName = string.Empty;
        ID = ":";
    }

    public SiteItemLevel(NAVItem item, Site site)
    {
        Site = site;
        Item = item;
        ItemNumber = item.Number;
        SiteName = site.Name;
        ID = $"{ItemNumber}:{SiteName}";

        site.ItemLevels.Add(this);
        item.SiteLevels.Add(this);
    }

    /// <summary>
    /// Set the Levels struct within the class based on general target
    /// levels compared to live stock.
    ///
    /// levels should NOT be null by the time this function is done.
    /// </summary>
    public void SetLiveLevels()
    {
        if (Item is null || Site is null)
        {
            levels = new Levels();
            return;
        }

        var newLevels = new Levels
        {
            MinEaches = OverrideDefaults ? MinEaches ?? 0 : Site.MinEaches ?? 0,
            MaxEaches = OverrideDefaults
                ? MaxEaches ?? TotalEachQty
                : Site.MaxEaches ?? TotalEachQty,
            MinCases = OverrideDefaults ? MinCases ?? 0 : Site.MinCases ?? 0,
            MaxCases = OverrideDefaults
                ? MaxCases ?? TotalCaseQty
                : Site.MaxCases ?? TotalCaseQty,
            MinPct = OverrideDefaults ? MinPct ?? 0 : Site.MinPct ?? 0,
            MaxPct = OverrideDefaults ? MaxPct ?? 1 : Site.MaxPct ?? 1
        };

        levels = newLevels;

        SetPctLevels();
    }

    /// <summary>
    /// Based on stock levels, convert the percentage to unit targets, and override the min/max units where appropriate.
    /// Then use the units to further limit the eaches and cases - if necessary.
    /// </summary>
    private void SetPctLevels()
    {
        if (levels is null) return;

        var newLevels = (Levels)levels;

        var minUnits = newLevels.MinEaches + newLevels.MinCases * QtyPerCase;
        var maxUnits = newLevels.MaxEaches + newLevels.MaxCases * QtyPerCase;

        var minPct = (int)(newLevels.MinPct * TotalBaseQty);
        var maxPct = (int)(newLevels.MaxPct * TotalBaseQty);

        if (minPct > minUnits) minUnits = minPct;
        if (maxPct < maxUnits) maxUnits = maxPct;

        newLevels.MinUnits = minUnits;
        newLevels.MaxUnits = maxUnits;

        // Now limit each & case based on units.
        if (newLevels.MaxEaches > newLevels.MaxUnits) newLevels.MaxEaches = newLevels.MaxUnits;
        if (newLevels.MaxCases * QtyPerCase > newLevels.MaxUnits) newLevels.MaxCases = newLevels.MaxUnits / QtyPerCase;

        levels = newLevels;
    }

    private bool EachBreak(int eachesAfter) => EachesWithinLimits && (eachesAfter < Levels.MinEaches || eachesAfter > Levels.MaxEaches);
    private bool CaseBreak(int casesAfter) => CasesWithinLimits && (casesAfter < Levels.MinCases || casesAfter > Levels.MaxCases);
    private bool UnitBreak(int unitsAfter) => UnitsWithinLimits && (unitsAfter < Levels.MinUnits || unitsAfter > Levels.MaxUnits);

    public bool BrokenByMove(int eaches, int cases, int units) => EachBreak(EachQty + eaches) || CaseBreak(CaseQty + cases) || UnitBreak(BaseQty + units);

    public override string ToString()
    {
        return Active
            ? OverrideDefaults
                ? "Custom"
                : "✔"
            : "✘";
    }
}