using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Deimos.Views.Controls;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class QAToolVM : INotifyPropertyChanged, IDBInteraction
{
    public DeimosVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public Dictionary<EQAView, IRefreshingControl> Controls { get; set; }

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

    private EQAView? selectedView;
    public EQAView? SelectedView
    {
        get => selectedView;
        set
        {
            selectedView = value;
            OnPropertyChanged();
            SetControl();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public QAToolVM(DeimosVM parentVM)
    {
        ParentVM = parentVM;
        Helios = ParentVM.Helios;
        ProgressBar = ParentVM.ProgressBar;
        
        Controls = new Dictionary<EQAView, IRefreshingControl>();

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (!Controls.TryGetValue(EQAView.Errors, out var errorManagementView)) return;
        await ((QAErrorManagementView)errorManagementView).RefreshDataAsync();
    }

    private void SetControl()
    {
        if (SelectedView is null) return;

        var view = (EQAView) SelectedView;

        if (!Controls.TryGetValue(view, out var newControl))
        {
            newControl = view switch
            {
                EQAView.Errors => new QAErrorManagementView(Helios, ProgressBar),
                EQAView.Stats => new QAOperatorStatsView(this),
                EQAView.Reports => new QAStatReportsView(this),
                _ => throw new ArgumentOutOfRangeException()
            };

            Controls[view] = newControl;
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