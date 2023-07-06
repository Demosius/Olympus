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

        void Action()
        {
            count = Chariot.UpdateTable(bins);
            if (count > 0) Chariot.SetTableUpdateTime(typeof(NAVBin));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return count;
    }

    public async Task<int> NAVItemsAsync(IEnumerable<NAVItem> items, DateTime dateTime)
    {
        var count = 0;

        void Action()
        {
            var navItems = items as NAVItem[] ?? items.ToArray();

            count += Chariot.UpdateTable(navItems);
            count += Chariot.UpdateTable(navItems.Select(i => i.Extension));

            if (count > 0) Chariot.SetTableUpdateTime(typeof(NAVItem), dateTime);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return count;
    }

    public async Task<int> NAVUoMsAsync(List<NAVUoM> uomList)
    {
        if (uomList.Count == 0) return 0;

        var lines = 0;

        void Action()
        {
            lines += Chariot.UpdateTable(uomList);

            if (lines == 0) return;

            lines += Chariot.SetTableUpdateTime(typeof(NAVUoM));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    public async Task<int> NAVStockAsync(List<NAVStock> stock)
    {
        if (stock.Count == 0) return 0;

        var lines = 0;

        void Action()
        {
            // Remove from stock table anything with zones equal to what is being put in.
            lines += Chariot.StockZoneDeletes(stock.Select(s => s.ZoneID).Distinct().ToList());
            lines += Chariot.InsertIntoTable(stock);

            lines += Chariot.SetTableUpdateTime(typeof(NAVStock));
            lines += Chariot.SetStockUpdateTimes(stock);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    public async Task<int> NAVStockAsync(List<NAVStock> newStock, List<string> zonesToRemove)
    {
        if (newStock.Count == 0) return 0;

        zonesToRemove.AddRange(newStock.Select(s => s.ZoneID).Distinct());
        var lines = 0;

        await Task.Run(() => Chariot.RunInTransaction(() =>
        {
            // Remove from stock table anything with zones equal to what is being put in.
            lines += Chariot.StockZoneDeletes(zonesToRemove);

            lines += Chariot.InsertIntoTable(newStock);

            lines += Chariot.SetTableUpdateTime(typeof(NAVStock));
            lines += Chariot.SetStockUpdateTimes(newStock);
        })).ConfigureAwait(false);

        return lines;
    }

    public async Task<int> NAVZonesAsync(List<NAVZone> zones) => await Chariot.UpdateTableAsync(zones).ConfigureAwait(false);

    public async Task<int> NAVTransferOrdersAsync(IEnumerable<NAVTransferOrder> transferOrders) => await Chariot.UpdateTableAsync(transferOrders).ConfigureAwait(false);

    public async Task<int> ZonesAsync(List<NAVZone> zones)
    {
        var count = 0;

        void Action()
        {
            count += Chariot.UpdateTable(zones);
            count += Chariot.UpdateTable(zones.Select(z => z.Extension));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return count;
    }

    public async Task<int> NAVLocationAsync(List<NAVLocation> locations) => await Chariot.UpdateTableAsync(locations).ConfigureAwait(false);

    public async Task<int> NAVDivisionAsync(List<NAVDivision> divs) => await Chariot.UpdateTableAsync(divs).ConfigureAwait(false);

    public async Task<int> NAVCategoryAsync(List<NAVCategory> cats) => await Chariot.UpdateTableAsync(cats).ConfigureAwait(false);

    public async Task<int> NAVPlatformAsync(List<NAVPlatform> pfs) => await Chariot.UpdateTableAsync(pfs).ConfigureAwait(false);

    public async Task<int> NAVGenre(List<NAVGenre> gens) => await Chariot.UpdateTableAsync(gens).ConfigureAwait(false);

    /// <summary>
    /// Replaces the current data with all new data, and updates connected zoneExtensions as applicable.
    /// </summary>
    /// <param name="newZones"></param>
    /// <returns></returns>
    public async Task<int> ReplaceZonesAsync(IEnumerable<NAVZone> newZones)
    {
        var lines = 0;
        var zoneList = newZones.ToList();

        void Action()
        {
            lines += Chariot.UpdateTable(zoneList.Select(z => z.Extension));
            lines += Chariot.ReplaceFullTable(zoneList);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    public async Task<int> ReplaceMixedCartonsAsync(IEnumerable<MixedCarton> mixedCartons)
    {
        var lines = 0;

        void Action()
        {
            var mcList = mixedCartons.ToList();
            var mcItems = mcList.SelectMany(mc => mc.Items);

            lines += Chariot.ReplaceFullTable(mcItems);
            lines += Chariot.ReplaceFullTable(mcList);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> SitesAsync(List<Site> sites)
    {
        var lines = 0;

        void Action()
        {
            lines += Chariot.UpdateTable(sites);
            lines += Chariot.UpdateTable(sites.SelectMany(s => s.Zones));
            lines += Chariot.UpdateTable(sites.SelectMany(s => s.Zones.Select(z => z.Extension)));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> SitesAsync(IEnumerable<Site> sites, List<NAVZone> zones)
    {
        var lines = 0;

        void Action()
        {
            lines += Chariot.UpdateTable(sites);
            lines += Chariot.UpdateTable(zones);
            lines += Chariot.UpdateTable(zones.Select(z => z.Extension));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> SiteItemLevelsAsync(IEnumerable<SiteItemLevel> siteItemLevels) =>
        await Chariot.UpdateTableAsync(siteItemLevels).ConfigureAwait(false);

    public int Site(Site site) => Chariot.Update(site);

    public async Task<int> BatchTOCartonDataAsync(List<BatchTOGroup> groups)
    {
        var lines = 0;

        void Action()
        {
            var cartonLines = groups.SelectMany(g => g.Lines).ToList();
            List<Batch> batches = cartonLines.Select(l => l.Batch).Where(b => b is not null).Distinct().ToList()!;

            lines += Chariot.UpdateTable(groups);
            lines += Chariot.UpdateTable(cartonLines);
            lines += Chariot.UpdateTable(batches);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> BatchTOCartonDataAsync(BatchTOGroup group)
    {
        var lines = 0;

        void Action()
        {
            List<Batch> batches = group.Lines.Select(l => l.Batch).Where(b => b is not null).Distinct().ToList()!;

            lines += Chariot.Update(group);
            lines += Chariot.UpdateTable(group.Lines);
            lines += Chariot.UpdateTable(batches);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    /// <summary>
    /// Check batches of given IDs to determine if their progress should be updated.
    ///
    /// Update from DataUploaded (or below) to label files created if all lines have been processed.
    /// Update from Files Created to Labels Printed if all listed output files do not exist.
    /// </summary>
    /// <param name="batchIDs"></param>
    /// <returns></returns>
    public async Task<int> BatchProgressCheck(List<string> batchIDs)
    {
        var lines = 0;

        void Action()
        {
            var toLines = Chariot.PullObjectList<BatchTOLine>(l => batchIDs.Contains(l.BatchID))
                .GroupBy(l => l.BatchID)
                .ToDictionary(g => g.Key, g => g.ToList());
            var batches = Chariot.PullObjectList<Batch>(batch => batchIDs.Contains(batch.ID));

            var updateableBatches = new List<Batch>();

            foreach (var batch in batches)
            {
                if (!toLines.TryGetValue(batch.ID, out var batchLines)) continue;
                switch (batch.Progress)
                {
                    // Figure out which check we want to run.
                    case <= EBatchProgress.DataUploaded:
                        if (!batchLines.All(l => l.IsFinalised)) continue;
                        batch.Progress = EBatchProgress.LabelFilesCreated;
                        updateableBatches.Add(batch);
                        break;

                    case EBatchProgress.LabelFilesCreated:
                        var files = batchLines.Select(l => Path.Join(l.FinalFileDirectory, l.FinalFileName))
                            .Distinct()
                            .ToList();
                        if (files.Any(File.Exists)) continue;
                        batch.Progress = EBatchProgress.LabelsPrinted;
                        updateableBatches.Add(batch);
                        break;

                }
            }

            lines += Chariot.UpdateTable(updateableBatches);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    /// <summary>
    /// Update using raw data from NAV, comparing new and old data to update relevant details
    /// leaving current updated data.
    /// </summary>
    /// <param name="rawBatches"></param>
    /// <returns></returns>
    public async Task<int> RawBatchesAsync(List<Batch> rawBatches)
    {
        var lines = 0;

        void Action()
        {
            var ids = rawBatches.Select(b => b.ID);
            var currentBatches = Chariot.PullObjectList<Batch>(b => ids.Contains(b.ID)).ToDictionary(b => b.ID, b => b);
            var batches = new List<Batch>();

            foreach (var batch in rawBatches)
            {
                if (currentBatches.TryGetValue(batch.ID, out var currentBatch))
                {
                    currentBatch.MergeRaw(batch);
                    batches.Add(currentBatch);
                }
                else
                {
                    batches.Add(batch);
                }
            }

            lines += Chariot.UpdateTable(batches);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> BatchAsync(Batch batch) => await Chariot.UpdateAsync(batch);

    public async Task<int> StoreAsync(Store store) => await Chariot.UpdateAsync(store);

    public async Task BatchesAsync(List<Batch> batches) => await Chariot.UpdateTableAsync(batches);

    public async Task StockNoteAsync(StockNote stockNote)
    {
        if (stockNote.Comment == string.Empty)
            await Task.Run(() => Chariot.Delete(stockNote));
        else
            await Chariot.InsertOrReplaceAsync(stockNote);
    }
}