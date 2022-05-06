using Olympus.Properties;
using Olympus.ViewModel.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Inventory;
using Uranus.Inventory.Model;

namespace Olympus.ViewModel.Components;

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
        }
    }

    #endregion

    public static string Info => "Click on the buttons to update the designated data.\n\n" +
                                 "Stock (Bin Contents), Bins (Bin List), and UoM (Item Units of Measure) require data copied from NAV.\n\n" +
                                 "Item List takes data from an external workbook (Price book Report) and requires nothing other than pressing the button.\n\n" +
                                 "Hover over the buttons to be shown the required columns (and where to get the data) for the specific data type.";

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
    public InventoryUpdaterVM()
    {
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

    public InventoryUpdaterVM(OlympusVM olympusVM) : this()
    {
        ParentVM = olympusVM;
    }

    // Methods
    public void GetUpdateTimes()
    {
        StockUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVStock));
        BinsUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVBin));
        UoMUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVUoM));
        ItemUpdateTime = App.Helios.InventoryReader.LastTableUpdate(typeof(NAVItem));
    }

    public void UpdateStock()
    {
        _ = Task.Run(() =>
        {
            if (App.Helios.InventoryUpdater.NAVStock(DataConversion.NAVRawStringToStock(General.ClipboardToString())))
                GetUpdateTimes();
        });
    }

    public void UpdateBins()
    {
        _ = Task.Run(() =>
        {
            if (App.Helios.InventoryUpdater.NAVBins(DataConversion.NAVRawStringToBins(General.ClipboardToString())))
                GetUpdateTimes();
        });
    }

    public void UpdateUoM()
    {
        _ = Task.Run(() =>
        {
            if (App.Helios.InventoryUpdater.NAVUoMs(DataConversion.NAVRawStringToUoMs(General.ClipboardToString())))
                GetUpdateTimes();
        });
    }

    public void UpdateItems()
    {
        _ = Task.Run(() =>
        {
            if (App.Helios.InventoryCreator.NAVItems(DataConversion.NAV_CSVToItems(Settings.Default.ItemCSVLocation),
                    InventoryReader.LastItemWriteTime(Settings.Default.ItemCSVLocation)))
                GetUpdateTimes();
        });
    }

    public static void ShowInfo()
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

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}