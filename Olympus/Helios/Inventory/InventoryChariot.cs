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
            throw new NotImplementedException();
        }

        public bool SetTableUpdateTime(string tableName)
        {
            throw new NotImplementedException();
        }

        /******************************* Set Data *****************************/
        public bool BinTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.BIN_COLUMNS, "bin");
        }

        public bool ItemTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.ITEM_COLUMNS, "item");
        }

        public bool UoMTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.UOM_COLUMNS, "uom");
        }

        public bool ZoneTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.ZONE_COLUMNS, "zone");
        }

        public bool StockTableUpdate(DataTable data)
        {
            return ReplaceFullTable(data, Constants.STOCK_COLUMNS, "stock");
        }
        
        /***************************** Get Data ******************************/
        public DataTable GetBinTable()
        {
            return PullFullTable("bin");
        }

        public DataTable GetItemTable()
        {
            return PullFullTable("item");
        }

        public DataTable GetZoneTable()
        {
            return PullFullTable("zone");
        }

        public DataTable GetStockTable()
        {
            return PullFullTable("stock");
        }

        public DataTable GetUoMTable()
        {
            return PullFullTable("uom");
        }

        public DataTable GetUpdateTable()
        {
            return PullFullTable("update");
        }

        public DataTable GetStockUpdateTable()
        {
            return PullFullTable("stock_update");
        }

        /**************************DATABASE MANAGEMENT************************/
        public override bool BuildDatabase()
        {
            if (File.Exists(FilePath)) return false;
            try
            {
                // Create File and reconnect.
                SQLiteConnection.CreateFile(FilePath);
                Connect();
                Conn.Open();
                // Build Tables
                foreach (string sql in TableDefinitions.DefinitionList)
                {
                    SQLiteCommand command = new SQLiteCommand(sql, Conn);
                    command.ExecuteNonQuery
                }
                Conn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

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

        public override bool EmptyDatabase()
        {
            throw new NotImplementedException();
        }

        public override bool RepairDatabase()
        {
            throw new NotImplementedException();
        }

        public override bool ValidateDatabase()
        {
            throw new NotImplementedException();
        }

        public override void CreateTable()
        {
            throw new NotImplementedException();
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
    }
}
