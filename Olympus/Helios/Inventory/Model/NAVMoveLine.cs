using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    public class NAVMoveLine
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        public EAction ActionType { get; set; }
        [ForeignKey(typeof(NAVLocation))]
        public string ZoneID { get; set; } // Combination of LocationCode and ZoneCode (e.g. 9600:PK)
        [ForeignKey(typeof(NAVBin))]
        public string BinID { get; set; } // Combination of ZoneID and BinCode (e.g. 9600:PR:PR18 058)
        [ForeignKey(typeof(NAVItem))]
        public int ItemNumber { get; set; }
        public string ZoneCode { get; set; }
        public string BinCode { get; set; }
        public int Qty { get; set; }
        public EUoM UoM { get; set; }

        public string LocationCode { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVZone Zone { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVBin Bin { get; set; }


        public NAVMoveLine()
        {
            // Moves copied from NAV do not have associated location code in clipbaord, so assume 9600 at default.
            LocationCode = "9600";
        }

    }
}
