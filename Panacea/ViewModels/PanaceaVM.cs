using Panacea.ViewModels.Components;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Panacea.ViewModels;

public class PanaceaVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    public FixedBinCheckerVM FixedBinCheckerVM { get; set; }
    public ItemsWithMultipleBinsVM ItemsWithMultipleBinsVM { get; set; }
    public BinsWithMultipleItemsVM BinsWithMultipleItemsVM { get; set; }
    public PurgeVM PurgeVM { get; set; }
    public NegativeCheckerVM NegativeCheckerVM { get; set; }
    public PotentialNegativeVM PotentialNegativeVM { get; set; }
    public ItemsWithNoPickBinVM ItemsWithNoPickBinVM { get; set; }

    #region INotifyProprtyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public PanaceaVM(Helios helios)
    {
        Helios = helios;

        FixedBinCheckerVM = new FixedBinCheckerVM(helios);
        ItemsWithMultipleBinsVM = new ItemsWithMultipleBinsVM(helios);
        BinsWithMultipleItemsVM = new BinsWithMultipleItemsVM(helios);
        PurgeVM = new PurgeVM(helios);
        NegativeCheckerVM = new NegativeCheckerVM(helios);
        PotentialNegativeVM = new PotentialNegativeVM(helios);
        ItemsWithNoPickBinVM = new ItemsWithNoPickBinVM(helios);

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        await new Task(() => { });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}