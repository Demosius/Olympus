using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model;

public class ZoneExtension
{
    [PrimaryKey, ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
    public EAccessLevel AccessLevel { get; set; }

    [OneToOne(nameof(ZoneID), nameof(NAVZone.Extension), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone Zone { get; set; }

    public ZoneExtension()
    {
        AccessLevel = EAccessLevel.Ground;
    }

    public ZoneExtension(NAVZone zone)
    {
        Zone = zone;
    }
}