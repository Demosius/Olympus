using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Olympus.Helios;
using Olympus.Helios.Inventory;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Globalization;

namespace Olympus.Prometheus
{
    /// <summary>
    /// Interaction logic for Prometheus.xaml
    /// </summary>
    public partial class Prometheus : Page
    {
        public DataSet UserData { get; set; }
        public DataSet StaffData { get; set; }
        public DataSet EquipmentData { get; set; }
        public DataSet InventoryData { get; set; }
        public DataTable DataTable { get; set; }

        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 25;

        public string PageString { get; set; } = "Page: 0 of 0";

        public Prometheus()
        {
            InitializeComponent();
            SetData();
            SetPageString();
        }

        public void SetData()
        {
            UserData = new DataSet();
            InventoryData = new DataSet();
            StaffData = new DataSet();
            EquipmentData = new DataSet();
        }

        private void SetTable(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            string[] tags = item.Tag.ToString().Split(',');
            string dbName = tags[0];
            string tblName = tags[1];
            DataSet dataSet;
            switch (dbName)
            {
                case "Inventory":
                    dataSet = InventoryData;
                    break;
                case "Equipment":
                    dataSet = EquipmentData;
                    break;
                case "Users":
                    dataSet = UserData;
                    break;
                case "Staff":
                    dataSet = StaffData;
                    break;
                default:
                    return;
            }
            if (!dataSet.Tables.Contains(tblName)) GetTable(dbName, tblName);

            DataTable = dataSet.Tables[tblName];
            PageIndex = 0;
            ShowTable();
        }

        private void GetTable(string dbName, string tblName)
        {
            DataTable dataTable;
            DataSet dataSet;
            switch (dbName)
            {
                case "Inventory":
                    dataTable = GetInventory.DataTable(tblName);
                    dataSet = InventoryData;
                    break;
                case "Equipment":
                    dataTable = GetEquipment.DataTable(tblName);
                    dataSet = EquipmentData;
                    break;
                case "Users":
                    dataTable = GetUsers.DataTable(tblName);
                    dataSet = UserData;
                    break;
                case "Staff":dataTable = GetStaff.DataTable(tblName);
                    dataSet = StaffData;
                    break;
                default:
                    return;
            }
            FixHeads(dataTable);
            dataTable.TableName = tblName;
            dataSet.Tables.Add(dataTable);
        }

        private void FixHeads(DataTable dataTable)
        {
            foreach (DataColumn col in dataTable.Columns)
            {
                col.ColumnName = FixHeader(col.ColumnName);
            }
        }

        private string FixHeader(string header)
        {
            header = Regex.Replace(header, "[A-Z]", " $0");
            header = header.Replace('_', ' ');
            TextInfo textInfo = new CultureInfo("en-AU", false).TextInfo;
            return textInfo.ToTitleCase(header);
        }

        private void ShowTable()
        {
            if (DataTable != null)
            {
                if (DataTable.Rows.Count == 0)
                {
                    DisplayData.DataContext = DataTable.AsDataView();
                }
                else
                {
                    while (DataTable.Rows.Count <= PageIndex * PageSize && PageIndex > 0) --PageIndex;

                    var display = DataTable.AsEnumerable().Skip(PageIndex * PageSize).Take(PageSize).CopyToDataTable();

                    DisplayData.DataContext = display.AsDataView();
                }
            }
            SetPageString();
        }

        private void PageFwd(object sender, RoutedEventArgs e)
        {
            if (DataTable == null)
                PageIndex = 0;
            else if ((PageIndex + 1) * PageSize <= DataTable.Rows.Count)
                ++PageIndex;
            ShowTable();
        }

        private void PageBack(object sender, RoutedEventArgs e)
        {
            if (PageIndex > 0) --PageIndex;
            ShowTable();
        }

        private void PageLast(object sender, RoutedEventArgs e)
        {
            if (DataTable == null)
                PageIndex = 0;
            else
                PageIndex = DataTable.Rows.Count / PageSize + 1;
            ShowTable();
        }

        private void PageFirst(object sender, RoutedEventArgs e)
        {
            PageIndex = 0;
            ShowTable();
        }

        private void SetPageString()
        {
            int pageTotal;
            if (DataTable == null)
                PageString = "No Data";
            else
            {
                pageTotal = DataTable.Rows.Count / PageSize;
                PageString = $"Page: {PageIndex + 1} of {pageTotal + 1}";
            }
            PageInfo.DataContext = PageString;
        }

        private void SetPageSize(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            PageSize = int.Parse(radioButton.Tag.ToString());
            ShowTable();
        }

    }
}
