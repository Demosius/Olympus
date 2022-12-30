using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class BatchGroupVM : INotifyPropertyChanged
{
    private BatchGroup BatchGroup { get;}

    public ObservableCollection<BatchVM> Batches { get; set; }

    public BatchGroupVM (BatchGroup batchGroup)
    {
        BatchGroup = batchGroup;
        Batches = new ObservableCollection<BatchVM> ();
        foreach (var batchVM in BatchGroup.Batches.Select(batch => new BatchVM (batch)))
        {
            Batches.Add (batchVM);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}