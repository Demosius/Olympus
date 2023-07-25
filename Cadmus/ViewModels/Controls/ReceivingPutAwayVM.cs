using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Helpers;
using Cadmus.Interfaces;
using Cadmus.Models;
using Cadmus.ViewModels.Commands;
using Cadmus.ViewModels.Labels;

namespace Cadmus.ViewModels.Controls;

public class ReceivingPutAwayVM : INotifyPropertyChanged, IPrintable, IDataLines
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

    public ObservableCollection<ReceivingPutAwayLabelVM> SelectedGridLabels { get; set; }

    public ObservableCollection<ReceivingPutAwayLabelVM> SelectedLabels { get; set; }

    #endregion

    #region Commands

    public PrintCommand PrintCommand { get; set; }
    public AddLineCommand AddLineCommand { get; set; }
    public ClearLinesCommand ClearLinesCommand { get; set; }
    public DeleteSelectedCommand DeleteSelectedCommand { get; set; }

    #endregion

    public ReceivingPutAwayVM()
    {
        Labels = new List<ReceivingPutAwayLabel>();
        labelVMs = new ObservableCollection<ReceivingPutAwayLabelVM>();
        SelectedLabels = new ObservableCollection<ReceivingPutAwayLabelVM>();
        SelectedGridLabels = new ObservableCollection<ReceivingPutAwayLabelVM>();

        // Set Commands
        PrintCommand = new PrintCommand(this);
        AddLineCommand = new AddLineCommand(this);
        ClearLinesCommand = new ClearLinesCommand(this);
        DeleteSelectedCommand = new DeleteSelectedCommand(this);
    }

    public void Print()
    {
        PrintUtility.PrintLabels(LabelVMs, SelectedLabels);
    }

    public void AddLine()
    {
        var label = new ReceivingPutAwayLabel();
        Labels.Add(label);
        LabelVMs.Add(new ReceivingPutAwayLabelVM(label));
    }

    public void ClearLines()
    {
        SelectedLabels.Clear();
        LabelVMs.Clear();
        Labels.Clear();
    }

    public void DeleteSelected()
    {
        foreach (var labelVM in SelectedLabels)
        {
            Labels.Remove(labelVM.Label);
            LabelVMs.Remove(labelVM);
        }

        SelectedLabels.Clear();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}