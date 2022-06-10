using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Models;

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
    [OneToMany(nameof(Models.NAVStock.LocationCode), nameof(Models.NAVStock.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVStock> NAVStock { get; set; }
    [OneToMany(nameof(Store.Number), nameof(Store.Location), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<Store> Stores { get; set; }

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
        Stores = new List<Store>();
        Stock = new Dictionary<int, Stock>();
    }

    public NAVLocation(string code, string name)
    {
        Code = code;
        Name = name;
        CompanyCode = string.Empty;
        Zones = new List<NAVZone>();
        MoveLines = new List<NAVMoveLine>();
        NAVStock = new List<NAVStock>();
        Stores = new List<Store>();
        Stock = new Dictionary<int, Stock>();
    }

    public NAVLocation(string code, string name, string companyCode, bool isWarehouse, bool isStore,
        bool activeForReplenishment, List<NAVZone> zones, List<NAVMoveLine> moveLines, List<NAVStock> navStock,
        List<Store> stores)
    {
        Code = code;
        Name = name;
        CompanyCode = companyCode;
        IsWarehouse = isWarehouse;
        IsStore = isStore;
        ActiveForReplenishment = activeForReplenishment;
        Zones = zones;
        MoveLines = moveLines;
        NAVStock = navStock;
        Stores = stores;
        Stock = new Dictionary<int, Stock>();
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