using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Argos.Interfaces;
using Argos.Properties;
using Cadmus.Helpers;
using Cadmus.Interfaces;
using Cadmus.ViewModels.Commands;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Uranus;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Argos.ViewModels.Components;

public class BatchTOGroupVM : INotifyPropertyChanged, IPrintable
{
    public Helios Helios { get; set; }
    public BatchTOGroup Group { get; set; }
    public IBatchTOGroupHandler ParentVM { get; set; }

    public bool LabelFileExists { get; }

    public bool UseRegion { get; set; }

    #region Group Object Access

    public Guid ID => Group.ID;
    public string CartonSizes => Group.CartonSizes;
    public string StartBays => Group.StartBays;
    public DateTime StartDate => Group.StartDate;
    public DateTime EndDate => Group.EndDate;
    public string BatchString => Group.BatchString;
    public string ZoneString => Group.ZoneString;
    public string WaveString => Group.WaveString;

    public string StartBayString => StartBays == "SP01-SP01" ? "SP01" : StartBays;

    public string BatchDescription => Group.Lines.First().Batch?.ToString() ?? BatchString;

    public BatchTOLine? FirstLine => Group.Lines.FirstOrDefault();

    public DateTime? InitializationDateTime => FirstLine?.OriginalProcessingTime;
    public DateTime? FinalizationDateTime => FirstLine?.FinalProcessingTime;
    public string OriginalFileName => FirstLine?.OriginalFileName ?? "";
    public string OriginalFileDirectory => FirstLine?.OriginalFileDirectory ?? "";
    public string LabelFileName => FirstLine?.FinalFileName ?? "";
    public string LabelFileDirectory => FirstLine?.FinalFileDirectory ?? "";

    public List<BatchTOLine> Lines => Group.Lines;
    public int LineCount => Lines.Count;

    public string ActualCartonSizes => Group.ActualCartonSizes;
    public string ActualStartBays => Group.ActualStartBays;

    public int FileNumber
    {
        get => Group.FileNumber;
        set => Group.FileNumber = value;
    }

    #endregion

    #region INotifyPropertyChanged Members

    private bool reverseSort;
    public bool ReverseSort
    {
        get => reverseSort;
        set
        {
            reverseSort = value;
            OnPropertyChanged();
            _ = SetObservableLinesAsync();
        }
    }

    private ObservableCollection<BatchTOLineVM>? observableLines;
    public ObservableCollection<BatchTOLineVM> ObservableLines
    {
        get => observableLines ??= GetObservableLines();
        set
        {
            observableLines = value;
            OnPropertyChanged();
        }
    }

    public bool CanSendToLanTech => CartonSizes is "A" or "B" or "C";

    private bool sendToLanTech = true;
    public bool SendToLanTech
    {
        get => sendToLanTech && CanSendToLanTech;
        set
        {
            sendToLanTech = value;
            OnPropertyChanged();
        }
    }

    private string stockDescriptor;
    public string StockDescriptor
    {
        get => stockDescriptor;
        set
        {
            stockDescriptor = value;
            OnPropertyChanged();
        }
    }

    private EFreightOption freightOption;
    public EFreightOption FreightOption
    {
        get => freightOption;
        set
        {
            freightOption = value;
            OnPropertyChanged();
            _ = SetFreightOption();
        }
    }

    #endregion

    #region Commands

    public PrintCommand PrintCommand { get; set; }

    #endregion

    public BatchTOGroupVM(BatchTOGroup group, Helios helios, IBatchTOGroupHandler parentVM)
    {
        Helios = helios;
        Group = group;
        ParentVM = parentVM;
        LabelFileExists = Group.IsFinalised && File.Exists(Path.Join(LabelFileDirectory, LabelFileName));

        stockDescriptor = string.Empty;

        PrintCommand = new PrintCommand(this);
    }

    public static int GetBatchFileNumber(string batchID, string directory)
    {
        var files = Directory.EnumerateFiles(directory, $"*{batchID}*").ToList();
        var total = files.Count;
        var numbers = (from file in files from Match match in Regex.Matches(Path.GetFileName(file), @"F(\d+)") select int.Parse(match.Groups[1].Value)).ToList();
        numbers.Add(total);
        return numbers.Max() + 1;
    }

    public async Task ProcessLabelsAsync()
    {
        // Generate recommended file name.
        var dir = Settings.Default.BatchSaveDirectory;

        // Make sure file name is unique.
        FileNumber = GetBatchFileNumber(BatchString, dir);
        var fileName = $"{BatchString}_{StartBayString}_{CartonLabel()}_F{FileNumber}_(Reformatted).xls";
        while (File.Exists(Path.Join(dir, fileName)))
        {
            fileName = $"{BatchString}_{StartBayString}_{CartonLabel()}_{FileNumber}_(Reformatted).xls";
            FileNumber++;
        }

        // Confirm save location with saveFileDialog window.
        var saveDialog = new SaveFileDialog
        {
            InitialDirectory = dir,
            FileName = fileName,
            DefaultExt = "xls"
        };

        if (saveDialog.ShowDialog() != true) return;

        // Get file name and save stored settings accordingly.
        var path = saveDialog.FileName;
        dir = Path.GetDirectoryName(path);
        Settings.Default.BatchSaveDirectory = dir;
        Settings.Default.Save();

        // Finalise lines and groups.
        Group.Finalise(path);

        // Create file.
        await CreateExcelFile(path);

        // Create carton file for LanTech.
        if (SendToLanTech)
            await GenerateLanTechCartonFileAsync().ConfigureAwait(false);

        // Update database.
        await Helios.InventoryUpdater.BatchTOCartonDataAsync(Group).ConfigureAwait(false);
    }

    // Get Appropriate Carton Label based on carton size list.
    private string CartonLabel() => Regex.Replace(CartonSizes, ",", "");

    private async Task CreateExcelFile(string path)
    {
        // Handle group and line data.
        UseRegion = Settings.Default.UseRegionColumn;
        var stores = new Dictionary<string, Store>();
        if (UseRegion) stores = (await Helios.InventoryReader.StoresAsync()).ToDictionary(s => s.Number, s => s);
        foreach (var line in Lines)
        {
            line.ItemNumber = 111111;
            line.Description = $"{line.CCN} {line.BatchID} F{FileNumber}";
            if (UseRegion && stores.TryGetValue(line.StoreNo, out var store))
                line.FreightRegion = store.FreightRegion(FreightOption);
        }

        // Create workbook and sheet.
        using var package = new ExcelPackage();

        var workbook = package.Workbook;
        var sheet = workbook.Worksheets.Add("NAV Batch");

        // Create headers.
        sheet.Cells[1, 1].Value = "Store No.";
        sheet.Cells[1, 2].Value = "CTNS";
        sheet.Cells[1, 3].Value = "Weight kg";
        sheet.Cells[1, 4].Value = "Cube m3";
        sheet.Cells[1, 5].Value = "CCN";
        sheet.Cells[1, 6].Value = "Carton Type";
        sheet.Cells[1, 7].Value = "Starting Pick Zone";
        sheet.Cells[1, 8].Value = "Ending Pick Zone";
        sheet.Cells[1, 9].Value = "Starting Pick Bin";
        sheet.Cells[1, 10].Value = "Ending Pick Bin";
        sheet.Cells[1, 11].Value = "TO Batch No.";
        sheet.Cells[1, 12].Value = "Date";
        sheet.Cells[1, 13].Value = "Total Units (Base)";
        sheet.Cells[1, 14].Value = "Wave Number";
        sheet.Cells[1, 15].Value = "Item";
        sheet.Cells[1, 16].Value = "Description";
        if (UseRegion)
            sheet.Cells[1, 17].Value = "Region";


        // Format Headers
        using (var range = sheet.Cells[1, 1, 1, UseRegion ? 17 : 16])
        {
            range.Style.Font.Bold = true;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        // Get data in the correct sort order.
        var lines = GetSortedLines;

        // Fill remaining data
        var row = 1;
        foreach (var line in lines)
        {
            row++;
            sheet.Cells[row, 1].Value = line.StoreNo;
            sheet.Cells[row, 2].Value = line.Cartons;
            sheet.Cells[row, 3].Value = line.Weight;
            sheet.Cells[row, 4].Value = line.Cube;
            sheet.Cells[row, 5].Value = line.CCN;
            sheet.Cells[row, 6].Value = line.CartonType;
            sheet.Cells[row, 7].Value = line.StartZone;
            sheet.Cells[row, 8].Value = line.EndZone;
            sheet.Cells[row, 9].Value = line.StartBin;
            sheet.Cells[row, 10].Value = line.EndBin;
            sheet.Cells[row, 11].Value = line.BatchID;
            sheet.Cells[row, 12].Value = line.Date.ToString("dd/MM/yy");
            sheet.Cells[row, 13].Value = line.UnitsBase;
            sheet.Cells[row, 14].Value = line.WaveNo;
            sheet.Cells[row, 15].Value = line.ItemNumber;
            sheet.Cells[row, 16].Value = line.Description;
            if (UseRegion)
                sheet.Cells[row, 17].Value = line.FreightRegion;

        }

        var xlFile = new FileInfo(path);

        package.SaveAs(xlFile);
    }

    public async Task GenerateLanTechCartonFileAsync()
    {
        // Make sure that we have the correct cartons size.
        if (CartonSizes is not ("A" or "B" or "C")) return;
        var cartonLetter = CartonSizes switch
        {
            "A" => "L",
            "B" => "M",
            "C" => "S",
            _ => "X"
        };
        if (cartonLetter == "X") return;

        // Get assumed file name.
        var ctnFileName = $"{BatchString}_{StartBays}_{cartonLetter}.txt";

        // Check if file already exists, and adjust accordingly.
        var count = 0;
        while (File.Exists(Path.Join(Settings.Default.LanTechExportDirectory, ctnFileName)))
        {
            ctnFileName = $"{BatchString}_{count}_{StartBays}_{cartonLetter}.txt";
            count++;
        }

        // Create file.
        await using var fs = File.Create(Path.Join(Settings.Default.LanTechExportDirectory, ctnFileName));
        // Add some text to file    
        var cartons = new UTF8Encoding(true).GetBytes($"{cartonLetter}|{LineCount}");
        fs.Write(cartons, 0, cartons.Length);
    }

    /* Splitting and Merging - Adjust data in database, expecting calling objects to then refresh data. */

    public async Task UpdateGroupsAsync(List<BatchTOGroup> groups)
    {
        // Handle based on whether or not this group is now empty.
        if (!Group.Lines.Any())
            await Helios.InventoryDeleter.BatchTOGroupAsync(Group);
        else
            groups.Add(Group);

        await Helios.InventoryUpdater.BatchTOCartonDataAsync(groups);
    }

    public async Task SplitByZoneAsync()
    {
        var groups = Group.SplitByZone(Settings.Default.LinkUp);
        await UpdateGroupsAsync(groups);
    }

    public async Task SplitByCartonsAsync(string split)
    {
        var groups = Group.SplitByCartonSize(split.Split(',').ToList());
        await UpdateGroupsAsync(groups);
    }

    public async Task SplitByStartBaysAsync(string split)
    {
        var groups = Group.SplitByStartBay(split);
        await UpdateGroupsAsync(groups);
    }

    public async Task SplitByWaveAsync(string split)
    {
        var groups = Group.SplitByWave(split);
        await UpdateGroupsAsync(groups);
    }

    public async Task SplitByCountAsync(string split)
    {
        var groups = new List<BatchTOGroup>();
        // Check input string to determine splitting method.
        if (Regex.IsMatch(split, "^\\d+$") && int.TryParse(split, out var n))
        {
            groups = Group.SplitEvenlyByCount(n);
        }
        else if (Regex.IsMatch(split, "^\\d+(:\\d+)+$"))
        {
            var splitRatio = split.Split(':').Select(int.Parse).ToList();
            groups = Group.SplitByRatio(splitRatio);
        }
        else if (Regex.IsMatch(split, "^\\d+(,\\d+)+$"))
        {
            var splitList = split.Split(',').Select(int.Parse).ToList();
            groups = Group.SplitByCount(splitList);
        }
        await UpdateGroupsAsync(groups);
    }

    public async Task MergeAsync(List<BatchTOGroupVM> mergingGroups)
    {
        var deletingGroups = new List<BatchTOGroup>();
        foreach (var group in mergingGroups.Where(group => group.BatchString == BatchString))
        {
            Group.Merge(group.Group);
            deletingGroups.Add(group.Group);
        }

        await Helios.InventoryDeleter.BatchTOGroupsAsync(deletingGroups);
        await Helios.InventoryUpdater.BatchTOCartonDataAsync(new List<BatchTOGroup> { Group });
    }

    public async Task RecoverOriginalFile()
    {
        var ogFileName = OriginalFileName;
        var ogDir = OriginalFileDirectory;

        // Check if back up exists.
        var backUpFilePath = Path.Join(Helios.BatchFileBackupDirectory, ogFileName);
        if (!File.Exists(backUpFilePath))
        {
            MessageBox.Show("Cannot find original file in backup directory.", "Recovery Failed", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        // Discover all new files created from this one and delete all lines (and groups) relevant.
        var newFiles = await Helios.InventoryDeleter.BatchFileRecovery(ogDir, ogFileName);

        // Delete new files.
        foreach (var newFile in newFiles)
            File.Delete(newFile);

        // Move file to og location.
        if (File.Exists(Path.Join(ogDir, ogFileName)))
            File.Delete(backUpFilePath);
        else
            File.Move(backUpFilePath, Path.Join(ogDir, ogFileName), false);
    }

    public async Task SetObservableLinesAsync() => ObservableLines = await GetObservableLinesAsync();

    public async Task<ObservableCollection<BatchTOLineVM>> GetObservableLinesAsync() => await Task.Run(GetObservableLines);

    public ObservableCollection<BatchTOLineVM> GetObservableLines() => new(GetSortedLineVMs);

    public List<BatchTOLineVM> GetSortedLineVMs => GetSortedLines.Select(l => new BatchTOLineVM(l)).ToList();

    public IEnumerable<BatchTOLine> GetSortedLines => ReverseSort
        ? Lines.OrderBy(l => l.CartonType).ThenByDescending(l => l.StartBin).ThenByDescending(l => l.EndBin)
        : Lines.OrderBy(l => l.CartonType).ThenBy(l => l.StartBin).ThenByDescending(l => l.EndBin);

    private async Task SetFreightOption()
    {
        await Task.Run(() =>
        {
            foreach (var observableLine in ObservableLines)
            {
                observableLine.SetFreightOption(FreightOption);
            }
        });
    }

    public void Print()
    {
        if (!ObservableLines.Any()) return;

        if (StockDescriptor == string.Empty)
        {
            MessageBox.Show("Please enter stock descriptor before printing.", "Missing Descriptor", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var labels = ObservableLines.Select(l => l.GetLabel(StockDescriptor)).ToList();
        PrintUtility.PrintLabels(labels, labels);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}