using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Model;

[Table("BinList")]
public class NAVBin
{
    // Combination of ZoneID and Code (e.g. 9600:PR:PR18 058)
    [PrimaryKey, ForeignKey(typeof(BinExtension))] public string ID { get; set; }
    // Combination of LocationCode and ZoneCode (e.g. 9600:PR)
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
    public string LocationCode { get; set; }
    public string ZoneCode { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Empty { get; set; }
    public bool Assigned { get; set; }
    public int Ranking { get; set; }
    public double UsedCube { get; set; }
    public double MaxCube { get; set; }
    public DateTime LastCCDate { get; set; }
    public DateTime LastPIDate { get; set; }

    [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone? Zone { get; set; }

    [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> NAVStock { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Stock> Stock { get; set; }
    [OneToMany(nameof(Move.TakeBinID), nameof(Move.TakeBin), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Move> FromMoves { get; set; }
    [OneToMany(nameof(Move.PlaceBinID), nameof(Move.PlaceBin), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Move> ToMoves { get; set; }
    [OneToMany(nameof(NAVMoveLine.BinID), nameof(NAVMoveLine.Bin), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVMoveLine> MoveLines { get; set; }

    [OneToOne(nameof(ID), nameof(BinExtension.BinID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public BinExtension? Extension { get; set; }

    [Ignore] public BinExtension? Dimensions => Extension;

    [Ignore]
    public Bay? Bay
    {
        get => Extension?.Bay;
        set
        {
            if (value is not null)
                (Extension ??= new BinExtension(ID, value.ID, this, value)).Bay = value;
        }
    }

    [Ignore] public EAccessLevel? AccessLevel => Zone?.AccessLevel;


    public NAVBin()
    {
        ID = string.Empty;
        ZoneID = string.Empty;
        LocationCode = string.Empty;
        ZoneCode = string.Empty;
        Code = string.Empty;
        Description = string.Empty;
        Empty = true;
        Assigned = false;
        LastCCDate = DateTime.MinValue;
        LastPIDate = DateTime.MinValue;
        NAVStock = new List<NAVStock>();
        Stock = new List<Stock>();
        FromMoves = new List<Move>();
        ToMoves = new List<Move>();
        MoveLines = new List<NAVMoveLine>();
    }

    public NAVBin(string id, string zoneID, string locationCode, string zoneCode, string code, string description, bool empty, bool assigned, int ranking, double usedCube, double maxCube, DateTime lastCcDate, DateTime lastPiDate, NAVZone zone, List<NAVStock> navStock, List<Stock> stock, List<Move> fromMoves, List<Move> toMoves, List<NAVMoveLine> moveLines, BinExtension extension)
    {
        ID = id;
        ZoneID = zoneID;
        LocationCode = locationCode;
        ZoneCode = zoneCode;
        Code = code;
        Description = description;
        Empty = empty;
        Assigned = assigned;
        Ranking = ranking;
        UsedCube = usedCube;
        MaxCube = maxCube;
        LastCCDate = lastCcDate;
        LastPIDate = lastPiDate;
        Zone = zone;
        NAVStock = navStock;
        Stock = stock;
        FromMoves = fromMoves;
        ToMoves = toMoves;
        MoveLines = moveLines;
        Extension = extension;
    }

    /// <summary>
    /// Given the current Code, Zone, and Location, adjusts the current ID accordingly.
    /// </summary>
    public void SetID()
    {
        ID = $"{LocationCode}:{ZoneCode}:{Code}";
    }

    // Merges matching items in Stock (NOT NAVStock)
    public void MergeStock()
    {
        // Try to merge every stock item with every other.
        // If merge is successful, remove the merged stock from the list.
        // Use backwards list, and merge from the i, so we remove from the end of the list safely.
        for (var i = Stock.Count - 1; i > 0; --i)
        {
            for (var j = i - 1; j >= 0; --j)
            {
                if (!Stock[j].Merge(Stock[i])) continue;

                Stock.RemoveAt(i);
                break;
            }
        }
    }

    // Takes examples of NAVStock and creates Stock versions.
    public void ConvertStock()
    {
        foreach (var stock in NAVStock.Select(ns => new Stock(ns)))
        {
            Stock.Add(stock);
        }
    }

    // Returns true if the given move represents the full quantity of the bin's contents.
    // Returns null if the move requires more than is available.
    public bool? IsFullQty(Move move)
    {
        var itemStock = Stock.Where(stock => stock.Item == move.Item).ToList();
        if (itemStock.Count != 1)
            return null; // Item is not at this bin location OR there is multiple instances of item stock - which should not occur.
        var theStock = itemStock[0];
        if (theStock.Cases?.Qty < move.TakeCases ||
            theStock.Packs?.Qty < move.TakePacks ||
            theStock.Eaches?.Qty < move.TakeEaches)
            return null; // Too much stock trying to move.
        if (itemStock.Count != Stock.Count)
            return false; // There is other stock at this location.
        return theStock.Cases?.Qty == move.TakeCases &&
               theStock.Packs?.Qty == move.TakePacks &&
               theStock.Eaches?.Qty == move.TakeEaches;
    }

    public override string ToString()
    {
        return $"{Code} - {ZoneCode} - {UsedCube}m³/{MaxCube}m³";
    }
}