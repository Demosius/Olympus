using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class ZoneExtension
{
    [PrimaryKey, ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
    public EAccessLevel AccessLevel { get; set; }
    public EZoneType ZoneType { get; set; }
    [ForeignKey(typeof(Site))] public string SiteName { get; set; }

    [OneToOne(nameof(ZoneID), nameof(NAVZone.Extension), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVZone? Zone { get; set; }

    [ManyToOne(nameof(SiteName), nameof(Models.Site.ZoneExtensions), CascadeOperations = CascadeOperation.CascadeRead)]
    public Site? Site { get; set; }

    public ZoneExtension()
    {
        ZoneID = string.Empty;
        AccessLevel = EAccessLevel.Ground;
        ZoneType = EZoneType.Overstock;
        SiteName = string.Empty;
    }

    public ZoneExtension(NAVZone zone)
    {
        ZoneID = zone.ID;
        Zone = zone;
        AccessLevel = EAccessLevel.Ground;
        ZoneType = EZoneType.Overstock;
        SiteName = string.Empty;
        zone.Extension = this;
    }
}