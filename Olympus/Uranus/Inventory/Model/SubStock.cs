using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Inventory.Model
{
    public class SubStock
    {
        [PrimaryKey] // Combination of StockID and UoMCode (e.g. 9600:PR:PR18 058:271284:CASE)
        public string ID { get; set; }
        [ForeignKey(typeof(Stock))] // Combination of LocationCode, ZoneCode, BinCode and ItemNumber (e.g. 9600:PR:PR18 058:271284)
        public string StockID { get; set; }

        // UOM
        private EUoM eUoM;
        private string uomCode;
        public string UoMCode
        {
            get => uomCode;
            set
            {
                uomCode = value;
                eUoM = EnumConverter.StringToUoM(UoMCode);
            }
        }
        [Ignore]
        public EUoM UoM
        {
            get => eUoM; 
            set
            {
                eUoM = value;
                uomCode = EnumConverter.UoMToString(eUoM);
            }
        }

        public int Qty { get; set; }
        public int PickQty { get; set; }
        public int PutAwayQty { get; set; }
        public int NegAdjQty { get; set; }
        public int PosAdjQty { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Stock Stock { get; set; }

        public SubStock(Stock stock)
        {
            Stock = stock;
        }

        public SubStock(Stock stock, EUoM uom) : this(stock)
        {
            StockID = Stock.ID;
            UoM = uom;
            ID = string.Join(":", StockID, UoMCode);
        }

        public SubStock(Stock stock, NAVStock navStock) : this(stock)
        {
            ID = navStock.ID;
            StockID = navStock.BinID + navStock.ItemNumber;
            UoMCode = navStock.UoMCode;

            Qty = navStock.Qty;
            PickQty = navStock.PickQty;
            PutAwayQty = navStock.PutAwayQty;
            NegAdjQty = navStock.NegAdjQty;
            PosAdjQty = navStock.PosAdjQty;
        }

        public void SetStockID()
        {
            StockID = Stock.ID;
            ID = string.Join(":", StockID, UoMCode);
        }

        // Returns true if there is something in substock that 
        // should prevent the stock from being moved.
        public bool PreventsMove()
        {
            return !(PickQty == 0 && PutAwayQty == 0 && 
                     NegAdjQty == 0 && PosAdjQty == 0);
        }
    }
}
