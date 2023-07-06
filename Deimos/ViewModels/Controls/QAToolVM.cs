using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Deimos.Views.Controls;
using Morpheus.ViewModels.Controls;
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

    public Dictionary<EQAView, UserControl> Controls { get; set; }

    #region INotifyPropertyChanged Members

    private UserControl? currentControl;
    public UserControl? CurrentControl
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
        
        Controls = new Dictionary<EQAView, UserControl>();

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        if (!Controls.TryGetValue(EQAView.Errors, out var control)) return;
        var errorManagementVM = control as QAErrorManagementVM;
        if (errorManagementVM == null) return;
        await ((QAErrorManagementVM)errorManagementVM).RefreshDataAsync();
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
                EQAView.Stats => new QAErrorManagementView(Helios, ProgressBar),
                EQAView.Reports => new QAErrorManagementView(Helios, ProgressBar),
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