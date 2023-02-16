using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Models;
using Morpheus;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Labels;

public class RefOrgeMasterLabelVM : INotifyPropertyChanged
{
    public RefOrgeMasterLabel Label { get; set; }

    #region INotifyPropertyChanged Members

    public int Priority
    {
        get => Label.Priority;
        set
        {
            Label.Priority = value;
            OnPropertyChanged();
        }
    }

    public string BatchName
    {
        get => Label.BatchName;
        set
        {
            Label.BatchName = value;
            OnPropertyChanged();
        }
    }

    public string OperatorName
    {
        get => Label.OperatorName;
        set
        {
            Label.OperatorName = value;
            OnPropertyChanged();
        }
    }

    public DateTime Date
    {
        get => Label.Date;
        set
        {
            Label.Date = value;
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

    public bool PickAsPacks
    {
        get => Label.PickAsPacks;
        set
        {
            Label.PickAsPacks = value;
            OnPropertyChanged();
        }
    }

    public bool Web
    {
        get => Label.Web;
        set
        {
            Label.Web = value;
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

    public int PackQty
    {
        get => Label.PackQty;
        set
        {
            Label.PackQty = value;
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

    public string PlaceBin
    {
        get => Label.PlaceBin;
        set
        {
            Label.PlaceBin = value;
            OnPropertyChanged();
        }
    }

    public string? CheckDigits
    {
        get => Label.CheckDigits;
        set
        {
            Label.CheckDigits = value;
            OnPropertyChanged();
        }
    }

    public string Barcode => Label.Barcode;

    public int ItemNumber
    {
        get => Label.ItemNumber;
        set
        {
            Label.ItemNumber = value;
            Label.Barcode = BarcodeUtility.Encode128(value.ToString());
            OnPropertyChanged();
            OnPropertyChanged(nameof(Barcode));
        }
    }

    public string? TotalGrab
    {
        get => Label.TotalGrab;
        set
        {
            Label.TotalGrab = value;
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
            // TODO: Change labelVM generation to suit new label qty ???
        }
    }

    public string ItemDescription
    {
        get => Label.ItemDescription;
        set
        {
            Label.ItemDescription = value;
            OnPropertyChanged();
        }
    }

    public string TrueOrderTakeBin
    {
        get => Label.TrueOrderTakeBin;
        set
        {
            Label.TrueOrderTakeBin = value;
            OnPropertyChanged();
        }
    }

    public string TakeZone
    {
        get => Label.TakeZone;
        set
        {
            Label.TakeZone = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public RefOrgeMasterLabelVM(RefOrgeMasterLabel label)
    {
        Label = label;
    }

    public RefOrgeMasterLabelVM(Move move) : this(new RefOrgeMasterLabel(move)) { }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}