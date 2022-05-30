using Hydra.ViewModels.Commands;
using Morpheus.Views;
using Styx;
using Styx.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
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

    #region INotifyPropertyChanged Members

    public ObservableCollection<SiteVM> Sites { get; set; }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }
    public AddNewSiteCommand AddNewSiteCommand { get; set; }

    #endregion

    public SiteManagerVM(HydraVM hydraVM)
    {
        HydraVM = hydraVM;
        Sites = new ObservableCollection<SiteVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);
        AddNewSiteCommand = new AddNewSiteCommand(this);
        Task.Run(() => SetDataSources(HydraVM.Helios!, HydraVM.Charon!));
    }

    public void RefreshData()
    {
        // TODO: Implement.
    }

    public void RepairData()
    {
        throw new System.NotImplementedException();
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
        var inputBox = new InputWindow("Enter new Site name: ", "Site Name");

        if (inputBox.ShowDialog() != true) return;

        var siteName = inputBox.InputText;

        if (siteName.Length == 0) return;
        if (Sites.Select(s => s.Site.Name).Contains(siteName))
        {
            MessageBox.Show("Site with this name already exists.", "No New Site Created", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var site = new SiteVM(new Site(siteName));

        Sites.Add(site);
    }
}