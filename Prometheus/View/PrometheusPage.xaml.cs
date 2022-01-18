using System;
using Uranus;
using System.Windows.Controls;
using Uranus.Staff;

namespace Prometheus.View
{
    /// <summary>
    /// Interaction logic for Prometheus.xaml
    /// </summary>
    public partial class PrometheusPage : Page, IProject
    {
        //public DataSet UserData { get; set; }
        //public DataSet StaffData { get; set; }
        //public DataSet EquipmentData { get; set; }
        //public DataSet InventoryData { get; set; }
        //public DataTable DisplayTable { get; set; }

        //public int PageIndex { get; set; } = 0;
        //public int PageSize { get; set; } = 25;

        //public string PageString { get; set; } = "Pag6e: 0 of 0";

        public PrometheusPage()
        {
            InitializeComponent();
            //this.DataContext = this;
            //SetData();
            //SetPageString();
        }

        public EProject EProject => EProject.Prometheus;

        public void RefreshData()
        {
            throw new NotImplementedException();
        }

        //public void SetData()
        //{
        //    UserData = new DataSet();
        //    InventoryData = new DataSet();
        //    StaffData = new DataSet();
        //    EquipmentData = new DataSet();
        //}

        //private void SetTable(object sender, RoutedEventArgs e)
        //{
        //    TreeViewItem item = (TreeViewItem)sender;
        //    string[] tags = item.Tag.ToString().Split(',');
        //    string dbName = tags[0];
        //    string tblName = tags[1];
        //    DataSet dataSet;
        //    switch (dbName)
        //    {
        //        case "Inventory":
        //            dataSet = InventoryData;
        //            break;
        //        case "Equipment":
        //            dataSet = EquipmentData;
        //            break;
        //        case "Users":
        //            dataSet = UserData;
        //            break;
        //        case "Staff":
        //            dataSet = StaffData;
        //            break;
        //        default:
        //            return;
        //    }
        //    if (!dataSet.Tables.Contains(tblName)) GetTable(dbName, tblName);

        //    DisplayTable = dataSet.Tables[tblName];
        //    PageIndex = 0;
        //    ShowTable();
        //}

        //private void GetTable(string dbName, string tblName)
        //{
        //    DataTable dataTable;
        //    DataSet dataSet;
        //    switch (dbName)
        //    {
        //        case "Inventory":
        //            dataTable = GetInventory.DataTable(tblName);
        //            dataSet = InventoryData;
        //            break;
        //        case "Equipment":
        //            dataTable = GetEquipment.DataTable(tblName);
        //            dataSet = EquipmentData;
        //            break;
        //        case "Users":
        //            dataTable = GetUsers.DataTable(tblName);
        //            dataSet = UserData;
        //            break;
        //        case "Staff":dataTable = GetStaff.DataTable(tblName);
        //            dataSet = StaffData;
        //            break;
        //        default:
        //            return;
        //    }
        //    FixHeads(dataTable);
        //    dataTable.TableName = tblName;
        //    dataSet.Tables.Add(dataTable);
        //}

        //private void FixHeads(DataTable dataTable)
        //{
        //    foreach (DataColumn col in dataTable.Columns)
        //    {
        //        col.ColumnName = FixHeader(col.ColumnName);
        //    }
        //}

        //private string FixHeader(string header)
        //{
        //    header = Regex.Replace(header, "[A-Z]", " $0");
        //    header = header.Replace('_', ' ');
        //    TextInfo textInfo = new CultureInfo("en-AU", false).TextInfo;
        //    return textInfo.ToTitleCase(header);
        //}

        //private void ShowTable()
        //{
        //    if (DisplayTable != null)
        //    {
        //        if (DisplayTable.Rows.Count == 0)
        //        {
        //            DisplayData.DataContext = DisplayTable.AsDataView();
        //        }
        //        else
        //        {
        //            while (DisplayTable.Rows.Count <= PageIndex * PageSize && PageIndex > 0) --PageIndex;

        //            var display = DisplayTable.AsEnumerable().Skip(PageIndex * PageSize).Take(PageSize).CopyToDataTable();

        //            DisplayData.DataContext = display.AsDataView();
        //        }
        //    }
        //    SetPageString();
        //}

        //private void PageFwd(object sender, RoutedEventArgs e)
        //{
        //    if (DisplayTable == null)
        //        PageIndex = 0;
        //    else if ((PageIndex + 1) * PageSize <= DisplayTable.Rows.Count)
        //        ++PageIndex;
        //    ShowTable();
        //}

        //private void PageBack(object sender, RoutedEventArgs e)
        //{
        //    if (PageIndex > 0) --PageIndex;
        //    ShowTable();
        //}

        //private void PageLast(object sender, RoutedEventArgs e)
        //{
        //    if (DisplayTable == null)
        //        PageIndex = 0;
        //    else
        //        PageIndex = DisplayTable.Rows.Count / PageSize + 1;
        //    ShowTable();
        //}

        //private void PageFirst(object sender, RoutedEventArgs e)
        //{
        //    PageIndex = 0;
        //    ShowTable();
        //}

        //private void SetPageString()
        //{
        //    int pageTotal;
        //    if (DisplayTable == null)
        //        PageString = "No Data";
        //    else
        //    {
        //        pageTotal = DisplayTable.Rows.Count / PageSize;
        //        PageString = $"Page: {PageIndex + 1} of {pageTotal + 1}";
        //    }
        //    PageInfo.DataContext = PageString;
        //}

        //private void SetPageSize(object sender, RoutedEventArgs e)
        //{
        //    RadioButton radioButton = (RadioButton)sender;
        //    PageSize = int.Parse(radioButton.Tag.ToString());
        //    ShowTable();
        //}

    }
}
