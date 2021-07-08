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
using Olympus;
using Olympus.Helios;
using System.Globalization;
using Olympus.Helios.Inventory.Model;

namespace Olympus.View.Components
{
    /// <summary>
    /// Interaction logic for InvUpdate.xaml
    /// </summary>
    public partial class InventoryUpdater : UserControl
    {
        public InventoryUpdater()
        {
            InitializeComponent();
            SetDates();
        }

        private void SetDates()
        {
            lblStockDT.DataContext = App.Charioteer.InventoryReader.LastTableUpdate(typeof(NAVStock)).ToString("dd/MM/yyyy HH:mm");
            lblBinDT.DataContext = App.Charioteer.InventoryReader.LastTableUpdate(typeof(NAVBin)).ToString("dd/MM/yyyy HH:mm");
            lblUoMDT.DataContext = App.Charioteer.InventoryReader.LastTableUpdate(typeof(NAVUoM)).ToString("dd/MM/yyyy HH:mm");
            lblItemDT.DataContext = App.Charioteer.InventoryReader.LastTableUpdate(typeof(NAVItem)).ToString("dd/MM/yyyy HH:mm");
        }

        private void UpdateItems(object sender, RoutedEventArgs e)
        {
            App.Charioteer.InventoryUpdater.NAVItems(DataConversion.NAVCSVToItems(), DateTime.Now);
            SetDates();
        }

        private void UpdateBins(object sender, RoutedEventArgs e)
        {
            App.Charioteer.InventoryUpdater.NAVBins(DataConversion.NAVClipToBins());
            SetDates();
        }

        private void UpdateStock(object sender, RoutedEventArgs e)
        {
            App.Charioteer.InventoryUpdater.NAVStock(DataConversion.NAVClipToStock());
            SetDates();
        }

        private void UpdateUoM(object sender, RoutedEventArgs e)
        {
            App.Charioteer.InventoryUpdater.NAVUoMs(DataConversion.NAVClipToUoMs());
            SetDates();
        }

        private void ShowInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Click on the large buttons to update the designated data.\n\n" +
                            $"Stock (Bin Contents), Bins (Bin List), and UoM (Item Units of Measure) require data coppied from NAV.\n\n" +
                            $"Items takes data from an external workbool (Pricebook Report) and requires nothing other than pressing the button.\n\n" +
                            $"Click on the small [xx Col] buttons to be shown the required columns (and where to get the data) for the specific data type.\n\n", 
                            $"Data Upload Help",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void BCInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Find the Data:\n" +
                            $"[NAV > Warehouse > Planning & Execution > Bin Contents]\n\n" +
                            $"Required Columns:\n\n" +
                            $"{String.Join("\n", Helios.Inventory.Constants.NAV_STOCK_COLUMNS.Keys)}\n\n" +
                            $"(Filter to Zone Code as required, and Location Code = '9600')\n" +
                            $"(Update to the minute, as required.)",
                            $"Stock/Bin Contents Requirements",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void BLInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Find the Data:\n" +
                            $"[NAV > Warehouse > Planning & Execution > Bin Contents >> Bin Code]\n\n" +
                            $"Required Columns:\n\n" +
                            $"{String.Join("\n", Helios.Inventory.Constants.NAV_BIN_COLUMNS.Keys)}\n\n" +
                            $"(No filtering required.)\n" +
                            $"(Update when changes are made to bin/zone layouts, or when Count Dates are required.)",
                            $"Bin List Requirements",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }

        private void UoMInfo(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Find the Data:\n" +
                            $"[NAV > Warehouse > Planning & Execution > Bin Contents >> Unit of Measure Code]\n\n" +
                            $"Required Columns:\n\n" +
                            $"{String.Join("\n", Helios.Inventory.Constants.NAV_UOM_COLUMNS.Keys)}\n\n" +
                            $"(Unfilter All, then filter Code to \"<>EACH\".)\n" +
                            $"(Update Daily. End of previous work day, or begining of current.)",
                            $"UoM Data Requirements",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}
