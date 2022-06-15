using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Models;

public class SubStock
{
    // Combination of StockID and UoMCode (e.g. 9600:PR:PR18 058:271284:CASE)
    [PrimaryKey] public string ID { get; set; }
    // Combination of LocationCode, ZoneCode, BinCode and ItemNumber (e.g. 9600:PR:PR18 058:271284)
    [ForeignKey(typeof(Stock))] public string StockID { get; set; }
    public int Qty { get; set; }
    public int PickQty { get; set; }
    public int PutAwayQty { get; set; }
    public int NegAdjQty { get; set; }
    public int PosAdjQty { get; set; }

    [Ignore] public int AvailableQty => Qty - PickQty - NegAdjQty;
    [Ignore] public int BalanceQty => Qty - PickQty + PutAwayQty - NegAdjQty + PosAdjQty;

    public EUoM UoM { get; set; }

    [ManyToOne(nameof(StockID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Stock? Stock { get; set; }

    public SubStock()
    {
        ID = string.Empty;
        StockID = string.Empty;
    }

    public SubStock(Stock stock) : this()
    {
        Stock = stock;
        StockID = stock.ID;
    }

    public SubStock(Stock stock, EUoM uom) : this(stock)
    {
        UoM = uom;
        ID = string.Join(":", StockID, UoM);
    }

    public SubStock(Stock stock, NAVStock navStock) : this(stock)
    {
        ID = navStock.ID;
        StockID = navStock.BinID + navStock.ItemNumber;

        if (!Enum.TryParse(navStock.UoMCode, out EUoM uom)) uom = EUoM.EACH;
        UoM = uom;

        Qty = navStock.Qty;
        PickQty = navStock.PickQty;
        PutAwayQty = navStock.PutAwayQty;
        NegAdjQty = navStock.NegAdjQty;
        PosAdjQty = navStock.PosAdjQty;
    }

    public void SetStockID()
    {
        StockID = Stock?.ID ?? string.Empty;
        ID = string.Join(":", StockID, UoM);
    }

    // Returns true if there is something in sub-stock that 
    // should prevent the stock from being moved.
    public bool PreventsMove()
    {
        return !(PickQty == 0 && PutAwayQty == 0 &&
                 NegAdjQty == 0 && PosAdjQty == 0);
    }
    
    /// <summary>
    /// Creates a full copy of the sub-stock.
    /// </summary>
    /// <returns></returns>
    public SubStock Copy()
    {
        var sub = new SubStock
        {
            ID = ID,
            NegAdjQty = NegAdjQty,
            PosAdjQty = PosAdjQty,
            Qty = Qty,
            PickQty = PickQty,
            PutAwayQty = PutAwayQty,
            StockID = StockID,
            UoM = UoM
        };
        return sub;
    }

    /// <summary>
    /// Creates a similar copy of the sub-stock with (at most - determined by available stock) the qty given
    /// and does not copy across other pending values (pick/put/adj.).
    /// </summary>
    /// <param name="qty"></param>
    /// <returns></returns>
    public SubStock Split(int qty)
    {
        if (qty > AvailableQty) qty = AvailableQty;
        var sub = new SubStock
        {
            ID = ID,
            Qty = qty,
            StockID = StockID,
            UoM = UoM
        };
        return sub;
    }

    /// <summary>
    /// Determine if the subStock as it stands in Item, Bin and UoM, has pending operations: i.e. values to pick, put, or count.
    /// </summary>
    /// <returns></returns>
    public bool Pending()
    {
        return NegAdjQty != 0 ||
               PosAdjQty != 0 ||
               PickQty != 0 ||
               PutAwayQty != 0;
    }
}