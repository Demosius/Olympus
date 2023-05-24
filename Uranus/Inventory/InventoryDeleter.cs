using System.Collections.Generic;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

public class InventoryDeleter
{
    public InventoryChariot Chariot { get; set; }

    public InventoryDeleter(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    // Deletes stock where zones are in the given list.
    public void StockZoneDeletes(List<string> zoneIDs) => Chariot.StockZoneDeletes(zoneIDs);

    public int Site(Site site) => Chariot.Delete(site);
}