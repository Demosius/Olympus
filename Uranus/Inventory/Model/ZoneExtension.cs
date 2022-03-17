using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model;

public class ZoneExtension
{
    [PrimaryKey, ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
    public EAccessLevel AccessLevel { get; set; }

    [OneToOne(nameof(ZoneID), nameof(NAVZone.Extension), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone? Zone { get; set; }

    public ZoneExtension()
    {
        ZoneID = string.Empty;
        AccessLevel = EAccessLevel.Ground;
    }

    public ZoneExtension(NAVZone zone)
    {
        ZoneID = zone.ID;
        Zone = zone;
        AccessLevel = EAccessLevel.Ground;
    }
}