using Uranus.Inventory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Inventory
{
    public class InventoryUpdater
    {
        public InventoryChariot Chariot { get; set; }

        public InventoryUpdater(ref InventoryChariot chariot)
        {
            Chariot = chariot;
        }

        public bool NAVBins(List<NAVBin> bins)
        {
            if (Chariot.UpdateTable(bins))
            {
                _ = Chariot.SetTableUpdateTime(typeof(NAVBin));
                return true;
            }
            return false;
        }

        public bool NAVItems(List<NAVItem> items, DateTime dateTime)
        {
            if (Chariot.UpdateTable(items))
            {
                _ = Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
                return true;
            }
            return false;
        }

        public bool NAVUoMs(List<NAVUoM> uoms)
        {
            if (uoms.Count == 0) return false;
            // Remove previous data with relevant UoMCode 
            // (Expect for ease/speed updates will happen one UoMCode at a time)
            Chariot.UoMCodeDelete(uoms.Select(s => s.Code).Distinct().ToList());
            if (Chariot.InsertIntoTable(uoms))
            {
                _ = Chariot.SetTableUpdateTime(typeof(NAVUoM));
                return true;
            }
            return false;
        }

        public bool NAVStock(List<NAVStock> stock)
        {
            if (stock.Count == 0) return false;
            // Remove from stock table anything with zones equal to what is being put in.
            Chariot.StockZoneDeletes(stock.Select(s => s.ZoneID).Distinct().ToList());
            if (Chariot.InsertIntoTable(stock))  
            {
                _ = Chariot.SetTableUpdateTime(typeof(NAVStock));
                _ = Chariot.SetStockUpdateTimes(stock);
                return true;
            }
            return false;
        }

        public bool NAVZones(List<NAVZone> zones) => Chariot.UpdateTable(zones);

        public bool NAVLocation(List<NAVLocation> locations) => Chariot.UpdateTable(locations);

        public bool NAVDivision(List<NAVDivision> divs) => Chariot.UpdateTable(divs);

        public bool NAVCategory(List<NAVCategory> cats) => Chariot.UpdateTable(cats);

        public bool NAVPlatform(List<NAVPlatform> pfs)  => Chariot.UpdateTable(pfs);

        public bool NAVGenre(List<NAVGenre> gens) => Chariot.UpdateTable(gens);

    }
}
