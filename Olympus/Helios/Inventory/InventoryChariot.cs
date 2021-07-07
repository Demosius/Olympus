using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Olympus.Helios.Inventory.Model;

namespace Olympus.Helios.Inventory
{
    /// <summary>
    ///  The chariot class for transferring inventory data back and forth between the database.
    ///  Primarily handles data in DataTable formats, both for input and output.
    /// </summary>
    public class InventoryChariot : MasterChariot
    {
        public InventoryChariot()
        {
            DatabaseName = Path.Combine(Environment.CurrentDirectory, "Sol", "Inventory", "Inventory.sqlite");
            InitializeDatabaseConnection();
        }

        public InventoryChariot(string solLocation)
        {
            DatabaseName = Path.Combine(solLocation, "Inventory", "Inventory.sqlite");
            InitializeDatabaseConnection();
        }

        /***************************** Get Data ******************************/
        /* BINS */
        public DataTable GetBin(string bin)
        {
            string query = $"SELECT * FROM [bin] WHERE code = '{bin}';";
            return PullTableWithQuery(query);
        }

        public DataTable GetBinsWithContents()
        {
            string query = $"SELECT bin.*, stock.* FROM bin INNER JOIN stock ON bin.code = stock.bin_code;";
            return PullTableWithQuery(query);
        }

        public List<SimpleBin> SimpleBins()
        {
            try
            {
                List<SimpleBin> list = new List<SimpleBin> { };
                Database.Open();
                string query = "SELECT * FROM [bin];";
                SQLiteCommand command = new SQLiteCommand(query, Database);
                SQLiteDataReader reader = command.ExecuteReader();
                string mc;

                while (reader.Read())
                {
                    mc = reader["max_cube"].ToString();
                    list.Add
                    (
                        new SimpleBin
                        (
                            location: reader["location"].ToString(),
                            zoneCode: reader["zone_code"].ToString(),
                            code: reader["code"].ToString(),
                            description: reader["description"].ToString(),
                            empty: Convert.ToBoolean(reader["empty"]),
                            assigned: Convert.ToBoolean(reader["assigned"]),
                            ranking: (int)reader["ranking"],
                            usedCube: (double)reader["used_cube"],
                            maxCube: double.Parse(mc == "" ? "0" : mc),
                            lastCCDate: DateTime.Parse(reader["last_cc_date"].ToString()),
                            lastPIDate: DateTime.Parse(reader["last_pi_date"].ToString())
                        )
                    ); 
                }

                Database.Close();
                return list;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new List<SimpleBin> { };
            }
        }

        /* ITEMS */

        public DataTable GetItem(int item)
        {
            string query = $"SELECT * FROM [item] WHERE number = {item};";
            return PullTableWithQuery(query);
        }

        /* ZONES */

        /* STOCK */

        public DataTable GetStockForBin(string bin)
        {
            string query = $"SELECT * FROM [stock] WHERE bin_code = '{bin}';";
            return PullTableWithQuery(query);
        }

        public DataTable GetStockFromItem(int Item)
        {
            string query = $"SELECT * FROM [stock] WHERE item_number = {Item};";
            return PullTableWithQuery(query);
        }

        /* UOM */
        public DataTable GetItemUoM(int item)
        {
            string query = $"SELECT * FROM [uom] WHERE item_number = {item};";
            return PullTableWithQuery(query);
        }

        /* UPDATES */
        public DateTime LastTableUpdate(string tableName)
        {
            string query = $"SELECT [last_update] FROM [update] WHERE tbl_name = '{tableName}';";
            DateTime dt;
            Database.Open();
            SQLiteCommand command = new SQLiteCommand(query, Database);
            object result = command.ExecuteScalar();
            if (result == null || result.ToString() == "")
            {
                Database.Close();
                return new DateTime();
            }
            dt = DateTime.Parse(result.ToString());
            Database.Close();
            return dt;
        }

        /* STOCK UPDATES */
        public DateTime LastStockUpdate(List<string> zones, string location = "9600")
        {
            string query = $"SELECT MIN([last_update]) FROM [stock_update] WHERE [location] = '{location}' AND [zone_code] IN ('{string.Join("', '", zones)}') ";
            DateTime dt;
            Database.Open();
            SQLiteCommand command = new SQLiteCommand(query, Database);
            object result = command.ExecuteScalar();
            if (result == null || result.ToString() == "") return new DateTime();
            dt = DateTime.Parse(result.ToString());
            Database.Close();
            return dt;
        }

        /* Zone-Location */
        public DataTable GetLocZoneUniqueTable(DataTable stockData)
        {
            try
            {
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(stockData, new Dictionary<string, string> { { "location", "Location Code" }, { "zone_code", "Zone Code" } });
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                DataView view = new DataView(stockData);
                return view.ToTable(true, "location", "zone_code");
            }
            catch (InvalidDataException)
            {
                // No need to show error, as the error will show on the next attempted transaction.
                return new DataTable();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new DataTable();
            }
        }

        /*                             Update Times                           */
        public bool SetStockUpdateTimes(DataTable locZoneTable)
        {
            DateTime dt = DateTime.Now;
            try
            {
                Database.Open();
                using (var transaction = Database.BeginTransaction())
                {
                    foreach (DataRow row in locZoneTable.Rows)
                    {
                        string sql = $"REPLACE INTO [stock_update] (location, zone_code, last_update) VALUES ('{row["location"]}', '{row["zone_code"]}', '{dt}');";
                        SQLiteCommand command = new SQLiteCommand(sql, Database);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }

                Database.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        public bool SetTableUpdateTime(string tableName, DateTime dateTime)
        {
            try
            {
                Database.Open();
                string sql = $"REPLACE INTO [update] (tbl_name, last_update) VALUES ('{tableName}', '{dateTime}');";
                SQLiteCommand command = new SQLiteCommand(sql, Database);
                command.ExecuteNonQuery();
                Database.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }


        /******************************* Put Data *****************************/
        public bool BinTableUpdate(DataTable data)
        {
            if (ReplaceFullTable(data, Constants.BIN_COLUMNS, "bin"))
            {
                SetTableUpdateTime("bin", DateTime.Now);
                return true;
            }
            return false;
        }

        public bool ItemTableUpdate(DataTable data, DateTime dateTime)
        {
            if (ReplaceFullTable(data, Constants.ITEM_COLUMNS, "item"))
            {
                SetTableUpdateTime("item", dateTime);
                return true;
            }
            return false;
        }

        public bool UoMTableUpdate(DataTable data)
        {
            if (OverlayTable(data, Constants.UOM_COLUMNS, "uom"))
            {
                SetTableUpdateTime("uom", DateTime.Now);
                return true;
            }
            return false;
        }

        public bool ZoneTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.ZONE_COLUMNS, "zone");
        }

        public bool StockTableUpdate(DataTable stockData)
        {
            DataTable locZoneTable = GetLocZoneUniqueTable(stockData);
            StockLocZoneDelete(locZoneTable);
            if (OverlayTable(stockData, Constants.STOCK_COLUMNS, "stock"))
            {
                SetTableUpdateTime("stock", DateTime.Now);
                SetStockUpdateTimes(locZoneTable);
                return true;
            }
            return false;
        }

        /***************************** Delete Data ****************************/
        /* Stock */
        public void StockLocZoneDelete(DataTable locZoneTable)
        {
            try
            {
                Database.Open();
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(locZoneTable, new List<string> {"location","zone_code"});
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                // Build Deletion transaction.
                using (var transaction = Database.BeginTransaction())
                {
                    SQLiteCommand command = Database.CreateCommand();
                    
                    foreach (DataRow row in locZoneTable.Rows)
                    {
                        command.CommandText =
                        $@"
                            DELETE FROM [stock]
                            WHERE location = '{row["location"]}' 
                            AND zone_code = '{row["zone_code"]}'
                        ";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();

                }

                Database.Close();
            }
            catch (InvalidDataException)
            {
                // pass
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
        }

        public override Type[] Tables { get; } = new Type[] {typeof(NAVBin), typeof(NAVCategory), typeof(NAVDivision), 
                                                             typeof(NAVGenre), typeof(NAVItem), typeof(NAVLocation), 
                                                             typeof(NAVPlatform), typeof(NAVStock), typeof(NAVUoM), 
                                                             typeof(NAVZone)};
    }
}
