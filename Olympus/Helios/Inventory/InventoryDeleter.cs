using Olympus.Helios.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory
{
    public class InventoryDeleter
    {
        public InventoryChariot Chariot { get; set; }

        public InventoryDeleter(ref InventoryChariot chariot)
        {
            Chariot = chariot;
        }

        // Deletes stock where zones are in the given list.
        public void StockZoneDeletes(List<string> zoneIDs) => Chariot.StockZoneDeletes(zoneIDs);
       
    }
}
