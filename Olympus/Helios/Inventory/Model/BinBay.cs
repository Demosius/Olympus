using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    // Used for connecting bays to bins.
    public class BinBay
    {
        [PrimaryKey, ForeignKey(typeof(NAVBin))]
        public string BinID { get; set; }
        [ForeignKey(typeof(Bay))]
        public string BayID { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVBin Bin { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Bay Bay { get; set; }
    }
}
