using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Models;

[Table("RealStock")]
public class Stock
{
    [PrimaryKey] public string ID { get; set; } // Combination of BinID and ItemNumber (e.g. 9600:PR:PR18 058:271284)
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

    public Stock()
    {
        ID = string.Empty;
        BinID = string.Empty;
        CaseID = string.Empty;
        PackID = string.Empty;
        EachID = string.Empty;
    }

    public Stock(string id, string binID, string caseID, string packID, string eachID)
    {
        ID = id;
        BinID = binID;
        CaseID = caseID;
        PackID = packID;
        EachID = eachID;
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

        Bin?.Stock.Add(this);
        Item?.Stock.Add(this);


        // Handle IDs : Item should stay constant, so isn't included in the re-used SetIDs method.
        ItemNumber = Item?.Number ?? navStock.ItemNumber;
        SetIDs();
    }

    public NAVZone? Zone => Bin?.Zone;

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
        // Handle object moving.
        if (Bin != null) _ = Bin.Stock.Remove(this);
        Bin = toBin;
        Bin.Stock.Add(this);
        // Handle ID changing.
        SetIDs();
        // Merge with potential matching stock in new bin location.
        toBin.MergeStock();
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
        Bin.Stock.Add(newStock);
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

    public bool Merge(Stock newStock)
    {
        // Stock ID must match (same bin and item, etc.) but must not be the same object.
        if (ReferenceEquals(this, newStock) || ID != newStock.ID)
            return false;

        // Increase Stock quantities.
        if (newStock.Eaches is not null)
        {
            Eaches ??= new SubStock(this, EUoM.EACH);
            Eaches.Qty += newStock.Eaches.Qty;
            newStock.Eaches.Qty = 0;
        }

        if (newStock.Packs is not null)
        {
            Packs ??= new SubStock(this, EUoM.PACK);
            Packs.Qty += newStock.Packs.Qty;
            newStock.Packs.Qty = 0;
        }

        // ReSharper disable once InvertIf
        if (newStock.Cases is not null)
        {
            Cases ??= new SubStock(this, EUoM.CASE);
            Cases.Qty += newStock.Cases.Qty;
            newStock.Cases.Qty = 0;
        }

        return true;
    }

    // Checks if there is anything stopping the stock from being able to be moved.
    // e.g. Pick Qty/Put Away Qty, etc.
    public bool CanMove()
    {
        return !((Cases?.PreventsMove() ?? false) || (Packs?.PreventsMove() ?? false) || (Eaches?.PreventsMove() ?? false));
    }
}