using System;
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
            var batchDates = batchTOLines.Select(l => (l.BatchID, l.Date)).Distinct().ToList();
            // See if there are any batches that do not already exist.
            var batchDict = Chariot.PullObjectList<Batch>().ToDictionary(b => b.ID, b => b);
            batchDates = batchDates.Where(v => !batchDict.ContainsKey(v.BatchID)).ToList();

            var newBatches = batchDates.Select(v => new Batch(v.BatchID)
                {Progress = EBatchProgress.DataUploaded, CreatedOn = v.Date, LastTimeCartonizedDate = v.Date});

            // Get updateable batches.
            var batchUpdates = new List<Batch>();
            foreach (var (batchID, _) in batchDates)
            {
                if (!batchDict.TryGetValue(batchID, out var batch)) continue;
                if (batch.UpdateBatchProgress(EBatchProgress.DataUploaded))
                    batchUpdates.Add(batch);
            }

            lines += Chariot.InsertIntoTable(newGroups);
            lines += Chariot.InsertIntoTable(newBatches);
            lines += Chariot.InsertIntoTable(batchTOLines);
            lines += Chariot.UpdateTable(batchUpdates);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> PickLinesAsync(List<PickLine> pickLines)
    {
        var lines = 0;

        void Action()
        {
            // Handle related batches.
            var batchDict = Chariot.PullObjectList<Batch>().ToDictionary(b => b.ID, b => b);

            var batchDates = pickLines
                .Where(l => l.BatchID != string.Empty).GroupBy(l => l.BatchID)
                .ToDictionary(g => g.Key, g => g.Max(l => l.DueDate));

            var newBatches = new List<Batch>(); 

            var updateableBatches = new List<Batch>();
            var lineDict = pickLines
                .Where(l => l.BatchID != string.Empty)
                .GroupBy(l => l.BatchID)
                .ToDictionary(g => g.Key, g => g.ToList());    
            foreach (var (batchID, date) in batchDates)
            {
                if (!lineDict.TryGetValue(batchID, out var lineGroup)) continue;
                var isNew = !batchDict.TryGetValue(batchID, out var batch);
                if (isNew) batch = new Batch(batchID) {CreatedOn = date, LastTimeCartonizedDate = date}; 
                if (batch is null) continue;

                batch.UpdateBatchProgress(EBatchProgress.AutoRun);
                batch.PickLines = lineGroup;
                batch.CalculateHits();
                updateableBatches.Add(batch);

                if (isNew)
                    newBatches.Add(batch);
                else
                    updateableBatches.Add(batch);
            }

            // Update database.
            lines += Chariot.UpdateTable(pickLines);
            lines += Chariot.InsertIntoTable(newBatches);
            lines += Chariot.UpdateTable(updateableBatches);
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);
        return lines;
    }

    public async Task<int> StoresAsync(List<Store> stores) => await Chariot.ReplaceFullTableAsync(stores);

    public async Task<bool> StoreAsync(Store newStore) => await Task.Run(() => Chariot.Create(newStore));
}