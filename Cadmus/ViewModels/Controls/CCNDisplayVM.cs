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

// ReSharper disable StringLiteralTypo

namespace Cadmus.ViewModels.Controls;

public class CCNDisplayVM : INotifyPropertyChanged, IPrintable, IDataLines
{
    public List<CartonLabel> Labels { get; set; }

    #region INotifyPropertyChanged Members

    private ObservableCollection<CartonLabelVM> labelVMs;
    public ObservableCollection<CartonLabelVM> LabelVMs
    {
        get => labelVMs;
        set
        {
            labelVMs = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CartonLabelVM> SelectedGridLabels { get; set; }

    public ObservableCollection<CartonLabelVM> SelectedLabels { get; set; }

    #endregion

    #region Commands

    public PrintCommand PrintCommand { get; set; }
    public AddLineCommand AddLineCommand { get; set; }
    public ClearLinesCommand ClearLinesCommand { get; set; }
    public DeleteSelectedCommand DeleteSelectedCommand { get; set; }

    #endregion

    public CCNDisplayVM()
    {
        Labels = new List<CartonLabel>();
        labelVMs = new ObservableCollection<CartonLabelVM>();
        SelectedLabels = new ObservableCollection<CartonLabelVM>();
        SelectedGridLabels = new ObservableCollection<CartonLabelVM>();
        
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
        var label = new CartonLabel();
        Labels.Add(label);
        LabelVMs.Add(new CartonLabelVM(label));
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

    /*private void GenerateSampleData()
    {
        var rand = new Random();
        var cartons = new[] {"A", "B", "C", "D", "E"};

        for (var i = 0; i < 5; i++)
        {
            var label = new CartonLabel
            {
                StoreNo = $"{rand.Next(100, 3000):0000}",
                Cartons = 1,
                Weight = rand.NextDouble() * 15,
                Cube = rand.NextDouble() * 0.23,
                CCN = $"{rand.Next(08000000, 25000000):00000000}",
                CartonType = cartons[rand.Next(0, 4)],
                StartZone = "PK",
                StartBin = "B054",
                EndZone = "SP PK",
                EndBin = "PG150101",
                TOBatchNo = $"TB{rand.Next(060000, 300000):000000}",
                Date = DateTime.Today,
                TotalUnits = rand.Next(1, rand.Next(1, 1502)),
                WaveNo = $"W{rand.Next(1, 40):00}",
                StockDescriptor = "MIX",
                Carrier = "BEX"
            };
            label.Barcode = BarcodeUtility.Encode128(label.CCN);
            Labels.Add(label);
            var vm = new CartonLabelVM(label);
            LabelVMs.Add(vm);
        }
    }*/

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}