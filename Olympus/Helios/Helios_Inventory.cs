using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows;
using Olympus.Helios.Inventory;

namespace Olympus.Helios
{

    public static class GetInventory
    {
        /* Full Data */
        public static DataSet DataSet()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.PullFullDataSet();
        }

        /* Data Tables */
        private static DataTable TableByName(string tableName)
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.PullFullTable(tableName);
        }

        public static DataTable DataTable(string tableName)
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            if (chariot.TableDefinitions.Keys.Contains(tableName) && !tableName.StartsWith("sqlite_"))
                return TableByName(tableName);
            return new DataTable();
        }

        public static DataTable BinTable()
        {
            return TableByName("bin");
        }

        public static DataTable ItemTable()
        {
            return TableByName("item");
        }

        public static DataTable UoMTable()
        {
            return TableByName("uom");
        }

        public static DataTable StockTable()
        {
            return TableByName("stock");
        }

        public static DataTable StockUpdateTable()
        {
            return TableByName("stock_update");
        }

        public static DataTable UpdateTable()
        {
            return TableByName("update");
        }

        public static DataTable ZoneTable()
        {
            return TableByName("zone");
        }

        /* Update times */
        public static DateTime LastStockUpdateTime()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.LastTableUpdate("stock");
        }

        public static DateTime LastBinUpdateTime()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.LastTableUpdate("bin");
        }

        public static DateTime LastItemUpdateTime()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.LastTableUpdate("item");
        }

        public static DateTime LastUoMUpdateTime()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.LastTableUpdate("uom");
        }


        /* Special/Specific pull types. */
        public static BinContents BinContents()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return new BinContents(chariot);
        }

        public static List<SimpleBin> SimpleBins()
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            return chariot.SimpleBins();
        }

        public static BinContents BCFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                BinContents bc = JsonSerializer.Deserialize<BinContents>(json);
                return bc;
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show($"{ex.Message}: {filePath}");
                return new BinContents(new List<SimpleStock> { });
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"{ex.Message}");
                return new BinContents(new List<SimpleStock> { });
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return new BinContents(new List<SimpleStock> { });
            }
        }

        /* Table Columns */
        public static Dictionary<string, string> BinColumnDict()
        {
            return Constants.BIN_COLUMNS;
        }

        public static Dictionary<string, string> UoMColumnDict()
        {
            return Constants.UOM_COLUMNS;
        }

        public static Dictionary<string, string> StockColumnDict()
        {
            return Constants.STOCK_COLUMNS;
        }
    }

    public static class PutInventory
    {
        public static bool BinsFromClipboard()
        {
            try
            {
                DataTable data = DataConversion.ClipboardToTable();
                List<string> missingCols = Utility.ValidateTableData(data, Constants.BIN_COLUMNS);
                if (missingCols.Count > 0) throw new InvalidDataException(missingCols);
                DataConversion.ConvertColumns(
                    dataTable: ref data,
                    dblColumns: new List<string> { "used_cube", "max_cube" },
                    intColumns: new List<string> { "ranking" },
                    dtColumns: new List<string> { "last_cc_date", "last_pi_date" },
                    boolColumns: new List<string> { "empty", "assigned" });
                InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
                return chariot.BinTableUpdate(data);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return false;
        }

        public static bool ItemsFromCSV(bool force = false)
        {
            InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
            string csvFile = App.Settings.ItemCSVLocation;

            // If the item data has been updated since the last file update don't spend time to update.
            if (chariot.LastTableUpdate("item") == File.GetLastWriteTime(csvFile) && !force) return false;

            try
            {
                DataTable data = DataConversion.CSVToTable(csvFile, Constants.ITEM_COLUMNS.Values.ToList(), "CompanyCode = 'AU'");
                // Validate data
                List<string> missingCols = Utility.ValidateTableData(data, Constants.ITEM_COLUMNS);
                if (missingCols.Count > 0) throw new InvalidDataException(missingCols);
                DataConversion.ConvertColumns(
                    dataTable: ref data,
                    dblColumns: new List<string> { "length", "width", "height", "weight", "cube" },
                    intColumns: new List<string> { "number", "category", "platform", "division", "genre" },
                    dtColumns: new List<string> { },
                    boolColumns: new List<string> { "preowned" });

                return chariot.ItemTableUpdate(data, File.GetLastWriteTime(csvFile));
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
            return false;
        }

        public static bool UoMFromClipboard()
        {
            try
            {
                DataTable data = DataConversion.ClipboardToTable();
                List<string> missingCols = Utility.ValidateTableData(data, Constants.UOM_COLUMNS);
                if (missingCols.Count > 0) throw new InvalidDataException(missingCols);
                DataConversion.ConvertColumns(
                    dataTable: ref data,
                    dblColumns: new List<string> { "length", "width", "height", "weight", "cube" },
                    intColumns: new List<string> { "item_number", "qty_per_uom", "max_qty" },
                    dtColumns: new List<string> { },
                    boolColumns: new List<string> { "inner_pack", "exclude_cartonization" });
                InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
                return chariot.UoMTableUpdate(data);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
return false;
        }

        public static bool StockFromClipboard()
        {
            try
            {
                DataTable data = DataConversion.ClipboardToTable();
                List<string> missingCols = Utility.ValidateTableData(data, Constants.STOCK_COLUMNS);
                if (missingCols.Count > 0) throw new InvalidDataException(missingCols);
                DataConversion.ConvertColumns(
                    dataTable: ref data,
                    dblColumns: new List<string> { },
                    intColumns: new List<string> { "item_number", "qty", "pick_qty", "put_away_qty", "neg_adj_qty", "pos_adj_qty" },
                    dtColumns: new List<string> { "date_created", "time_created" },
                    boolColumns: new List<string> { "fixed" });
                InventoryChariot chariot = new InventoryChariot(App.Settings.SolLocation);
                return chariot.StockTableUpdate(data);
            }
            catch (InvalidDataException ex)
            {
                ex.DisplayErrorMessage();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
            }
return false;
        }

        public static bool BCFromDB(string dir)
        {
            try
            {
                BinContents binContents = new BinContents();
                string json = JsonSerializer.Serialize(binContents, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"{dir}/BC_{binContents.DateTime:yyyyMMdd-hhmm}.json", json);
                return true;
            }
            catch (NotSupportedException ex)
            {
                BinContents binContents = new BinContents();
                MessageBox.Show($"{ex.Message}: \"{dir}/BC_{binContents.DateTime:yyyyMMdd-hhmm}.json\"");
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }
    }

    public static class PostInventory
    {

    }

    public static class DeleteInventory
    {

    }

    /// <summary>
    ///  Represents a basic class for use of holding stock items in a list and converting to JSON.
    /// </summary>
    public class BinContents
    {
        public DateTime DateTime { get; set; }
        public List<SimpleStock> Stock { get; set; }

        // Default constructor.
        public BinContents() { }

        // Construct from database.
        public BinContents(InventoryChariot chariot)
        {
            DateTime = chariot.LastTableUpdate("stock");
            DataTable data = chariot.PullFullTable("stock");
            Stock = DataTableToStockList(data);
        }

        public BinContents(List<SimpleStock> stock)
        {
            DateTime = DateTime.Now;
            Stock = stock;
        }

        public BinContents(DataTable data)
        {
            DateTime = DateTime.Now;
            Stock = DataTableToStockList(data);
        }

        public BinContents(DateTime dateTime, List<SimpleStock> stock)
        {
            DateTime = dateTime;
            Stock = stock;
        }

        public BinContents(DateTime dateTime, DataTable data)
        {
            DateTime = dateTime;
            Stock = DataTableToStockList(data);
        }

        public static List<SimpleStock> DataTableToStockList(DataTable data)
        {
            List<SimpleStock> newList = new List<SimpleStock> { };

            foreach (DataRow row in data.Rows)
            {
                newList.Add(new SimpleStock(
                    location: row["location"].ToString(),
                    zoneCode: row["zone_code"].ToString(), 
                    binCode: row["bin_code"].ToString(), 
                    itemNumber: (int)row["item_number"], 
                    barCode: row["barcode"].ToString(), 
                    uomCode: row["uom_code"].ToString(), 
                    qty: (int)row["qty"], 
                    pickQty: (int)row["pick_qty"], 
                    putAwayQty: (int)row["put_away_qty"], 
                    negAdjQty: (int)row["neg_adj_qty"], 
                    posAdjQty: (int)row["pos_adj_qty"], 
                    dateCreated: DateTime.Parse(row["date_created"].ToString()), 
                    timeCreated:DateTime.Parse(row["time_created"].ToString()), 
                    _fixed: Convert.ToBoolean(row["fixed"])));
            }

            return newList;
        }
    }
}
