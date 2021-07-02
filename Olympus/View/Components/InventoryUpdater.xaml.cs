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
            lblStockDT.DataContext = GetInventory.LastStockUpdateTime().ToString("dd/MM/yyyy HH:mm");
            lblBinDT.DataContext = GetInventory.LastBinUpdateTime().ToString("dd/MM/yyyy HH:mm");
            lblUoMDT.DataContext = GetInventory.LastUoMUpdateTime().ToString("dd/MM/yyyy HH:mm");
            lblItemDT.DataContext = GetInventory.LastItemUpdateTime().ToString("dd/MM/yyyy HH:mm");
        }

        private void UpdateItems(object sender, RoutedEventArgs e)
        {
            PutInventory.ItemsFromCSV();
            SetDates();
        }

        private void UpdateBins(object sender, RoutedEventArgs e)
        {
            PutInventory.BinsFromClipboard();
            SetDates();
        }

        private void UpdateStock(object sender, RoutedEventArgs e)
        {
            PutInventory.StockFromClipboard();
            SetDates();
        }

        private void UpdateUoM(object sender, RoutedEventArgs e)
        {
            PutInventory.UoMFromClipboard();
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
                            $"{String.Join("\n", GetInventory.StockColumnDict().Values)}\n\n" +
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
                            $"{String.Join("\n", GetInventory.BinColumnDict().Values)}\n\n" +
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
                            $"{String.Join("\n", GetInventory.UoMColumnDict().Values)}\n\n" +
                            $"(Unfilter All, then filter Code to \"<>EACH\".)\n" +
                            $"(Update Daily. End of previous work day, or begining of current.)",
                            $"UoM Data Requirements",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}
