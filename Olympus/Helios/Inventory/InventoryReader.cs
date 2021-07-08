using Olympus.Helios.Inventory.Model;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory
{
    public class InventoryReader
    {
        public InventoryChariot Chariot { get; set; }

        public InventoryReader(ref InventoryChariot chariot)
        {
            Chariot = chariot;
        }

        /* BINS */
        // binID is <locationCode>:<zoneCode>:<binCode>
        public NAVBin NAVBin(string binID, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVBin>(binID, pullType);

        public List<NAVBin> NAVBins(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVBin>(pullType);

        public List<NAVBin> NAVBins(string binCode, PullType pullType = PullType.ObjectOnly)
        {
            if (pullType == PullType.ObjectOnly)
                return Chariot.Database.Query<NAVBin>("SELECT FROM ? WHERE [Code] = ?;", Chariot.GetTableName(typeof(NAVBin)), binCode);
            bool recursive = pullType == PullType.FullRecursive;
            return Chariot.Database.GetAllWithChildren<NAVBin>(bin => bin.Code == binCode, recursive);
        }

        /* ITEMS */
        public NAVItem NAVItem(int itemNumber, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVItem>(itemNumber, pullType);

        public List<NAVItem> NAVItems(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVItem>(pullType);

        /* ZONES */
        // zoneId = locationCode + zoneCode
        public NAVZone NAVZone(string zoneID, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVZone>(zoneID, pullType);

        public List<NAVZone> NAVZones(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVZone>(pullType);

        public List<NAVZone> NAVZones(string zoneCode, PullType pullType = PullType.ObjectOnly)
        {
            if (pullType == PullType.ObjectOnly)
                return Chariot.Database.Query<NAVZone>("SELECT FROM ? WHERE [Code] = ?;", Chariot.GetTableName(typeof(NAVZone)), zoneCode);
            bool recursive = pullType == PullType.FullRecursive;
            return Chariot.Database.GetAllWithChildren<NAVZone>(zone => zone.Code == zoneCode, recursive);
        }

        /* STOCK */
        // Stock.ID = <locationCode>:<zoneCode>:<binCode>:<itemNumber>:<uomCode>
        public NAVStock NAVStock(string stockID, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVStock>(stockID, pullType);

        // BinID = <locationCode>:<zoneCode>:<binCode> || UoMID = <itemNumber>:<uomCode>
        public NAVStock NAVStock(string binID, string uomID, PullType pullType = PullType.ObjectOnly) => NAVStock(string.Join(":", binID, uomID), pullType);

        // ZoneID = <locationCode>:<zoneCode> || UoMID = <itemNumber>:<uomCode>
        public NAVStock NAVStock(string zoneID, string binCode, string uomID, PullType pullType = PullType.ObjectOnly) => NAVStock(string.Join(":", zoneID, binCode, uomID), pullType);

        // ZoneID = <locationCode>:<zoneCode>
        public NAVStock NAVStock(string zoneID, string binCode, int itemNumber, string uomCode, PullType pullType = PullType.ObjectOnly) => NAVStock(string.Join(":", zoneID, binCode, itemNumber, uomCode), pullType);
    
        // UoMID = <itemNumber>:<uomCode>
        public NAVStock NAVStock(string locationCode, string zoneCode, string binCode, string uomID, PullType pullType = PullType.ObjectOnly) => NAVStock(string.Join(":", locationCode, zoneCode, binCode, uomID), pullType);

        public NAVStock NAVStock(string locationCode, string zoneCode, string binCode, int itemNumber, string uomCode, PullType pullType = PullType.ObjectOnly) => NAVStock(string.Join(":", locationCode, zoneCode, binCode, itemNumber, uomCode), pullType);

        public List<NAVStock> NAVAllStock(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVStock>(pullType);
       
        public List<NAVStock> NAVItemStock(int itemNumber, PullType pullType = PullType.ObjectOnly)
        {
            if (pullType == PullType.ObjectOnly)
                return Chariot.Database.Query<NAVStock>("SELECT FROM ? WHERE [ItemNumber] = ?;", Chariot.GetTableName(typeof(NAVZone)), itemNumber);
            bool recursive = pullType == PullType.FullRecursive;
            return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.ItemNumber == itemNumber, recursive);
        }

        public List<NAVStock> NAVBinStock(string binCode, PullType pullType = PullType.ObjectOnly)
        {
            if (pullType == PullType.ObjectOnly)
                return Chariot.Database.Query<NAVStock>("SELECT FROM ? WHERE [BinCode] = ?;", Chariot.GetTableName(typeof(NAVZone)), binCode);
            bool recursive = pullType == PullType.FullRecursive;
            return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.BinCode == binCode, recursive);
        }

        /* UOM */
        public NAVUoM NAVUoM(string uomID, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVUoM>(uomID, pullType);
       
        public List<NAVUoM> NAVUoMs(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVUoM>(pullType);

        public List<NAVUoM> NAVItemUoMs(int itemNumber, PullType pullType = PullType.ObjectOnly)
        {
            if (pullType == PullType.ObjectOnly)
                return Chariot.Database.Query<NAVUoM>("SELECT FROM ? WHERE [ItemNumber] = ?;", Chariot.GetTableName(typeof(NAVUoM)), itemNumber);
            bool recursive = pullType == PullType.FullRecursive;
            return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.ItemNumber == itemNumber, recursive);
        }

        public List<NAVUoM> NAVUoMsByCode(string uomCode, PullType pullType = PullType.ObjectOnly)
        {
            if (pullType == PullType.ObjectOnly)
                return Chariot.Database.Query<NAVUoM>("SELECT FROM ? WHERE [Code] = ?;", Chariot.GetTableName(typeof(NAVUoM)), uomCode);
            bool recursive = pullType == PullType.FullRecursive;
            return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.Code == uomCode, recursive);
        }

        public List<NAVUoM> NAVUoMsByCode(EUoM eUoM, PullType pullType = PullType.ObjectOnly)
        {
            string uomCode = Chariot.GetUoMString(eUoM);
            return NAVUoMsByCode(uomCode, pullType);
        }

        /* LOCATION */
        public NAVLocation NAVLocation(string locationCode, PullType pullType = PullType.ObjectOnly)  => Chariot.PullObject<NAVLocation>(locationCode, pullType);

        public List<NAVLocation> NAVLocations(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVLocation>(pullType);

        /* DIVISION */
        public NAVDivision NAVDivision(int divCode, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVDivision>(divCode, pullType);
        
        public List<NAVDivision> NAVDivisions(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVDivision>(pullType);

        /* CATEGORY */
        public NAVCategory NAVCategory(int catCode, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVCategory>(catCode, pullType);
        
        public List<NAVCategory> NAVCategorys(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVCategory>(pullType);

        /* Platform */
        public NAVPlatform NAVPlatform(int pfCode, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVPlatform>(pfCode, pullType);

        public List<NAVPlatform> NAVPlatforms(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVPlatform>(pullType);

        /* GENRE */
        public NAVGenre NAVGenre(int genCode, PullType pullType = PullType.ObjectOnly) => Chariot.PullObject<NAVGenre>(genCode, pullType);

        public List<NAVGenre> NAVGenres(PullType pullType = PullType.ObjectOnly) => Chariot.PullObjectList<NAVGenre>(pullType);

        /* TABLE UPDATES */
        public DateTime LastTableUpdate(Type type)
        {
            try
            {
                string tableName = Chariot.GetTableName(type);
                TableUpdate tableUpdate = Chariot.Database.Find<TableUpdate>(tableName);
                return tableUpdate.LastUpdate;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new DateTime();
            }
        }

        /* STOCK UPDATES */
        public DateTime LastStockUpdate(List<string> zoneIDs)
        {
            // NAV list of stock updates
            List<BinContentsUpdate> contentsUpdates = Chariot.Database.Table<BinContentsUpdate>().ToList();
            // If one of the zoneIds is not present in list, it isn't present so return new datetime value.
            List<string> existZones = contentsUpdates.Select(bcu => bcu.ZoneID).ToList();
            foreach (string zoneID in zoneIDs)
            {
                if (!existZones.Contains(zoneID))
                    return new DateTime();
            }
            // NAV the min/smallest/oldest update time/s from the list.
            return contentsUpdates.Where(bcu => zoneIDs.Contains(bcu.ZoneID)).ToList().Min().LastUpdate;
        }

    }
}
