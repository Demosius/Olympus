﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("BinList")]
    public class NAVBin
    {
        [PrimaryKey, ForeignKey(typeof(BinDimensions))] // Combination of ZoneID and Code (e.g. 9600:PR:PR18 058)
        public string ID { get; set; }
        [ForeignKey(typeof(NAVZone))] // Combination of LocationCode and ZoneCode (e.g. 9600:PR)
        public string ZoneID { get; set; }
        public string LocationCode { get; set; }
        public string ZoneCode { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Empty { get; set; }
        public bool Assigned { get; set; }
        public int Ranking { get; set; }
        public double UsedCube { get; set; }
        public double MaxCube { get; set; }
        public DateTime LastCCDate { get; set; }
        public DateTime LastPIDate { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVZone Zone { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVStock> NAVStock { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Stock> Stock { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public BinBay BinBay { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public BinDimensions Dimensions { get; set; }

        [Ignore]
        public Bay Bay
        {
            get => BinBay.Bay; 
            set
            {
                BinBay.Bay = value;
            }
        }

        public NAVBin() { }

        // Merges matching items in Stock (NOT NAVStock)
        public void MergeStock()
        {
            // Try to merge every stock item with every other.
            // If merge is succesful, remove the merged stock from the list.
            // Use backwards list, and merge from the i, so we remove from the end of the list safely.
            for (int i = Stock.Count -1; i > 0; --i)
            {
                for (int j = i-1; j>=0; --j)
                {
                    if (Stock[j].Merge(Stock[i]))
                    {
                        Stock.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // Takes examples of NAVStock and creates Stock versions.
        public void ConvertStock()
        {
            // Make sure both lists are not null.
            if (NAVStock is null) NAVStock = new List<NAVStock> { };
            if (Stock is null) Stock = new List<Stock> { };

            foreach (NAVStock ns in NAVStock)
            {
                Stock stock = new Stock(ns);
                Stock.Add(stock);
            }
        }
    }
}
