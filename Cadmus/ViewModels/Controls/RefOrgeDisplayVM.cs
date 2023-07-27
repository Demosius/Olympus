using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

    public BasicStockDataSet? StockDataSet { get; set; }

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

    private int? labelMax;

    public int? LabelMax
    {
        get => labelMax;
        set
        {
            labelMax = value;
            OnPropertyChanged();
            SetLabelMax();
        }
    }

    public string LabelMaxString
    {
        get => LabelMax?.ToString() ?? "";
        set
        {
            _ = int.TryParse(value, out var val);
            LabelMax = val > 0 ? val : null;
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
        StockDataSet = null;
    }

    private void SetLabelMax()
    {
        foreach (var masterVM in MasterVMs) masterVM.SetLabelMax(LabelMax);
        GenerateDisplayLabels();
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

    public async Task AddMoves()
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
            if (!MasterVMs.Any() && !LabelVMs.Any()) StockDataSet = null;
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }

        // Get other data from DB.
        try
        {
            StockDataSet ??= await Helios.InventoryReader.BasicStockDataSetAsync(moveLines.Select(m => m.ZoneCode).Distinct().ToList(), new List<string> { "9600" });
            StockDataSet?.SetMoveLineData(moveLines);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Missing Data", MessageBoxButton.OK, MessageBoxImage.Error);
            if (!MasterVMs.Any() && !LabelVMs.Any()) StockDataSet = null;
            Mouse.OverrideCursor = Cursors.Arrow;
            return;
        }

        // Convert MoveLines to moves.
        List<Move> moves;
        try
        {
            moves = Move.GenerateMoveList(moveLines);
        }
        catch (Exception ex)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MessageBox.Show($"Error generating moves. Data is likely out of date.\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Generate Mixed Carton moves, if applicable.
        var mixedCartonTemplates = (await Helios.InventoryReader.MixedCartonTemplatesAsync()).ToList();

        var mixCtnMoves = MixedCartonMove.GenerateMixedCartonMoveList(ref mixedCartonTemplates, ref moves);
        moves.AddRange(mixCtnMoves);

        // Convert Moves to masterLabels
        var newMasters = moves.Select(m => new RefOrgeMasterLabel(m, LabelMax)).ToList();
        Masters.AddRange(newMasters);

        var newMasterVMs = newMasters.Select(m => new RefOrgeMasterLabelVM(m, this)).ToList();
        foreach (var refOrgeMasterLabelVM in newMasterVMs)
        {
            MasterVMs.Add(refOrgeMasterLabelVM);
        }

        CalculateTotalGrabs();

        GenerateDisplayLabels(newMasterVMs);

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    private void CalculateTotalGrabs()
    {
        foreach (var refOrgeMasterLabelVM in MasterVMs)
        {
            refOrgeMasterLabelVM.CalculateTotalGrabs();
        }
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

    public void GenerateDisplayLabels(IEnumerable<RefOrgeMasterLabelVM> newMasterVMs)
    {
        var labels = new List<RefOrgeLabelVM>();
        foreach (var masterLabelVM in newMasterVMs)
        {
            labels.AddRange(masterLabelVM.GetDisplayLabels());
        }

        foreach (var refOrgeLabelVM in labels)
        {
            LabelVMs.Add(refOrgeLabelVM);
        }
    }

    public void GenerateDisplayLabels(RefOrgeMasterLabelVM masterLabel)
    {
        // Get first label matching given master.
        var label = LabelVMs.FirstOrDefault(l => l.Master == masterLabel);
        if (label is null)
        {
            GenerateDisplayLabels(new List<RefOrgeMasterLabelVM> { masterLabel });
            return;
        }

        // Find index of this label.
        var index = LabelVMs.IndexOf(label);
        if (index == -1)
        {
            GenerateDisplayLabels(new List<RefOrgeMasterLabelVM> { masterLabel });
            return;
        }

        // Remove relevant labels from list.
        while (LabelVMs[index].Master == masterLabel) LabelVMs.RemoveAt(index);

        // Create and insert new appropriate labels to original list.
        var newLabels = masterLabel.GetDisplayLabels();
        foreach (var refOrgeLabelVM in newLabels)
        {
            LabelVMs.Insert(index, refOrgeLabelVM);
            index++;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}