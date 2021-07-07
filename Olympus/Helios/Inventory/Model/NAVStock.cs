using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("BinContents")]
    public class NAVStock
    {
        [PrimaryKey] // Combination of BinID and UoMID (e.g. 9600:PR:PR18 058:271284:CASE)
        public int ID { get; set; }
        [ForeignKey(typeof(NAVBin))] // Combination of LocationCode, ZoneCode, and BinCode (e.g. 9600:PR:PR18 058)
        public string BinID { get; set; }
        [ForeignKey(typeof(NAVUoM))] // Combination of ItemNumber and UoMCode (e.g. 271284:CASE)
        public string UoMID { get; set; }

        public string LocationCode { get; set; }
        public string ZoneCode { get; set; }
        public string BinCode { get; set; }
        [ForeignKey(typeof(NAVItem))]
        public int ItemNumber { get; set; }
        public string UoMCode { get; set; }
        public int Qty { get; set; }
        public int PickQty { get; set; }
        public int PutAwayQty { get; set; }
        public int NegAdjQty { get; set; }
        public int PosAdjQty { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool Fixed { get; set; }

        [ManyToOne]
        public NAVBin Bin { get; set; }
        [ManyToOne]
        public NAVUoM UoM { get; set; }
        [ManyToOne]
        public NAVItem Item { get; set; }

    }
}
