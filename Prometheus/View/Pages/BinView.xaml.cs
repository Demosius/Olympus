using Prometheus.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            string headerName = e.Column.Header.ToString();

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
