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
}
