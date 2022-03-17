using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace Uranus.Inventory.Model;

[Table("BCZoneUpdateTimes")]
public class BinContentsUpdate
{
    [PrimaryKey, ForeignKey(typeof(NAVZone))] public string ZoneID { get; set; } // Combination of LocationCode and ZoneCode (e.g. 9600:PK)
    [ForeignKey(typeof(NAVLocation))] public string LocationCode { get; set; }
    public string ZoneCode { get; set; }
    public DateTime LastUpdate { get; set; }

    public BinContentsUpdate()
    {
        ZoneID = string.Empty;
        LocationCode = string.Empty;
        ZoneCode = string.Empty;
        LastUpdate = DateTime.MinValue;
    }

    public BinContentsUpdate(string zoneID, string locationCode, string zoneCode, DateTime lastUpdate)
    {
        ZoneID = zoneID;
        LocationCode = locationCode;
        ZoneCode = zoneCode;
        LastUpdate = lastUpdate;
    }
}