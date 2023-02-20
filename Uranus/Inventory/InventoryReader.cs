using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Uranus.Inventory.Models;

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
    public NAVBin? NAVBin(string binID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVBin>(binID, pullType);

    public List<NAVBin> NAVBins(Expression<Func<NAVBin, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVBin> NAVBins(string binCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVBin>("SELECT FROM ? WHERE [Code] = ?;",
                Chariot.GetTableName(typeof(NAVBin)), binCode) ?? new List<NAVBin>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVBin>(bin => bin.Code == binCode, recursive);
    }

    /* ITEMS */
    public NAVItem? NAVItem(int itemNumber, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVItem>(itemNumber, pullType);

    public List<NAVItem> NAVItems(Expression<Func<NAVItem, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public static DateTime LastItemWriteTime(string itemCSVLocation) => File.GetLastWriteTime(itemCSVLocation);

    /* ZONES */
    // zoneId = locationCode + zoneCode
    public NAVZone? NAVZone(string zoneID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVZone>(zoneID, pullType);

    public List<NAVZone> NAVZones(Expression<Func<NAVZone, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVZone> NAVZones(string zoneCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVZone>("SELECT FROM ? WHERE [Code] = ?;",
                Chariot.GetTableName(typeof(NAVZone)), zoneCode) ?? new List<NAVZone>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVZone>(zone => zone.Code == zoneCode, recursive);
    }

    /// <summary>
    /// Zones with matched zone-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVZone> Zones(Expression<Func<NAVZone, bool>>? filter = null)
    {
        IEnumerable<NAVZone>? returnZones = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            var zones = Chariot.PullObjectList(filter).ToDictionary(z => z.ID, z => z);
            var extensions = Chariot.PullObjectList<ZoneExtension>().ToDictionary(e => e.ZoneID, e => e);
            var newExtensions = new List<ZoneExtension>();

            foreach (var (id, zone) in zones)
            {
                if (extensions.TryGetValue(id, out var extension))
                {
                    zone.Extension = extension;
                    extension.Zone = zone;
                }
                else
                {
                    extension = new ZoneExtension(zone);
                    newExtensions.Add(extension);
                }
            }

            Chariot.InsertIntoTable(newExtensions);
            returnZones = zones.Values;
        });
        returnZones ??= new List<NAVZone>();

        return returnZones;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVItem> Items(Expression<Func<NAVItem, bool>>? filter = null)
    {
        IEnumerable<NAVItem>? returnItems = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            var items = Chariot.PullObjectList(filter).ToDictionary(i => i.Number, i => i);
            var extensions = Chariot.PullObjectList<ItemExtension>().ToDictionary(e => e.ItemNumber, e => e);
            var newExtensions = new List<ItemExtension>();

            foreach (var (no, item) in items)
            {
                if (extensions.TryGetValue(no, out var extension))
                {
                    item.Extension = extension;
                    extension.Item = item;
                }
                else
                {
                    extension = new ItemExtension(item);
                    newExtensions.Add(extension);
                }
            }

            Chariot.InsertIntoTable(newExtensions);
            returnItems = items.Values;
        });
        returnItems ??= new List<NAVItem>();

        return returnItems;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVBin> Bins(Expression<Func<NAVBin, bool>>? filter = null)
    {
        IEnumerable<NAVBin>? returnBins = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            var bins = Chariot.PullObjectList(filter).ToDictionary(i => i.ID, i => i);
            var bays = Chariot.PullObjectList<Bay>().ToDictionary(b => b.ID, b => b);
            var extensions = Chariot.PullObjectList<BinExtension>().ToDictionary(e => e.BinID, e => e);
            var newExtensions = new List<BinExtension>();

            foreach (var (no, bin) in bins)
            {
                if (extensions.TryGetValue(no, out var extension))
                {
                    bin.Extension = extension;
                    extension.Bin = bin;
                }
                else
                {
                    extension = new BinExtension(bin);
                    newExtensions.Add(extension);
                }

                if (bays.TryGetValue(extension.BayID, out var bay))
                {
                    bay.BayBins.Add(extension);
                    bay.Bins.Add(bin);
                    extension.Bay = bay;
                    bin.Bay = bay;
                }
            }

            Chariot.InsertIntoTable(newExtensions);
            returnBins = bins.Values;
        });
        returnBins ??= new List<NAVBin>();

        return returnBins;
    }

    /* STOCK */
    // Stock.ID = <locationCode>:<zoneCode>:<binCode>:<itemNumber>:<uomCode>
    public NAVStock? NAVStock(string stockID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVStock>(stockID, pullType);

    // BinID = <locationCode>:<zoneCode>:<binCode> || UoMID = <itemNumber>:<uomCode>
    public NAVStock? NAVStock(string binID, string uomID, EPullType pullType = EPullType.ObjectOnly) =>
        NAVStock(string.Join(":", binID, uomID), pullType);

    // ZoneID = <locationCode>:<zoneCode> || UoMID = <itemNumber>:<uomCode>
    public NAVStock? NAVStock(string zoneID, string binCode, string uomID, EPullType pullType = EPullType.ObjectOnly) =>
        NAVStock(string.Join(":", zoneID, binCode, uomID), pullType);

    // ZoneID = <locationCode>:<zoneCode>
    public NAVStock? NAVStock(string zoneID, string binCode, int itemNumber, string uomCode,
        EPullType pullType = EPullType.ObjectOnly) =>
        NAVStock(string.Join(":", zoneID, binCode, itemNumber, uomCode), pullType);

    // UoMID = <itemNumber>:<uomCode>
    public NAVStock? NAVStock(string locationCode, string zoneCode, string binCode, string uomID,
        EPullType pullType = EPullType.ObjectOnly) =>
        NAVStock(string.Join(":", locationCode, zoneCode, binCode, uomID), pullType);

    public NAVStock? NAVStock(string locationCode, string zoneCode, string binCode, int itemNumber, string uomCode,
        EPullType pullType = EPullType.ObjectOnly) =>
        NAVStock(string.Join(":", locationCode, zoneCode, binCode, itemNumber, uomCode), pullType);

    public List<NAVStock> NAVAllStock(Expression<Func<NAVStock, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVStock> NAVItemStock(int itemNumber, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVStock>("SELECT FROM ? WHERE [ItemNumber] = ?;",
                Chariot.GetTableName(typeof(NAVZone)), itemNumber) ?? new List<NAVStock>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.ItemNumber == itemNumber, recursive);
    }

    public List<NAVStock> NAVBinStock(string binCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVStock>("SELECT FROM ? WHERE [BinCode] = ?;",
                Chariot.GetTableName(typeof(NAVZone)), binCode) ?? new List<NAVStock>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.BinCode == binCode, recursive);
    }

    /* UOM */
    public NAVUoM? NAVUoM(string uomID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVUoM>(uomID, pullType);

    public List<NAVUoM> NAVUoMs(Expression<Func<NAVUoM, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVUoM> NAVItemUoMs(int itemNumber, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVUoM>("SELECT FROM ? WHERE [ItemNumber] = ?;",
                Chariot.GetTableName(typeof(NAVUoM)), itemNumber) ?? new List<NAVUoM>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.ItemNumber == itemNumber, recursive);
    }

    public List<NAVUoM> NAVUoMsByCode(string uomCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Database?.Query<NAVUoM>("SELECT FROM ? WHERE [Code] = ?;",
                Chariot.GetTableName(typeof(NAVUoM)), uomCode) ?? new List<NAVUoM>();
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.Code == uomCode, recursive);
    }

    public List<NAVUoM> NAVUoMsByCode(EUoM eUoM, EPullType pullType = EPullType.ObjectOnly)
    {
        var uomCode = EnumConverter.UoMToString(eUoM);
        return NAVUoMsByCode(uomCode, pullType);
    }

    /* LOCATION */
    public NAVLocation? NAVLocation(string locationCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVLocation>(locationCode, pullType);

    public List<NAVLocation> NAVLocations(Expression<Func<NAVLocation, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* DIVISION */
    public NAVDivision? NAVDivision(int divCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVDivision>(divCode, pullType);

    public List<NAVDivision> NAVDivisions(Expression<Func<NAVDivision, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* CATEGORY */
    public NAVCategory? NAVCategory(int catCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVCategory>(catCode, pullType);

    public List<NAVCategory> NAVCategories(Expression<Func<NAVCategory, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* Platform */
    public NAVPlatform? NAVPlatform(int pfCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVPlatform>(pfCode, pullType);

    public List<NAVPlatform> NAVPlatforms(Expression<Func<NAVPlatform, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* GENRE */
    public NAVGenre? NAVGenre(int genCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVGenre>(genCode, pullType);

    public List<NAVGenre> NAVGenres(Expression<Func<NAVGenre, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

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
            returnVal = navItems.Select(item =>
                new SkuMaster(item, stock, uomDict, bins, divisions, categories, platforms, genres));
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

    /// <summary>
    /// Gets all sites with the appropriately attached zones (with their zone extensions).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Site> Sites(out List<NAVZone> zones)
    {
        List<NAVZone>? zoneList = null;
        Dictionary<string, Site>? siteDict = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            zoneList = Zones().ToList();
            siteDict = Chariot.PullObjectList<Site>().ToDictionary(s => s.Name, s => s);
        });
        zoneList ??= new List<NAVZone>();
        siteDict ??= new Dictionary<string, Site>();
        zones = zoneList;

        foreach (var zone in zones.Where(zone => zone.SiteName != ""))
        {
            if (!siteDict.TryGetValue(zone.SiteName, out var site))
            {
                zone.SiteName = "";
            }
            else
            {
                site.Zones.Add(zone);
                zone.Site = site;
            }
        }

        return siteDict.Values;
    }

    /// <summary>
    /// Get the full set of data required for standard FixedBinChecker functionality.
    /// </summary>
    /// <returns></returns>
    public FixedBinCheckDataSet? FixedBinCheckDataSet(List<string> fromZones, List<string> fixedZones)
    {
        FixedBinCheckDataSet? dataSet = null;

        var allZones = fromZones.Concat(fixedZones);

        Chariot.Database?.RunInTransaction(() =>
        {
            var items = Items().ToList();
            var zones = Zones(zone => allZones.Contains(zone.Code));
            var stock = Chariot.PullObjectList<NAVStock>(stock => allZones.Contains(stock.ZoneCode));
            var bins = Bins(bin => allZones.Contains(bin.ZoneCode));
            var uomList = NAVUoMs();

            dataSet = new FixedBinCheckDataSet(items, zones, bins, stock, uomList);
        });

        dataSet ??= new FixedBinCheckDataSet(new List<NAVItem>(), new List<NAVZone>(), new List<NAVBin>(),
            new List<NAVStock>(), new List<NAVUoM>());

        return dataSet;
    }

    /// <summary>
    /// Get the full set of data required for standard Hydra functionality.
    /// </summary>
    /// <returns></returns>
    public HydraDataSet? HydraDataSet(bool includeStock = true)
    {
        HydraDataSet? dataSet = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            var items = Items().ToList();
            var sites = Sites(out var zones).ToList();
            var levels = Chariot.PullObjectList<SiteItemLevel>().ToDictionary(l => (l.ItemNumber, l.SiteName), l => l);
            var stock = includeStock
                ? Chariot.PullObjectList<NAVStock>()
                : new List<NAVStock>();
            var bins = includeStock
                ? Bins()
                : new List<NAVBin>();
            var uomList = includeStock
                ? Chariot.PullObjectList<NAVUoM>()
                : new List<NAVUoM>();
            var newLevels = new List<SiteItemLevel>();
            var locations = Chariot.PullObjectList<NAVLocation>();

            foreach (var item in items)
            {
                foreach (var site in sites)
                {
                    if (levels.TryGetValue((item.Number, site.Name), out var level))
                    {
                        item.SiteLevels.Add(level);
                        site.ItemLevels.Add(level);
                        level.Site = site;
                        level.Item = item;
                    }
                    else
                    {
                        level = new SiteItemLevel(item, site);
                        newLevels.Add(level);
                    }
                }
            }

            IEnumerable<SiteItemLevel> allLevels;
            if (newLevels.Any())
            {
                Chariot.InsertIntoTable(newLevels);
                allLevels = newLevels.Concat(levels.Values);
            }
            else
            {
                allLevels = levels.Values;
            }

            dataSet = new HydraDataSet(items, sites, zones, allLevels, bins, stock, uomList, locations);
        });

        dataSet ??= new HydraDataSet(new List<NAVItem>(), new List<Site>(), new List<NAVZone>(),
            new List<SiteItemLevel>(), new List<NAVBin>(), new List<NAVStock>(), new List<NAVUoM>(),
            new List<NAVLocation>());

        return dataSet;
    }

    public BasicStockDataSet? BasicStockDataSet(List<string> zoneCodes)
    {
        BasicStockDataSet? dataSet = null;

        Chariot.Database?.RunInTransaction(() =>
        {
            var items = Items().ToList();
            var zones = Zones(zone => zoneCodes.Contains(zone.Code));
            var stock = Chariot.PullObjectList<NAVStock>(stock => zoneCodes.Contains(stock.ZoneCode));
            var bins = Bins(bin => zoneCodes.Contains(bin.ZoneCode));
            var uomList = NAVUoMs();

            dataSet = new BasicStockDataSet(items, zones, bins, stock, uomList);
        });

        dataSet ??= new BasicStockDataSet(new List<NAVItem>(), new List<NAVZone>(), new List<NAVBin>(),
            new List<NAVStock>(), new List<NAVUoM>());

        return dataSet;
    }

    public IEnumerable<MixedCarton> MixedCartons(Expression<Func<MixedCarton, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly) 
        => Chariot.PullObjectList(filter, pullType);
}