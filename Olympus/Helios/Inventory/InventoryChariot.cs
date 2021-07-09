using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Olympus.Helios.Inventory.Model;
using SQLiteNetExtensions.Extensions;

namespace Olympus.Helios.Inventory
{
    public enum EUoM
    {
        EACH,
        PACK,
        CASE
    }

    /// <summary>
    ///  The chariot class for transferring inventory data back and forth between the database.
    ///  Primarily handles data in DataTable formats, both for input and output.
    /// </summary>
    public class InventoryChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Inventory.sqlite";

        public override Type[] Tables { get; } = new Type[] {typeof(NAVBin), typeof(NAVCategory), typeof(NAVDivision),
                                                             typeof(NAVGenre), typeof(NAVItem), typeof(NAVLocation),
                                                             typeof(NAVPlatform), typeof(NAVStock), typeof(NAVUoM),
                                                             typeof(NAVZone), typeof(TableUpdate), typeof(BinContentsUpdate),
                                                             typeof(Bay), typeof(Stock)};

        /*************************** Constructors ****************************/

        public InventoryChariot()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, "Inventory");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory, "Sol", "Inventory");
                InitializeDatabaseConnection();
            }
        }

        public InventoryChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Inventory");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory, "Sol", "Inventory");
                InitializeDatabaseConnection();
            }
        }

        public override void ResetConnection()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, "Inventory");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory, "Sol", "Inventory");
                InitializeDatabaseConnection();
            }
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
                List<BinContentsUpdate> contentsUpdates = new List<BinContentsUpdate> { };
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
                UpdateTable(contentsUpdates);

                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        public bool SetTableUpdateTime(Type type, DateTime dateTime = new DateTime())
        {
            if (dateTime == new DateTime()) dateTime = DateTime.Now;
            try
            {
                TableUpdate update = new TableUpdate()
                {
                    TableName = Database.GetMapping(type).TableName,
                    LastUpdate = dateTime
                };
                Database.Update(update);
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
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
                        Database.Execute("DELETE FROM ? WHERE [ZoneID] = ?;", GetTableName(typeof(NAVStock)), zoneID);
                    }
                });
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
        }

        /**************************** CONVERT Data ***************************/
        public string GetUoMString(EUoM eUoM)
        {
            if (eUoM == EUoM.CASE)
                return "CASE";
            else if (eUoM == EUoM.PACK)
                return "PACK";
            else
                return "EACH";
        }
    }
}
