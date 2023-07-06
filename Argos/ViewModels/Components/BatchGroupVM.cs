using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class BatchGroupVM : INotifyPropertyChanged
{
    private BatchGroup BatchGroup { get;}
    public Helios Helios { get; set; }

    public ObservableCollection<BatchVM> Batches { get; set; }

    public BatchGroupVM (BatchGroup batchGroup, Helios helios)
    {
        Helios = helios;
        BatchGroup = batchGroup;
        Batches = new ObservableCollection<BatchVM>();
        foreach (var batchVM in BatchGroup.Batches.Select(batch => new BatchVM(batch, Helios)))
        {
            Batches.Add(batchVM);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}