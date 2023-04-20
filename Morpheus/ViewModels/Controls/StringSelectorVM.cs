using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Morpheus.ViewModels.Controls;

public class StringSelectorVM : INotifyPropertyChanged
{
    public string Name { get; set; }
    public bool Selected { get; set; }

    public StringSelectorVM(string name, bool selected = false)
    {
        Name = name;
        Selected = selected;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}