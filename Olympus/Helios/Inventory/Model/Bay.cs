using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Olympus.Helios.Inventory.Model
{
    public class Bay
    {
        [PrimaryKey]
        public string ID { get; set; } // Bay name. Called ID for consitency.
        [ForeignKey(typeof(NAVZone))]
        public string ZoneID { get; }

        [ManyToOne]
        public NAVZone Zone { get; set; }
        [OneToMany]
        public List<NAVBin> Bins { get; set; }

    }
}
