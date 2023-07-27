using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WindowsInput;
using System.Windows.Forms;
using Morpheus;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using WindowsInput.Native;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Hermes.ViewModels.Controls;

public class AutoCounterVM : INotifyPropertyChanged, IDBInteraction, IFilters
{
    public Helios Helios { get; set; }

    public List<BinVM> AllBins { get; set; }

    public bool UseUserData { get; set; }
    public bool NoUserData => UserDataTable.Rows.Count == 0 || !UserDataTable.Columns.Contains("BinCode");

    #region INotifyPropertyChanged Members

    public ObservableCollection<BinVM> Bins { get; set; }

    private DataTable userDataTable;
    public DataTable UserDataTable
    {
        get => userDataTable;
        set
        {
            userDataTable = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NoUserData));
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public AutoCounterVM(Helios helios)
    {
        Helios = helios;

        AllBins = new List<BinVM>();
        Bins = new ObservableCollection<BinVM>();

        userDataTable = new DataTable();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public void InsertUserData()
    {
        DataTable dt;
        try
        {
            var rawData = General.ClipboardToString();
            dt = DataConversion.RawStringToTable(rawData);
        }
        catch (Exception ex)
        {
            MessageBox.Show($@"Failed to upload data: {ex.Message}", @"Invalid Data", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        DataColumn? binColumn = null;

        foreach (DataColumn dataColumn in dt.Columns)
        {
            // Check for Bin Code column.
            if (Regex.IsMatch(dataColumn.ColumnName, @"(?i)bin\s?code"))
            {
                binColumn = dataColumn;
                break;
            }
            // Check for Code Column
            if (Regex.IsMatch(dataColumn.ColumnName, @"(?i)code")) binColumn = dataColumn;
        }

        if (binColumn is null)
        {
            MessageBox.Show(@"No valid data found on clipboard.", @"No Data", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        binColumn.ColumnName = "BinCode";

        // Establish a counted column to track counting progress
        var cCol = new DataColumn("Counted", typeof(bool));
        dt.Columns.Add(cCol);
        var i = dt.Columns.IndexOf(cCol);
        foreach (DataRow dataRow in dt.Rows)
            dataRow[i] = false;

        UserDataTable = dt;
    }

    public async Task GenerateEmptyBinList()
    {
        UseUserData = false;
        await RefreshDataAsync();
    }

    public async Task GenerateUserBinList()
    {
        UseUserData = true;
        if (NoUserData)
        {
            MessageBox.Show(@"There is no valid user data to use.", @"No Data", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }
        await RefreshDataAsync();
    }

    public async Task RefreshDataAsync()
    {
        AllBins.Clear();

        if (UseUserData && NoUserData)
        {
            ApplyFilters();
            return;
        }

        AllBins = (UseUserData ?
            await Helios.InventoryReader.NAVBinsAsync(b => UserDataTable.AsEnumerable().Select(r => r.Field<string>("BinCode")).ToHashSet().Contains(b.Code)) :
            await Helios.InventoryReader.NAVBinsAsync(b => b.Empty))
            .Select(b => new BinVM(b, Helios))
            .ToList();

        if (UseUserData)
        {
            // Check to see that all user bins are contained in bin list.
            var binCodes = AllBins.Select(b => b.Code).ToHashSet();
            var userBins = UserDataTable.AsEnumerable().Select(r => r.Field<string>("BinCode")).Where(s => s is not null).Distinct();
            var missingCodes = userBins.Where(b => !binCodes.Contains(b!));
            // Create new Bin Objects for missing bin codes.
            foreach (var missingCode in missingCodes)
                AllBins.Add(new BinVM(missingCode!, Helios));
        }

        ApplyFilters();
    }

    public void ClearFilters()
    {
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        throw new NotImplementedException();
    }

    public void ClearData()
    {
        Bins.Clear();
        AllBins.Clear();
        UserDataTable.Clear();
    }

    public async Task StartCount()
    {
        var inputSimulator = new InputSimulator();

        Console.WriteLine(@"Move your mouse to the center of the 'Confirm' button, and press enter.");
        Console.ReadLine();
        var confirmPos = Cursor.Position;

        var errorPos = new System.Drawing.Point(confirmPos.X - 112, confirmPos.Y + 173);

        Console.WriteLine($@"Confirm button position: {confirmPos}");
        Console.WriteLine(@"Move your mouse to the center of the 'Empty' button, and press enter.");
        Console.ReadLine();
        var emptyPos = Cursor.Position;

        Console.WriteLine(@"Move your mouse to the center of the text entry, and press enter.");
        Console.ReadLine();
        var textPos = Cursor.Position;

        foreach (var bin in Bins)
        {
            await bin.Count();
            if (bin.UserGenerated)
                foreach (var row in UserDataTable.AsEnumerable().Where(row => row.Field<string>("BinCode") == bin.Code))
                    row["Counted"] = true;

            inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(textPos.X, textPos.Y);
            // Optionally add a small delay here if needed
            // System.Threading.Thread.Sleep(100);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.HOME);
            for (var i = 0; i < 15; i++) inputSimulator.Keyboard.KeyPress(VirtualKeyCode.DELETE);
            inputSimulator.Keyboard.TextEntry(bin.Code);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(errorPos.X, errorPos.Y);
            inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(emptyPos.X, emptyPos.Y);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            inputSimulator.Mouse.MoveMouseToPositionOnVirtualDesktop(confirmPos.X, confirmPos.Y);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            // Optionally add a small delay between actions if needed
            // System.Threading.Thread.Sleep(100);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}