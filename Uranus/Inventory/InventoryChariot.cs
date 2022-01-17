using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Uranus.Inventory.Model;

namespace Uranus.Inventory
{
    /// <summary>
    ///  The chariot class for transferring inventory data back and forth between the database.
    ///  Primarily handles data in DataTable formats, both for input and output.
    /// </summary>
    public class InventoryChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Inventory.sqlite";

        public override Type[] Tables { get; } = new Type[] 
        {
            typeof(Batch), typeof(Bay), typeof(BayZone), typeof(BinContentsUpdate),
            typeof(BinExtension), typeof(Move), typeof(NAVBin), typeof(NAVCategory),
            typeof(NAVDivision), typeof(NAVGenre), typeof(NAVItem), typeof(NAVLocation),
            typeof(NAVMoveLine), typeof(NAVPlatform), typeof(NAVStock), typeof(NAVTransferOrder),
            typeof(NAVUoM), typeof(NAVZone), typeof(Stock), typeof(Store),
            typeof(SubStock), typeof(TableUpdate), typeof(ZoneAccessLevel)
        };

        /*************************** Constructors ****************************/

        public InventoryChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Inventory");
                InitializeDatabaseConnection();
            }
            catch { throw; }
        }

        /// <summary>
        /// Resets the connection using the given location string.
        /// </summary>
        /// <param name="solLocation">A directory location, in which the Inventory database does/should reside.</param>
        public void ResetConnection(string solLocation)
        {
            // First thing is to nullify the current databse (connection).
            Database = null;

            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Inventory");
                InitializeDatabaseConnection();
            }
            catch (Exception) { throw; }
        }

        /***************************** CREATE Data ****************************/

        /*                             Update Times                           */
        public bool SetStockUpdateTimes(List<NAVStock> stock)
        {
            DateTime dateTime = DateTime.Now;
            try
            {
                // Convert stock to list of BCUpdate items.
                List<NAVStock> distinctStockByZone = stock.GroupBy(s => s.ZoneID).Select(g => g.First()).ToList();
                List<BinContentsUpdate> contentsUpdates = new() { };
                foreach (NAVStock s in distinctStockByZone)
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
            catch (Exception) { throw; }
        }

        public bool SetTableUpdateTime(Type type, DateTime dateTime = new DateTime())
        {
            if (dateTime == new DateTime()) dateTime = DateTime.Now;
            try
            {
                TableUpdate update = new()
                {
                    TableName = Database.GetMapping(type).TableName,
                    LastUpdate = dateTime
                };
                _ = Database.InsertOrReplace(update);
                return true;
            }
            catch (Exception) { throw; }
        }

        /***************************** READ Data ******************************/
        
        /***************************** UPDATE Data ****************************/

        /***************************** DELETE Data ****************************/
        /* Stock */
        // Used by more than just deleter.
        public void StockZoneDeletes(List<string> zoneIDs)
        {
            try
            {
                Database.RunInTransaction(() =>
                {
                    foreach (string zoneID in zoneIDs)
                    {
                        string tableName = GetTableName(typeof(NAVStock));
                        _ = Database.Execute($"DELETE FROM [{tableName}] WHERE [ZoneID]=?;", zoneID);
                    }
                });
            }
            catch (Exception) { throw; }
        }

        /* UOM */
        public void UoMCodeDelete(List<string> uomCodes)
        {
            try
            {
                Database.RunInTransaction(() =>
                {
                    foreach (string uom in uomCodes)
                    {
                        string tableName = GetTableName(typeof(NAVUoM));
                        _ = Database.Execute($"DELETE FROM [{tableName}] WHERE [Code]=?;", uom);
                    }
                });
            }
            catch (Exception) { throw; }
        }
    }
}
