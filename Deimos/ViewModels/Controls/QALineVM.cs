using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Inventory;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class QALineVM : INotifyPropertyChanged, IDBInteraction
{
    public QALine QALine { get; set; }
    public QAErrorManagementVM ParentVM { get; set; }
    public Helios Helios { get; set; }

    #region QALine Access

    public Employee? Picker
    {
        get => QALine.Picker;
        set { QALine.Picker = value; OnPropertyChanged(); OnPropertyChanged(nameof(PickerName)); }
    }

    public QACarton? Carton => QALine.QACarton;

    public Employee? QAOperator => Carton?.QAOperator;

    public Mispick? Mispick => QALine.Mispick;

    public string ID => QALine.ID;

    public string PickerRFID
    {
        get => QALine.PickerRFID;
        set
        {
            QALine.PickerRFID = value;
            OnPropertyChanged();
            _ = SetPickerAsync();
            _ = SaveAsync();
        }
    }

    public string CartonID => QALine.CartonID;

    public string BinCode
    {
        get => QALine.BinCode;
        set
        {
            QALine.BinCode = value;
            OnPropertyChanged();
        }
    }

    public string ItemDescription => QALine.ItemDescription;

    public string PickerName => QALine.PickerName;

    public string ItemNumber => QALine.ItemNumber.ToString();

    public string ErrorType
    {
        get => QALine.ErrorType;
        set
        {
            QALine.ErrorType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ErrorCategory));
            _ = SetFault();
            _ = SaveAsync();
        }
    }

    public int PickQty => QALine.PickQty;

    public EUoM UoM => QALine.UoM;

    public int QtyPerUoM => QALine.QtyPerUoM;

    public int PickQtyBase => QALine.PickQtyBase;

    public int QAQty
    {
        get => QALine.QAQty;
        set
        {
            QALine.QAQty = value;
            VarianceQty = PickQty - QAQty;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public int VarianceQty
    {
        get => QALine.VarianceQty;
        set
        {
            QALine.VarianceQty = value;
            OnPropertyChanged();
        }
    }

    public EQAStatus QAStatus
    {
        get => QALine.QAStatus;
        set
        {
            QALine.QAStatus = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ErrorCategory));
            _ = SaveAsync();
            Count();
        }
    }

    public DateTime Date
    {
        get => QALine.Date;
        set
        {
            QALine.Date = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public bool External
    {
        get => QALine.ErrorAllocation == EErrorAllocation.External;
        set
        {
            QALine.External = value && QALine.ErrorCategory is EErrorCategory.OtherExternal or EErrorCategory.OtherDept;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ErrorCategory));
            _ = SaveAsync();
            Count();
        }
    }

    public bool Seek
    {
        get => QALine.Seek;
        set
        {
            QALine.Seek = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Fixed));
            _ = SaveAsync();
        }
    }

    public bool Fixed 
    {
        get => QALine.Seek && QALine.Fixed;
        set
        {
            QALine.Fixed = value;
            OnPropertyChanged();
            _ = SaveAsync();
        }
    }

    public bool PickerError => QALine.PickerError;
    public bool ReceiveError => QALine.ReceiveError;
    public bool ReplenError => QALine.ReplenError;
    public bool StockingError => QALine.StockingError;
    public bool HeatMapError => QALine.HeatMapError;
    public bool QAError => QALine.QAError;
    public bool SystemError => QALine.SystemError;
    public bool SupplierError => QALine.SupplierError;

    public EErrorCategory ErrorCategory => QALine.ErrorCategory;

    public EErrorAllocation ErrorAllocation => QALine.ErrorAllocation;

    #endregion

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public QALineVM(QALine line, QAErrorManagementVM parentVM)
    {
        QALine = line;
        ParentVM = parentVM;
        Helios = ParentVM.Helios;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    /// <summary>
    /// Creates or deletes Mispick based on whether the line is AtFault.
    /// </summary>
    /// <returns></returns>
    public async Task SetFault()
    {
        OnPropertyChanged(nameof(ErrorCategory));
        switch (QALine.PickerError)
        {
            case true when Mispick is null:
                var mispick = QALine.GenerateMispick();
                if (mispick is not null) await Helios.StaffUpdater.MispickAsync(mispick);
                break;
            case false when Mispick is not null:
                await Helios.StaffDeleter.MispickAsync(Mispick);
                QALine.Mispick = null;
                break;
        }
        Count();
    }

    public async Task RefreshDataAsync()
    {
        var line = await Helios.StaffReader.QALineAsync(ID);
        if (line is null)
        {
            await SaveAsync();
            return;
        }

        QALine = line;
    }

    public void SetExternal(bool b)
    {
        QALine.External = b;
        OnPropertyChanged(nameof(External));
        OnPropertyChanged(nameof(ErrorCategory));
    }

    public void Count() => ParentVM.Count();

    public async Task SetPickerAsync()
    {
        Picker = PickerRFID == string.Empty ? null : await Helios.StaffReader.EmployeeFromRFAsync(PickerRFID);
        if (Mispick is null) return;
        Mispick.Employee = Picker;
        Mispick.AssignedDematicID = Picker?.DematicID ?? "";
        Mispick.AssignedRF_ID = Picker?.RF_ID ?? "";
        await Helios.StaffUpdater.MispickAsync(Mispick);
    }

    public async Task SaveAsync() => await Helios.StaffUpdater.QALineAsync(QALine);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}