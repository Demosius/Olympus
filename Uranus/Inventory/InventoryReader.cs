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
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task<List<NAVBin>> NAVBinsAsync(string binCode, EPullType pullType = EPullType.ObjectOnly) =>
        await NAVBinsAsync(b => b.Code == binCode, pullType);

    /* ITEMS */
    public NAVItem? NAVItem(int itemNumber, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVItem>(itemNumber, pullType);

    public async Task<List<NAVItem>> NAVItemsAsync(Expression<Func<NAVItem, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public static DateTime LastItemWriteTime(string itemCSVLocation) => File.GetLastWriteTime(itemCSVLocation);

    /* ZONES */
    // zoneId = locationCode + zoneCode
    public NAVZone? NAVZone(string zoneID, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVZone>(zoneID, pullType);

    public async Task<List<NAVZone>> NAVZonesAsync(Expression<Func<NAVZone, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task<List<NAVZone>> NAVZonesAsync(string zoneCode, EPullType pullType = EPullType.ObjectOnly) =>
        await NAVZonesAsync(z => z.Code == zoneCode, pullType);

    /// <summary>
    /// Zones with matched zone-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVZone>> ZonesAsync(Expression<Func<NAVZone, bool>>? filter = null)
    {
        IEnumerable<NAVZone>? returnZones = null;

        async void Action()
        {
            var zoneTask = Chariot.PullObjectListAsync(filter);
            var extensionTask = Chariot.PullObjectListAsync<ZoneExtension>();
            var newExtensions = new List<ZoneExtension>();

            await Task.WhenAll(zoneTask, extensionTask);

            var zones = (await zoneTask).ToDictionary(z => z.ID, z => z);
            var extensions = (await extensionTask).ToDictionary(e => e.ZoneID, e => e);

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

            _ = Chariot.InsertIntoTableAsync(newExtensions);
            returnZones = zones.Values;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        returnZones ??= new List<NAVZone>();

        return returnZones;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVItem>> ItemsAsync(Expression<Func<NAVItem, bool>>? filter = null)
    {
        IEnumerable<NAVItem>? returnItems = null;

        async void Action()
        {
            var itemTask = Chariot.PullObjectListAsync(filter);
            var extensionTask = Chariot.PullObjectListAsync<ItemExtension>();
            var newExtensions = new List<ItemExtension>();

            await Task.WhenAll(itemTask, extensionTask);

            var items = (await itemTask).ToDictionary(i => i.Number, i => i);
            var extensions = (await extensionTask).ToDictionary(e => e.ItemNumber, e => e);

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

            _ = Chariot.InsertIntoTableAsync(newExtensions);
            returnItems = items.Values;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        returnItems ??= new List<NAVItem>();

        return returnItems;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVBin>> BinsAsync(Expression<Func<NAVBin, bool>>? filter = null)
    {
        IEnumerable<NAVBin>? returnBins = null;

        async void Action()
        {
            var binTask = Chariot.PullObjectListAsync(filter);
            var bayTask = Chariot.PullObjectListAsync<Bay>();
            var extensionTask = Chariot.PullObjectListAsync<BinExtension>();
            var newExtensions = new List<BinExtension>();

            await Task.WhenAll(bayTask, extensionTask, binTask);

            var bins = (await binTask).ToDictionary(i => i.ID, i => i);
            var bays = (await bayTask).ToDictionary(b => b.ID, b => b);
            var extensions = (await extensionTask).ToDictionary(e => e.BinID, e => e);

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

            _ = Chariot.InsertIntoTableAsync(newExtensions);
            returnBins = bins.Values;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
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

    public async Task<List<NAVStock>> NAVAllStockAsync(Expression<Func<NAVStock, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

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

    public async Task<List<NAVUoM>> NAVUoMsAsync(Expression<Func<NAVUoM, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

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

    public async Task<List<NAVLocation>> NAVLocationsAsync(Expression<Func<NAVLocation, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /* DIVISION */
    public NAVDivision? NAVDivision(int divCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVDivision>(divCode, pullType);

    public async Task<List<NAVDivision>> NAVDivisionsAsync(Expression<Func<NAVDivision, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /* CATEGORY */
    public NAVCategory? NAVCategory(int catCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVCategory>(catCode, pullType);

    public async Task<List<NAVCategory>> NAVCategoriesAsync(Expression<Func<NAVCategory, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /* Platform */
    public NAVPlatform? NAVPlatform(int pfCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVPlatform>(pfCode, pullType);

    public async Task<List<NAVPlatform>> NAVPlatformsAsync(Expression<Func<NAVPlatform, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /* GENRE */
    public NAVGenre? NAVGenre(int genCode, EPullType pullType = EPullType.ObjectOnly) =>
        Chariot.PullObject<NAVGenre>(genCode, pullType);

    public async Task<List<NAVGenre>> NAVGenresAsync(Expression<Func<NAVGenre, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

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
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /* STORES */
    public async Task<List<Store>> StoresAsync(Expression<Func<Store, bool>>? filter = null,
        EPullType pullType = EPullType.ObjectOnly) => await Chariot.PullObjectListAsync(filter, pullType);

    /// <summary>
    /// Pulls multiple sets of data from the inventory database and combines them into the SKUMaster data set.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SkuMaster>> GetMastersAsync()
    {
        IEnumerable<SkuMaster> returnVal = new List<SkuMaster>();

        async void Action()
        {
            var itemTask = NAVItemsAsync();
            var stockTask = NAVAllStockAsync();
            var uomTask = NAVUoMsAsync();
            var binTask = NAVBinsAsync();
            var divisionTask = NAVDivisionsAsync();
            var categoryTask = NAVCategoriesAsync();
            var platformTask = NAVPlatformsAsync();
            var genreTask = NAVGenresAsync();

            await Task.WhenAll(itemTask, stockTask, uomTask, binTask, divisionTask, categoryTask, platformTask, genreTask);

            var navItems = await itemTask;
            var stock = (await stockTask).GroupBy(s => s.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToList());
            var uomDict = (await uomTask).GroupBy(u => u.ItemNumber)
                .ToDictionary(g => g.Key, g => g.ToDictionary(u => u.Code, u => u));
            var bins = (await binTask).ToDictionary(b => b.ID, b => b);
            var divisions = (await divisionTask).ToDictionary(d => d.Code, d => d.Description);
            var categories = (await categoryTask).ToDictionary(c => c.Code, c => c.Description);
            var platforms = (await platformTask).ToDictionary(p => p.Code, p => p.Description);
            var genres = (await genreTask).ToDictionary(g => g.Code, g => g.Description);

            returnVal = navItems.Select(item => new SkuMaster(item, stock, uomDict, bins, divisions, categories, platforms, genres));
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return returnVal;
    }

    /// <summary>
    /// Pulls multiple sets of data from the inventory database and combines them into the SKUMaster data set.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<SkuMaster>> ImplicitGetMastersAsync() =>
        (await NAVItemsAsync(pullType: EPullType.IncludeChildren)).Select(i => new SkuMaster(i));


    /// <summary>
    /// Gets all sites with the appropriately attached zones (with their zone extensions).
    /// </summary>
    /// <returns>Tuple (Sites, Zones)</returns>
    public async Task<(IEnumerable<Site>, IEnumerable<NAVZone>)> SitesAsync()
    {
        List<NAVZone>? zoneList = null;
        Dictionary<string, Site>? siteDict = null;

        async void Action()
        {
            var zoneTask = ZonesAsync();
            var siteTask = Chariot.PullObjectListAsync<Site>();

            await Task.WhenAll(zoneTask, siteTask);

            zoneList = (await zoneTask).ToList();
            siteDict = (await siteTask).ToDictionary(s => s.Name, s => s);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        zoneList ??= new List<NAVZone>();
        siteDict ??= new Dictionary<string, Site>();
        var zones = zoneList;

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

        return (siteDict.Values, zones);
    }

    /// <summary>
    /// Get the full set of data required for standard FixedBinChecker functionality.
    /// </summary>
    /// <returns></returns>
    public async Task<FixedBinCheckDataSet?> FixedBinCheckDataSetAsync(List<string> fromZones, List<string> fixedZones)
    {
        FixedBinCheckDataSet? dataSet = null;

        var allZones = fromZones.Concat(fixedZones);

        async void Action()
        {
            var itemTask = ItemsAsync();
            var zoneTask = ZonesAsync(zone => allZones.Contains(zone.Code));
            var stockTask = Chariot.PullObjectListAsync<NAVStock>(stock => allZones.Contains(stock.ZoneCode));
            var binTask = BinsAsync(bin => allZones.Contains(bin.ZoneCode));
            var uomTask = NAVUoMsAsync();

            await Task.WhenAll(itemTask, zoneTask, stockTask, binTask, uomTask);

            dataSet = new FixedBinCheckDataSet(await itemTask, await zoneTask, await binTask, await stockTask, await uomTask);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        dataSet ??= new FixedBinCheckDataSet(new List<NAVItem>(), new List<NAVZone>(), new List<NAVBin>(),
            new List<NAVStock>(), new List<NAVUoM>());

        return dataSet;
    }

    /// <summary>
    /// Get stock relevant to Hydra Data.
    /// </summary>
    /// <returns>Tuple: (Stock, Bins, UoMs)</returns>
    public async Task<(List<NAVStock>, IEnumerable<NAVBin>, List<NAVUoM>)> HydraStockAsync()
    {
        List<NAVStock>? stock = null;
        IEnumerable<NAVBin>? bins = null;
        List<NAVUoM>? uomList = null;

        async void Action()
        {
            var stockTask = Chariot.PullObjectListAsync<NAVStock>();
            var binTask = BinsAsync();
            var uomTask = Chariot.PullObjectListAsync<NAVUoM>();

            await Task.WhenAll(stockTask, binTask, uomTask);

            stock = await stockTask;
            bins = await binTask;
            uomList = await uomTask;
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        stock ??= new List<NAVStock>();
        bins ??= new List<NAVBin>();
        uomList ??= new List<NAVUoM>();

        return (stock, bins, uomList);
    }

    /// <summary>
    /// Get the full set of data required for standard Hydra functionality.
    /// </summary>
    /// <returns></returns>
    public async Task<HydraDataSet?> HydraDataSetAsync(bool includeStock = true)
    {
        HydraDataSet? dataSet = null;

        List<NAVStock> stock;
        IEnumerable<NAVBin> bins;
        List<NAVUoM> uomList;

        async void Action()
        {
            var stockTask = includeStock
                ? HydraStockAsync()
                : new Task<(List<NAVStock> stock, IEnumerable<NAVBin> bins, List<NAVUoM> uomList)>(() =>
                    (new List<NAVStock>(), new List<NAVBin>(), new List<NAVUoM>()));
            var itemTask = ItemsAsync();
            var siteTask = SitesAsync();
            var levelTask = Chariot.PullObjectListAsync<SiteItemLevel>();
            var locationTask = Chariot.PullObjectListAsync<NAVLocation>();

            await Task.WhenAll(itemTask, siteTask, levelTask, locationTask, stockTask);

            (stock, bins, uomList) = await stockTask;

            var items = (await itemTask).ToList();
            var (s, zones) = await siteTask;
            var sites = s.ToList();
            var levels = (await levelTask).ToDictionary(l => (l.ItemNumber, l.SiteName), l => l);
            var newLevels = new List<SiteItemLevel>();
            var locations = await locationTask;

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
                _ = Chariot.InsertIntoTableAsync(newLevels);
                allLevels = newLevels.Concat(levels.Values);
            }
            else
            {
                allLevels = levels.Values;
            }

            dataSet = new HydraDataSet(items, sites, zones, allLevels, bins, stock, uomList, locations);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        dataSet ??= new HydraDataSet(new List<NAVItem>(), new List<Site>(), new List<NAVZone>(),
            new List<SiteItemLevel>(), new List<NAVBin>(), new List<NAVStock>(), new List<NAVUoM>(),
            new List<NAVLocation>());

        return dataSet;
    }

    public async Task<BasicStockDataSet?> BasicStockDataSetAsync(IEnumerable<string> zoneCodes, IEnumerable<string> locations)
    {
        BasicStockDataSet? dataSet = null;

        async void Action()
        {
            var itemTask = ItemsAsync();
            var zoneTask = ZonesAsync(zone => zoneCodes.Contains(zone.Code) && locations.Contains(zone.LocationCode));
            var stockTask = Chariot.PullObjectListAsync<NAVStock>(stock => zoneCodes.Contains(stock.ZoneCode) && locations.Contains(stock.LocationCode));
            var binTask = BinsAsync(bin => zoneCodes.Contains(bin.ZoneCode) && locations.Contains(bin.LocationCode));
            var uomTask = NAVUoMsAsync();

            await Task.WhenAll(itemTask, zoneTask, stockTask, binTask, uomTask);

            dataSet = new BasicStockDataSet(
                await itemTask, await zoneTask, await binTask, await stockTask, await uomTask);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        dataSet ??= new BasicStockDataSet(new List<NAVItem>(), new List<NAVZone>(), new List<NAVBin>(),
            new List<NAVStock>(), new List<NAVUoM>());

        return dataSet;
    }

    public async Task<TOStockDataSet?> TOStockDataSetAsync(IEnumerable<string> zoneCodes, IEnumerable<string> locations)
    {
        TOStockDataSet? dataSet = null;

        async void Action()
        {
            var items = ItemsAsync();
            var zones = ZonesAsync(zone => zoneCodes.Contains(zone.Code) && locations.Contains(zone.LocationCode));
            var stock = Chariot.PullObjectListAsync<NAVStock>(stock =>
                zoneCodes.Contains(stock.ZoneCode) && locations.Contains(stock.LocationCode));
            var bins = BinsAsync(bin => zoneCodes.Contains(bin.ZoneCode) && locations.Contains(bin.LocationCode));
            var uomList = NAVUoMsAsync();
            var toList = TOListAsync();
            var stores = StoresAsync();
            var platforms = NAVPlatformsAsync();
            var divisions = NAVDivisionsAsync();
            var categories = NAVCategoriesAsync();
            var genres = NAVGenresAsync();

            await Task.WhenAll(items, zones, stock, bins, uomList, toList,
                stores, platforms, divisions, categories, genres);

            dataSet = new TOStockDataSet(await items, await zones, await bins, await stock, await uomList, await toList,
                await stores, await platforms, await genres, await categories, await divisions);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        dataSet ??= new TOStockDataSet(new List<NAVItem>(), new List<NAVZone>(), new List<NAVBin>(),
            new List<NAVStock>(), new List<NAVUoM>(), new List<NAVTransferOrder>(), new List<Store>(),
            new List<NAVPlatform>(), new List<NAVGenre>(), new List<NAVCategory>(), new List<NAVDivision>());

        return dataSet;
    }

    public int TOLineCount() => Chariot.Database?.ExecuteScalar<int>("SELECT count(*) FROM TOLineBatchAnalysis;") ?? 0;

    public async Task<IEnumerable<MixedCarton>> MixedCartonsAsync(Expression<Func<MixedCarton, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task<IEnumerable<MixedCartonItem>> MixedCartonItemsAsync(Expression<Func<MixedCartonItem, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
        => await Chariot.PullObjectListAsync(filter, pullType);

    public async Task<IEnumerable<MixedCarton>> MixedCartonTemplatesAsync(Expression<Func<MixedCarton, bool>>? filter = null, EPullType pullType = EPullType.ObjectOnly)
    {
        Dictionary<Guid, MixedCarton>? mixedCartons = null;
        List<Guid>? ids;
        IEnumerable<MixedCartonItem>? mcItems = null;
        IEnumerable<int>? itemNumbers;
        Dictionary<int, NAVItem>? items = null;

        async void Action()
        {
            mixedCartons = (await MixedCartonsAsync(filter, pullType)).ToDictionary(mc => mc.ID, mc => mc);
            ids = mixedCartons.Keys.ToList();
            mcItems = await MixedCartonItemsAsync(item => ids.Contains(item.MixedCartonID));
            itemNumbers = mcItems.Select(item => item.ItemNumber).Distinct();
            items = (await ItemsAsync(i => itemNumbers.Contains(i.Number))).ToDictionary(i => i.Number, i => i);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        mcItems ??= new List<MixedCartonItem>();
        items ??= new Dictionary<int, NAVItem>();
        mixedCartons ??= new Dictionary<Guid, MixedCarton>();

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

        return mixedCartons.Values;
    }

    public async Task<(List<MixedCarton>, List<MixedCartonItem>, List<NAVItem>)> GetMixedCartonData()
    {
        List<MixedCarton>? mcList = null;
        List<MixedCartonItem>? mcItems = null;
        List<NAVItem>? items = null;

        async void Action()
        {
            var itemTask = ItemsAsync();
            var mcTask = MixedCartonsAsync();
            var mcItemTask = MixedCartonItemsAsync();

            await Task.WhenAll(itemTask, mcTask, mcItemTask);

            items = (await itemTask).ToList();
            mcList = (await mcTask).ToList();
            mcItems = (await mcItemTask).ToList();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        mcList ??= new List<MixedCarton>();
        mcItems ??= new List<MixedCartonItem>();
        items ??= new List<NAVItem>();

        return (mcList, mcItems, items);
    }
}