using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

public class Bay
{
    [PrimaryKey] public string ID { get; set; } // Bay name. Called ID for consistency.

    [OneToMany(nameof(BinExtension.BayID), nameof(BinExtension.Bay), CascadeOperations = CascadeOperation.All)]
    public List<BinExtension> BayBins { get; set; }

    [ManyToMany(typeof(BayZone), nameof(BayZone.BayID), nameof(NAVZone.Bays), CascadeOperations = CascadeOperation.All)]
    public List<NAVZone> Zones { get; set; }

    // Does not hold bins directly.
    // Instead uses the bin extension references to get bins.
    // [Ignore] public List<NAVBin> Bins => BayBins.Select(bb => bb.Bin).ToList();

    private List<NAVBin>? bins;
    [Ignore]
    public List<NAVBin> Bins
    {
        get => bins ??= BayBins.Select(e => e.Bin!).ToList();
        set => bins = value;
    }

    [Ignore] public Dictionary<int, Stock> Stock { get; set; }

    public Bay()
    {
        ID = string.Empty;
        BayBins = new List<BinExtension>();
        Zones = new List<NAVZone>();
        Stock = new Dictionary<int, Stock>();
    }

    public Bay(string id)
    {
        ID = id;
        BayBins = new List<BinExtension>();
        Zones = new List<NAVZone>();
        Stock = new Dictionary<int, Stock>();
    }

    public Bay(string id, List<BinExtension> bayBins, List<NAVZone> zones)
    {
        ID = id;
        BayBins = bayBins;
        Zones = zones;
        Stock = new Dictionary<int, Stock>();
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