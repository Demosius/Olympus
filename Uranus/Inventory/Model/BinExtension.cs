using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory.Model
{
    // Used for connecting bays to bins.
    public class BinExtension
    {
        [PrimaryKey, ForeignKey(typeof(NAVBin))]
        public string BinID { get; set; }
        [ForeignKey(typeof(Bay))]
        public string BayID { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVBin Bin { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Bay Bay { get; set; }
    }
}
