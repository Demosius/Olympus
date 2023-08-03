using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Deimos.Interfaces;
using Deimos.ViewModels.Commands;
using Morpheus;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Quest.Properties;
using Quest.Views.Components;
using Serilog;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Quest.ViewModels;

public enum EQuestPage
{
    [Description("PickRate Tracker")]
    PickRateTracker,
    [Description("Mapping")]
    Mapping,
    [Description("Display")]
    Display,
}

public class QuestVM : INotifyPropertyChanged, IDBInteraction, IPickEvents
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public Dictionary<EQuestPage, IRefreshingControl> Controls { get; set; }

    #region INotifyPropertyChanged Members

    private IRefreshingControl? currentControl;
    public IRefreshingControl? CurrentControl
    {
        get => currentControl;
        set
        {
            currentControl = value;
            OnPropertyChanged();
        }
    }


    private EQuestPage? selectedPrintable;
    public EQuestPage? SelectedPrintable
    {
        get => selectedPrintable;
        set
        {
            selectedPrintable = value;
            OnPropertyChanged();
            SetControl();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public UploadPickEventsCommand UploadPickEventsCommand { get; set; }

    #endregion

    public QuestVM(Helios helios, Charon charon, ProgressBarVM progressBar)
    {
        Helios = helios;
        Charon = charon;
        ProgressBar = progressBar;

        Controls = new Dictionary<EQuestPage, IRefreshingControl>();

        RefreshDataCommand = new RefreshDataCommand(this);
        UploadPickEventsCommand = new UploadPickEventsCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (CurrentControl is null) return;
        await CurrentControl.RefreshDataAsync();
    }

    public async Task UploadPickEvents()
    {
        ProgressBar.StartTask("Uploading pick event data...");
        try
        {
            await Helios.StaffUpdater.UploadPickHistoryDataAsync(General.ClipboardToString(), Settings.Default.PTLBreak,
                Settings.Default.RFTBreak);
            await RefreshDataAsync();
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(ex.Message, "Failed to Upload", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to upload Pick Event data.");
            ProgressBar.EndTask();
            throw;
        }
        ProgressBar.EndTask();
    }

    private void SetControl()
    {
        if (SelectedPrintable is null) return;

        var questPage = (EQuestPage)SelectedPrintable;

        if (!Controls.TryGetValue(questPage, out var newControl))
        {
            newControl = questPage switch
            {
                EQuestPage.PickRateTracker => new PickRateTrackerView(Helios, ProgressBar),
                EQuestPage.Mapping => new MappingView(),
                EQuestPage.Display => new DisplayView(),
                _ => throw new ArgumentOutOfRangeException()
            };

            Controls[questPage] = newControl;
        }

        CurrentControl = newControl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}