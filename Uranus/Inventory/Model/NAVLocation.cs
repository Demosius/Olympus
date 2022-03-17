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
    [OneToMany(nameof(Store.Number), nameof(Store.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Store> Stores { get; set; }

    public NAVLocation()
    {
        Code = string.Empty;
        Name = string.Empty;
        CompanyCode = string.Empty;
        IsWarehouse = false;
        IsStore = false;
        ActiveForReplenishment = false;
        Zones = new List<NAVZone>();
        MoveLines = new List<NAVMoveLine>();
        Stock = new List<NAVStock>();
        Stores = new List<Store>();
    }

    public NAVLocation(string code, string name, string companyCode, bool isWarehouse, bool isStore, bool activeForReplenishment, List<NAVZone> zones, List<NAVMoveLine> moveLines, List<NAVStock> stock, List<Store> stores)
    {
        Code = code;
        Name = name;
        CompanyCode = companyCode;
        IsWarehouse = isWarehouse;
        IsStore = isStore;
        ActiveForReplenishment = activeForReplenishment;
        Zones = zones;
        MoveLines = moveLines;
        Stock = stock;
        Stores = stores;
    }
}