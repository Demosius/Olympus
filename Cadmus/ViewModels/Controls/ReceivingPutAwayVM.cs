using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Models;

namespace Cadmus.ViewModels.Controls;

public class ReceivingPutAwayVM : INotifyPropertyChanged
{
    public List<ReceivingPutAwayLabel> Labels { get; set; }

    #region INotifyPropertyChanged Members


    private ObservableCollection<ReceivingPutAwayLabelVM> labelVMs;
    public ObservableCollection<ReceivingPutAwayLabelVM> LabelVMs
    {
        get => labelVMs;
        set
        {
            labelVMs = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ReceivingPutAwayVM()
    {
        Labels = new List<ReceivingPutAwayLabel>();
        labelVMs = new ObservableCollection<ReceivingPutAwayLabelVM>
        {
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
            new(new ReceivingPutAwayLabel()),
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}