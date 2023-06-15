using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Argos.ViewModels.Components;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Argos.ViewModels;

public class ArgosVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    public MainBatchVM MainBatchVM { get; set; }
    public CCNCommandVM CCNCommandVM { get; set; }

    #region INotifyPropertyChanged Members

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

    private ArgosVM(Helios helios)
    {
        Helios = helios;
        MainBatchVM = MainBatchVM.CreateEmpty(Helios);
        CCNCommandVM = CCNCommandVM.CreateEmptyVM(Helios);
        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task<ArgosVM> InitializeAsync()
    {
        MainBatchVM = await MainBatchVM.CreateAsync(Helios);
        CCNCommandVM = await CCNCommandVM.CreateAsync(Helios);
        return this;
    }

    public static Task<ArgosVM> CreateAsync(Helios helios)
    {
        var ret = new ArgosVM(helios);
        return ret.InitializeAsync();
    }

    public static ArgosVM CreateEmpty(Helios helios) => new(helios);

    public async Task RefreshDataAsync()
    {
        await MainBatchVM.RefreshDataAsync();
        await CCNCommandVM.RefreshDataAsync();
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}