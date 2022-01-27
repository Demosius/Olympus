using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model
{
    public class ZoneAccessLevel
    {
        [PrimaryKey, ForeignKey(typeof(NAVZone))]
        public string ZoneID { get; set; }
        public EAccessLevel AccessLevel { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVZone Zone { get; set; }
    }
}
