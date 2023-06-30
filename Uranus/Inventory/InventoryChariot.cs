using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

/// <summary>
///  The chariot class for transferring inventory data back and forth between the database.
///  Primarily handles data in DataTable formats, both for input and output.
/// </summary>
public sealed class InventoryChariot : MasterChariot
{
    public override string DatabaseName => "Inventory.sqlite";

    public override Type[] Tables { get; } =
    {
        typeof(Batch), typeof(BatchGroup), typeof(BatchGroupLink), typeof(Bay), typeof(BayZone), typeof(BinContentsUpdate),
        typeof(BinExtension), typeof(Move), typeof(NAVBin), typeof(NAVCategory),
        typeof(NAVDivision), typeof(NAVGenre), typeof(NAVItem), typeof(NAVLocation),
        typeof(NAVMoveLine), typeof(NAVPlatform), typeof(NAVStock), typeof(NAVTransferOrder),
        typeof(NAVUoM), typeof(NAVZone), typeof(Stock), typeof(Store),
        typeof(SubStock), typeof(TableUpdate), typeof(ZoneExtension), typeof(ItemExtension),
        typeof(Site), typeof(SiteItemLevel), typeof(MixedCarton), typeof(MixedCartonItem),
        typeof(BatchTOLine), typeof(BatchTOGroup), typeof(PickLine), typeof(StockNote),
    };

    /*************************** Constructors ****************************/

    public InventoryChariot(string solLocation)
    {
        // Try first to use the given directory, if not then use local file.
        BaseDataDirectory = Path.Combine(solLocation, "Inventory");
        InitializeDatabaseConnection();
    }

    /// <summary>
    /// Resets the connection using the given location string.
    /// </summary>
    /// <param name="solLocation">A directory location, in which the Inventory database does/should reside.</param>
    public override void ResetConnection(string solLocation)
    {
        // First thing is to nullify the current database (connection).
        Database = null;

        BaseDataDirectory = Path.Combine(solLocation, "Inventory");
        InitializeDatabaseConnection();
    }

    /***************************** CREATE Data ****************************/

    /*                             Update Times                           */
    public int SetStockUpdateTimes(List<NAVStock> stock)
    {
        var dateTime = DateTime.Now;
        // Convert stock to list of BCUpdate items.
        var distinctStockByZone = stock.GroupBy(s => s.ZoneID).Select(g => g.First());
        var contentsUpdates = distinctStockByZone
            .Select(
                s => new BinContentsUpdate
                {
                    ZoneID = s.ZoneID,
                    ZoneCode = s.ZoneCode,
                    LocationCode = s.LocationCode,
                    LastUpdate = dateTime
                })
            .ToList();

        // Update Database
        return UpdateTable(contentsUpdates);
    }

    public async Task<int> SetStockUpdateTimesAsync(List<NAVStock> stock)
    {
        var dateTime = DateTime.Now;
        // Convert stock to list of BCUpdate items.
        var distinctStockByZone = stock.GroupBy(s => s.ZoneID).Select(g => g.First());
        var contentsUpdates = distinctStockByZone
            .Select(
                s => new BinContentsUpdate
                {
                    ZoneID = s.ZoneID, 
                    ZoneCode = s.ZoneCode, 
                    LocationCode = s.LocationCode, 
                    LastUpdate = dateTime
                })
            .ToList();

        // Update Database
        return await UpdateTableAsync(contentsUpdates);
    }

    public int SetTableUpdateTime(Type type, DateTime dateTime = new())
    {
        if (dateTime == new DateTime()) dateTime = DateTime.Now;
        TableUpdate update = new()
        {
            TableName = Database?.GetMapping(type).TableName ?? type.ToString(),
            LastUpdate = dateTime
        };

        return Database?.InsertOrReplace(update) ?? 0;
    }

    /***************************** READ Data ******************************/

    /***************************** UPDATE Data ****************************/

    /***************************** DELETE Data ****************************/
    /* Stock */
    // Used by more than just deleter.
    public int StockZoneDeletes(List<string> zoneIDs)
    {
        var lines = 0;

        if (Database is null) return 0;

        var tableName = GetTableName(typeof(NAVStock));
        var zoneString = string.Join("','", zoneIDs);
        lines += Database.Execute($"DELETE FROM [{tableName}] WHERE [ZoneID] IN ('{zoneString}');");
        
        return lines;
    }

    /* UOM */
    public void UoMCodeDelete(List<string> uomCodes)
    {
        Database?.RunInTransaction(() =>
        {
            foreach (var uom in uomCodes)
            {
                var tableName = GetTableName(typeof(NAVUoM));
                _ = Database.Execute($"DELETE FROM [{tableName}] WHERE [Code]=?;", uom);
            }
        });
    }
}