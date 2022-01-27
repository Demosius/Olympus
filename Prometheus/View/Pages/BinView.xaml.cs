using Prometheus.ViewModel.Helpers;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Prometheus.View.Pages
{
    /// <summary>
    /// Interaction logic for BinView.xaml
    /// </summary>
    public partial class BinView : BREADBase
    {
        public BinView()
        {
            InitializeComponent();
        }

        public override EDataType DataType => EDataType.NAVBin;

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var headerName = e.Column.Header.ToString();

            //Cancel the column you don't want to generate
            if (new List<string>() { "ID", "ZoneID", "Zone", "NAVStock", "Stock", "Extension", "Dimensions", "Bay", "AccessLevel" }.Contains(headerName))
            {
                e.Cancel = true;
            }

            //update column details when generating
            if (headerName == "LocationCode")
            {
                e.Column.Header = "Location";
            }
            else if (headerName == "ZoneCode")
            {
                e.Column.Header = "Zone";
            }
            else if (headerName == "UsedCube")
            {
                e.Column.Header = "Used Cube";
            }
            else if (headerName == "MaxCube")
            {
                e.Column.Header = "Max Cube";
            }
            else if (headerName == "LastCCDate")
            {
                e.Column.Header = "Last Cycle Count Date";
            }
            else if (headerName == "LastPIDate")
            {
                e.Column.Header = "Last Physical Inventory Date";
            }
        }
    }
}
