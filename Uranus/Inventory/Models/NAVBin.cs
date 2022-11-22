using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Models;

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
        set => (Extension ??= new BinExtension(this)).SetBay(value);
    }

    [Ignore] public Dictionary<int, Stock> Stock { get; set; }

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
        Stock = new Dictionary<int, Stock>();
        FromMoves = new List<Move>();
        ToMoves = new List<Move>();
        MoveLines = new List<NAVMoveLine>();
    }

    public NAVBin(string code, NAVZone zone)
    {
        Code = code;
        LocationCode = zone.LocationCode;
        Zone = zone;
        ZoneCode = zone.Code;
        ZoneID = zone.ID;
        ID = $"{LocationCode}:{ZoneCode}:{Code}";
        Zone.Bins.Add(this);

        Description = string.Empty;
        NAVStock = new List<NAVStock>();
        Stock = new Dictionary<int, Stock>();
        FromMoves = new List<Move>();
        ToMoves = new List<Move>();
        MoveLines = new List<NAVMoveLine>();
    }

    public NAVBin(string id, string zoneID, string locationCode, string zoneCode, string code, string description,
        bool empty, bool assigned, int ranking, double usedCube, double maxCube, DateTime lastCcDate,
        DateTime lastPiDate, NAVZone zone, List<NAVStock> navStock, Dictionary<int, Stock> stock, List<Move> fromMoves,
        List<Move> toMoves, List<NAVMoveLine> moveLines, BinExtension extension)
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
    /*public void MergeStock()
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
    }*/

    // Takes examples of NAVStock and creates Stock versions.
    public void ConvertStock()
    {
        foreach (var stock in NAVStock.Select(ns => new Stock(ns)))
        {
            Stock.Add(stock.ItemNumber, stock);
        }
    }

    // Returns true if the given move represents the full quantity of the bin's contents.
    // Returns null if the move requires more than is available.
    public bool? IsFullQty(Move move)
    {
        // Make sure the item exists at this bin location.
        if (!Stock.TryGetValue(move.ItemNumber, out var stock)) return null;

        if (stock.Cases?.Qty < move.TakeCases ||
            stock.Packs?.Qty < move.TakePacks ||
            stock.Eaches?.Qty < move.TakeEaches)
            return null; // Too much stock trying to move.

        if (Stock.Count > 1)
            return false; // There is other stock at this location.

        // True if numbers now match exactly.
        return stock.Cases?.Qty == move.TakeCases &&
               stock.Packs?.Qty == move.TakePacks &&
               stock.Eaches?.Qty == move.TakeEaches;
    }

    public override string ToString()
    {
        return $"{Code} - {ZoneCode} - {UsedCube}m³/{MaxCube}m³";
    }

    public void AddStock(Stock newStock)
    {
        if (Stock.TryGetValue(newStock.ItemNumber, out var oldStock))
            oldStock.Merge(newStock);
        else
            Stock.Add(newStock.ItemNumber, newStock);
    }

    public void RemoveStock(Stock stock)
    {
        if (!Stock.TryGetValue(stock.ItemNumber, out var currentStock)) return;

        if (currentStock.MatchQty(stock) && !currentStock.Pending()) Stock.Remove(stock.ItemNumber);
        else currentStock.Sub(stock);
    }
}