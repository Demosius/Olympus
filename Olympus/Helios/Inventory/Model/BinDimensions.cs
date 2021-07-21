using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    public class BinDimensions
    {
        [PrimaryKey, ForeignKey(typeof(NAVBin))]
        public string ID { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVBin Bin { get; set; }

    }
}
