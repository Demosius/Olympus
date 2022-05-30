using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Hydra.ViewModels.Controls;

public class ZoneListingVM : INotifyPropertyChanged
{
    public ObservableCollection<ZoneVM> ZoneVMs { get; set; }

    public ZoneListingVM()
    {
        ZoneVMs = new ObservableCollection<ZoneVM>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}