using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Uranus.Inventory.Model;

namespace Uranus.Inventory;

public class InventoryReader
{
    public InventoryChariot Chariot { get; set; }

    public InventoryReader(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    /* BINS */
    // binID is <locationCode>:<zoneCode>:<binCode>
    public NAVBin NAVBin(string binID, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVBin>(binID, pullType);

    public List<NAVBin> NAVBins(Expression<Func<NAVBin, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVBin> NAVBins(string binCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVBin>("SELECT FROM ? WHERE [Code] = ?;", Chariot.GetTableName(typeof(NAVBin)), binCode) ?? new List<NAVBin>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVBin>(bin => bin.Code == binCode, recursive);
    }

    /* ITEMS */
    public NAVItem NAVItem(int itemNumber, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVItem>(itemNumber, pullType);

    public List<NAVItem> NAVItems(Expression<Func<NAVItem, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public static DateTime LastItemWriteTime(string itemCSVLocation) => File.GetLastWriteTime(itemCSVLocation);

    /* ZONES */
    // zoneId = locationCode + zoneCode
    public NAVZone NAVZone(string zoneID, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVZone>(zoneID, pullType);

    public List<NAVZone> NAVZones(Expression<Func<NAVZone, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVZone> NAVZones(string zoneCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVZone>("SELECT FROM ? WHERE [Code] = ?;", Chariot.GetTableName(typeof(NAVZone)), zoneCode) ?? new List<NAVZone>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVZone>(zone => zone.Code == zoneCode, recursive);
    }

    /* STOCK */
    // Stock.ID = <locationCode>:<zoneCode>:<binCode>:<itemNumber>:<uomCode>
    public NAVStock NAVStock(string stockID, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVStock>(stockID, pullType);

    // BinID = <locationCode>:<zoneCode>:<binCode> || UoMID = <itemNumber>:<uomCode>
    public NAVStock NAVStock(string binID, string uomID, EPullType pullType = EPullType.ObjectOnly) => NAVStock(string.Join(":", binID, uomID), pullType);

    // ZoneID = <locationCode>:<zoneCode> || UoMID = <itemNumber>:<uomCode>
    public NAVStock NAVStock(string zoneID, string binCode, string uomID, EPullType pullType = EPullType.ObjectOnly) => NAVStock(string.Join(":", zoneID, binCode, uomID), pullType);

    // ZoneID = <locationCode>:<zoneCode>
    public NAVStock NAVStock(string zoneID, string binCode, int itemNumber, string uomCode, EPullType pullType = EPullType.ObjectOnly) => NAVStock(string.Join(":", zoneID, binCode, itemNumber, uomCode), pullType);

    // UoMID = <itemNumber>:<uomCode>
    public NAVStock NAVStock(string locationCode, string zoneCode, string binCode, string uomID, EPullType pullType = EPullType.ObjectOnly) => NAVStock(string.Join(":", locationCode, zoneCode, binCode, uomID), pullType);

    public NAVStock NAVStock(string locationCode, string zoneCode, string binCode, int itemNumber, string uomCode, EPullType pullType = EPullType.ObjectOnly) => NAVStock(string.Join(":", locationCode, zoneCode, binCode, itemNumber, uomCode), pullType);

    public List<NAVStock> NAVAllStock(Expression<Func<NAVStock, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVStock> NAVItemStock(int itemNumber, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVStock>("SELECT FROM ? WHERE [ItemNumber] = ?;", Chariot.GetTableName(typeof(NAVZone)), itemNumber) ?? new List<NAVStock>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.ItemNumber == itemNumber, recursive);
    }

    public List<NAVStock> NAVBinStock(string binCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVStock>("SELECT FROM ? WHERE [BinCode] = ?;", Chariot.GetTableName(typeof(NAVZone)), binCode) ?? new List<NAVStock>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.BinCode == binCode, recursive);
    }

    /* UOM */
    public NAVUoM NAVUoM(string uomID, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVUoM>(uomID, pullType);

    public List<NAVUoM> NAVUoMs(Expression<Func<NAVUoM, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVUoM> NAVItemUoMs(int itemNumber, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVUoM>("SELECT FROM ? WHERE [ItemNumber] = ?;", Chariot.GetTableName(typeof(NAVUoM)), itemNumber) ?? new List<NAVUoM>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.ItemNumber == itemNumber, recursive);
    }

    public List<NAVUoM> NAVUoMsByCode(string uomCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVUoM>("SELECT FROM ? WHERE [Code] = ?;", Chariot.GetTableName(typeof(NAVUoM)), uomCode) ?? new List<NAVUoM>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.Code == uomCode, recursive);
    }

    public List<NAVUoM> NAVUoMsByCode(EUoM eUoM, EPullType pullType = EPullType.ObjectOnly)
    {
        var uomCode = EnumConverter.UoMToString(eUoM);
        return NAVUoMsByCode(uomCode, pullType);
    }

    /* LOCATION */
    public NAVLocation NAVLocation(string locationCode, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVLocation>(locationCode, pullType);

    public List<NAVLocation> NAVLocations(Expression<Func<NAVLocation, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* DIVISION */
    public NAVDivision NAVDivision(int divCode, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVDivision>(divCode, pullType);

    public List<NAVDivision> NAVDivisions(Expression<Func<NAVDivision, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* CATEGORY */
    public NAVCategory NAVCategory(int catCode, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVCategory>(catCode, pullType);

    public List<NAVCategory> NAVCategories(Expression<Func<NAVCategory, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* Platform */
    public NAVPlatform NAVPlatform(int pfCode, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVPlatform>(pfCode, pullType);

    public List<NAVPlatform> NAVPlatforms(Expression<Func<NAVPlatform, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* GENRE */
    public NAVGenre NAVGenre(int genCode, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObject<NAVGenre>(genCode, pullType);

    public List<NAVGenre> NAVGenres(Expression<Func<NAVGenre, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* TABLE UPDATES */
    public DateTime LastTableUpdate(Type type)
    {
        var tableName = Chariot.GetTableName(type);
        var tableUpdate = Chariot.Database?.Find<TableUpdate>(tableName);
        return tableUpdate?.LastUpdate ?? new DateTime();
    }

    /* STOCK UPDATES */
    public DateTime LastStockUpdate(List<string> zoneIDs)
    {
        // NAV list of stock updates
        var contentsUpdates = Chariot.Database?.Table<BinContentsUpdate>().ToList() ?? new List<BinContentsUpdate>();

        // If one of the zoneIds is not present in list, it isn't present so return new datetime value.
        var existZones = contentsUpdates.Select(bcu => bcu.ZoneID).ToList();

        // NAV the min/smallest/oldest update time/s from the list.
        return zoneIDs.Any(zoneID => !existZones.Contains(zoneID))
            ? new DateTime()
            : contentsUpdates.Where(bcu => zoneIDs.Contains(bcu.ZoneID)).ToList().Min()!.LastUpdate;
    }

    /// <summary>
    /// Pulls multiple sets of data from the inventory database and combines them into the SKUMaster data set.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SkuMaster> GetMasters()
    {
        IEnumerable<SkuMaster> returnVal = new List<SkuMaster>();

        Chariot.Database?.RunInTransaction(() =>
        {
            var navItems = NAVItems();
            var stock = NAVAllStock()
                .GroupBy(s => s.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToList());
            var uomDict = NAVUoMs()
                .GroupBy(u => u.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToDictionary(u => u.Code, u => u));
            var bins = NAVBins().ToDictionary(b => b.ID, b => b);
            var divisions = NAVDivisions().ToDictionary(d => d.Code, d => d.Description);
            var categories = NAVCategories().ToDictionary(c => c.Code, c => c.Description);
            var platforms = NAVPlatforms().ToDictionary(p => p.Code, p => p.Description);
            var genres = NAVGenres().ToDictionary(g => g.Code, g => g.Description);
            returnVal = navItems.Select(item => new SkuMaster(item, stock, uomDict, bins, divisions, categories, platforms, genres));
        });

        return returnVal;
    }

    /// <summary>
    /// Pulls multiple sets of data from the inventory database and combines them into the SKUMaster data set.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SkuMaster> ImplicitGetMasters()
    {
        return NAVItems(pullType: EPullType.IncludeChildren).Select(i => new SkuMaster(i));
    }
}