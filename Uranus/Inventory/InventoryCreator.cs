﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

public class InventoryCreator
{
    public InventoryChariot Chariot { get; set; }

    public InventoryCreator(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    public async Task<int> NAVBinsAsync(List<NAVBin> bins)
    {
        var lines = 0;
        void Action()
        {
            lines = Chariot.ReplaceFullTable(bins);
            if (lines > 0)
                Chariot.SetTableUpdateTime(typeof(NAVBin));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> NAVItemsAsync(List<NAVItem> items, DateTime dateTime)
    {
        var lines = 0;

        void Action()
        {
            lines = Chariot.ReplaceFullTable(items);
            if (lines > 0)
                Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> NAVUoMsAsync(List<NAVUoM> uomList)
    {
        var lines = 0;
        void Action()
        {
            lines = Chariot.ReplaceFullTable(uomList);
            if (lines > 0)
                Chariot.SetTableUpdateTime(typeof(NAVUoM));
        }
        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> NAVStockAsync(List<NAVStock> stock)
    {
        var lines = 0;

        void Action()
        {
            lines = Chariot.ReplaceFullTable(stock);
            if (lines <= 0) return;

            Chariot.EmptyTable<BinContentsUpdate>();
            Chariot.SetTableUpdateTime(typeof(NAVStock));
            Chariot.SetStockUpdateTimes(stock);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> NAVTransferOrdersAsync(IEnumerable<NAVTransferOrder> transferOrders) => await Chariot.ReplaceFullTableAsync(transferOrders).ConfigureAwait(false);

    public async Task<int> NAVZoneAsync(List<NAVZone> zones) => await Chariot.ReplaceFullTableAsync(zones).ConfigureAwait(false);

    public async Task<int> NAVLocation(List<NAVLocation> locations) => await Chariot.ReplaceFullTableAsync(locations).ConfigureAwait(false);

    public async Task<int> NAVDivision(List<NAVDivision> divs) => await Chariot.ReplaceFullTableAsync(divs).ConfigureAwait(false);

    public async Task<int> NAVCategory(List<NAVCategory> cats) => await Chariot.ReplaceFullTableAsync(cats).ConfigureAwait(false);

    public async Task<int> NAVPlatform(List<NAVPlatform> pfs) => await Chariot.ReplaceFullTableAsync(pfs).ConfigureAwait(false);

    public async Task<int> NAVGenre(List<NAVGenre> gens) => await Chariot.ReplaceFullTableAsync(gens).ConfigureAwait(false);

    public async Task<int> SiteAsync(Site site) => await Chariot.InsertOrUpdateAsync(site).ConfigureAwait(false);

    public async Task<int> BatchTODataAsync(List<BatchTOGroup> newGroups)
    {
        var lines = 0;

        void Action()
        {
            var batchTOLines = newGroups.SelectMany(g => g.Lines).ToList();
            // Get Batch IDs.
            var batchIDs = batchTOLines.Select(l => l.BatchID).Distinct();
            // See if there are any batches that do not already exist.
            var batchDict = Chariot.PullObjectList<Batch>().ToDictionary(b => b.ID, b => b);
            batchIDs = batchIDs.Where(id => !batchDict.ContainsKey(id));

            var newBatches = batchIDs.Select(id => new Batch(id));

            lines += Chariot.InsertIntoTable(newGroups);
            lines += Chariot.InsertIntoTable(newBatches);
            lines += Chariot.InsertIntoTable(batchTOLines);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }
}