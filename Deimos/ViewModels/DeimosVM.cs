using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Deimos.Interfaces;
using Deimos.ViewModels.Commands;
using Deimos.ViewModels.Controls;
using Morpheus;
using Morpheus.ViewModels.Controls;
using Serilog;
using Uranus;
using Uranus.Annotations;
using PDU = Deimos.PickDataUtility;


namespace Deimos.ViewModels;

public enum EQAView
{
    [Description("Error Management")]
    Errors,
    [Description("Operator Stats")]
    Stats,
    [Description("Stats/Reports")]
    Reports
}

public class DeimosVM : INotifyPropertyChanged, IPickEvents, IMispickData
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public ErrorAllocationVM ErrorAllocationVM { get; set; }
    public QAToolVM QAToolVM { get; set; }

    #region INotifyPropertChanged Members


    #endregion

    #region Commands

    public UploadPickEventsCommand UploadPickEventsCommand { get; set; }
    public UploadMispickDataCommand UploadMispickDataCommand { get; set; }
    public UploadQACartonsCommand UploadQACartonsCommand { get; set; }
    public UploadQALinesCommand UploadQALinesCommand { get; set; }

    #endregion

    public DeimosVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;

        ErrorAllocationVM = new ErrorAllocationVM(this);
        QAToolVM = new QAToolVM(this);

        UploadPickEventsCommand = new UploadPickEventsCommand(this);
        UploadMispickDataCommand = new UploadMispickDataCommand(this);
        UploadQACartonsCommand = new UploadQACartonsCommand(this);
        UploadQALinesCommand = new UploadQALinesCommand(this);
    }

    public async Task UploadPickEvents()
    {
        var lines = 0;

        try
        {
            // First check the clipboard for relevant data.
            ProgressBar.StartTask("Uploading Pick Events from Clipboard...");
            lines += await Helios.StaffUpdater.UploadPickHistoryDataAsync(General.ClipboardToString());
            ProgressBar.EndTask();
            if (lines > 0)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show($"{lines} lines affected.", "Upload Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show("Failed to upload data. Check that clipboard contents are correct, and try again.",
                    "Upload Failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        // Clipboard data not accurate. Offer to check for excel/csv files instead.
        catch (InvalidDataException dataException)
        {
            ProgressBar.EndTask();
            Mouse.OverrideCursor = Cursors.Arrow;
            if (MessageBox.Show(
                    $"Could not recognize data on clipboard. Would you like to search for the appropriate file(s)?\n\nMissing Columns:\n{string.Join(" || ", dataException.MissingColumns)}",
                    "File Load", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var progress = ProgressBar.StartTask("Pick Event File Load...");
                try
                {
                    lines += await Task.Run(async () => await PDU.PickEventFileLoadAsync(Helios, progress).ConfigureAwait(false));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to upload pick events from file(s).");
                    MessageBox.Show($"Failed to upload pick events from file(s):\n\n{e}", "Upload Failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                ProgressBar.EndTask();
            }
        }
        catch (Exception ex)
        {
            ProgressBar.EndTask();
            Mouse.OverrideCursor = Cursors.Arrow;
            Log.Error(ex, "Failed to upload pick events.");
            MessageBox.Show($"Unexpected Error:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (lines > 0) await ErrorAllocationVM.RefreshDataAsync();
    }

    public async Task UploadMispickData()
    {
        var lines = 0;
        Mouse.OverrideCursor = Cursors.Wait;

        try
        {
            ProgressBar.StartTask("Uploading Mispick data from clipboard...");
            var clipboardData = General.ClipboardToString();
            lines += await Helios.StaffCreator.UploadMispickDataAsync(clipboardData);
            ProgressBar.EndTask();

            Mouse.OverrideCursor = Cursors.Arrow;

            if (lines > 0)
            {
                MessageBox.Show($"{lines} lines affected.", "Upload Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "No mispick data uploaded. If the data is valid, it is possible that the database already contains matching mispicks.",
                    "Upload Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (InvalidDataException dataException)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            ProgressBar.EndTask();
            if (MessageBox.Show(
                    $"Could not recognize data on clipboard. Would you like to search for the appropriate file(s)?\n\nMissing Columns:\n{string.Join(" || ", dataException.MissingColumns)}",
                    "File Load", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var progress = ProgressBar.StartTask("Mispick File Load...");
                lines += await Task.Run(async () => await PDU.MispickFileLoadAsync(Helios, progress).ConfigureAwait(false));
                ProgressBar.EndTask();
            }
        }
        catch (Exception ex)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            ProgressBar.EndTask();
            Log.Error(ex, "Failed to upload mispick data.");
            MessageBox.Show($"Unexpected Error:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (lines > 0)
        {
            await ErrorAllocationVM.RefreshDataAsync();
        }

        Mouse.OverrideCursor = Cursors.Arrow;
    }

    public async Task UploadQACartons()
    {
        var lines = 0;

        try
        {
            ProgressBar.StartTask("Uploading QA Carton data from clipboard...");
            var clipboardData = General.ClipboardToString();
            var qaCartons = DataConversion.RawStringToQACartons(clipboardData);
            lines += await Helios.StaffCreator.QACartonsAsync(qaCartons);
            ProgressBar.EndTask();

            if (lines > 0)
            {
                MessageBox.Show($"{lines} lines affected.", "Upload Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "No QA Carton data uploaded. If the data is valid, it is possible that the database already contains matching data.",
                    "Upload Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (InvalidDataException dataException)
        {
            ProgressBar.EndTask();
            MessageBox.Show(dataException.Message, "Missing QA Carton Data", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            ProgressBar.EndTask();
            Log.Error(ex, "Failed to upload QA Carton data.");
            MessageBox.Show($"Unexpected Error:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (lines > 0) await QAToolVM.RefreshDataAsync();
    }

    public async Task UploadQALines()
    {
        var lines = 0;

        try
        {
            ProgressBar.StartTask("Uploading QA Line data from clipboard...");
            var clipboardData = General.ClipboardToString();
            var qaLines = DataConversion.RawStringToQALines(clipboardData);
            lines += await Helios.StaffCreator.QALinesAsync(qaLines);
            ProgressBar.EndTask();

            if (lines > 0)
            {
                MessageBox.Show($"{lines} lines affected.", "Upload Success", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "No QA Line data uploaded. If the data is valid, it is possible that the database already contains matching data.",
                    "Upload Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (InvalidDataException dataException)
        {
            ProgressBar.EndTask();
            MessageBox.Show(dataException.Message, "Missing QA Line Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            
        }
        catch (Exception ex)
        {
            ProgressBar.EndTask();
            Log.Error(ex, "Failed to upload QA Line data.");
            MessageBox.Show($"Unexpected Error:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (lines > 0) await QAToolVM.RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}