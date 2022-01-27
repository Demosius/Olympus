using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Model
{
    [Table("BCZoneUpdateTimes")]
    class BinContentsUpdate
    {
        [PrimaryKey, ForeignKey(typeof(NAVZone))]
        public string ZoneID { get; set; } // Combination of LocationCode and ZoneCode (e.g. 9600:PK)
        [ForeignKey(typeof(NAVLocation))]
        public string LocationCode { get; set; }
        public string ZoneCode { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
