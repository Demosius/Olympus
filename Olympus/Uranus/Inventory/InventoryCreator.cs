using Olympus.Uranus.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Uranus.Inventory
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
            if (Chariot.ReplaceFullTable(bins))
            {
                Chariot.SetTableUpdateTime(typeof(NAVBin));
                return true;
            }
            return false;
        }

        public bool NAVItems(List<NAVItem> items, DateTime dateTime)
        {
            if (Chariot.ReplaceFullTable(items))
            {
                Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
                return true;
            }
            return false;
        }

        public bool NAVUoMs(List<NAVUoM> uoms)
        {
            if (Chariot.ReplaceFullTable(uoms))
            {
                Chariot.SetTableUpdateTime(typeof(NAVUoM));
                return true;
            }
            return false;
        }

        public bool NAVStock(List<NAVStock> stock)
        {
            if (Chariot.ReplaceFullTable(stock))
            {
                Chariot.EmptyTable<BinContentsUpdate>();
                Chariot.SetTableUpdateTime(typeof(NAVStock));
                Chariot.SetStockUpdateTimes(stock);
                return true;
            }
            return false;
        }

        public bool NAVZone(List<NAVZone> zones) => Chariot.ReplaceFullTable(zones);

        public bool NAVLocation(List<NAVLocation> locations) => Chariot.ReplaceFullTable(locations);

        public bool NAVDivision(List<NAVDivision> divs) => Chariot.ReplaceFullTable(divs);

        public bool NAVCategory(List<NAVCategory> cats) => Chariot.ReplaceFullTable(cats);

        public bool NAVPlatform(List<NAVPlatform> pfs) => Chariot.ReplaceFullTable(pfs);

        public bool NAVGenre(List<NAVGenre> gens) => Chariot.ReplaceFullTable(gens);

    }
}
