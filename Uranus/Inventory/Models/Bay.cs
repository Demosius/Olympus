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
    [Ignore] public List<NAVBin> Bins => BayBins.Select(bb => bb.Bin).ToList();

    public Bay()
    {
        ID = string.Empty;
        BayBins = new List<BinExtension>();
        Zones = new List<NAVZone>();
    }

    public Bay(string id, List<BinExtension> bayBins, List<NAVZone> zones)
    {
        ID = id;
        BayBins = bayBins;
        Zones = zones;
    }

}