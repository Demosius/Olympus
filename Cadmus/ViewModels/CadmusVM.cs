using Cadmus.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;

namespace Cadmus.ViewModels;

public class CadmusVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }

    public CadmusVM(Helios helios)
    {
        Helios = helios;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}