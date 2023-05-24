using Cadmus.Annotations;
using Morpheus;
using Olympus.Properties;
using Olympus.ViewModels.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Olympus.ViewModels.Windows;
using Olympus.Views.Windows;
using Serilog;
using Uranus;
using Uranus.Inventory;
using Uranus.Inventory.Models;
using Mouse = System.Windows.Input.Mouse;

namespace Olympus.ViewModels.Components;

public class InventoryUpdaterVM : INotifyPropertyChanged
{
    public OlympusVM ParentVM { get; set; }

    // Last update times.
    private DateTime stockUpdateTime;
    public string StockUpdateString => stockUpdateTime.ToString("dd/MM/yyyy HH:mm");

    #region INotifyPropertyChanged Members

    public DateTime StockUpdateTime
    {
        get => stockUpdateTime;
        set
        {
            stockUpdateTime = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StockUpdateString));
        }
    }
    private DateTime binsUpdateTime;
    public string BinsUpdateString => binsUpdateTime.ToString("dd/MM/yyyy HH:mm");

    public DateTime BinsUpdateTime
    {
        get => binsUpdateTime;
        set
        {
            binsUpdateTime = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BinsUpdateString));
        }
    }
    private DateTime uomUpdateTime;
    public string UoMUpdateString => uomUpdateTime.ToString("dd/MM/yyyy HH:mm");

    public DateTime UoMUpdateTime
    {
        get => uomUpdateTime;
        set
        {
            uomUpdateTime = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UoMUpdateString));
        }
    }
    private DateTime itemUpdateTime;
    public string ItemUpdateString => itemUpdateTime.ToString("dd/MM/yyyy HH:mm");

    public DateTime ItemUpdateTime
    {
        get => itemUpdateTime;
        set
        {
            itemUpdateTime = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ItemUpdateString));
        }
    }

    #endregion

    public string Info => "Click on the buttons to update the designated data.\n\n" +
                                 "Stock (Bin Contents), Bins (Bin List), and UoM (Item Units of Measure) require data copied from NAV.\n\n" +
                                 "Item List takes data from an external workbook (Price book Report) and requires nothing other than pressing the button.\n\n" +
                                 "Hover over the buttons to be shown the required columns (and where to get the data) for the specific data type.\n\n" +
                                 $"(Data taken from: [ {Settings.Default.ItemCSVLocation} ])";

    public static string UoMInfo => "Find the Data:\n" +
                                    "[NAV > Warehouse > Planning & Execution > Bin Contents >> Unit of Measure Code]\n\n" +
                                    "Required Columns:\n\n" +
                                    $"{string.Join("\n", Constants.NAV_UoMColumns.Keys)}\n\n" +
                                    "(Un-filter All, then filter Code to \"<>EACH\".)\n" +
                                    "(Update Daily. End of previous work day, or beginning of current.)";

    public static string BCInfo => "Find the Data:\n" +
                                   "[NAV > Warehouse > Planning & Execution > Bin Contents]\n\n" +
                                   "Required Columns:\n\n" +
                                   $"{string.Join("\n", Constants.NAVStockColumns.Keys)}\n\n" +
                                   "(Filter to Zone Code as required, and Location Code = '9600')\n" +
                                   "(Update to the minute, as required.)";

    public static string BLInfo => "Find the Data:\n" +
                                   "[NAV > Warehouse > Planning & Execution > Bin Contents >> Bin Code]\n\n" +
                                   "Required Columns:\n\n" +
                                   $"{string.Join("\n", Constants.NAVBinColumns.Keys)}\n\n" +
                                   "(Make sure data is not filtered.)\n" +
                                   "(Update when changes are made to bin/zone layouts, or when Count Dates are required.)";
    // Commands
    public UpdateStockCommand UpdateStockCommand { get; set; }
    public UpdateBinsCommand UpdateBinsCommand { get; set; }
    public UpdateUoMCommand UpdateUoMCommand { get; set; }
    public UpdateItemsCommand UpdateItemsCommand { get; set; }
    public ShowBcColCommand ShowBcColCommand { get; set; }
    public ShowBinListColumnCommand ShowBinListColumnCommand { get; set; }
    public ShowUlColCommand ShowUlColCommand { get; set; }
    public ShowInfoCommand ShowInfoCommand { get; set; }

    // Constructors
    public InventoryUpdaterVM(OlympusVM olympusVM)
    {
        ParentVM = olympusVM;

        GetUpdateTimes();

        UpdateStockCommand = new UpdateStockCommand(this);
        UpdateBinsCommand = new UpdateBinsCommand(this);
        UpdateUoMCommand = new UpdateUoMCommand(this);
        UpdateItemsCommand = new UpdateItemsCommand(this);
        ShowBcColCommand = new ShowBcColCommand(this);
        ShowBinListColumnCommand = new ShowBinListColumnCommand(this);
        ShowUlColCommand = new ShowUlColCommand(this);
        ShowInfoCommand = new ShowInfoCommand(this);
    }

    // Methods
    public void GetUpdateTimes()
    {
        StockUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVStock));
        BinsUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVBin));
        UoMUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVUoM));
        ItemUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVItem));
    }

    public async Task UpdateStock()
    {
        BinContentsUpdaterWindow? window = null;
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var newStock = DataConversion.NAVRawStringToStock(General.ClipboardToString());

            var vm = await BinContentsUpdaterVM.CreateAsync(App.Helios, newStock);

            // Check that there are any zones to consider.
            int lines;
            if (vm.ZonesMissing)
            {
                window = new BinContentsUpdaterWindow(vm);

                Mouse.OverrideCursor = Cursors.Arrow;

                if (window.ShowDialog() != true) return;
                lines = window.UploadedLines;
            }
            else
            {
                lines = await App.Helios.InventoryUpdater.NAVStockAsync(newStock);
                Mouse.OverrideCursor = Cursors.Arrow;
            }

            if (lines > 0)
            {
                GetUpdateTimes();
                MessageBox.Show("Update successful.", "Success", MessageBoxButton.OK);
            }
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "An unexpected error has occurred:\n\n" +
                $"\t{ex.Message}\n\n" +
                "Please notify Olympus Development when possible.",
                "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Log.Error(ex, "Occurred when uploading Bin Contents data.");
        }
        window?.Close();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public async Task UpdateBins()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            if (await App.Helios.InventoryUpdater.NAVBinsAsync(
                    DataConversion.NAVRawStringToBins(General.ClipboardToString())) > 0)
            {
                GetUpdateTimes();
                MessageBox.Show("Update successful.", "Success", MessageBoxButton.OK);
            }
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "An unexpected error has occurred:\n\n" +
                $"\t{ex.Message}\n\n" +
                "Please notify Olympus Development when possible.",
                "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public async Task UpdateUoM()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            if (await App.Helios.InventoryUpdater.NAVUoMsAsync(
                    DataConversion.NAVRawStringToUoMs(General.ClipboardToString())) > 0)
            {
                GetUpdateTimes();
                MessageBox.Show("Update successful.", "Success", MessageBoxButton.OK);
            }
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "An unexpected error has occurred:\n\n" +
                $"\t{ex.Message}\n\n" +
                "Please notify Olympus Development when possible.",
                "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public async Task UpdateItems()
    {
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            if (await App.Helios.InventoryCreator.NAVItemsAsync(DataConversion.NAV_CSVToItems(Settings.Default.ItemCSVLocation),
                    InventoryReader.LastItemWriteTime(Settings.Default.ItemCSVLocation)) > 0)
            {
                GetUpdateTimes();
                MessageBox.Show("Update successful.", "Success", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Did not update Item Data.\n\nLikely already up to date.", "Failed", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "An unexpected error has occurred:\n\n" +
                $"\t{ex.Message}\n\n" +
                "Please notify Olympus Development when possible.",
                "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void ShowInfo()
    {
        _ = MessageBox.Show(Info,
            "Data Upload Help",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public static void ShowBCInfo()
    {
        _ = MessageBox.Show(BCInfo,
            "Stock/Bin Contents Requirements",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public static void ShowBLInfo()
    {
        _ = MessageBox.Show(BLInfo,
            "Bin List Requirements",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public static void ShowUoMInfo()
    {
        _ = MessageBox.Show(UoMInfo,
            "UoM Data Requirements",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}