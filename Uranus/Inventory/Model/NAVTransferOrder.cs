using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory.Model
{
    [Table("TOLineBatchAnalysis")]
    public class NAVTransferOrder
    {
        [PrimaryKey] // Document/Transfer No. 
        public string ID { get; set; }
        [ForeignKey(typeof(Store))]
        public string StoreNumber { get; set; }     // Transfer-to Code
        [ForeignKey(typeof(NAVItem))]
        public int ItemNumber { get; set; }
        public int Qty { get; set; }
        public EUoM UoM { get; set; }
        public int AvailableQty { get; set; }
        public DateTime CreationTime { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Store Store { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVItem Item { get; set; }

    }
}
