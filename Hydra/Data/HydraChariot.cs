using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uranus;
using Uranus.Inventory.Models;

namespace Hydra.Data;

public sealed class HydraChariot : MasterChariot
{
    public override string DatabaseName { get; }
    public override Type[] Tables { get; } =
    {
        typeof(Bay), typeof(BayZone), typeof(BinExtension), typeof(Move),
        typeof(NAVBin), typeof(NAVItem), typeof(NAVLocation), typeof(NAVStock),
        typeof(NAVUoM), typeof(NAVZone), typeof(Stock), typeof(Store),
        typeof(SubStock), typeof(ZoneExtension), typeof(ItemExtension), typeof(Site),
        typeof(SiteItemLevel)
    };

    public HydraChariot(string fullDatabasePath)
    {
        BaseDataDirectory = Path.GetDirectoryName(fullDatabasePath) ?? "";
        DatabaseName = Path.GetFileName(fullDatabasePath);
        InitializeDatabaseConnection();

    }

    public HydraChariot(string databaseDirectory, string databaseName)
    {
        BaseDataDirectory = databaseDirectory;
        DatabaseName = databaseName;
        InitializeDatabaseConnection();
    }

    public async Task<int> SendDataAsync(HydraDataSet? dataSet, IEnumerable<Move> moves)
    {
        var lines = 0;

        async void Action()
        {
            var tasks = new List<Task<int>> {UpdateTableAsync(moves)};
            if (dataSet is not null)
            {
                tasks.Add(UpdateTableAsync(dataSet.Stock));
                tasks.Add(UpdateTableAsync(dataSet.Zones.Values));
                tasks.Add(UpdateTableAsync(dataSet.Items.Values));
                tasks.Add(UpdateTableAsync(dataSet.Bins.Values));
                tasks.Add(UpdateTableAsync(dataSet.Locations.Values));
                tasks.Add(UpdateTableAsync(dataSet.SiteItemLevels.Values));
                tasks.Add(UpdateTableAsync(dataSet.Sites.Values));
                tasks.Add(UpdateTableAsync(dataSet.NAVStock));
                tasks.Add(UpdateTableAsync(dataSet.UoMs));
            }

            lines += (await Task.WhenAll(tasks)).Sum();
        }

        await new Task(() => Database?.RunInTransaction(Action));

        return lines;
    }

    public async Task<(IEnumerable<Move>, HydraDataSet)> GetOldMovesAsync()
    {
        var dataSetTask = HydraDataSetAsync();
        var moveTask = PullObjectListAsync<Move>();

        await Task.WhenAll(dataSetTask, moveTask);

        return (await moveTask, await dataSetTask);
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
            var stockTask = PullObjectListAsync<NAVStock>();
            var binTask = BinsAsync();
            var uomTask = PullObjectListAsync<NAVUoM>();

            await Task.WhenAll(stockTask, binTask, uomTask);

            stock = await stockTask;
            bins = await binTask;
            uomList = await uomTask;
        }

        await new Task(() => Database?.RunInTransaction(Action));

        stock ??= new List<NAVStock>();
        bins ??= new List<NAVBin>();
        uomList ??= new List<NAVUoM>();

        return (stock, bins, uomList);
    }

    /// <summary>
    /// Get the full set of data required for standard Hydra functionality.
    /// </summary>
    /// <returns></returns>
    public async Task<HydraDataSet> HydraDataSetAsync(bool includeStock = true)
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
            var levelTask = PullObjectListAsync<SiteItemLevel>();
            var locationTask = PullObjectListAsync<NAVLocation>();

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
                _ = InsertIntoTableAsync(newLevels);
                allLevels = newLevels.Concat(levels.Values);
            }
            else
            {
                allLevels = levels.Values;
            }

            dataSet = new HydraDataSet(items, sites, zones, allLevels, bins, stock, uomList, locations);
        }

        await new Task(() => Database?.RunInTransaction(Action));

        dataSet ??= new HydraDataSet(new List<NAVItem>(), new List<Site>(), new List<NAVZone>(),
            new List<SiteItemLevel>(), new List<NAVBin>(), new List<NAVStock>(), new List<NAVUoM>(),
            new List<NAVLocation>());

        return dataSet;
    }


    /// <summary>
    /// Gets all sites with the appropriately attached zones (with their zone extensions).
    /// </summary>
    /// <returns></returns>
    public async Task<(IEnumerable<Site>, List<NAVZone>)> SitesAsync()
    {
        List<NAVZone>? zones = null;
        Dictionary<string, Site>? siteDict = null;

        async void Action()
        {
            var zoneTask = Zones();
            var siteTask = PullObjectListAsync<Site>();

            await Task.WhenAll(zoneTask, siteTask);

            zones = (await zoneTask).ToList();
            siteDict = (await siteTask).ToDictionary(s => s.Name, s => s);
        }

        await new Task(() => Database?.RunInTransaction(Action));

        zones ??= new List<NAVZone>();
        siteDict ??= new Dictionary<string, Site>();
        
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
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVItem>> ItemsAsync()
    {
        IEnumerable<NAVItem>? returnItems = null;

        async void Action()
        {
            var itemTask = PullObjectListAsync<NAVItem>();
            var extensionTask = PullObjectListAsync<ItemExtension>();
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

            await InsertIntoTableAsync(newExtensions);
            returnItems = items.Values;
        }

        await new Task(() => Database?.RunInTransaction(Action));
        returnItems ??= new List<NAVItem>();

        return returnItems;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVBin>> BinsAsync()
    {
        IEnumerable<NAVBin>? returnBins = null;

        async void Action()
        {
            var binTask = PullObjectListAsync<NAVBin>();
            var bayTask = PullObjectListAsync<Bay>();
            var extensionTask = PullObjectListAsync<BinExtension>();

            var newExtensions = new List<BinExtension>();

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

            await InsertIntoTableAsync(newExtensions);
            returnBins = bins.Values;
        }

        await new Task(() => Database?.RunInTransaction(Action));
        returnBins ??= new List<NAVBin>();

        return returnBins;
    }

    /// <summary>
    /// Zones with matched zone-extension objects.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<NAVZone>> Zones()
    {
        IEnumerable<NAVZone>? returnZones = null;

        async void Action()
        {
            var zoneTask = PullObjectListAsync<NAVZone>();
            var extensionTask = PullObjectListAsync<ZoneExtension>();
            var newExtensions = new List<ZoneExtension>();

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

            await InsertIntoTableAsync(newExtensions);
            returnZones = zones.Values;
        }

        await new Task(() => Database?.RunInTransaction(Action));

        returnZones ??= new List<NAVZone>();

        return returnZones;
    }

}