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
        public static DataSet DataSet()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.PullFullDataSet();
        }

        public static DataTable Bins()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.GetBinTable();
        }

        public static DataTable Stock()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.GetStockTable();
        }

        public static DateTime LastStockUpdate()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.LastTableUpdate("stock");
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
    }

    public static class PutInventory
    {
        public static bool BinsFromClipboard()
        {
            DataTable data = DataConversion.ClipboardToTable();
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.BinTableUpdate(data);
        }

        public static bool ItemsFromCSV(bool force = false)
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            string csvFile = Toolbox.GetItemCSV();

            // If the item data has been updated since the last file update don't spend time to update.
            if (chariot.LastTableUpdate("item") == File.GetLastWriteTime(csvFile) && !force) return false;

            DataTable data = DataConversion.CSVToTable(csvFile, Constants.ITEM_COLUMNS.Values.ToList(), "CompanyCode = 'AU'");
            // Set preowned column data.
            data.Columns.Add(new DataColumn("preowned"));
            foreach (DataRow row in data.Rows)
            {
                row["preowned"] = row["NewUsed"].ToString() == "Used";
            }
            return chariot.ItemTableUpdate(data, File.GetLastWriteTime(csvFile));
        }

        public static bool UoMFromClipboard()
        {
            DataTable data = DataConversion.ClipboardToTable();
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.UoMTableUpdate(data);
        }

        public static bool StockFromClipboard()
        {
            DataTable data = DataConversion.ClipboardToTable();
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.StockTableUpdate(data);
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

        // Construct from batabase.
        public BinContents(InventoryChariot chariot)
        {
            DateTime = chariot.LastTableUpdate("stock");
            DataTable data = chariot.GetStockTable();
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

        private List<SimpleStock> DataTableToStockList(DataTable data)
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
