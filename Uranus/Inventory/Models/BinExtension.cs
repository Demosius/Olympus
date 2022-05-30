using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

// Used for connecting bays to bins.
public class BinExtension
{
    [PrimaryKey, ForeignKey(typeof(NAVBin))] public string BinID { get; set; }
    [ForeignKey(typeof(Bay))] public string BayID { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Length { get; set; }

    [OneToOne(nameof(BinID), nameof(NAVBin.Extension), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin Bin { get; set; }

    [ManyToOne(nameof(BayID), nameof(Models.Bay.BayBins), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Bay Bay { get; set; }

    public BinExtension()
    {
        BinID = string.Empty;
        BayID = string.Empty;
        Bin = new NAVBin();
        Bay = new Bay();
    }

    public BinExtension(string binID, string bayID, NAVBin bin, Bay bay)
    {
        BinID = binID;
        BayID = bayID;
        Bin = bin;
        Bay = bay;
    }
}