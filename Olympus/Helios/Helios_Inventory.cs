using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Olympus.Helios.Inventory;

namespace Olympus.Helios
{
    public static class PushInventory
    {
        public static bool BinsFromClipboard()
        {
            DataTable data = DataConversion.ClipboardToTable();
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.BinTableUpdate(data);
        }

        public static bool ItemsFromCSV()
        {
            DataTable data = DataConversion.CSVToTable(Toolbox.GetItemCSV(), Constants.ITEM_COLUMNS.Values.ToList(), "CompanyCode = 'AU'");
            // Set preowned column data.
            data.Columns.Add(new DataColumn("preowned"));
            foreach (DataRow row in data.Rows)
            {
                row["preowned"] = row["NewUsed"].ToString() == "Used";
            }
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.ItemTableUpdate(data);
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
            InventoryChariot chariot = new InventoryChariot();
            return chariot.StockTableUpdate(data);
        }
    }

    public static class PullInventory
    {
        public static DataTable Bins()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
            return chariot.GetBinTable();
        }

        public static string Bins(string lik)
        {
            return lik;
        }
    }

    /// <summary>
    ///  Represents a basic class for use of holding stock items in a list and converting to JSON.
    /// </summary>
    public class BinContents
    {
        public DateTime DateTime { get; set; }
        public List<SimpleStock> Stock { get; set; }

        public BinContents()
        {
            InventoryChariot chariot = new InventoryChariot(Toolbox.GetSol());
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

        private List<SimpleStock> DataTableToStockList(DataTable data)
        {
            List<SimpleStock> newList = new List<SimpleStock> { };

            foreach (DataRow row in data.Rows)
            {
                newList.Add(new SimpleStock(row["location"], row["zone_code"], row["bin"], row["location"], row["location"], row["location"], row["location"], row["location"], row["location"], row["location"], row["location"], row["location"], row["location"]));
            }

            return newList;
        }
    }
}
