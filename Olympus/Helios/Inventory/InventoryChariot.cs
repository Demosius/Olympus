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
    ///  The chariot class for transferring inventory data back and forth between the database.
    ///  Primarily handles data in DataTable formats, both for input and output.
    /// </summary>
    public class InventoryChariot : MasterChariot
    {
        public InventoryChariot()
        {
            FilePath = Path.Combine(Environment.CurrentDirectory, "Sol", "Inventory", "Inventory.sqlite");
            Connect();
        }

        public InventoryChariot(string solLocation)
        {
            FilePath = Path.Combine(solLocation, "Inventory", "Inventory.sqlite");
            Connect();
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
                Conn.Open();
                string query = "SELECT * FROM [bin];";
                SQLiteCommand command = new SQLiteCommand(query, Conn);
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

                Conn.Close();
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
            Conn.Open();
            SQLiteCommand command = new SQLiteCommand(query, Conn);
            object result = command.ExecuteScalar();
            if (result == null || result.ToString() == "")
            {
                Conn.Close();
                return new DateTime();
            }
            dt = DateTime.Parse(result.ToString());
            Conn.Close();
            return dt;
        }

        /* STOCK UPDATES */
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
                Conn.Open();
                using (var transaction = Conn.BeginTransaction())
                {
                    foreach (DataRow row in locZoneTable.Rows)
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

        public bool SetTableUpdateTime(string tableName, DateTime dateTime)
        {
            try
            {
                Conn.Open();
                string sql = $"REPLACE INTO [update] (tbl_name, last_update) VALUES ('{tableName}', '{dateTime}');";
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
                Conn.Open();
                // Check for any missing columns.
                List<string> missingCols = Utility.ValidateTableData(locZoneTable, new List<string> {"location","zone_code"});
                if (missingCols.Count > 0) throw new InvalidDataException("Invalid Bin Data.", missingCols);

                // Build Deletion transaction.
                using (var transaction = Conn.BeginTransaction())
                {
                    SQLiteCommand command = Conn.CreateCommand();
                    
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

                Conn.Close();
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

        /***************************Table Definitions**************************/
        private static readonly string BinDefinition =
            @"-- auto-generated definition
            create table bin
            (
                location     text not null,
                zone_code    text not null
                    references zone,
                code         text not null,
                description  text,
                empty        boolean,
                assigned     boolean,
                ranking      int,
                used_cube    real,
                max_cube     real,
                last_cc_date text,
                last_pi_date text,
                constraint bin_pk
                    primary key (location, code)
            );

            create index bin_code_index
                on bin (code);";

        private static readonly string ItemDefinition =
            @"-- auto-generated definition
            create table item
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
                weight      real,
                preowned    boolean default FALSE not null
            );

            create unique index item_number_uindex
                on item (number);";

        private static readonly string UoMDefinition =
            @"-- auto-generated definition
            create table uom
            (
                code                  text not null,
                item_number           int  not null
                    references item,
                qty_per_uom           int  not null,
                max_qty               int,
                inner_pack            boolean,
                exclude_cartonization boolean,
                length                real,
                width                 real,
                height                real,
                cube                  real,
                weight                real,
                constraint uom_pk
                    primary key (code, item_number)
            );";

        private static readonly string StockDefinition =
            @"-- auto-generated definition
            create table stock
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
                fixed        boolean,
                constraint stock_pk
                    primary key (location, bin_code, item_number, uom_code)
            );";

        private static readonly string ZoneDefinition =
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

        private static readonly string StockUpdateDefinition =
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

        private static readonly string UpdateDefinition =
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

        public override Dictionary<string, string> TableDefinitions
        {
            get 
            {
                return new Dictionary<string, string>
                {
                    { "bin", BinDefinition },
                    { "item", ItemDefinition },
                    { "stock", StockDefinition },
                    { "stock_update", StockUpdateDefinition },
                    { "uom", UoMDefinition },
                    { "update", UpdateDefinition },
                    { "zone", ZoneDefinition }
                };
            }
        }
    }
}
