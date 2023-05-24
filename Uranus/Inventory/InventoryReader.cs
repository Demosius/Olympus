using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

    public async Task<List<NAVBin>> NAVBinsAsync(Expression<Func<NAVBin, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVBin> NAVBins(Expression<Func<NAVBin, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public async Task<List<NAVBin>> NAVBinsAsync(string binCode, EPullType pullType = EPullType.ObjectOnly) =>
        await NAVBinsAsync(b => b.Code == binCode, pullType).ConfigureAwait(false);

    /* ITEMS */
    public NAVItem? NAVItem(int itemNumber, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVItem>(itemNumber, pullType);

    public async Task<List<NAVItem>> NAVItemsAsync(Expression<Func<NAVItem, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVItem> NAVItems(Expression<Func<NAVItem, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public static DateTime LastItemWriteTime(string itemCSVLocation) => File.GetLastWriteTime(itemCSVLocation);

    /* ZONES */
    // zoneId = locationCode + zoneCode
    public NAVZone? NAVZone(string zoneID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVZone>(zoneID, pullType);

    public async Task<List<NAVZone>> NAVZonesAsync(Expression<Func<NAVZone, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public async Task<List<NAVZone>> NAVZonesAsync(string zoneCode, EPullType pullType = EPullType.ObjectOnly) =>
        await NAVZonesAsync(z => z.Code == zoneCode, pullType).ConfigureAwait(false);

    /// <summary>
    /// Zones with matched zone-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVZone>> ZonesAsync(Expression<Func<NAVZone, bool>>? filter = null)
    {
        IEnumerable<NAVZone>? returnZones = null;
        var newExtensions = new List<ZoneExtension>();

        void Action()
        {
            var zones = Chariot.PullObjectList(filter).ToDictionary(z => z.ID, z => z);
            var extensions = Chariot.PullObjectList<ZoneExtension>().ToDictionary(e => e.ZoneID, e => e);

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

            returnZones = zones.Values;
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        if (newExtensions.Any()) await Chariot.InsertIntoTableAsync(newExtensions);

        returnZones ??= new List<NAVZone>();

        return returnZones;
    }

    public IEnumerable<NAVZone> Zones(out List<ZoneExtension> createdExtensions, Expression<Func<NAVZone, bool>>? filter = null)
    {
        IEnumerable<NAVZone>? returnZones = null;
        var newExtensions = new List<ZoneExtension>();

        void Action()
        {
            var zones = Chariot.PullObjectList(filter).ToDictionary(z => z.ID, z => z);
            //var zones = new Dictionary<string, NAVZone>();
            var extensions = Chariot.PullObjectList<ZoneExtension>().ToDictionary(e => e.ZoneID, e => e);
            //var extensions = new Dictionary<string, ZoneExtension>();

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

            returnZones = zones.Values;
        }

        Chariot.RunInTransaction(Action);

        createdExtensions = newExtensions;

        returnZones ??= new List<NAVZone>();

        return returnZones;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVItem>> ItemsAsync(Expression<Func<NAVItem, bool>>? filter = null)
    {
        var items = new Dictionary<int, NAVItem>();

        void Action()
        {
            items = Chariot.PullObjectList(filter).ToDictionary(i => i.Number, i => i);
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
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return items.Values;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVItem> Items(Expression<Func<NAVItem, bool>>? filter = null)
    {
        var items = new Dictionary<int, NAVItem>();

        void Action()
        {
            items = Chariot.PullObjectList(filter).ToDictionary(i => i.Number, i => i);
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
        }

        Chariot.RunInTransaction(Action);

        return items.Values;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVBin>> BinsAsync(Expression<Func<NAVBin, bool>>? filter = null)
    {
        IEnumerable<NAVBin>? returnBins = null;

        void Action()
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

                if (!bays.TryGetValue(extension.BayID, out var bay)) continue;

                bay.BayBins.Add(extension);
                bay.Bins.Add(bin);
                extension.Bay = bay;
                bin.Bay = bay;
            }

            Chariot.InsertIntoTable(newExtensions);
            returnBins = bins.Values;
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        returnBins ??= new List<NAVBin>();

        return returnBins;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVBin> Bins(out List<BinExtension> createdExtensions, Expression<Func<NAVBin, bool>>? filter = null)
    {
        IEnumerable<NAVBin>? returnBins = null;
        var newExtensions = new List<BinExtension>();

        void Action()
        {
            var bins = Chariot.PullObjectList(filter).ToDictionary(i => i.ID, i => i);
            var bays = Chariot.PullObjectList<Bay>().ToDictionary(b => b.ID, b => b);
            var extensions = Chariot.PullObjectList<BinExtension>().ToDictionary(e => e.BinID, e => e);

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

                if (!bays.TryGetValue(extension.BayID, out var bay)) continue;

                bay.BayBins.Add(extension);
                bay.Bins.Add(bin);
                extension.Bay = bay;
                bin.Bay = bay;
            }

            returnBins = bins.Values;
        }

        Chariot.RunInTransaction(Action);
        returnBins ??= new List<NAVBin>();

        createdExtensions = newExtensions;

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

    public async Task<List<NAVStock>> NAVAllStockAsync(Expression<Func<NAVStock, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVStock> NAVAllStock(Expression<Func<NAVStock, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVStock> NAVItemStock(int itemNumber, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Query<NAVStock>("SELECT FROM ? WHERE [ItemNumber] = ?;",
                Chariot.GetTableName(typeof(NAVZone)), itemNumber);
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.ItemNumber == itemNumber, recursive);
    }

    public List<NAVStock> NAVBinStock(string binCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Query<NAVStock>("SELECT FROM ? WHERE [BinCode] = ?;",
                Chariot.GetTableName(typeof(NAVZone)), binCode);
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVStock>(stock => stock.BinCode == binCode, recursive);
    }

    /* UOM */
    public NAVUoM? NAVUoM(string uomID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVUoM>(uomID, pullType);

    public async Task<List<NAVUoM>> NAVUoMsAsync(Expression<Func<NAVUoM, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVUoM> NAVUoMs(Expression<Func<NAVUoM, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    public List<NAVUoM> NAVItemUoMs(int itemNumber, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Query<NAVUoM>("SELECT FROM ? WHERE [ItemNumber] = ?;",
                Chariot.GetTableName(typeof(NAVUoM)), itemNumber);
        var recursive = pullType == EPullType.FullRecursive;
        return Chariot.Database.GetAllWithChildren<NAVUoM>(uom => uom.ItemNumber == itemNumber, recursive);
    }

    public List<NAVUoM> NAVUoMsByCode(string uomCode, EPullType pullType = EPullType.ObjectOnly)
    {
        if (pullType == EPullType.ObjectOnly)
            return Chariot.Query<NAVUoM>("SELECT FROM ? WHERE [Code] = ?;",
                Chariot.GetTableName(typeof(NAVUoM)), uomCode);
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

    public async Task<List<NAVLocation>> NAVLocationsAsync(Expression<Func<NAVLocation, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    /* DIVISION */
    public NAVDivision? NAVDivision(int divCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVDivision>(divCode, pullType);

    public async Task<List<NAVDivision>> NAVDivisionsAsync(Expression<Func<NAVDivision, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVDivision> NAVDivisions(Expression<Func<NAVDivision, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* CATEGORY */
    public NAVCategory? NAVCategory(int catCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVCategory>(catCode, pullType);

    public async Task<List<NAVCategory>> NAVCategoriesAsync(Expression<Func<NAVCategory, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVCategory> NAVCategories(Expression<Func<NAVCategory, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* Platform */
    public NAVPlatform? NAVPlatform(int pfCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVPlatform>(pfCode, pullType);

    public async Task<List<NAVPlatform>> NAVPlatformsAsync(Expression<Func<NAVPlatform, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVPlatform> NAVPlatforms(Expression<Func<NAVPlatform, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* GENRE */
    public NAVGenre? NAVGenre(int genCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVGenre>(genCode, pullType);

    public async Task<List<NAVGenre>> NAVGenresAsync(Expression<Func<NAVGenre, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

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

    /* Transfer Order Lines */
    public async Task<List<NAVTransferOrder>> TOListAsync(Expression<Func<NAVTransferOrder, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<NAVTransferOrder> TOList(Expression<Func<NAVTransferOrder, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /* STORES */
    public async Task<List<Store>> StoresAsync(Expression<Func<Store, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public List<Store> Stores(Expression<Func<Store, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => Chariot.PullObjectList(filter, pullType);

    /// <summary>
    /// Pulls multiple sets of data from the inventory database and combines them into the SKUMaster data set.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SkuMaster>> GetMastersAsync()
    {
        IEnumerable<SkuMaster> returnVal = new List<SkuMaster>();

        void Action()
        {
            var items = NAVItems();
            var stockList = NAVAllStock();
            var uomList = NAVUoMs();
            var binList = NAVBins();
            var divisionList = NAVDivisions();
            var categoryList = NAVCategories();
            var platformList = NAVPlatforms();
            var genreList = NAVGenres();

            var stock = stockList.GroupBy(s => s.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToList());
            var uomDict = uomList.GroupBy(u => u.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToDictionary(u => u.Code, u => u));
            var bins = binList.ToDictionary(b => b.ID, b => b);
            var divisions = divisionList.ToDictionary(d => d.Code, d => d.Description);
            var categories = categoryList.ToDictionary(c => c.Code, c => c.Description);
            var platforms = platformList.ToDictionary(p => p.Code, p => p.Description);
            var genres = genreList.ToDictionary(g => g.Code, g => g.Description);

            returnVal = items.Select(item => new SkuMaster(item, stock, uomDict, bins, divisions, categories, platforms, genres));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return returnVal;
    }
    /// <summary>
    /// Pulls multiple sets of data from the inventory database and combines them into the SKUMaster data set.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SkuMaster>> ImplicitGetMastersAsync() =>
        (await NAVItemsAsync(pullType: EPullType.IncludeChildren).ConfigureAwait(false)).Select(i => new SkuMaster(i));


    /// <summary>
    /// Gets all sites with the appropriately attached zones (with their zone extensions).
    /// </summary>
    /// <returns>Tuple (Sites, Zones)</returns>
    public async Task<(IEnumerable<Site>, IEnumerable<NAVZone>)> SitesAsync()
    {
        var zones = new List<NAVZone>();
        var siteDict = new Dictionary<string, Site>();
        var newExtensions = new List<ZoneExtension>();

        void Action()
        {
            zones = Zones(out newExtensions).ToList();
            siteDict = Chariot.PullObjectList<Site>().ToDictionary(s => s.Name, s => s);

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
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        if (newExtensions.Any()) await Chariot.UpdateTableAsync(newExtensions).ConfigureAwait(false);

        return (siteDict.Values, zones);
    }

    public (IEnumerable<Site>, IEnumerable<NAVZone>) Sites(out List<ZoneExtension> createdZoneExtensions)
    {
        var zones = new List<NAVZone>();
        var siteDict = new Dictionary<string, Site>();
        var zoneExtensions = new List<ZoneExtension>();

        void Action()
        {
            zones = Zones(out zoneExtensions).ToList();
            siteDict = Chariot.PullObjectList<Site>().ToDictionary(s => s.Name, s => s);

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
        }

        Chariot.RunInTransaction(Action);
        createdZoneExtensions = zoneExtensions;

        return (siteDict.Values, zones);
    }

    /// <summary>
    /// Get the full set of data required for standard FixedBinChecker functionality.
    /// </summary>
    /// <returns></returns>
    public async Task<FixedBinCheckDataSet?> FixedBinCheckDataSetAsync(List<string> fromZones, List<string> fixedZones)
    {
        var dataSet = new FixedBinCheckDataSet();
        var newZoneExtensions = new List<ZoneExtension>();
        var newBinExtensions = new List<BinExtension>();

        void Action()
        {
            var allZones = fromZones.Concat(fixedZones);

            var items = Items();
            var zones = Zones(out newZoneExtensions, zone => allZones.Contains(zone.Code));
            var stock = Chariot.PullObjectList<NAVStock>(stock => allZones.Contains(stock.ZoneCode));
            var bins = Bins(out newBinExtensions, bin => allZones.Contains(bin.ZoneCode));
            var navUoMs = NAVUoMs();

            dataSet = new FixedBinCheckDataSet(items, zones, bins, stock, navUoMs);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        if (newZoneExtensions.Any() || newBinExtensions.Any())
        {
            await Task.Run(() =>
            {
                Chariot.RunInTransaction(() =>
                {
                    Chariot.UpdateTable(newZoneExtensions);
                    Chariot.UpdateTable(newBinExtensions);
                });
            }).ConfigureAwait(false);
        }

        return dataSet;
    }

    /// <summary>
    /// Get stock relevant to Hydra Data.
    /// </summary>
    /// <returns>Tuple: (Stock, Bins, UoMs)</returns>
    public async Task<(List<NAVStock>, IEnumerable<NAVBin>, List<NAVUoM>)> HydraStockAsync()
    {
        var stock = new List<NAVStock>();
        var bins = new List<NAVBin>();
        var uomList = new List<NAVUoM>();
        var newBinExtensions = new List<BinExtension>();

        void Action()
        {
            stock = Chariot.PullObjectList<NAVStock>();
            bins = Bins(out newBinExtensions).ToList();
            uomList = Chariot.PullObjectList<NAVUoM>();
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        if (newBinExtensions.Any()) await Chariot.UpdateTableAsync(newBinExtensions);

        return (stock, bins, uomList);
    }

    public (List<NAVStock>, IEnumerable<NAVBin>, List<NAVUoM>) HydraStock(out List<BinExtension> createdBinExtensions)
    {
        var stock = new List<NAVStock>();
        var bins = new List<NAVBin>();
        var uomList = new List<NAVUoM>();
        var newBinExtensions = new List<BinExtension>();

        void Action()
        {
            stock = Chariot.PullObjectList<NAVStock>();
            bins = Bins(out newBinExtensions).ToList();
            uomList = Chariot.PullObjectList<NAVUoM>();
        }

        Chariot.RunInTransaction(Action);
        createdBinExtensions = newBinExtensions;

        return (stock, bins, uomList);
    }


    /// <summary>
    /// Get the full set of data required for standard Hydra functionality.
    /// </summary>
    /// <returns></returns>
    public async Task<HydraDataSet?> HydraDataSetAsync(bool includeStock = true)
    {
        var dataSet = new HydraDataSet();
        var newZoneExtensions = new List<ZoneExtension>();
        var newBinExtensions = new List<BinExtension>();

        void Action()
        {
            var (stock, bins, uomList) = includeStock
                ? HydraStock(out newBinExtensions)
                : (new List<NAVStock>(), new List<NAVBin>(), new List<NAVUoM>());

            var (s, zones) = Sites(out newZoneExtensions);
            var sites = s.ToList();
            var items = Items().ToList();
            var levels = Chariot.PullObjectList<SiteItemLevel>().ToDictionary(l => (l.ItemNumber, l.SiteName), l => l);
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
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        if (newZoneExtensions.Any() || newBinExtensions.Any())
        {
            await Task.Run(() =>
            {
                Chariot.RunInTransaction(() =>
                {
                    Chariot.UpdateTable(newZoneExtensions);
                    Chariot.UpdateTable(newBinExtensions);
                });
            }).ConfigureAwait(false);
        }

        return dataSet;
    }

    public async Task<BasicStockDataSet?> BasicStockDataSetAsync(IEnumerable<string> zoneCodes, IEnumerable<string> locations)
    {
        var dataSet = new BasicStockDataSet();
        var newZoneExtensions = new List<ZoneExtension>();
        var newBinExtensions = new List<BinExtension>();

        void Action()
        {
            var items = Items();
            var zones = Zones(out newZoneExtensions, zone => zoneCodes.Contains(zone.Code) && locations.Contains(zone.LocationCode));
            var stock = Chariot.PullObjectList<NAVStock>(stock =>
                zoneCodes.Contains(stock.ZoneCode) && locations.Contains(stock.LocationCode));
            var bins = Bins(out newBinExtensions, bin => zoneCodes.Contains(bin.ZoneCode) && locations.Contains(bin.LocationCode));
            var uomList = NAVUoMs();

            dataSet = new BasicStockDataSet(items, zones, bins, stock, uomList);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        if (newZoneExtensions.Any() || newBinExtensions.Any())
        {
            await Task.Run(() =>
            {
                Chariot.RunInTransaction(() =>
                {
                    Chariot.UpdateTable(newZoneExtensions);
                    Chariot.UpdateTable(newBinExtensions);
                });
            }).ConfigureAwait(false);
        }

        return dataSet;
    }

    public async Task<TOStockDataSet?> TOStockDataSetAsync(IEnumerable<string> zoneCodes, IEnumerable<string> locations)
    {
        var dataSet = new TOStockDataSet();
        var newZoneExtensions = new List<ZoneExtension>();
        var newBinExtensions = new List<BinExtension>();

        void Action()
        {
            var items = Items();
            var zones = Zones(out newZoneExtensions, zone => zoneCodes.Contains(zone.Code) && locations.Contains(zone.LocationCode));
            var stock = Chariot.PullObjectList<NAVStock>(stock =>
                zoneCodes.Contains(stock.ZoneCode) && locations.Contains(stock.LocationCode));
            var bins = Bins(out newBinExtensions, bin => zoneCodes.Contains(bin.ZoneCode) && locations.Contains(bin.LocationCode));
            var uomList = NAVUoMs();
            var toList = TOList();
            var stores = Stores();
            var platforms = NAVPlatforms();
            var divisions = NAVDivisions();
            var categories = NAVCategories();
            var genres = NAVGenres();

            dataSet = new TOStockDataSet(items, zones, bins, stock, uomList, toList,
                 stores, platforms, genres, categories, divisions);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        if (newZoneExtensions.Any() || newBinExtensions.Any())
        {
            await Task.Run(() =>
            {
                Chariot.RunInTransaction(() =>
                {
                    Chariot.UpdateTable(newZoneExtensions);
                    Chariot.UpdateTable(newBinExtensions);
                });
            }).ConfigureAwait(false);
        }

        return dataSet;
    }

    public int TOLineCount() => Chariot.ExecuteScalar<int>("SELECT count(*) FROM TOLineBatchAnalysis;");

    public async Task<IEnumerable<MixedCarton>> MixedCartonsAsync(Expression<Func<MixedCarton, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public IEnumerable<MixedCarton> MixedCartons(Expression<Func<MixedCarton, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => Chariot.PullObjectList(filter, pullType);

    public async Task<IEnumerable<MixedCartonItem>> MixedCartonItemsAsync(Expression<Func<MixedCartonItem, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter, pullType).ConfigureAwait(false);

    public IEnumerable<MixedCartonItem> MixedCartonItems(Expression<Func<MixedCartonItem, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => Chariot.PullObjectList(filter, pullType);

    public async Task<IEnumerable<MixedCarton>> MixedCartonTemplatesAsync(Expression<Func<MixedCarton, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
    {
        var mixedCartons = new Dictionary<Guid, MixedCarton>();

        void Action()
        {
            mixedCartons = MixedCartons(filter, pullType).ToDictionary(mc => mc.ID, mc => mc);
            var ids = mixedCartons.Keys.ToList();
            var mcItems = MixedCartonItems(item => ids.Contains(item.MixedCartonID)).ToList();
            var itemNumbers = mcItems.Select(item => item.ItemNumber).Distinct();
            var items = Items(i => itemNumbers.Contains(i.Number)).ToDictionary(i => i.Number, i => i);

            foreach (var mixedCartonItem in mcItems)
            {
                if (items.TryGetValue(mixedCartonItem.ItemNumber, out var item))
                {
                    mixedCartonItem.Item = item;
                    item.MixedCartons.Add(mixedCartonItem);
                }

                if (!mixedCartons.TryGetValue(mixedCartonItem.MixedCartonID, out var mixedCarton)) continue;
                mixedCartonItem.MixedCarton = mixedCarton;
                mixedCarton.Items.Add(mixedCartonItem);
            }
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return mixedCartons.Values;
    }

    public async Task<(List<MixedCarton>, List<MixedCartonItem>, List<NAVItem>)> GetMixedCartonDataAsync()
    {
        var items = new List<NAVItem>();
        var mcList = new List<MixedCarton>();
        var mcItems = new List<MixedCartonItem>();

        void Action()
        {
            items = Items().ToList();
            mcList = MixedCartons().ToList();
            mcItems = MixedCartonItems().ToList();
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return (mcList, mcItems, items);
    }
}