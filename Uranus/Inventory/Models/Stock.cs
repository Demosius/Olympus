using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Models;

[Table("RealStock")]
public class Stock
{
    [PrimaryKey] public string ID { get; set; } // Combination of BinID and ItemNumber (e.g. 9600:PR:PR18 058:271284)
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }    // Combination of LocationCode and ZoneCode (e.g. 9600:PR)
    [ForeignKey(typeof(NAVBin))] public string BinID { get; set; } // Combination of LocationCode, ZoneCode, and BinCode (e.g. 9600:PR:PR18 058)
    [ForeignKey(typeof(NAVItem))] public int ItemNumber { get; set; }

    [ForeignKey(typeof(SubStock))] public string CaseID { get; set; }
    [ForeignKey(typeof(SubStock))] public string PackID { get; set; }
    [ForeignKey(typeof(SubStock))] public string EachID { get; set; }

    [OneToOne(nameof(CaseID), nameof(SubStock.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public SubStock? Cases { get; set; }
    [OneToOne(nameof(PackID), nameof(SubStock.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public SubStock? Packs { get; set; }
    [OneToOne(nameof(EachID), nameof(SubStock.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public SubStock? Eaches { get; set; }

    [ManyToOne(nameof(BinID), nameof(NAVBin.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin? Bin { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem? Item { get; set; }

    [Ignore] public NAVZone? Zone => Bin?.Zone;
    [Ignore] public NAVLocation? Location => Zone?.Location;
    [Ignore] public Bay? Bay => Bin?.Bay;
    [Ignore] public Site? Site => Zone?.Site;

    [Ignore] public int BaseQty => Eaches?.Qty ?? 0 + (Packs?.Qty ?? 0 * Item?.Pack?.QtyPerUoM ?? 0) + (Cases?.Qty ?? 0 * Item?.Case?.QtyPerUoM ?? 0);

    public Stock()
    {
        ID = string.Empty;
        BinID = string.Empty;
        ZoneID = string.Empty;
        CaseID = string.Empty;
        PackID = string.Empty;
        EachID = string.Empty;
    }

    public Stock(string id, string binID, string caseID, string packID, string eachID, string zoneID)
    {
        ID = id;
        BinID = binID;
        CaseID = caseID;
        PackID = packID;
        EachID = eachID;
        ZoneID = zoneID;
    }

    public Stock(NAVStock navStock) : this()
    {
        // Handle the UoMs
        var uom = (EUoM)Enum.Parse(typeof(EUoM), navStock.UoMCode);
        switch (uom)
        {
            case EUoM.CASE:
                Cases = new SubStock(this, navStock);
                break;
            case EUoM.PACK:
                Packs = new SubStock(this, navStock);
                break;
            case EUoM.EACH:
            default:
                Eaches = new SubStock(this, navStock);
                break;
        }

        // Objects
        Bin = navStock.Bin;
        Item = navStock.Item;

        // Handle IDs : Item should stay constant, so isn't included in the re-used SetIDs method.
        ItemNumber = Item?.Number ?? navStock.ItemNumber;
        SetIDs();
    }

    /// <summary>
    /// Stock adds itself to all attached objects where appropriate.
    /// </summary>
    public void AddStock()
    {
        Location?.AddStock(this);
        Site?.AddStock(this);
        Zone?.AddStock(this);
        Bay?.AddStock(this);

        Item?.AddStock(this);
        // Bin last, as it may merge.
        Bin?.AddStock(this);
    }

    /// <summary>
    /// Stock removes itself from all attached objects, where appropriate.
    /// </summary>
    public void RemoveStock()
    {
        Location?.RemoveStock(this);
        Site?.RemoveStock(this);
        Zone?.RemoveStock(this);
        Bay?.RemoveStock(this);

        Item?.RemoveStock(this);
        Bin?.RemoveStock(this);
    }

    private void SetIDs()
    {
        BinID = Bin?.ID ?? BinID;
        ID = string.Join(":", BinID, ItemNumber);
        Cases?.SetStockID();
        Packs?.SetStockID();
        Eaches?.SetStockID();
        CaseID = Cases?.ID ?? CaseID;
        PackID = Packs?.ID ?? PackID;
        EachID = Eaches?.ID ?? EachID;
    }

    // Move full stock to specified bin.
    public void FullMove(NAVBin toBin)
    {
        // TODO: 
        /*// Handle object moving.
        if (Bin != null) _ = Bin.Stock.Remove(this);
        Bin = toBin;
        Bin.Stock.Add(this);
        // Handle ID changing.
        SetIDs();
        // Merge with potential matching stock in new bin location.
        toBin.MergeStock();*/
    }

    // Partial move.
    public void Move(NAVBin toBin, int eaches = 0, int packs = 0, int cases = 0)
    {
        var splitStock = Split(eaches, packs, cases);
        splitStock?.Move(toBin);
    }

    // Pulls out the specified QTYs and return a separate stock object.
    private Stock? Split(int eaches = 0, int packs = 0, int cases = 0)
    {
        if (Bin is null || Item is null) return null;

        Stock newStock = new()
        {
            Item = Item,
            ItemNumber = Item.Number,
            Bin = Bin
        };

        newStock.SetIDs();
        // TODO: Fix
        // Bin.Stock.Add(newStock); 

        // Make sure to not take more than available.
        if (eaches > (Eaches?.Qty ?? 0)) eaches = Eaches?.Qty ?? 0;
        if (packs > (Packs?.Qty ?? 0)) packs = Packs?.Qty ?? 0;
        if (cases > (Cases?.Qty ?? 0)) cases = Cases?.Qty ?? 0;
        // Move qty.
        if (Eaches != null) Eaches.Qty -= eaches;
        if (Packs != null) Packs.Qty -= packs;
        if (Cases != null) Cases.Qty -= cases;

        if (newStock.Eaches != null) newStock.Eaches.Qty = eaches;
        if (newStock.Packs != null) newStock.Packs.Qty = packs;
        if (newStock.Cases != null) newStock.Cases.Qty = cases;

        return newStock;
    }

    /// <summary>
    /// Merges new stock into this one, and clears to other stock out in the process to keep the overall balance.
    /// </summary>
    /// <param name="newStock"></param>
    /// <returns></returns>
    public bool Merge(Stock newStock)
    {
        var returnVal = Add(newStock);

        if (!returnVal) return false;

        newStock.Clear();

        return true;
    }

    // Checks if there is anything stopping the stock from being able to be moved.
    // e.g. Pick Qty/Put Away Qty, etc.
    public bool CanMove()
    {
        return !((Cases?.PreventsMove() ?? false) || (Packs?.PreventsMove() ?? false) || (Eaches?.PreventsMove() ?? false));
    }

    public bool MatchQty(Stock other)
    {
        return Cases?.Qty == other.Cases?.Qty &&
               Packs?.Qty == other.Packs?.Qty &&
               Eaches?.Qty == other.Eaches?.Qty;
    }

    /// <summary>
    /// Adds values from new stock to this one without removing them.
    /// </summary>
    /// <param name="newStock"></param>
    public bool Add(Stock newStock)
    {
        // Stock ID must match (same bin and item, etc.) but must not be the same object.
        if (ReferenceEquals(this, newStock) || ID != newStock.ID)
            return false;

        // Increase Stock quantities.
        if (newStock.Eaches is not null)
        {
            Eaches ??= new SubStock(this, EUoM.EACH);
            Eaches.Qty += newStock.Eaches.Qty;
        }

        if (newStock.Packs is not null)
        {
            Packs ??= new SubStock(this, EUoM.PACK);
            Packs.Qty += newStock.Packs.Qty;
        }

        // ReSharper disable once InvertIf
        if (newStock.Cases is not null)
        {
            Cases ??= new SubStock(this, EUoM.CASE);
            Cases.Qty += newStock.Cases.Qty;
        }

        return true;
    }

    public bool Sub(Stock stock)
    {
        // Stock ID must match (same bin and item, etc.) but must not be the same object.
        if (ReferenceEquals(this, stock) || ID != stock.ID)
            return false;

        // Decrease Stock quantities.
        // Allow negative values. Checks should be done before move creation/attempt.
        if (stock.Eaches is not null)
        {
            Eaches ??= new SubStock(this, EUoM.EACH);
            Eaches.Qty -= stock.Eaches.Qty;
        }

        if (stock.Packs is not null)
        {
            Packs ??= new SubStock(this, EUoM.PACK);
            Packs.Qty -= stock.Packs.Qty;
        }

        // ReSharper disable once InvertIf
        if (stock.Cases is not null)
        {
            Cases ??= new SubStock(this, EUoM.CASE);
            Cases.Qty -= stock.Cases.Qty;
        }

        return true;
    }

    public void Clear()
    {
        if (Eaches != null) Eaches.Qty = 0;
        if (Packs != null) Packs.Qty = 0;
        if (Cases != null) Cases.Qty = 0;
    }

    public Stock Copy()
    {
        var stock = new Stock
        {
            ID = ID,
            Eaches = Eaches?.Copy(),
            Packs = Packs?.Copy(),
            Cases = Cases?.Copy(),
            Bin = null,
            BinID = string.Empty,
            CaseID = CaseID,
            EachID = EachID,
            Item = Item,
            ItemNumber = ItemNumber,
            ZoneID = ZoneID
        };

        if (stock.Eaches != null) stock.Eaches.Stock = stock;
        if (stock.Packs != null) stock.Packs.Stock = stock;
        if (stock.Cases != null) stock.Cases.Stock = stock;

        return stock;
    }

    public bool IsEmpty()
    {
        return Cases?.Qty == 0 && Packs?.Qty == 0 && Eaches?.Qty == 0;
    }

    /// <summary>
    /// Determine if the stock as it stands in Item and Bin, has pending operations: i.e. values to pick, put, or count.
    /// </summary>
    /// <returns></returns>
    public bool Pending()
    {
        return (Cases?.Pending() ?? false) || 
               (Packs?.Pending() ?? false) || 
               (Eaches?.Pending() ?? false);
    }
}