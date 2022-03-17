using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Model;

public class BayZone
{
    [ForeignKey(typeof(Bay))] public string BayID { get; set; }
    [ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; }

    public BayZone()
    {
        BayID = string.Empty;
        ZoneID = string.Empty;
    }

    public BayZone(string bayID, string zoneID)
    {
        BayID = bayID;
        ZoneID = zoneID;
    }
}