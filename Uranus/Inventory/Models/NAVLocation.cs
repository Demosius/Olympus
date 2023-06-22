using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

[Table("LocationList")]
public class NAVLocation
{
    [PrimaryKey, ForeignKey(typeof(Store))] public string Code { get; set; }
    public string Name { get; set; }
    public string CompanyCode { get; set; }
    public bool IsWarehouse { get; set; }
    public bool IsStore { get; set; }
    public bool ActiveForReplenishment { get; set; }

    [OneToMany(nameof(NAVZone.LocationCode), nameof(NAVZone.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVZone> Zones { get; set; }
    [OneToMany(nameof(NAVMoveLine.LocationCode), nameof(NAVMoveLine.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVMoveLine> MoveLines { get; set; }
    [OneToMany(nameof(Models.NAVStock.LocationCode), nameof(Models.NAVStock.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> NAVStock { get; set; }
    [OneToMany(nameof(PickLine.CartonID), nameof(PickLine.BatchTOLine), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<PickLine> PickLines { get; set; }

    [OneToOne(nameof(Code), nameof(Models.Store.Location), CascadeOperations = CascadeOperation.CascadeRead)]
    public Store? Store { get; set; }

    [Ignore] public Dictionary<int, Stock> Stock { get; set; }

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
        NAVStock = new List<NAVStock>();
        Stock = new Dictionary<int, Stock>();
        PickLines = new List<PickLine>();
    }

    public NAVLocation(string code, string name)
    {
        Code = code;
        Name = name;
        CompanyCode = string.Empty;
        Zones = new List<NAVZone>();
        MoveLines = new List<NAVMoveLine>();
        NAVStock = new List<NAVStock>();
        Stock = new Dictionary<int, Stock>();
        PickLines = new List<PickLine>();
    }

    public void AddStock(Stock newStock)
    {
        if (Stock.TryGetValue(newStock.ItemNumber, out var oldStock))
            oldStock.Add(newStock);
        else
            Stock.Add(newStock.ItemNumber, newStock.Copy());
    }

    public void RemoveStock(Stock stock)
    {
        if (!Stock.TryGetValue(stock.ItemNumber, out var currentStock)) return;

        currentStock.Sub(stock);
        if (currentStock.IsEmpty()) Stock.Remove(stock.ItemNumber);

    }
}