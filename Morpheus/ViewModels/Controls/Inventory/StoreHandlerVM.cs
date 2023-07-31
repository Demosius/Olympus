using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Morpheus.Properties;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Morpheus.Views.Windows;
using Serilog;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;
using InvalidDataException = Uranus.InvalidDataException;

namespace Morpheus.ViewModels.Controls.Inventory;

public class StoreHandlerVM : INotifyPropertyChanged, IDBInteraction, IFilters, ICreateDelete<StoreVM>, IClipboardUpload, IFileUpload
{
    public Helios Helios { get; set; }

    public List<StoreVM> AllStores { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<StoreVM> Stores { get; set; }

    private StoreVM? selectedItem;
    public StoreVM? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            OnPropertyChanged();
        }
    }

    public string DataFile
    {
        get => Settings.Default.StoreFile;
        set
        {
            Settings.Default.StoreFile = value;
            OnPropertyChanged();
            Settings.Default.Save();
        }
    }


    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public CreateNewItemCommand CreateNewItemCommand { get; set; }
    public UploadDataCommand UploadDataCommand { get; set; }
    public DataFromFileCommand DataFromFileCommand { get; set; }
    public DeleteSelectedItemCommand<StoreVM> DeleteSelectedItemCommand { get; set; }
    public SetDataFileCommand SetDataFileCommand { get; set; }

    #endregion

    private StoreHandlerVM(Helios helios)
    {
        Helios = helios;

        AllStores = new List<StoreVM>();
        Stores = new ObservableCollection<StoreVM>();

        filterString = string.Empty;

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        CreateNewItemCommand = new CreateNewItemCommand(this);
        UploadDataCommand = new UploadDataCommand(this);
        DataFromFileCommand = new DataFromFileCommand(this);
        DeleteSelectedItemCommand = new DeleteSelectedItemCommand<StoreVM>(this);
        SetDataFileCommand = new SetDataFileCommand(this);
    }

    private async Task<StoreHandlerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<StoreHandlerVM> CreateAsync(Helios helios)
    {
        var ret = new StoreHandlerVM(helios);
        return ret.InitializeAsync();
    }

    public static StoreHandlerVM CreateEmpty(Helios helios) => new(helios);

    public async Task RefreshDataAsync()
    {
        AllStores = (await Helios.InventoryReader.StoresAsync()).Select(s => new StoreVM(s, Helios)).ToList();
        ApplyFilters();
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        OnPropertyChanged(nameof(FilterString));
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var stores = AllStores.Where(s =>
            FilterString == string.Empty ||
            Regex.IsMatch(s.CCNRegion, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.Number, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.Region, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.RoadCCN, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.RoadRegion, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.State, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.Wave, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.TransitDays.ToString(), FilterString,
                RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.SortingLane.ToString(), FilterString,
                RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.Type.ToString(), FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.Volume.ToString(), FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(s.MBRegion, FilterString, RegexOptions.IgnoreCase));

        Stores.Clear();
        foreach (var store in stores) Stores.Add(store);
    }

    public async Task DataFromFileAsync()
    {
        try
        {
            await Helios.InventoryCreator.StoresAsync(await DataConversion.FileToStoresAsync(DataFile));
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Failed to Pull Stores", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when pulling store data from file.");
            throw;
        }

        await RefreshDataAsync();
    }

    public void SetDataFile()
    {
        var dir = Path.GetDirectoryName(DataFile);
        var fileName = Path.GetFileName(DataFile);

        var fd = new OpenFileDialog
        {
            Multiselect = false,
            InitialDirectory = dir,
            FileName = fileName
        };

        if (fd.ShowDialog() != true) return;

        DataFile = fd.FileName;
    }

    public async Task DeleteSelectedItemAsync()
    {
        if (SelectedItem is null) return;
        var store = SelectedItem.Store;

        AllStores.Remove(SelectedItem);
        Stores.Remove(SelectedItem);
        await Helios.InventoryDeleter.StoreAsync(store);
        SelectedItem = null;
    }

    public async Task CreateNewItemAsync()
    {
        var inputWindow = new InputWindow("Enter Store Number", "New Store");
        if (inputWindow.ShowDialog() != true) return;
        var newStoreNumber = inputWindow.InputText;
        if (AllStores.Select(s => s.Number).ToList().Contains(newStoreNumber))
        {
            MessageBox.Show($"A store with the number {newStoreNumber} already exists.", "New Store Failed",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var newStore = new Store { Number = newStoreNumber };
        var newVM = new StoreVM(newStore, Helios);
        AllStores.Add(newVM);
        Stores.Add(newVM);
        SelectedItem = newVM;
        await Helios.InventoryCreator.StoreAsync(newStore);
    }

    public async Task UploadDataAsync()
    {
        try
        {
            await Helios.InventoryCreator.StoresAsync(DataConversion.RawStringToStores(General.ClipboardToString()));
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Failed to Pull Stores", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error when pulling store data from file.");
            throw;
        }

        await RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}