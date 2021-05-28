using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Olympus.Helios.Inventory
{
    /// <summary>
    ///  The chariot class for transferring data back and forth between the database.
    ///  Primarily handles data in DataTable formats, both for input and output.
    /// </summary>
    public class InventoryChariot : MasterChariot
    {
        public InventoryChariot() 
        {
            FilePath = "./Sol/Inventory/Inventory.sqlite";
            Connect();
        }

        public InventoryChariot(string solLocation)
        {
            FilePath = solLocation + "/Inventory/Inventory.sqlite";
            Connect();
        }

        /***************************** Update Times **************************/
        public bool SetStockUpdateTimes(DataTable stockData)
        {
            DataView view = new DataView(stockData);
            DataTable uniqueTable = view.ToTable(true, "location", "zone_code");
            DateTime dt = DateTime.Now;
            try
            {
                Conn.Open();
                using (var transaction = Conn.BeginTransaction())
                {
                    foreach (DataRow row in uniqueTable.Rows)
                    {
                        string sql = $"REPLACE INTO [stock_update] (location, zone_code, last_update) VALUES ('{row["location"]}', '{row["zone_code"]}', '{dt}');";
                        SQLiteCommand command = new SQLiteCommand(sql, Conn);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }

                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        public bool SetTableUpdateTime(string tableName)
        {
            try
            {
                Conn.Open();
                string sql = $"REPLACE INTO [update] (tbl_name, last_update) VALUES ('{tableName}', '{DateTime.Now}');";
                SQLiteCommand command = new SQLiteCommand(sql, Conn);
                command.ExecuteNonQuery();
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /******************************* Set Data *****************************/
        public bool BinTableUpdate(DataTable data)
        {
            if (ReplaceFullTable(data, Constants.BIN_COLUMNS, "bin"))
            {
                SetTableUpdateTime("bin");
                return true;
            }
            return false;
        }

        public bool ItemTableUpdate(DataTable data)
        {
            if (ReplaceFullTable(data, Constants.ITEM_COLUMNS, "item"))
            {
                SetTableUpdateTime("item");
                return true;
            }
            return false;
        }

        public bool UoMTableUpdate(DataTable data)
        {
            if (ReplaceFullTable(data, Constants.UOM_COLUMNS, "uom"))
            {
                SetTableUpdateTime("uom");
                return true;
            }
            return false;
        }

        public bool ZoneTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.ZONE_COLUMNS, "zone");
        }

        public bool StockTableUpdate(DataTable data)
        {
            if (ReplaceFullTable(data, Constants.STOCK_COLUMNS, "stock"))
            {
                SetTableUpdateTime("stock");
                SetStockUpdateTimes(data);
                return true;
            }
            return false;
        }
        
        /***************************** Get Data ******************************/
        /* BINS */
        public DataTable GetBinTable()
        {
            return PullFullTable("bin");
        }

        public DataTable GetBin(string bin)
        {
            string query = $"SELECT * FROM [bin] WHERE code = '{bin}';";
            return PullTableWithQuery(query);
        }

        /* ITEMS */
        public DataTable GetItemTable()
        {
            return PullFullTable("item");
        }

        public DataTable GetItem(int item)
        {
            string query = $"SELECT * FROM [item] WHERE number = {item};";
            return PullTableWithQuery(query);
        }

        /* ZONES */
        public DataTable GetZoneTable()
        {
            return PullFullTable("zone");
        }

        /* STOCK */
        public DataTable GetStockTable()
        {
            return PullFullTable("stock");
        }

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
        public DataTable GetUoMTable()
        {
            return PullFullTable("uom");
        }

        public DataTable GetItemUoM(int item)
        {
            string query = $"SELECT * FROM [uom] WHERE item_number = {item};";
            return PullTableWithQuery(query);
        }

        /* UPDATES */
        public DataTable GetUpdateTable()
        {
            return PullFullTable("update");
        }

        public DateTime LastTableUpdate(string tableName)
        {
            string query = $"SELECT [last_update] FROM [update] WHERE tbl_name = '{tableName}';";
            DateTime dt;
            Conn.Open();
            SQLiteCommand command = new SQLiteCommand(query, Conn);
            object result = command.ExecuteScalar();
            if (result == null || result.ToString() == "") return new DateTime();
            dt = DateTime.Parse(result.ToString());
            Conn.Close();
            return dt;
        }

        /* STOCK UPDATES */
        public DataTable GetStockUpdateTable()
        {
            return PullFullTable("stock_update");
        }

        public DateTime LastStockUpdate(List<string> zones, string location = "9600")
        {
            string query = $"SELECT MIN([last_update]) FROM [stock_update] WHERE [location] = '{location}' AND [zone_code] IN ('{string.Join("', '", zones)}') ";
            DateTime dt;
            Conn.Open();
            SQLiteCommand command = new SQLiteCommand(query, Conn);
            object result = command.ExecuteScalar();
            if (result == null || result.ToString() == "") return new DateTime();
            dt = DateTime.Parse(result.ToString());
            Conn.Close();
            return dt;
        }

        /**************************DATABASE MANAGEMENT************************/
        /// <summary>
        ///  Builds the database from scratch.
        ///  Fails if the file already exists.
        /// </summary>
        /// <returns></returns>
        public override bool BuildDatabase()
        {
            if (File.Exists(FilePath)) return false;
            try
            {
                // Create File and reconnect.
                SQLiteConnection.CreateFile(FilePath);
                Connect();
                // Build Tables
                CreateTables();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///  Deletes the database file.
        /// </summary>
        /// <returns></returns>
        public override bool DeleteDatabase()
        {
            if (!File.Exists(FilePath)) return true;
            try
            {
                File.Delete(FilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///  Empties all tables of data.
        /// </summary>
        /// <returns></returns>
        public override bool EmptyDatabase()
        {
            try
            {
                Conn.Open();
                using (var transaction = Conn.BeginTransaction()) 
                {
                    foreach (string table in TableDefinitions.DefinitionDict.Keys)
                    {
                        string sql = $"DELETE FROM {table};";
                        SQLiteCommand command = new SQLiteCommand(sql, Conn);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Goes through validation checks, but aims to repair the database if it failes.
        ///  Most avenues lead to rebuilding from scratch.
        /// </summary>
        /// <returns></returns>
        public override bool RepairDatabase()
        {
            try
            {
                // If file does not exist, build from scratch.
                if (!File.Exists(FilePath))
                {
                    if (!BuildDatabase())
                        return false;
                }
                // If connection cannot be made, try to delete and then rebuild from scratch.
                Conn = new SQLiteConnection($@"URI=file:{FilePath}");
                if (Conn == null)
                {
                    DeleteDatabase();
                    BuildDatabase();
                }
                Conn.Open();
                // If Opening failes, rebuild from scratch.
                if (Conn.State == ConnectionState.Closed)
                {
                    DeleteDatabase();
                    BuildDatabase();
                }

                // Check for each table, and create it if it is missing.
                // Check tables exist.
                DataTable tables = Conn.GetSchema("Tables");
                string tblName;
                // Create a dictionary for checking tables.
                Dictionary<string, bool> tableCheck = new Dictionary<string, bool> { };
                foreach (string name in TableDefinitions.DefinitionDict.Keys)
                    tableCheck.Add(name, false);

                // Check through table schema to make sure all necessary tables exist.
                // (Extra tables are not of concern.)
                foreach (DataRow row in tables.Rows)
                {
                    tblName = row["TABLE_NAME"].ToString();
                    if (tableCheck.ContainsKey(tblName))
                    {
                        tableCheck[tblName] = true;
                    }
                }

                foreach (KeyValuePair<string, bool> pair in tableCheck)
                {
                    if (!pair.Value)
                    {
                        CreateTable(pair.Key);
                    }
                }

                Conn.Close();
                return true;
            }
            catch (FailedConnectionException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Checks if the database is valid.
        ///  Checks to the level of table existance - not columns at this point.
        /// </summary>
        /// <returns></returns>
        public override bool ValidateDatabase()
        {
            try
            {
                // Check file exists.
                if (!File.Exists(FilePath)) return false;

                // Check connection works.
                Connect();

                // Check Connection can be opened.
                Conn.Open();

                // Check tables exist.
                DataTable tables = Conn.GetSchema("Tables");
                string tblName;
                // Create a dictionary for checking tables.
                Dictionary<string, bool> tableCheck = new Dictionary<string, bool> { };
                foreach (string name in TableDefinitions.DefinitionDict.Keys)
                    tableCheck.Add(name, false);

                // Check through table schema to see if all necessary tables exist.
                // (Extra tables are not of concern.)
                foreach (DataRow row in tables.Rows)
                {
                    tblName = row["TABLE_NAME"].ToString();
                    if (tableCheck.ContainsKey(tblName))
                    {
                        tableCheck[tblName] = true;
                    }
                }

                foreach (bool pass in tableCheck.Values)
                {
                    if (!pass)
                    {
                        return false;
                    }
                }

                // Close the connection.
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }
        
        /// <summary>
        ///  Create table based on definition derived by string.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override bool CreateTable(string tableName)
        {
            tableName = tableName.ToLower();
            if (!TableDefinitions.DefinitionDict.ContainsKey(tableName))
            {
                MessageBox.Show($"{tableName} is not a valid table name.");
                return false;
            }
            try
            {
                Conn.Open();
                SQLiteCommand command = new SQLiteCommand(TableDefinitions.DefinitionDict[tableName], Conn);
                command.ExecuteNonQuery();
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /// <summary>
        ///  Create all required tables based on table definitions.
        /// </summary>
        /// <returns></returns>
        public override bool CreateTables()
        {
            try
            {
                Conn.Open();
                using (var transaction = Conn.BeginTransaction())
                {
                    foreach (string sql in TableDefinitions.DefinitionList)
                    {
                        SQLiteCommand command = new SQLiteCommand(sql, Conn);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }
    }

    static class TableDefinitions
    {
        public static readonly string BinDefinition = 
            @"create table bin
            (
                location     text not null,
                zone_code    text not null
                    references zone,
                code         text not null,
                description  text,
                empty        int,
                assigned     int,
                ranking      int,
                used_cube    real,
                max_cube     real,
                last_cc_date text,
                last_pi_date text,
                constraint bin_pk
                    primary key(location, code)
            );

            create index bin_code_index
                on bin(code);";

        public static readonly string ItemDefinition =
            @"create table item
            (
                number      int not null
                    constraint item_pk
                        primary key,
                description text,
                category    int,
                platform    int,
                division    int,
                genre       int,
                length      real,
                width       real,
                height      real,
                cube        real,
                weight      real
            );

            create unique index item_number_uindex
                on item (number);";

        public static readonly string UoMDefinition =
            @"create table uom
            (
                code                  text not null,
                item_number           int  not null
                    references item,
                qty_per_uom           int  not null,
                max_qty               int,
                inner_pack            int,
                exclude_cartonization int,
                length                real,
                width                 real,
                height                real,
                cube                  real,
                weight                real,
                constraint uom_pk
                    primary key (code, item_number)
            );";

        public static readonly string StockDefinition =
            @"create table stock
            (
                location     text not null,
                zone_code    text not null
                    references zone,
                bin_code     text not null
                    constraint stock_bin_code_fk
                        references bin (code),
                item_number  int  not null
                    references item,
                barcode      text,
                uom_code     text not null
                    constraint stock_uom_code_fk
                        references uom (code),
                qty          int  not null,
                pick_qty     int,
                put_away_qty int,
                neg_adj_qty  int,
                pos_adj_qty  int,
                date_created text,
                time_created text,
                fixed        int,
                constraint stock_pk
                    primary key (location, bin_code, item_number, uom_code)
            );";

        public static readonly string ZoneDefinition =
            @"create table zone
            (
                code        text not null
                    constraint zone_pk
                        primary key,
                type        text not null,
                description text
            );

            create unique index zone_code_uindex
                on zone (code);";

        public static readonly string StockUpdateDefinition =
            @"create table stock_update
            (
                location    text not null,
                zone_code   text not null,
                last_update text,
                constraint stock_update_pk
                    primary key (location, zone_code),
                constraint stock_update_stock_location_zone_code_fk
                    foreign key (location, zone_code) references stock (location, zone_code)
                        on update cascade on delete cascade
            );";

        public static readonly string UpdateDefinition =
            @"create table ""update""
            (
                tbl_name text not null
                    constraint update_pk
                        primary key
                    constraint update_sqlite_master_tbl_name_fk
                        references sqlite_master(tbl_name)
                        on update cascade on delete cascade,
                last_update text
            );

            create unique index update_tbl_name_uindex
                on ""update"" (tbl_name);";

        public static readonly List<string> DefinitionList = new List<string>
        {
            BinDefinition,
            ItemDefinition,
            StockDefinition,
            StockUpdateDefinition,
            UoMDefinition,
            UpdateDefinition,
            ZoneDefinition
        };

        public static readonly Dictionary<string, string> DefinitionDict = new Dictionary<string, string>
        {
            {"bin", BinDefinition },
            {"item", ItemDefinition },
            {"stock", StockDefinition },
            {"stock_update", StockUpdateDefinition },
            {"uom", UoMDefinition },
            {"update", UpdateDefinition },
            {"zone", ZoneDefinition }
        };
    }
}
