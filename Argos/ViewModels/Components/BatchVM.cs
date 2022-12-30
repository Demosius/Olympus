using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class BatchVM : INotifyPropertyChanged
{
    private Batch Batch { get; }

    public string ID => Batch.ID;

    #region INotifyPropertyChanged Members


    public string Description
    {
        get => Batch.Description;
        set
        {
            Batch.Description = value;
            OnPropertyChanged();
        }
    }

    public DateTime CreatedOn
    {
        get => Batch.CreatedOn;
        set
        {
            Batch.CreatedOn = value;
            OnPropertyChanged();
        }
    }

    public string CreatedBy
    {
        get => Batch.CreatedBy;
        set
        {
            Batch.CreatedBy = value;
            OnPropertyChanged();
        }
    }

    public DateTime LastTimeCartonizedDate
    {
        get => Batch.LastTimeCartonizedDate;
        set
        {
            Batch.LastTimeCartonizedDate = value;
            OnPropertyChanged();
        }
    }

    public DateTime LastTimeCartonizedTime
    {
        get => Batch.LastTimeCartonizedTime;
        set
        {
            Batch.LastTimeCartonizedTime = value;
            OnPropertyChanged();
        }
    }

    public int Cartons
    {
        get => Batch.Cartons;
        set
        {
            Batch.Cartons = value;
            OnPropertyChanged();
        }
    }

    public int Units
    {
        get => Batch.Units;
        set
        {
            Batch.Units = value;
            OnPropertyChanged();
        }
    }

    public int Hits
    {
        get => Batch.Hits; 
        set
        {
            Batch.Hits = value;
            OnPropertyChanged();
        }
    }

    public int BulkHits
    {
        get => Batch.BulkHits; 
        set
        {
            Batch.BulkHits = value;
            OnPropertyChanged();
        }
    }

    public int PKHits
    {
        get => Batch.PKHits; 
        set
        {
            Batch.PKHits = value;
            OnPropertyChanged();
        }
    }

    public int SP01Hits
    {
        get => Batch.SP01Hits; 
        set
        {
            Batch.SP01Hits = value;
            OnPropertyChanged();
        }
    }

    public int Priority
    {
        get => Batch.Priority;
        set
        {
            Batch.Priority = value;
            OnPropertyChanged();
        }
    }

    #endregion
        
    public BatchVM(Batch batch)
    {
        Batch = batch;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}