using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

[Table("LocationList")]
public class NAVLocation
{
    [PrimaryKey] public string Code { get; set; }
    public string Name { get; set; }
    public string CompanyCode { get; set; }
    public bool IsWarehouse { get; set; }
    public bool IsStore { get; set; }
    public bool ActiveForReplenishment { get; set; }

    [OneToMany(nameof(NAVZone.LocationCode), nameof(NAVZone.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVZone> Zones { get; set; }
    [OneToMany(nameof(NAVMoveLine.LocationCode), nameof(NAVMoveLine.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVMoveLine> MoveLines { get; set; }
    [OneToMany(nameof(NAVStock.LocationCode), nameof(NAVStock.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> Stock { get; set; }
}