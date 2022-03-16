using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uranus.Inventory.Model;

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
        typeof(Batch), typeof(Bay), typeof(BayZone), typeof(BinContentsUpdate),
        typeof(BinExtension), typeof(Move), typeof(NAVBin), typeof(NAVCategory),
        typeof(NAVDivision), typeof(NAVGenre), typeof(NAVItem), typeof(NAVLocation),
        typeof(NAVMoveLine), typeof(NAVPlatform), typeof(NAVStock), typeof(NAVTransferOrder),
        typeof(NAVUoM), typeof(NAVZone), typeof(Stock), typeof(Store),
        typeof(SubStock), typeof(TableUpdate), typeof(ZoneExtension)
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
    public bool SetStockUpdateTimes(List<NAVStock> stock)
    {
        var dateTime = DateTime.Now;
        // Convert stock to list of BCUpdate items.
        var distinctStockByZone = stock.GroupBy(s => s.ZoneID).Select(g => g.First()).ToList();
        List<BinContentsUpdate> contentsUpdates = new();
        foreach (var s in distinctStockByZone)
        {
            contentsUpdates.Add(new BinContentsUpdate
            {
                ZoneID = s.ZoneID,
                ZoneCode = s.ZoneCode,
                LocationCode = s.LocationCode,
                LastUpdate = dateTime
            });
        }

        // Update Database
        _ = UpdateTable(contentsUpdates);

        return true;
    }

    public bool SetTableUpdateTime(Type type, DateTime dateTime = new())
    {
        if (dateTime == new DateTime()) dateTime = DateTime.Now;
        TableUpdate update = new()
        {
            TableName = Database.GetMapping(type).TableName,
            LastUpdate = dateTime
        };
        _ = Database.InsertOrReplace(update);
        return true;
    }

    /***************************** READ Data ******************************/
        
    /***************************** UPDATE Data ****************************/

    /***************************** DELETE Data ****************************/
    /* Stock */
    // Used by more than just deleter.
    public void StockZoneDeletes(List<string> zoneIDs)
    {
        Database.RunInTransaction(() =>
        {
            foreach (var zoneID in zoneIDs)
            {
                var tableName = GetTableName(typeof(NAVStock));
                _ = Database.Execute($"DELETE FROM [{tableName}] WHERE [ZoneID]=?;", zoneID);
            }
        });
    }

    /* UOM */
    public void UoMCodeDelete(List<string> uomCodes)
    {
        Database.RunInTransaction(() =>
        {
            foreach (var uom in uomCodes)
            {
                var tableName = GetTableName(typeof(NAVUoM));
                _ = Database.Execute($"DELETE FROM [{tableName}] WHERE [Code]=?;", uom);
            }
        });
    }
}