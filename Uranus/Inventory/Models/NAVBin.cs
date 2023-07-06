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
    [OneToMany(nameof(PickLine.CartonID), nameof(PickLine.BatchTOLine), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickLine> PickLines { get; set; }

    [OneToOne(nameof(ID), nameof(BinExtension.BinID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public BinExtension? Extension { get; set; }

    [Ignore] public BinExtension? Dimensions => Extension;

    [Ignore] public List<MixedCarton> MixedCartons { get; set; }

    [Ignore]
    public Bay? Bay
    {
        get => Extension?.Bay;
        set => (Extension ??= new BinExtension(this)).SetBay(value);
    }

    [Ignore]
    public string CheckDigits
    {
        get => Extension?.CheckDigits ?? string.Empty;
        set => (Extension ??= new BinExtension(this)).CheckDigits = value;
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
        PickLines = new List<PickLine>();
        MixedCartons = new List<MixedCarton>();
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
        PickLines = new List<PickLine>();
        MixedCartons = new List<MixedCarton>();
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
        Stock = Models.Stock.FromNAVStock(NAVStock).ToDictionary(s => s.ItemNumber, s => s);
    }

    /// <summary>
    /// Converts bin contents stock to Mixed Carton stock, if applicable.
    /// </summary>
    /// <param name="mcIDTool">MixedCarton Identification tool, used to match and identify mixed cartons.</param>
    /// <param name="createNewMixedCartons">If true will create a new Mixed carton template if one is apparent in data.</param>
    /// <returns></returns>
    public bool MixedCartonStockConversion(MixedCartonIdentificationTool mcIDTool, bool createNewMixedCartons = false)
    {
        // Get potentially relevant stock lines.
        var createdMC = false;
        var invalidStock = new List<Stock>();
        var allStock = Stock.Values.ToList();
        var validStock = createNewMixedCartons ? allStock : mcIDTool.GetValidStock(allStock, out invalidStock);

        var mixedCarton = mcIDTool.GetMixedCartonFromStock(validStock, createNewMixedCartons);

        while (mixedCarton is not null)
        {
            mixedCarton.Bins.Add(this);
            MixedCartons.Add(mixedCarton);

            createdMC = true;
            var mcStock = mixedCarton.GetValidStock(ref validStock);

            invalidStock.Add(new MixedCartonStock(mixedCarton, ref mcStock));

            if (mcStock.Any()) validStock.AddRange(mcStock);

            mixedCarton = mcIDTool.GetMixedCartonFromStock(validStock, createNewMixedCartons);
        }

        invalidStock.AddRange(validStock);

        Stock = invalidStock.ToDictionary(s => s.ItemNumber, s => s);

        return createdMC;
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
            newStock.Merged = oldStock.Merge(newStock);
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