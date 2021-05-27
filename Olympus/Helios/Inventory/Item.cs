using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Inventory
{
    public class Item
    {
        public int Number { get; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int Platform { get; set; }
        public int Division { get; set; }
        public int Genre { get; set; }
        public UoM Each { get; set; }
        public UoM Pack { get; set; }
        public UoM Case { get; set; }

        public Dictionary<string, Stock> Stock { get; }

        public Item(int itemNumber, string description, int category, int platform, int division, int genre)
        {
            Number = itemNumber;
            Description = description;
            Category = category;
            Platform = platform;
            Division = division;
            Genre = genre;
            // Set each to null untill they are set.
            Each = null;
            Pack = null;
            Case = null;

            Stock = new Dictionary<string, Stock> { };
        }

        public void AddStock(Stock stock)
        {
            Stock.Add(stock.Bin.Code, stock);
        }

        public void RemoveStock(Stock stock)
        {
            Stock.Remove(stock.Bin.Code);
        }
    }
}
