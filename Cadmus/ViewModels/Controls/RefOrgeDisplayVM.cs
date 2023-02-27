using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Cadmus.Annotations;
using Cadmus.Helpers;
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

    public ObservableCollection<RefOrgeMasterLabelVM> SelectedMasterLabels { get; set; }

    public ObservableCollection<RefOrgeLabelVM> SelectedLabels { get; set; }

    #endregion
    #region Commands

    public PrintCommand PrintCommand { get; set; }
    public AddLineCommand AddLineCommand { get; set; }
    public ClearLinesCommand ClearLinesCommand { get; set; }
    public DeleteSelectedCommand DeleteSelectedCommand { get; set; }
    public AddMovesCommand AddMovesCommand { get; set; }

    #endregion

    public RefOrgeDisplayVM(Helios helios)
    {
        Helios = helios;

        labelVMs = new ObservableCollection<RefOrgeLabelVM>();
        Masters = new List<RefOrgeMasterLabel>();
        masterVMs = new ObservableCollection<RefOrgeMasterLabelVM>();
        SelectedLabels = new ObservableCollection<RefOrgeLabelVM>();
        SelectedMasterLabels = new ObservableCollection<RefOrgeMasterLabelVM>();

        PrintCommand = new PrintCommand(this);
        AddLineCommand = new AddLineCommand(this);
        AddMovesCommand = new AddMovesCommand(this);
        ClearLinesCommand = new ClearLinesCommand(this);
        DeleteSelectedCommand = new DeleteSelectedCommand(this);
    }

    public void Print()
    {
        PrintUtility.PrintLabels(LabelVMs, SelectedLabels);
    }

    public void AddLine()
    {
        var newLabel = new RefOrgeMasterLabel();
        Masters.Add(newLabel);
        MasterVMs.Add(new RefOrgeMasterLabelVM(newLabel, this));
    }

    public void ClearLines()
    {
        Masters.Clear();
        MasterVMs.Clear();
        LabelVMs.Clear();
        SelectedLabels.Clear();
        SelectedMasterLabels.Clear();
    }

    public void DeleteSelected()
    {
        foreach (var masterLabel in SelectedMasterLabels)
        {
            MasterVMs.Remove(masterLabel);
            Masters.Remove(masterLabel.Label);
        }

        GenerateDisplayLabels();
    }

    public void AddMoves()
    {
        Mouse.OverrideCursor = Cursors.Wait;
        List<NAVMoveLine> moveLines;
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
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }

        // Get other data from DB.
        var dataSet = Helios.InventoryReader.BasicStockDataSet(moveLines.Select(m => m.ZoneCode).Distinct().ToList(), new List<string> {"9600"});
        dataSet?.SetMoveLineData(moveLines);

        // Convert MoveLines to moves.
        var moves = Move.GenerateMoveList(moveLines);

        // Generate Mixed Carton moves, if applicable.
        var mixedCartonTemplates = Helios.InventoryReader.MixedCartonTemplates().ToList();

        var mixCtnMoves = MixedCartonMove.GenerateMixedCartonMoveList(ref mixedCartonTemplates, ref moves);
        moves.AddRange(mixCtnMoves);

        // Convert Moves to masterLabels
        Masters.AddRange(moves.Select(m => new RefOrgeMasterLabel(m)));

        MasterVMs = new ObservableCollection<RefOrgeMasterLabelVM>(Masters.Select(m => new RefOrgeMasterLabelVM(m, this)));

        GenerateDisplayLabels();

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public void GenerateDisplayLabels()
    {
        // For each master label, generate the appropriate number of display labels.
        SelectedLabels.Clear();

        var labels = new List<RefOrgeLabelVM>();
        foreach (var masterLabelVM in MasterVMs)
        {
            labels.AddRange(masterLabelVM.GetDisplayLabels());
        }

        LabelVMs = new ObservableCollection<RefOrgeLabelVM>(labels);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}