using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public int SendData(HydraDataSet? dataSet, IEnumerable<Move> moves)
    {
        var lines = 0;

        Database?.RunInTransaction(() =>
        {
            lines += UpdateTable(moves);
            lines += UpdateTable(dataSet.Stock);
            lines += UpdateTable(dataSet.Zones.Values);
            lines += UpdateTable(dataSet.Items.Values);
            lines += UpdateTable(dataSet.Bins.Values);
            lines += UpdateTable(dataSet.Locations.Values);
            lines += UpdateTable(dataSet.SiteItemLevels.Values);
            lines += UpdateTable(dataSet.Sites.Values);
            lines += UpdateTable(dataSet.NAVStock);
            lines += UpdateTable(dataSet.UoMs);
        });

        return lines;
    }

    public IEnumerable<Move> GetOldMoves(out HydraDataSet dataSet)
    {
        dataSet = HydraDataSet();

        return PullObjectList<Move>();
    }

    /// <summary>
    /// Get the full set of data required for standard Hydra functionality.
    /// </summary>
    /// <returns></returns>
    public HydraDataSet HydraDataSet(bool includeStock = true)
    {
        HydraDataSet? dataSet = null;

        Database?.RunInTransaction(() =>
        {
            var items = Items().ToList();
            var sites = Sites(out var zones).ToList();
            var levels = PullObjectList<SiteItemLevel>().ToDictionary(l => (l.ItemNumber, l.SiteName), l => l);
            var stock = includeStock
                ? PullObjectList<NAVStock>()
                : new List<NAVStock>();
            var bins = includeStock
                ? Bins()
                : new List<NAVBin>();
            var uomList = includeStock
                ? PullObjectList<NAVUoM>()
                : new List<NAVUoM>();
            var newLevels = new List<SiteItemLevel>();
            var locations = PullObjectList<NAVLocation>();

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
                InsertIntoTable(newLevels);
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


    /// <summary>
    /// Gets all sites with the appropriately attached zones (with their zone extensions).
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Site> Sites(out List<NAVZone> zones)
    {
        List<NAVZone>? zoneList = null;
        Dictionary<string, Site>? siteDict = null;

        Database?.RunInTransaction(() =>
        {
            zoneList = Zones().ToList();
            siteDict = PullObjectList<Site>().ToDictionary(s => s.Name, s => s);
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
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVItem> Items()
    {
        IEnumerable<NAVItem>? returnItems = null;

        Database?.RunInTransaction(() =>
        {
            var items = PullObjectList<NAVItem>().ToDictionary(i => i.Number, i => i);
            var extensions = PullObjectList<ItemExtension>().ToDictionary(e => e.ItemNumber, e => e);
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

            InsertIntoTable(newExtensions);
            returnItems = items.Values;
        });
        returnItems ??= new List<NAVItem>();

        return returnItems;
    }

    /// <summary>
    /// Items with matched item-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVBin> Bins()
    {
        IEnumerable<NAVBin>? returnBins = null;

        Database?.RunInTransaction(() =>
        {
            var bins = PullObjectList<NAVBin>().ToDictionary(i => i.ID, i => i);
            var bays = PullObjectList<Bay>().ToDictionary(b => b.ID, b => b);
            var extensions = PullObjectList<BinExtension>().ToDictionary(e => e.BinID, e => e);
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

            InsertIntoTable(newExtensions);
            returnBins = bins.Values;
        });
        returnBins ??= new List<NAVBin>();

        return returnBins;
    }

    /// <summary>
    /// Zones with matched zone-extension objects.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<NAVZone> Zones()
    {
        IEnumerable<NAVZone>? returnZones = null;

        Database?.RunInTransaction(() =>
        {
            var zones = PullObjectList<NAVZone>().ToDictionary(z => z.ID, z => z);
            var extensions = PullObjectList<ZoneExtension>().ToDictionary(e => e.ZoneID, e => e);
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

            InsertIntoTable(newExtensions);
            returnZones = zones.Values;
        });
        returnZones ??= new List<NAVZone>();

        return returnZones;
    }

}