using Cadmus.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cadmus.ViewModels;

public class CadmusVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}