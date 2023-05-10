using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

public class InventoryUpdater
{
    public InventoryChariot Chariot { get; set; }

    public InventoryUpdater(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    public async Task<int> NAVBinsAsync(List<NAVBin> bins)
    {
        var count = 0;

        async void Action()
        {
            count = await Chariot.UpdateTableAsync(bins);
            if (count > 0) Chariot.SetTableUpdateTime(typeof(NAVBin));
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        return count;
    }

    public async Task<int> NAVItemsAsync(IEnumerable<NAVItem> items, DateTime dateTime)
    {
        var count = 0;

        async void Action()
        {
            var navItems = items as NAVItem[] ?? items.ToArray();

            var itemTask = Chariot.UpdateTableAsync(navItems);
            var extensionTask = Chariot.UpdateTableAsync(navItems.Select(i => i.Extension));

            count = (await Task.WhenAll(itemTask, extensionTask)).Sum();

            if (count > 0) Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        return count;
    }

    public async Task<int> NAVUoMsAsync(List<NAVUoM> uomList)
    {
        if (uomList.Count == 0) return 0;

        var lines = 0;

        async void Action()
        {
            lines += await Chariot.UpdateTableAsync(uomList);

            if (lines == 0) return;

            lines += Chariot.SetTableUpdateTime(typeof(NAVUoM));
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return lines;
    }

    public async Task<int> NAVStockAsync(List<NAVStock> stock)
    {
        if (stock.Count == 0) return 0;

        var lines = 0;

        async void Action()
        {
            // Remove from stock table anything with zones equal to what is being put in.
            lines += Chariot.StockZoneDeletes(stock.Select(s => s.ZoneID).Distinct().ToList());
            lines += await Chariot.InsertIntoTableAsync(stock);

            lines += Chariot.SetTableUpdateTime(typeof(NAVStock));
            lines += await Chariot.SetStockUpdateTimesAsync(stock);
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return lines;
    }

    public async Task<int> NAVStockAsync(List<NAVStock> newStock, List<string> zonesToRemove)
    {
        if (newStock.Count == 0) return 0;

        zonesToRemove.AddRange(newStock.Select(s => s.ZoneID).Distinct());
        var lines = 0;

        /*Chariot.Database?.RunInTransaction(() =>
        {*/
            // Remove from stock table anything with zones equal to what is being put in.
            lines += Chariot.StockZoneDeletes(zonesToRemove);

            lines += await Chariot.InsertIntoTableAsync(newStock);

            lines += Chariot.SetTableUpdateTime(typeof(NAVStock));
            lines += await Chariot.SetStockUpdateTimesAsync(newStock);
        /*});*/

        return lines;
    }

    public async Task<int> NAVZonesAsync(List<NAVZone> zones) => await Chariot.UpdateTableAsync(zones);

    public async Task<int> NAVTransferOrdersAsync(IEnumerable<NAVTransferOrder> transferOrders) => await Chariot.UpdateTableAsync(transferOrders);

    public async Task<int> ZonesAsync(List<NAVZone> zones)
    {
        var count = 0;

        async void Action()
        {
            var zoneTask = Chariot.UpdateTableAsync(zones);
            var extensionTask = Chariot.UpdateTableAsync(zones.Select(z => z.Extension));

            count += (await Task.WhenAll(zoneTask, extensionTask)).Sum();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        return count;
    }

    public async Task<int> NAVLocationAsync(List<NAVLocation> locations) => await Chariot.UpdateTableAsync(locations);

    public async Task<int> NAVDivisionAsync(List<NAVDivision> divs) => await Chariot.UpdateTableAsync(divs);

    public async Task<int> NAVCategoryAsync(List<NAVCategory> cats) => await Chariot.UpdateTableAsync(cats);

    public async Task<int> NAVPlatformAsync(List<NAVPlatform> pfs) => await Chariot.UpdateTableAsync(pfs);

    public async Task<int> NAVGenre(List<NAVGenre> gens) => await Chariot.UpdateTableAsync(gens);

    /// <summary>
    /// Replaces the current data with all new data, and updates connected zoneExtensions as applicable.
    /// </summary>
    /// <param name="newZones"></param>
    /// <returns></returns>
    public async Task<int> ReplaceZonesAsync(IEnumerable<NAVZone> newZones)
    {
        var lines = 0;
        var zoneList = newZones.ToList();

        async void Action()
        {
            var extensionTask = Chariot.UpdateTableAsync(zoneList.Select(z => z.Extension));
            var zoneTask = Chariot.ReplaceFullTableAsync(zoneList);

            lines += (await Task.WhenAll(extensionTask, zoneTask)).Sum();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));

        return lines;
    }

    public async Task<int> ReplaceMixedCartonsAsync(IEnumerable<MixedCarton> mixedCartons)
    {
        var lines = 0;

        async void Action()
        {
            var mcList = mixedCartons.ToList();
            var mcItems = mcList.SelectMany(mc => mc.Items);

            var itemTask = Chariot.ReplaceFullTableAsync(mcItems);
            var mixCtnTask = Chariot.ReplaceFullTableAsync(mcList);

            lines += (await Task.WhenAll(itemTask, mixCtnTask)).Sum();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        return lines;
    }

    public async Task<int> SitesAsync(List<Site> sites)
    {
        var lines = 0;

        async void Action()
        {
            var siteTask = Chariot.UpdateTableAsync(sites);
            var zoneTask = Chariot.UpdateTableAsync(sites.SelectMany(s => s.Zones));
            var extenstionTask = Chariot.UpdateTableAsync(sites.SelectMany(s => s.Zones.Select(z => z.Extension)));

            lines += (await Task.WhenAll(siteTask, extenstionTask, zoneTask)).Sum();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        return lines;
    }

    public async Task<int> SitesAsync(IEnumerable<Site> sites, List<NAVZone> zones)
    {
        var lines = 0;

        async void Action()
        {
            var siteTask = Chariot.UpdateTableAsync(sites);
            var zoneTask = Chariot.UpdateTableAsync(zones);
            var extensionTask = Chariot.UpdateTableAsync(zones.Select(z => z.Extension));

            lines += (await Task.WhenAll(siteTask, extensionTask, zoneTask)).Sum();
        }

        await new Task(() => Chariot.Database?.RunInTransaction(Action));
        return lines;
    }

    public async Task<int> SiteItemLevelsAsync(IEnumerable<SiteItemLevel> siteItemLevels) => await Chariot.UpdateTableAsync(siteItemLevels);

    public int Site(Site site) => Chariot.Update(site);

}