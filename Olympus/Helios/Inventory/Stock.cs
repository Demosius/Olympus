using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Inventory
{
    public class Stock
    {

        public Bin Bin { get; set; }
        public Item Item { get; set; }
        public int CaseQty { get; set; }
        public int PackQty { get; set; }
        public int EachQty { get; set; }

        public Stock(Bin bin, Item item, int eachQty, int packQty, int caseQty)
        {
            Bin = bin;
            Item = item;
            EachQty = eachQty;
            PackQty = packQty;
            CaseQty = caseQty;
        }

        /// <summary>
        ///  Merges the new stock into this Stock item.
        ///  Use on stock in target bin location, and remove the 'moving' stock.
        ///  Will throw an error if stock is not of the same item.
        /// </summary>
        /// <param name="newStock"></param>
        public void Merge(Stock newStock)
        {
            if (Item == newStock.Item) throw new Exception("Cannot merge stock of different items.");
            CaseQty += newStock.CaseQty;
            PackQty += newStock.PackQty;
            EachQty += newStock.EachQty;
        }

        /// <summary>
        ///  Splits off a given amount of the stock.
        ///  New stock is given an association with an invalid bin and is not associated within the Item, 
        ///  so will need to be moved immediately to be of use.
        /// </summary>
        /// <param name="eachQty"></param>
        /// <param name="packQty"></param>
        /// <param name="caseQty"></param>
        /// <returns></returns>
        public Stock Split(int eachQty = 0, int packQty = 0, int caseQty = 0)
        {
            if (eachQty > EachQty || packQty > PackQty || caseQty > CaseQty) throw new Exception("Cannot split off more than exists in stock.");
            Stock newStock = new Stock(new Bin(), Item, eachQty, packQty, caseQty);
            return newStock;
        }

        /// <summary>
        ///  Removes itself as a reference from both the associated Item and Bin.
        ///  Garbage collection should then take care of it if it is not re-added to a bin.
        /// </summary>
        public void Remove()
        {
            Item.RemoveStock(this);
            Bin.RemoveStock(this);
        }

        /// <summary>
        ///  Move the stock to a different bin.
        ///  Will merge if stock of the same item already exists.
        /// </summary>
        /// <param name="bin"></param>
        public void Move(Bin bin)
        {   
            // Remove the stock from relevant bin and item.
            Remove();

            // If bin already has matching item stock, merge the data.
            if (bin.Stock.ContainsKey(this.Item.Number))
            {
                bin.Stock[Item.Number].Merge(this);
            }
            else
            {
                bin.AddStock(this);
                Item.AddStock(this);
            }
        }
    }

    /// <summary>
    ///  A simplified version of stock that is not connected to any other objects, and can be converted directly to and from bin contents.
    /// </summary>
    public class SimpleStock
    {
        public string Location { get; set; }
        public string ZoneCode { get; set; }
        public string BinCode { get; set; }
        public int ItemNumber { get; set; }
        public string BarCode { get; set; }
        public string UoMCode { get; set; }
        public int Qty { get; set; }
        public int PickQty { get; set; }
        public int PutAwayQty { get; set; }
        public int NegAdjQty { get; set; }
        public int PosAdjQty { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool Fixed { get; set; }

        public SimpleStock() { }

        public SimpleStock(string location, string zoneCode, string binCode, int itemNumber, string barCode, string uomCode, 
                            int qty, int pickQty, int putAwayQty, int negAdjQty, int posAdjQty, DateTime dateCreated, DateTime timeCreated, 
                            bool _fixed)
        {
            Location = location;
            ZoneCode = zoneCode;
            BinCode = binCode;
            ItemNumber = itemNumber;
            BarCode = barCode;
            UoMCode = uomCode;
            Qty = qty;
            PickQty = pickQty;
            PutAwayQty = putAwayQty;
            NegAdjQty = negAdjQty;
            PosAdjQty = posAdjQty;
            DateCreated = dateCreated;
            TimeCreated = timeCreated;
            Fixed = _fixed;
        }
    }
}
