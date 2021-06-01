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
            FilePath = "./Sol/Inventory/Inventory.sqlite";
            Connect();
        }

        public InventoryChariot(string solLocation)
        {
            FilePath = solLocation + "/Inventory/Inventory.sqlite";
            Connect();
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

        /*                             Update Times                           */
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


        /******************************* Put Data *****************************/
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

        /**************************Table Definitions************************/
        private static readonly string BinDefinition =
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

        private static readonly string ItemDefinition =
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

        private static readonly string UoMDefinition =
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

        private static readonly string StockDefinition =
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
