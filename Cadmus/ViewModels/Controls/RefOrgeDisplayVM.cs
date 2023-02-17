using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cadmus.Annotations;
using Cadmus.Interfaces;
using Cadmus.Models;
using Cadmus.ViewModels.Commands;
using Cadmus.ViewModels.Labels;
using Morpheus;
using Uranus;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Controls;

public class RefOrgeDisplayVM : INotifyPropertyChanged, IPrintable, IDataLines
{
    public Helios Helios { get; set; }

    public List<RefOrgeMasterLabel> Masters { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<RefOrgeMasterLabelVM> masterVMs;
    public ObservableCollection<RefOrgeMasterLabelVM> MasterVMs
    {
        get => masterVMs;
        set
        {
            masterVMs = value;
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
        masterVMs = new ObservableCollection<RefOrgeMasterLabelVM>();
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
        var newLabel = new RefOrgeMasterLabel();
        Masters.Add(newLabel);
        MasterVMs.Add(new RefOrgeMasterLabelVM(newLabel));
    }

    public void AddMoves()
    {
        List<NAVMoveLine> moveLines = new List<NAVMoveLine>();
        try
        {
            // Get data from clipboard.
            var raw = General.ClipboardToString();

            // Convert data to moveLines
            moveLines = DataConversion.NAVRawStringToMoveLines(raw);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return;
        }

        // Get other data from DB.
        var dataSet = Helios.InventoryReader.BasicStockDataSet(moveLines.Select(m => m.ZoneCode).Distinct().ToList());
        dataSet?.SetMoveLineData(moveLines);

        // Convert MoveLines to moves.


        // Convert Moves to masterLabels
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}