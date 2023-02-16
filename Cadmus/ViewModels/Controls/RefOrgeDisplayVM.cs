using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Interfaces;
using Cadmus.Models;
using Cadmus.ViewModels.Commands;
using Cadmus.ViewModels.Labels;

namespace Cadmus.ViewModels.Controls;

internal class RefOrgeDisplayVM : INotifyPropertyChanged, IPrintable, IDataLines
{
    public List<RefOrgeMasterLabel> Masters { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<RefOrgeMasterLabelVM> masterVms;
    public ObservableCollection<RefOrgeMasterLabelVM> MasterVMs
    {
        get => masterVms;
        set
        {
            masterVms = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<RefOrgeLabelVM> labelVMs;
    public ObservableCollection<RefOrgeLabelVM> LabelVMs
    {
        get => labelVMs;
        set
        {
            labelVMs = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<RefOrgeLabelVM> SelectedLabels { get; set; }

    #endregion
    #region Commands

    public PrintCommand PrintCommand { get; set; }
    public AddLineCommand AddLineCommand { get; set; }

    #endregion

    public RefOrgeDisplayVM()
    {
        labelVMs = new ObservableCollection<RefOrgeLabelVM>();
        Masters = new List<RefOrgeMasterLabel>();
        SelectedLabels = new ObservableCollection<RefOrgeLabelVM>();
        
        PrintCommand = new PrintCommand(this);
        AddLineCommand = new AddLineCommand(this);
    }

    public void Print()
    {
        throw new NotImplementedException();
    }

    public void AddLine()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}