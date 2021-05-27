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
}
