using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Model;

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

    public EUoM UoM { get; set; }

    [ManyToOne(nameof(StockID), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public Stock Stock { get; set; }

    public SubStock(Stock stock)
    {
        Stock = stock;
    }

    public SubStock(Stock stock, EUoM uom) : this(stock)
    {
        StockID = Stock.ID;
        UoM = uom;
        ID = string.Join(":", StockID, UoM);
    }

    public SubStock(Stock stock, NAVStock navStock) : this(stock)
    {
        ID = navStock.ID;
        StockID = navStock.BinID + navStock.ItemNumber;
        UoM = (EUoM)Enum.Parse(typeof(EUoM), navStock.UoMCode);

        Qty = navStock.Qty;
        PickQty = navStock.PickQty;
        PutAwayQty = navStock.PutAwayQty;
        NegAdjQty = navStock.NegAdjQty;
        PosAdjQty = navStock.PosAdjQty;
    }

    public void SetStockID()
    {
        StockID = Stock.ID;
        ID = string.Join(":", StockID, UoM);
    }

    // Returns true if there is something in sub-stock that 
    // should prevent the stock from being moved.
    public bool PreventsMove()
    {
        return !(PickQty == 0 && PutAwayQty == 0 &&
                 NegAdjQty == 0 && PosAdjQty == 0);
    }
}