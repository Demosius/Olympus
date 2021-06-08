using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Olympus.Helios;
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

        public Prometheus()
        {
            InitializeComponent();
        }

        public void SetData()
        {
            UserData = GetUsers.DataSet();
            InventoryData = GetInventory.DataSet();
            StaffData = GetStaff.DataSet();
            EquipmentData = GetEquipment.DataSet();
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
                    if (InventoryData == null) InventoryData = GetInventory.DataSet();
                    dataSet = InventoryData;
                    break;
                case "Equipment":
                    if (EquipmentData == null) EquipmentData = GetEquipment.DataSet();
                    dataSet = EquipmentData;
                    break;
                case "Users":
                    if (UserData == null) UserData = GetUsers.DataSet();
                    dataSet = UserData;
                    break;
                case "Staff":
                    if (StaffData == null) StaffData = GetStaff.DataSet();
                    dataSet = StaffData;
                    break;
                default:
                    return;
            }

            DataTable = dataSet.Tables[tblName];
            PageIndex = 0;
            ShowTable();
        }

        public void ShowTable()
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

        public void PageFwd()
        {
            ++PageIndex;
            ShowTable();
        }

        public void PageBack()
        {
            if (PageIndex > 0) --PageIndex;
            ShowTable();
        }

    }
}
