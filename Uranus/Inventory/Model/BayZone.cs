using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model;

public class BayZone
{
    [ForeignKey(typeof(Bay))] public string BayID { get; set; }
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }
}