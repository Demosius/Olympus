using System;
using System.Collections.Generic;
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

        await Task.Run(() => Chariot.RunInTransaction(Action));
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

        await Task.Run(() => Chariot.RunInTransaction(Action));
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
        await Task.Run(() => Chariot.RunInTransaction(Action));
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

        await Task.Run(() => Chariot.RunInTransaction(Action));
        return lines;
    }

    public async Task<int> NAVTransferOrdersAsync(IEnumerable<NAVTransferOrder> transferOrders) => await Chariot.ReplaceFullTableAsync(transferOrders);

    public async Task<int> NAVZoneAsync(List<NAVZone> zones) => await Chariot.ReplaceFullTableAsync(zones);

    public async Task<int> NAVLocation(List<NAVLocation> locations) => await Chariot.ReplaceFullTableAsync(locations);

    public async Task<int> NAVDivision(List<NAVDivision> divs) => await Chariot.ReplaceFullTableAsync(divs);

    public async Task<int> NAVCategory(List<NAVCategory> cats) => await Chariot.ReplaceFullTableAsync(cats);

    public async Task<int> NAVPlatform(List<NAVPlatform> pfs) => await Chariot.ReplaceFullTableAsync(pfs);

    public async Task<int> NAVGenre(List<NAVGenre> gens) => await Chariot.ReplaceFullTableAsync(gens);

    public async Task<int> SiteAsync(Site site) => await Chariot.InsertOrUpdateAsync(site);
}