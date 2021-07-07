using SQLite;
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
        [PrimaryKey] // Combination of ZoneID and Code (e.g. 9600:PR:PR18 058)
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

        [ManyToOne]
        public NAVZone Zone { get; set; }
        [OneToMany]
        public NAVStock Stock { get; set; }
    }
}
