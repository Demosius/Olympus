using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uranus.Inventory.Model;

[Table("BinList")]
public class NAVBin
{
    [PrimaryKey, ForeignKey(typeof(BinExtension))] // Combination of ZoneID and Code (e.g. 9600:PR:PR18 058)
    public string ID { get; set; }
    [ForeignKey(typeof(NAVZone))] // Combination of LocationCode and ZoneCode (e.g. 9600:PR)
    public string ZoneID { get; set; }
    public string LocationCode { get; set; }
    public string ZoneCode { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Empty { get; set; }
    public bool Assigned { get; set; }
    public int Ranking { get; set; }
    public double UsedCube { get; set; }
    public double MaxCube { get; set; }
    public DateTime LastCcDate { get; set; }
    public DateTime LastPiDate { get; set; }

    [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone Zone { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> NAVStock { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Stock> Stock { get; set; }
    [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public BinExtension Extension { get; set; }

    [Ignore]
    public BinExtension Dimensions => Extension;

    [Ignore]
    public Bay Bay
    {
        get => Extension?.Bay; 
        set => Extension.Bay = value;
    }
    [Ignore]
    public EAccessLevel? AccessLevel => Zone?.AccessLevel;

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
        for (var i = Stock.Count -1; i > 0; --i)
        {
            for (var j = i-1; j>=0; --j)
            {
                if (Stock[j].Merge(Stock[i]))
                {
                    Stock.RemoveAt(i);
                    break;
                }
            }
        }
    }

    // Takes examples of NAVStock and creates Stock versions.
    public void ConvertStock()
    {
        // Make sure both lists are not null.
        NAVStock ??= new List<NAVStock>();
        Stock ??= new List<Stock>();

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
        if (theStock.Cases.Qty < move.TakeCases || 
            theStock.Packs.Qty < move.TakePacks || 
            theStock.Eaches.Qty < move.TakeEaches)
            return null; // Too much stock trying to move.
        if (itemStock.Count != Stock.Count) 
            return false; // There is other stock at this location.
        return theStock.Cases.Qty == move.TakeCases &&
               theStock.Packs.Qty == move.TakePacks &&
               theStock.Eaches.Qty == move.TakeEaches;
    }

    public override string ToString()
    {
        return $"{Code} - {ZoneCode} - {UsedCube}m³/{MaxCube}m³"; 
    }
}