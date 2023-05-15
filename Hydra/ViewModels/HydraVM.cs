using Hydra.ViewModels.Controls;
using Styx;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels;

public class HydraVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public RunVM RunVM { get; set; } = null!;
    public SiteManagerVM SiteManagerVM { get; set; } = null!;
    public ZoneHandlerVM ZoneHandlerVM { get; set; } = null!;
    public ItemLevelsVM ItemLevelsVM { get; set; } = null!;

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    private HydraVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task<HydraVM> InitializeAsync()
    {
        var dataSet = await Helios.InventoryReader.HydraDataSetAsync();
        var siteList = new List<Site>();
        var zoneList = new List<NAVZone>();

        if (dataSet is null)
        {
            dataSet = new HydraDataSet();
            var (sites, zones) = await Helios.InventoryReader.SitesAsync();
            siteList.AddRange(sites);
            zoneList.AddRange(zones);
        }
        else
        {
            siteList = dataSet.Sites.Values.ToList();
            zoneList = dataSet.Zones.Values.ToList();
        }

        RunVM = new RunVM(this, Helios, Charon, siteList);
        SiteManagerVM = new SiteManagerVM(this, Helios, Charon, zoneList, siteList);
        ZoneHandlerVM = new ZoneHandlerVM(this, Helios, Charon, zoneList);
        ItemLevelsVM = new ItemLevelsVM(this, dataSet);

        return this;
    }

    public static Task<HydraVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new HydraVM(helios, charon);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        var tasks = new List<Task>
        {
            RunVM.RefreshDataAsync(),
            SiteManagerVM.RefreshDataAsync(),
            ZoneHandlerVM.RefreshDataAsync()
        };

        await Task.WhenAll(tasks);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}