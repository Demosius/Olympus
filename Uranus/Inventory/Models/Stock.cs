using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("RealStock")]
public class Stock : IEnumerable
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

    [Ignore] public int EachQty => Eaches?.Qty ?? 0;
    [Ignore] public int PackQty => Packs?.Qty ?? 0;
    [Ignore] public int CaseQty => Cases?.Qty ?? 0;

    [Ignore] public int UnitsInPacks => (Packs?.Qty ?? 0) * (Item?.QtyPerPack ?? 1);
    [Ignore] public int UnitsInCases => (Cases?.Qty ?? 0) * (Item?.QtyPerCase ?? 1);

    [Ignore] public bool Merged { get; set; } = false;

    [Ignore] public int BaseQty => EachQty + UnitsInPacks + UnitsInCases;
    [Ignore]
    public bool NonCommitted => (Eaches is null || Eaches.NonCommitted) &&
                                         (Packs is null || Packs.NonCommitted) &&
                                         (Cases is null || Cases.NonCommitted);
    [Ignore]
    public bool HasNegativeUoM => (Eaches?.IsNegative ?? false) ||
                                            (Packs?.IsNegative ?? false) ||
                                             (Cases?.IsNegative ?? false);

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
        if (!Enum.TryParse(navStock.UoMCode, out EUoM uom)) uom = EUoM.EACH;
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

    public Stock(NAVItem item, NAVBin bin) : this()
    {
        Bin = bin;
        Item = item;
        ItemNumber = item.Number;
        SetIDs();
    }

    /// <summary>
    /// Changes the bin associated with the stock. Typically used for stock movements.
    /// Changes associated IDs for sub-stock and this stock itself.
    /// </summary>
    /// <param name="newBin"></param>
    public void ChangeBin(NAVBin newBin)
    {
        Bin = newBin;
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
        RemoveStock();
        ChangeBin(toBin);
        AddStock();
    }

    // Partial move.
    public void Move(NAVBin toBin, int eaches = 0, int packs = 0, int cases = 0)
    {
        if (IsFull(eaches, packs, cases))
        {
            FullMove(toBin);
        }
        else
        {
            var stock = Copy();
            stock.RemoveStock();
            stock.ChangeBin(toBin);
            stock.AddStock();
        }
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
    /// If stock is of the same bin when adding, set sub stock to equal new if it doesn't already exist.
    /// </summary>
    /// <param name="newStock"></param>
    public bool Add(Stock newStock)
    {
        // Stock must not be the same object.
        if (ReferenceEquals(this, newStock)) return false;

        // Increase Stock quantities.
        if (newStock.Eaches is not null)
        {
            if (Eaches is null)
                Eaches = newStock.BinID == BinID ? newStock.Eaches.Copy() : Eaches ??= new SubStock(this, EUoM.EACH, newStock.EachQty);
            else
                Eaches.Qty += newStock.EachQty;
        }

        if (newStock.Packs is not null)
        {
            if (Packs is null)
                Packs ??= newStock.BinID == BinID ? newStock.Packs.Copy() : new SubStock(this, EUoM.PACK, newStock.PackQty);
            else
                Packs.Qty += newStock.PackQty;
        }

        // ReSharper disable once InvertIf
        if (newStock.Cases is not null)
        {
            if (Cases is null)
                Cases ??= newStock.BinID == BinID ? newStock.Cases.Copy() : new SubStock(this, EUoM.CASE, newStock.CaseQty);
            else
                Cases.Qty += newStock.CaseQty;
        }

        return true;
    }

    public bool Sub(Stock stock)
    {
        // Stock must not be the same object.
        if (ReferenceEquals(this, stock))
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

    /// <summary>
    /// Creates a copy of the stock, including the sub stock and relevant qty values.
    /// This copy is not to be associated with the existing bin - and will typically
    /// be used as an abstract stock level balance.
    /// </summary>
    /// <returns></returns>
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
            PackID = PackID,
            EachID = EachID,
            Item = Item,
            ItemNumber = ItemNumber,
            ZoneID = ZoneID
        };

        // Make sure the new sub-stock references the new stock instead of this parent stock.
        if (stock.Eaches != null) stock.Eaches.Stock = stock;
        if (stock.Packs != null) stock.Packs.Stock = stock;
        if (stock.Cases != null) stock.Cases.Stock = stock;

        return stock;
    }

    /// <summary>
    /// Creates a copy of the stock, but only applies the quantity values given
    /// and does not take with it the applied sub-stock values such as pending moves and adjustments.
    ///
    /// We do not remove from this parent stock as we split it, as that should be handled by any applicable movement functions.
    /// </summary>
    /// <returns></returns>
    public Stock Split(int eachQty, int packQty, int caseQty)
    {
        var stock = new Stock
        {
            ID = ID,
            Eaches = Eaches?.Split(eachQty),
            Packs = Packs?.Split(packQty),
            Cases = Cases?.Split(caseQty),
            Bin = Bin,
            BinID = BinID,
            CaseID = CaseID,
            PackID = PackID,
            EachID = EachID,
            Item = Item,
            ItemNumber = ItemNumber,
            ZoneID = ZoneID
        };

        // Make sure the new sub-stock references the new stock instead of this parent stock.
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

    /// <summary>
    /// Determine if the given values represents the full quantity of stock.
    /// </summary>
    /// <param name="eaches"></param>
    /// <param name="packs"></param>
    /// <param name="cases"></param>
    /// <returns></returns>
    public bool IsFull(int eaches, int packs, int cases)
    {
        return eaches == EachQty && packs == PackQty && cases == CaseQty;
    }

    /// <summary>
    /// Converts the internal quantities to the given values per UoM.
    /// If it cannot be done perfectly, will do what is possible and leave leftovers as eaches.
    /// </summary>
    /// <param name="eaches"></param>
    /// <param name="packs"></param>
    /// <param name="cases"></param>
    public void Convert(int eaches, int packs, int cases)
    {
        // Take out what units can be taken.
        var eachAv = Eaches?.AvailableQty ?? 0;
        if (eachAv < 0) eachAv = 0;
        var packAv = Packs?.AvailableQty ?? 0;
        if (packAv < 0) packAv = 0;
        var caseAv = Cases?.AvailableQty ?? 0;
        if (caseAv < 0) caseAv = 0;

        var qtyPerPack = Item?.QtyPerPack ?? 1;
        var qtyPerCase = Item?.QtyPerCase ?? 1;

        var units = 0;

        if (Eaches is not null)
        {
            Eaches.Qty -= eachAv;
            units += eachAv;
        }

        if (Packs is not null)
        {
            Packs.Qty -= packAv;
            units += packAv * qtyPerPack;
        }

        if (Cases is not null)
        {
            Cases.Qty -= caseAv;
            units += caseAv * qtyPerCase;
        }

        var unitsForCases = cases * qtyPerCase;

        if (unitsForCases > 0 && Cases is not null)
        {
            if (unitsForCases > units) unitsForCases = units / qtyPerCase * qtyPerCase;
            units -= unitsForCases;
            Cases.Qty += unitsForCases / qtyPerCase;
        }

        var unitsForPacks = packs * qtyPerPack;
        if (unitsForPacks > 0 && Packs is not null)
        {
            if (unitsForPacks > units) unitsForPacks = units / qtyPerPack * qtyPerPack;
            units -= unitsForPacks;
            Packs.Qty += unitsForPacks / qtyPerPack;
        }

        if (Eaches is not null)
        {
            Eaches.Qty += units;
        }
    }

    /// <summary>
    /// Gets a string representing the units of measure of the item at this bin location.
    /// e.g. EACH or EACH,PACK etc.
    /// </summary>
    /// <returns></returns>
    public string GetUoMString()
    {
        var list = new List<string>();
        if (Cases is not null) list.Add("CASE");
        if (Eaches is not null) list.Add("EACH");
        if (Packs is not null) list.Add("PACK");

        return $"{Bin?.Code} ({string.Join(",", list)})";
    }

    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}