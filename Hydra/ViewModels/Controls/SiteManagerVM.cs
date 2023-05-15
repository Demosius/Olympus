using Hydra.ViewModels.Commands;
using Serilog;
using Styx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

public class SiteManagerVM : INotifyPropertyChanged, IDBInteraction
{
    public HydraVM HydraVM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

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
    public AddNewSiteCommand AddNewSiteCommand { get; set; }
    public DeleteSiteCommand DeleteSiteCommand { get; set; }
    public SaveSitesCommand SaveSitesCommand { get; set; }

    #endregion

    private SiteManagerVM(HydraVM hydraVM, Helios helios, Charon charon)
    {
        HydraVM = hydraVM;
        Helios = helios;
        Charon = charon;
        Sites = new ObservableCollection<SiteVM>();
        AllZones = new List<NAVZone>();

        RefreshDataCommand = new RefreshDataCommand(this);
        AddNewSiteCommand = new AddNewSiteCommand(this);
        DeleteSiteCommand = new DeleteSiteCommand(this);
        SaveSitesCommand = new SaveSitesCommand(this);

        NoSite = new SiteVM(new Site("NoSite"), DeleteSiteCommand);
        noSiteZoneListing = new ZoneListingVM(new List<NAVZone>(), NoSite);
    }

    public SiteManagerVM(HydraVM hydraVM, Helios helios, Charon charon, List<NAVZone> zones, List<Site> siteList) : this(hydraVM, helios, charon)
    {
        foreach (var site in siteList)
            Sites.Add(new SiteVM(site, DeleteSiteCommand));

        foreach (var zone in zones) AllZones.Add(zone);

        var noZones = zones.Where(z => z.SiteName == "").ToList();
        var noSite = new Site("NoSite") { Zones = noZones };

        NoSite = new SiteVM(noSite, DeleteSiteCommand);

        NoSiteZoneListing = NoSite.ZoneListingVM;
    }

    private async Task<SiteManagerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<SiteManagerVM> CreateAsync(HydraVM hydraVM, Helios helios, Charon charon)
    {
        var ret = new SiteManagerVM(hydraVM, helios, charon);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        Sites.Clear();
        AllZones.Clear();

        var (sites, navZones) = await Helios.InventoryReader.SitesAsync();
        var zones = navZones.ToList();

        foreach (var site in sites)
            Sites.Add(new SiteVM(site, DeleteSiteCommand));

        foreach (var zone in zones) AllZones.Add(zone);

        var noZones = zones.Where(z => z.SiteName == "").ToList();
        var noSite = new Site("NoSite") { Zones = noZones };

        NoSite = new SiteVM(noSite, DeleteSiteCommand);

        NoSiteZoneListing = NoSite.ZoneListingVM;
    }
    

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void AddNewSite()
    {
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

        _ = Helios.InventoryCreator.SiteAsync(site.Site);

        Sites.Add(site);
    }

    public async Task DeleteSite(SiteVM siteVM)
    {
        if (!Sites.Contains(siteVM)) return;

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

        await SaveZones();
    }

    public async Task SaveZones()
    {
        try
        {
            await Helios.InventoryUpdater.ZonesAsync(AllZones);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save zones from site manager.");
            throw;
        }
    }

    public async Task SaveSites()
    {
        try
        {
            await Helios.InventoryUpdater.SitesAsync(Sites.Select(vm => vm.Site), AllZones);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save zones from site manager.");
            throw;
        }

        MessageBox.Show("Saved Changes", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}