using Hydra.ViewModels.Controls;
using Styx;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Hydra.ViewModels;

public class HydraVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public RunVM RunVM { get; set; }
    public SiteManagerVM SiteManagerVM { get; set; }
    public ZoneHandlerVM ZoneHandlerVM { get; set; }
    public ItemLevelsVM ItemLevelsVM { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public HydraVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        RunVM = new RunVM(this, Helios, Charon);
        SiteManagerVM = new SiteManagerVM(this, Helios, Charon);
        ZoneHandlerVM = new ZoneHandlerVM(this, Helios, Charon);

        ItemLevelsVM = new ItemLevelsVM(this);

        RefreshDataCommand = new RefreshDataCommand(this);
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