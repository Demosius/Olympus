using Uranus.Inventory.Model;
using System;
using System.Collections.Generic;

namespace Uranus.Inventory
{
    public class InventoryCreator
    {
        public InventoryChariot Chariot { get; set; }

        public InventoryCreator(ref InventoryChariot chariot)
        {
            Chariot = chariot;
        }

        public bool NAVBins(List<NAVBin> bins)
        {
            if (!Chariot.ReplaceFullTable(bins)) return false;
            _ = Chariot.SetTableUpdateTime(typeof(NAVBin));
            return true;
        }

        public bool NAVItems(List<NAVItem> items, DateTime dateTime)
        {
            if (!Chariot.ReplaceFullTable(items)) return false;
            _ = Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
            return true;
        }

        public bool NAVUoMs(List<NAVUoM> uomList)
        {
            if (!Chariot.ReplaceFullTable(uomList)) return false;
            _ = Chariot.SetTableUpdateTime(typeof(NAVUoM));
            return true;
        }

        public bool NAVStock(List<NAVStock> stock)
        {
            if (!Chariot.ReplaceFullTable(stock)) return false;
            _ = Chariot.EmptyTable<BinContentsUpdate>();
            _ = Chariot.SetTableUpdateTime(typeof(NAVStock));
            _ = Chariot.SetStockUpdateTimes(stock);
            return true;
        }

        public bool NAVZone(List<NAVZone> zones) => Chariot.ReplaceFullTable(zones);

        public bool NAVLocation(List<NAVLocation> locations) => Chariot.ReplaceFullTable(locations);

        public bool NAVDivision(List<NAVDivision> divs) => Chariot.ReplaceFullTable(divs);

        public bool NAVCategory(List<NAVCategory> cats) => Chariot.ReplaceFullTable(cats);

        public bool NAVPlatform(List<NAVPlatform> pfs) => Chariot.ReplaceFullTable(pfs);

        public bool NAVGenre(List<NAVGenre> gens) => Chariot.ReplaceFullTable(gens);

    }
}
