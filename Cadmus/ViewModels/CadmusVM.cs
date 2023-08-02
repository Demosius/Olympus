using System;
using System.Collections.Generic;
using Cadmus.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Cadmus.Views.Controls;
using Morpheus.ViewModels.Controls;
using Uranus;

namespace Cadmus.ViewModels;

public enum EPrintable
{
    [Description("Receiving Put-Away Labels")]
    ReceivingPutAwayLabels,
    [Description("Replen Labels")]
    ReplenLabels,
    [Description("Mixed Carton Stock Report")]
    MCStockReport,
    [Description("Carton Labels")]
    CartonLabels,
}

public class CadmusVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    public Dictionary<EPrintable, Control> Controls { get; set; }

    #region INotifyPropertyChanged Members


    private Control? currentControl;
    public Control? CurrentControl
    {
        get => currentControl;
        set
        {
            currentControl = value;
            OnPropertyChanged();
        }
    }


    private EPrintable? selectedPrintable;
    public EPrintable? SelectedPrintable
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

    public CadmusVM(Helios helios, ProgressBarVM progressBar)
    {
        Helios = helios;
        ProgressBar = progressBar;

        Controls = new Dictionary<EPrintable, Control>();
    }

    private void SetControl()
    {
        if (SelectedPrintable is null) return;

        var printable = (EPrintable)SelectedPrintable;

        if (!Controls.TryGetValue(printable, out var newControl))
        {
            newControl = printable switch
            {
                EPrintable.ReceivingPutAwayLabels => new ReceivingPutAway(),
                EPrintable.ReplenLabels => new RefOrgeDisplayView(Helios),
                EPrintable.MCStockReport => new MixedCartonSOHView(Helios, ProgressBar),
                EPrintable.CartonLabels => new CCNDisplayView(),
                _ => throw new ArgumentOutOfRangeException()
            };

            Controls[printable] = newControl;
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