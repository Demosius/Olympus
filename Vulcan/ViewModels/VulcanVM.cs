using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Vulcan.Properties;

namespace Vulcan.ViewModels;

public class VulcanVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public RefreshDataCommand RefreshDataCommand { get; set; }

    public VulcanVM(Helios helios)
    {
        Helios = helios;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        await new Task(() => { });
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}