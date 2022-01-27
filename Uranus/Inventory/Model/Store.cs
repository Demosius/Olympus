using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Uranus.Inventory.Model
{
    public class Store
    {
        [PrimaryKey, ForeignKey(typeof(NAVLocation))]
        public string Number { get; set; }
        public int WaveNumber { get; set; }
        public int TransitDays { get; set; }
        public EVolume Volume { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<NAVTransferOrder> TransferOrders { get; set; }

        public Store() { }

        public string Wave()
        {
            return $"W{WaveNumber:00}";
        }
    }
}
