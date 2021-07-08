using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Olympus.Helios.Inventory.Model
{
    [Table("RealStock")]
    public class Stock
    {
        [PrimaryKey]
        public string ID { get; set; } // Combination of BinID and ItemNumber (e.g. 9600:PR:PR18 058:271284)
        [ForeignKey(typeof(NAVBin))]
        public string BinID { get; set; } // Combination of LocationCode, ZoneCode, and BinCode (e.g. 9600:PR:PR18 058)
        [ForeignKey(typeof(NAVItem))]
        public string ItemNumber { get; set; }

        public int CaseQty { get; set; }
        public int PackQty { get; set; }
        public int EachQty { get; set; }

        [ManyToOne]
        public NAVBin Bin { get; set; }
        [ManyToOne]
        public NAVItem Item { get; set; }
        
    }

}
