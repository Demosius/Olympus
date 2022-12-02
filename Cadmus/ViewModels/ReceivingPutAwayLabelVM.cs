using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Models;

namespace Cadmus.ViewModels;

public class ReceivingPutAwayLabelVM : INotifyPropertyChanged
{
    public ReceivingPutAwayLabel Label { get; set; }

    public string LabelCountDisplay => $"{Label.LabelNumber} OF {Label.LabelTotal}";
    
    #region INotifyPropertyChanged Members

    public string TakeZone
    {
        get => Label.TakeZone;
        set
        {
            Label.TakeZone = value;
            OnPropertyChanged();
        }
    }

    public string TakeBin
    {
        get => Label.TakeBin;
        set
        {
            Label.TakeBin = value;
            OnPropertyChanged();
        }
    }

    public int CaseQty
    {
        get => Label.CaseQty;
        set
        {
            Label.CaseQty = value;
            OnPropertyChanged();
        }
    }

    public int PackQty
    {
        get => Label.PackQty;
        set
        {
            Label.PackQty = value;
            OnPropertyChanged();
        }
    }

    public int EachQty
    {
        get => Label.EachQty;
        set
        {
            Label.EachQty = value;
            OnPropertyChanged();
        }
    }

    public int QtyPerCase
    {
        get => Label.QtyPerCase;
        set
        {
            Label.QtyPerCase = value;
            OnPropertyChanged();
        }
    }

    public int QtyPerPack
    {
        get => Label.QtyPerPack;
        set
        {
            Label.QtyPerPack = value;
            OnPropertyChanged();
        }
    }

    public string Barcode
    {
        get => Label.Barcode;
        set
        {
            Label.Barcode = value;
            OnPropertyChanged();
        }
    }

    public int ItemNumber
    { 
        get => Label.ItemNumber;
        set
        {
            Label.ItemNumber = value;
            OnPropertyChanged();
        }
    }

    public int LabelNumber
    {
        get => Label.LabelNumber;
        set
        {
            Label.LabelNumber = value;
            OnPropertyChanged();
        }
    }

    public int LabelTotal
    {
        get => Label.LabelTotal;
        set
        {
            Label.LabelTotal = value;
            OnPropertyChanged();
            OnPropertyChanged(LabelCountDisplay);
        }
    }

    public string Description
    {
        get => Label.Description;
        set
        {
            Label.Description = value;
            OnPropertyChanged();
            OnPropertyChanged(LabelCountDisplay);
        }
    }

    private string takeDisplayString;
    public string TakeDisplayString
    {
        get => takeDisplayString;
        set
        {
            takeDisplayString = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ReceivingPutAwayLabelVM(ReceivingPutAwayLabel label)
    {
        Label = label;

        takeDisplayString = string.Empty;

        SetTakeDisplayString();
    }

    public void SetTakeDisplayString()
    {
        string? s = null;

        if (CaseQty > 0)
            s = $"{CaseQty} CASE ({CaseQty * QtyPerCase})";

        if (EachQty > 0)
            s = $"{(s is null ? "" : $"{s}/n")}{EachQty} EACH ({EachQty})";

        if (PackQty > 0)
            s = $"{(s is null ? "" : $"{s}/n")}{PackQty} PACK ({PackQty + QtyPerPack})";

        TakeDisplayString = s ?? string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}