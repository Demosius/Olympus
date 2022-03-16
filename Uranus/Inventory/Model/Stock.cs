using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Model;

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
    public SubStock Cases { get; set; }
    [OneToOne(nameof(PackID), nameof(SubStock.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public SubStock Packs { get; set; }
    [OneToOne(nameof(EachID), nameof(SubStock.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public SubStock Eaches { get; set; }

    [ManyToOne(nameof(BinID), nameof(NAVBin.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVBin Bin { get; set; }
    [ManyToOne(nameof(ItemNumber), nameof(NAVItem.Stock), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVItem Item { get; set; }

    public Stock() { }

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
        ItemNumber = Item.Number;
        SetIDs();
    }

    private void SetIDs()
    {
        BinID = Bin.ID;
        ID = string.Join(":", BinID, ItemNumber);
        Cases.SetStockID();
        Packs.SetStockID();
        Eaches.SetStockID();
        CaseID = Cases.ID;
        PackID = Packs.ID;
        EachID = Eaches.ID;
    }

    // Move full stock to specified bin.
    public void FullMove(NAVBin toBin)
    {
        // Handle object moving.
        _ = Bin.Stock.Remove(this);
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
        splitStock.Move(toBin);
    }

    // Pulls out the specified QTYs and return a separate stock object.
    private Stock Split(int eaches = 0, int packs = 0, int cases = 0)
    {
        Stock newStock = new()
        {
            Item = Item,
            ItemNumber = Item.Number,
            Bin = Bin
        };
        newStock.SetIDs();
        Bin.Stock.Add(newStock);
        // Make sure to not take more than available.
        if (eaches > Eaches.Qty) eaches = Eaches.Qty;
        if (packs > Packs.Qty) packs = Packs.Qty;
        if (cases > Cases.Qty) cases = Cases.Qty;
        // Move qty.
        Eaches.Qty -= eaches;
        Packs.Qty -= packs;
        Cases.Qty -= cases;
        newStock.Eaches.Qty = eaches;
        newStock.Packs.Qty = packs;
        newStock.Cases.Qty = cases;
        return newStock;
    }

    public bool Merge(Stock newStock)
    {
        // Stock ID must match (same bin and item, etc.) but must not be the same object.
        if (ReferenceEquals(this, newStock) || ID != newStock.ID)
            return false;
        // Increase Stock quantities.
        Eaches.Qty += newStock.Eaches.Qty;
        Packs.Qty += newStock.Packs.Qty;
        Cases.Qty += newStock.Cases.Qty;
        // Empty newStock.
        newStock.Eaches.Qty = 0;
        newStock.Packs.Qty = 0;
        newStock.Cases.Qty = 0;
        return true;
    }

    // Checks if there is anything stopping the stock from being able to be moved.
    // e.g. Pick Qty/Put Away Qty, etc.
    public bool CanMove()
    {
        return !(Cases.PreventsMove() || Packs.PreventsMove() || Eaches.PreventsMove());
    }
}