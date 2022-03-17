using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model;

public class Store
{
    [PrimaryKey, ForeignKey(typeof(NAVLocation))] public string Number { get; set; }
    public int WaveNumber { get; set; }
    public int TransitDays { get; set; }
    public EVolume Volume { get; set; }

    [OneToMany(nameof(NAVTransferOrder.StoreNumber), nameof(NAVTransferOrder.Store), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public List<NAVTransferOrder> TransferOrders { get; set; }

    [ManyToOne(nameof(Number), nameof(NAVLocation.Stores), CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    public NAVLocation? Location { get; set; }

    public Store()
    {
        Number = string.Empty;
        TransferOrders = new List<NAVTransferOrder>();
    }

    public Store(string number, int waveNumber, int transitDays, EVolume volume, List<NAVTransferOrder> transferOrders, NAVLocation? location)
    {
        Number = number;
        WaveNumber = waveNumber;
        TransitDays = transitDays;
        Volume = volume;
        TransferOrders = transferOrders;
        Location = location;
    }

    public string Wave()
    {
        return $"W{WaveNumber:00}";
    }
}