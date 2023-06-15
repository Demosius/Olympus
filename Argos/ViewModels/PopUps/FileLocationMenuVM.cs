using System.ComponentModel;
using System.Runtime.CompilerServices;
using Argos.Properties;
using Argos.ViewModels.Commands;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Ookii.Dialogs.Wpf;
using Uranus;
using Uranus.Annotations;

namespace Argos.ViewModels.PopUps;

public class FileLocationMenuVM : INotifyPropertyChanged, IConfirm
{
    public Helios Helios { get; set; }

    public string BackUpFilePath { get; set; }

    #region MyRegion

    private string batchLoadPath;
    public string BatchLoadPath
    {
        get => batchLoadPath;
        set
        {
            batchLoadPath = value;
            OnPropertyChanged();
        }
    }

    private string batchSavePath;
    public string BatchSavePath
    {
        get => batchSavePath;
        set
        {
            batchSavePath = value;
            OnPropertyChanged();
        }
    }

    private string lanTechPath;
    public string LanTechPath
    {
        get => lanTechPath;
        set
        {
            lanTechPath = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public SetBatchLoadPathCommand SetBatchLoadPathCommand { get; set; }
    public SetBatchSavePathCommand SetBatchSavePathCommand { get; set; }
    public SetLanTechExportPathCommand SetLanTechExportPathCommand { get; set; }
    public ConfirmCommand ConfirmCommand { get; set; }
    public ConfirmAndCloseCommand ConfirmAndCloseCommand { get; set; }

    #endregion

    public FileLocationMenuVM(Helios helios)
    {
        Helios = helios;
        batchLoadPath = Settings.Default.BatchLoadDirectory;
        batchSavePath = Settings.Default.BatchSaveDirectory;
        lanTechPath = Settings.Default.LanTechExportDirectory;
        BackUpFilePath = Helios.BatchFileBackupDirectory;

        SetBatchLoadPathCommand = new SetBatchLoadPathCommand(this);
        SetBatchSavePathCommand = new SetBatchSavePathCommand(this);
        SetLanTechExportPathCommand = new SetLanTechExportPathCommand(this);
        ConfirmAndCloseCommand = new ConfirmAndCloseCommand(this);
        ConfirmCommand = new ConfirmCommand(this);
    }
    
    public void SetLanTechExportPath()
    {
        var dlg = new VistaFolderBrowserDialog
        {
            SelectedPath = LanTechPath
        };
        if (dlg.ShowDialog() != true) return;
        LanTechPath = dlg.SelectedPath;
    }

    public void SetBatchSavePath()
    {
        var dlg = new VistaFolderBrowserDialog()
        {
            SelectedPath = BatchSavePath
        };
        if (dlg.ShowDialog() != true) return;
        BatchSavePath = dlg.SelectedPath;
    }

    public void SetBatchLoadPath()
    {
        var dlg = new VistaFolderBrowserDialog()
        {
            SelectedPath = BatchLoadPath
        };
        if (dlg.ShowDialog() != true) return;
        BatchLoadPath = dlg.SelectedPath;
    }

    public bool Confirm()
    {
        Settings.Default.LanTechExportDirectory = LanTechPath;
        Settings.Default.BatchSaveDirectory = BatchSavePath;
        Settings.Default.BatchLoadDirectory = BatchLoadPath;
        Settings.Default.Save();
        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}