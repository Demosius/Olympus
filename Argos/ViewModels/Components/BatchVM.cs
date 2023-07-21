using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Argos.ViewModels.Commands;
using Argos.Views.PopUps;
using Morpheus.Views.Windows;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class BatchVM : INotifyPropertyChanged, IDBInteraction
{
    public Batch Batch { get; private set; }
    public Helios Helios { get; set; }

    public string ID => Batch.ID;

    public bool IsWeb => Tags.Contains("WEB", StringComparer.OrdinalIgnoreCase);
    public bool IsMakeBulk => Tags.Contains("MB", StringComparer.OrdinalIgnoreCase);

    #region Direct Batch Access

    public string Description
    {
        get => Batch.Description;
        set
        {
            Batch.Description = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public DateTime CreatedOn
    {
        get => Batch.CreatedOn;
        set
        {
            Batch.CreatedOn = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string CreatedBy
    {
        get => Batch.CreatedBy;
        set
        {
            Batch.CreatedBy = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public DateTime LastTimeCartonizedDate
    {
        get => Batch.LastTimeCartonizedDate;
        set
        {
            Batch.LastTimeCartonizedDate = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public DateTime LastTimeCartonizedTime
    {
        get => Batch.LastTimeCartonizedTime;
        set
        {
            Batch.LastTimeCartonizedTime = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int Cartons
    {
        get => Batch.Cartons;
        set
        {
            Batch.Cartons = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int Units
    {
        get => Batch.Units;
        set
        {
            Batch.Units = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int Hits
    {
        get => Batch.Hits;
        set
        {
            Batch.Hits = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int BulkHits
    {
        get => Batch.BulkHits;
        set
        {
            Batch.BulkHits = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int PKHits
    {
        get => Batch.PKHits;
        set
        {
            Batch.PKHits = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int SP01Hits
    {
        get => Batch.SP01Hits;
        set
        {
            Batch.SP01Hits = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public int Priority
    {
        get => Batch.Priority;
        set
        {
            Batch.Priority = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public EBatchProgress BatchProgress
    {
        get => Batch.Progress;
        set
        {
            Batch.Progress = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public EBatchFillProgress BatchFillProgress
    {
        get => Batch.FillProgress;
        set
        {
            Batch.FillProgress = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public bool Persist
    {
        get => Batch.Persist;
        set
        {
            Batch.Persist = value;
            OnPropertyChanged();
            _ = Save();
        }
    }

    public string TagString
    {
        get => Batch.TagString;
        set
        {
            Batch.TagString = value;
            Batch.Tags = TagString.Split(',').ToList();
            Tags = new ObservableCollection<string>(Batch.Tags);
            OnPropertyChanged();
            OnPropertyChanged(nameof(Tags));
            _ = Save();
        }
    }
    
    public int LineCount
    {
        get => Batch.LineCount;
        set
        {
            Batch.LineCount = value;
            OnPropertyChanged();
        }
    }
    
    public int CalculatedUnits
    {
        get => Batch.CalculatedUnits;
        set
        {
            Batch.CalculatedUnits = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region INotifyPropertyChanged Members

    public ObservableCollection<PickLine> PickLines { get; set; }
    public ObservableCollection<string> Tags { get; set; }

    private string? selectedTag;
    public string? SelectedTag
    {
        get => selectedTag;
        set
        {
            selectedTag = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public DeleteTagCommand DeleteTagCommand { get; set; }
    public ShowPickLinesCommand ShowPickLinesCommand { get; set; }
    public AddTagCommand AddTagCommand { get; set; }

    #endregion

    public BatchVM(Batch batch, Helios helios)
    {
        Helios = helios;
        Batch = batch;
        Batch.SetTags();

        PickLines = new ObservableCollection<PickLine>(Batch.PickLines);
        Tags = new ObservableCollection<string>(Batch.Tags);
        
        RefreshDataCommand = new RefreshDataCommand(this);
        DeleteTagCommand = new DeleteTagCommand(this);
        ShowPickLinesCommand = new ShowPickLinesCommand(this);
        AddTagCommand = new AddTagCommand(this);
    }

    public async Task<int> Save() => await Helios.InventoryUpdater.BatchAsync(Batch);

    public async Task RefreshDataAsync()
    {
        Batch = await Helios.InventoryReader.BatchAsync(Batch.ID) ?? Batch;
    }

    public async Task CalculateHits()
    {
        var pickLines = await Helios.InventoryReader.PickLinesAsync(l => l.BatchID == ID);
        CalculateHits(pickLines);
        await Save();
    }

    public void CalculateHits(List<PickLine> pickLines)
    {
        Batch.CalculateHits(pickLines);
        OnPropertyChanged(nameof(Hits));
        OnPropertyChanged(nameof(PKHits));
        OnPropertyChanged(nameof(BulkHits));
        OnPropertyChanged(nameof(SP01Hits));
        OnPropertyChanged(nameof(LineCount));
        OnPropertyChanged(nameof(CalculatedUnits));
    }

    public async Task DeleteTagAsync()
    {
        if (SelectedTag is null) return;
        RemoveTag(SelectedTag);
        await Save();
    }

    public void RemoveTag(string tag)
    {
        if (!Tags.Remove(tag)) return;
        Batch.TagString = string.Join(',', Tags);
        Batch.Tags = new List<string>(Tags);
        OnPropertyChanged(nameof(TagString));
    }

    public async Task AddTagAsync()
    {
        var inputWindow = new InputWindow("Enter Tag", "New Tag");
        if (inputWindow.ShowDialog() != true) return;
        var newTag = inputWindow.InputText;

        AddTag(newTag);

        await Save();
    }

    public void AddTag(string newTag)
    {
        if (Tags.Contains(newTag)) return;
        Tags.Add(newTag);
        Batch.Tags = new List<string>(Tags);
        Batch.TagString = string.Join(',', Tags);
        OnPropertyChanged(nameof(TagString));
    }

    public async Task ShowPickLinesAsync()
    {
        var pickLines = await Helios.InventoryReader.PickLinesAsync(l => l.BatchID == ID);
        var pickWindow = new PickLineWindow(pickLines);
        pickWindow.ShowDialog();
    }

    public void SetBatchFillProgress(EBatchFillProgress progress)
    {
        Batch.FillProgress = progress;
        OnPropertyChanged(nameof(BatchFillProgress));
    }

    public void SetBatchProgress(EBatchProgress progress)
    {
        Batch.Progress = progress;
        OnPropertyChanged(nameof(BatchProgress));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return Batch.ToString();
    }
}