using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.PopUps;

public class PickLinesVM : INotifyPropertyChanged
{
    public ObservableCollection<PickLine> PickLines { get; set; }

    public PickLinesVM(IEnumerable<PickLine> pickLines)
    {
        PickLines = new ObservableCollection<PickLine>(pickLines);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}