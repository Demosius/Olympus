using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cadmus.Annotations;
using Cadmus.Models;
using Cadmus.ViewModels.Controls;
using iText.Kernel.Colors;
using Morpheus;
using Uranus.Inventory.Models;

namespace Cadmus.ViewModels.Labels;

public class RefOrgeMasterLabelVM : INotifyPropertyChanged
{
    public RefOrgeMasterLabel Label { get; set; }
    public RefOrgeDisplayVM DisplayVM { get; set; }

    public List<RefOrgeLabelVM> DisplayLabels { get; set; }

    #region INotifyPropertyChanged Members

    public int Priority
    {
        get => Label.Priority;
        set
        {
            Label.Priority = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public string BatchName
    {
        get => Label.BatchName;
        set
        {
            Label.BatchName = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public string OperatorName
    {
        get => Label.OperatorName;
        set
        {
            Label.OperatorName = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public DateTime Date
    {
        get => Label.Date;
        set
        {
            Label.Date = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public string TakeBin
    {
        get => Label.TakeBin;
        set
        {
            Label.TakeBin = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public bool PickAsPacks
    {
        get => Label.PickAsPacks;
        set
        {
            Label.PickAsPacks = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public bool Web
    {
        get => Label.Web;
        set
        {
            Label.Web = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public int EachQty
    {
        get => Label.EachQty;
        set
        {
            Label.EachQty = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int PackQty
    {
        get => Label.PackQty;
        set
        {
            Label.PackQty = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int CaseQty
    {
        get => Label.CaseQty;
        set
        {
            Label.CaseQty = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int QtyPerCase
    {
        get => Label.QtyPerCase;
        set
        {
            Label.QtyPerCase = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public int QtyPerPack
    {
        get => Label.QtyPerPack;
        set
        {
            Label.QtyPerPack = value;
            OnPropertyChanged();
            SetTakeDisplayString();
        }
    }

    public string PlaceBin
    {
        get => Label.PlaceBin;
        set
        {
            Label.PlaceBin = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public string? CheckDigits
    {
        get => Label.CheckDigits;
        set
        {
            Label.CheckDigits = value;
            OnPropertyChanged();
            UpdateDisplay();
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
            UpdateDisplay();
            UpdateDisplay(nameof(Barcode));
        }
    }

    public string? TotalGrab
    {
        get => Label.TotalGrab;
        set
        {
            Label.TotalGrab = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public int LabelTotal
    {
        get => Label.LabelTotal;
        set
        {
            Label.LabelTotal = value;
            OnPropertyChanged();
            DisplayVM.GenerateDisplayLabels(this);
        }
    }

    public string ItemDescription
    {
        get => Label.ItemDescription;
        set
        {
            Label.ItemDescription = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public string TrueOrderTakeBin
    {
        get => Label.TrueOrderTakeBin;
        set
        {
            Label.TrueOrderTakeBin = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    public string TakeZone
    {
        get => Label.TakeZone;
        set
        {
            Label.TakeZone = value;
            OnPropertyChanged();
            UpdateDisplay();
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
            UpdateDisplay();
        }
    }


    public string MixedContentDisplay
    {
        get => Label.MixedContentDisplay;
        set
        {
            Label.MixedContentDisplay = value;
            OnPropertyChanged();
            UpdateDisplay();
        }
    }
    
    public EMoveType MoveType
    {
        get => Label.MoveType;
        set
        {
            if (OperatorName == MoveType.ToString())
                Label.OperatorName = string.Empty;
            Label.MoveType = value;
            if (OperatorName == string.Empty) 
                OperatorName = MoveType.ToString();
            OnPropertyChanged();
            UpdateDisplay();
        }
    }

    #endregion

    public bool MixedCarton => Label.MixedCarton;

    public RefOrgeMasterLabelVM(RefOrgeMasterLabel label, RefOrgeDisplayVM displayVM)
    {
        Label = label;
        DisplayVM = displayVM;
        DisplayLabels = new List<RefOrgeLabelVM>();
        takeDisplayString = string.Empty;

        SetTakeDisplayString();
    }

    public RefOrgeMasterLabelVM(Move move, RefOrgeDisplayVM displayVM) : this(new RefOrgeMasterLabel(move, displayVM.LabelMax), displayVM) { }

    public List<RefOrgeLabelVM> GetDisplayLabels()
    {
        DisplayLabels.Clear();

        for (var i = 0; i < LabelTotal; i++)
        {
            DisplayLabels.Add(new RefOrgeLabelVM(this, i + 1));
        }

        return DisplayLabels;
    }

    public void SetLabelMax(int? labelMax)
    {
        CalculateRequiredLabels(labelMax);
        OnPropertyChanged(nameof(LabelTotal));
    }

    public void CalculateRequiredLabels(int? maxLabels = null) => Label.CalculateRequiredLabels(maxLabels);

    public void CalculateTotalGrabs()
    {
        Label.CalculateTotalGrabs();
        OnPropertyChanged(nameof(TotalGrab));
        UpdateDisplay(nameof(TotalGrab));
    }

    private void SetTakeDisplayString()
    {
        string? s = null;

        if (CaseQty > 0)
            s = $"{CaseQty} CASE ({QtyPerCase})";

        if (PackQty > 0)
            s = $"{(s is null ? "" : $"{s}/n")}{PackQty} PACK ({QtyPerPack})";

        if (EachQty > 0)
            s = $"{(s is null ? "" : $"{s}/n")}{EachQty} EACH (1)";

        TakeDisplayString = s ?? string.Empty;
    }

    /// <summary>
    /// Updates (OnPropertyChanged) values for sub-labels.
    /// Used for when data is updated on the master to show those changes on the sub labels.
    /// </summary>
    private void UpdateDisplay([CallerMemberName] string? propertyName = null)
    {
        foreach (var labelVM in DisplayLabels)
        {
            labelVM.UpdateDisplay(propertyName);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}