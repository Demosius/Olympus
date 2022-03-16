using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

[Table("ZoneList")]
public class NAVZone
{
    [PrimaryKey] public string ID { get; set; } // Combination of LocationCode and Code (e.g. 9600:PK)
    public string Code { get; set; }
    [ForeignKey(typeof(NAVLocation))] public string LocationCode { get; set; }
    public string Description { get; set; }
    public int Ranking { get; set; }
    public EAccessLevel AccessLevel { get; set; }

    [ManyToOne(nameof(LocationCode), nameof(NAVLocation.Zones), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVLocation Location { get; set; }

    [OneToMany(nameof(NAVBin.ZoneCode), nameof(NAVBin.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVBin> Bins { get; set; }
    [OneToMany(nameof(NAVMoveLine.ZoneID), nameof(NAVMoveLine.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVMoveLine> MoveLines { get; set; }
    [OneToMany(nameof(NAVStock.ZoneID), nameof(NAVStock.Zone), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> Stock { get; set; }

    [ManyToMany(typeof(BayZone), nameof(BayZone.ZoneID), nameof(Bay.Zones), CascadeOperations = CascadeOperation.All)]
    public List<Bay> Bays { get; set; }
}