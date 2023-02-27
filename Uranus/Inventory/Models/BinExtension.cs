using System;
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
    public string CheckDigits { get; set; }

    [OneToOne(nameof(BinID), nameof(NAVBin.Extension), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin? Bin { get; set; }

    [ManyToOne(nameof(BayID), nameof(Models.Bay.BayBins), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Bay? Bay { get; set; }


    public BinExtension()
    {
        BinID = string.Empty;
        BayID = string.Empty;
        CheckDigits = string.Empty;
    }

    public BinExtension(NAVBin bin, Bay bay)
    {
        BinID = bin.ID;
        BayID = bay.ID;
        Bin = bin;
        Bay = bay;
        CheckDigits = string.Empty;
    }

    public BinExtension(NAVBin bin)
    {
        Bin = bin;
        BinID = bin.ID;
        BayID = string.Empty;
        CheckDigits = string.Empty;     
    }

    public void SetBay(Bay? bay)
    {
        Bay = bay;
        BayID = bay?.ID ?? string.Empty;
    }
}