using Hydra.ViewModels.Commands;
using Serilog;
using Styx;
using Styx.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Morpheus.Views.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class SiteManagerVM : INotifyPropertyChanged, IDBInteraction, IDataSource
{
    public HydraVM HydraVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    public SiteVM NoSite { get; set; }

    public List<NAVZone> AllZones { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<SiteVM> Sites { get; set; }

    private ZoneListingVM noSiteZoneListing;
    public ZoneListingVM NoSiteZoneListing
    {
        get => noSiteZoneListing;
        set
        {
            noSiteZoneListing = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public AddNewSiteCommand AddNewSiteCommand { get; set; }
    public DeleteSiteCommand DeleteSiteCommand { get; set; }
    public SaveSitesCommand SaveSitesCommand { get; set; }

    #endregion

    public SiteManagerVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        Sites = new ObservableCollection<SiteVM>();
        AllZones = new List<NAVZone>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        AddNewSiteCommand = new AddNewSiteCommand(this);
        DeleteSiteCommand = new DeleteSiteCommand(this);
        SaveSitesCommand = new SaveSitesCommand(this);

        NoSite = new SiteVM(new Site("NoSite"), DeleteSiteCommand);
        noSiteZoneListing = new ZoneListingVM(new List<NAVZone>(), NoSite);

        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void RefreshData()
    {
        if (Helios is null) return;

        Sites.Clear();
        AllZones.Clear();

        var sites = Helios.InventoryReader.Sites(out var zones);

        foreach (var site in sites)
        {
            Sites.Add(new SiteVM(site, DeleteSiteCommand));
        }

        foreach (var zone in zones) AllZones.Add(zone);

        var noZones = zones.Where(z => z.SiteName == "").ToList();
        var noSite = new Site("NoSite")
        {
            Zones = noZones
        };

        NoSite = new SiteVM(noSite, DeleteSiteCommand);

        NoSiteZoneListing = NoSite.ZoneListingVM;
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        RefreshData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void AddNewSite()
    {
        if (Helios is null) throw new DataException("Helios is null within active site manager.");

        var inputBox = new InputWindow("Enter new Site name: ", "Site Name");

        if (inputBox.ShowDialog() != true) return;

        var siteName = inputBox.InputText;

        if (siteName.Length == 0) return;
        if (siteName == "NoSite" || Sites.Select(s => s.Site.Name).Contains(siteName))
        {
            MessageBox.Show("Site with this name already exists.", "No New Site Created", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }


        var site = new SiteVM(new Site(siteName), DeleteSiteCommand);
        Helios.InventoryCreator.Site(site.Site);

        Sites.Add(site);
    }

    public void DeleteSite(SiteVM siteVM)
    {
        if (Helios is null || !Sites.Contains(siteVM)) return;

        if (MessageBox.Show($"Are you sure you want to delete site: {siteVM.Site.Name}?", "Confirm Site Deletion",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        var zones = new List<ZoneVM>(siteVM.ZoneListingVM.ZoneVMs);
        siteVM.ZoneListingVM.ZoneVMs.Clear();

        foreach (var zoneVM in zones)
        {
            NoSite.ZoneListingVM.AddZone(zoneVM);
        }

        Helios.InventoryDeleter.Site(siteVM.Site);

        Sites.Remove(siteVM);

        SaveZones();
    }

    public void SaveZones()
    {
        try
        {
            Helios?.InventoryUpdater.Zones(AllZones);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save zones from site manager.");
            throw;
        }
    }

    public void SaveSites()
    {
        try
        {
            Helios?.InventoryUpdater.Sites(Sites.Select(vm => vm.Site), AllZones);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save zones from site manager.");
            throw;
        }

        MessageBox.Show("Saved Changes", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}