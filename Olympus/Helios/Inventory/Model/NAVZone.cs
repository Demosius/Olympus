using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    [Table("ZoneList")]
    public class NAVZone
    {
        [PrimaryKey] // Combination of LocationCode and Code (e.g. 9600:PK)
        public string ID { get; set; } 
        public string Code { get; set; }
        [ForeignKey(typeof(NAVLocation))]
        public string LocationCode { get; set; }
        public string Description { get; set; }
        public int Ranking { get; set; }

        [ManyToOne]
        public NAVLocation Location { get; set; }
        [OneToMany]
        public List<NAVBin> Bins { get; set; }
    }
}
