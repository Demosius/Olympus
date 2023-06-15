using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Argos.ViewModels.Components;

public class MainBatchVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<BatchVM> Batches { get; set; }
    public ObservableCollection<BatchGroupVM> BatchGroups { get; set; }

    private DateTime date;
    public DateTime Date
    {
        get => date;
        set
        {
            date = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    private MainBatchVM(Helios helios)
    {
        Helios = helios;

        Date = DateTime.Today;

        Batches = new ObservableCollection<BatchVM>();
        BatchGroups = new ObservableCollection<BatchGroupVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task<MainBatchVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<MainBatchVM> CreateAsync(Helios helios)
    {
        var ret = new MainBatchVM(helios);
        return ret.InitializeAsync();
    }

    public static MainBatchVM CreateEmpty(Helios helios) => new(helios);

    public async Task RefreshDataAsync()
    {
        Batches.Clear();
        BatchGroups.Clear();

        var batches = await Helios.InventoryReader.BatchesAsync(Date);

        foreach (var batch in batches)
            Batches.Add(new BatchVM(batch));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}