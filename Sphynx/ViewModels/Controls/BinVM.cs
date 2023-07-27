using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Sphynx.ViewModels.Controls;

public class BinVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public NAVBin Bin { get; set; }
    public bool UserGenerated { get; set; }
    public DateTime OriginalCountDate { get; set; }

    #region Bin Access

    public string Code => Bin.Code;
    public string Location => Bin.LocationCode;
    public string Zone => Bin.ZoneCode;
    public bool Empty => Bin.Empty;

    public DateTime LastPIDate
    {
        get => Bin.LastPIDate;
        set
        {
            Bin.LastPIDate = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region INotifyPropertyChanged Members

    private bool counted;
    public bool Counted
    {
        get => counted;
        set
        {
            counted = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    

    #endregion

    public BinVM(string binCode, Helios helios)
    {
        OriginalCountDate = DateTime.MinValue;
        Bin = new NAVBin {Code = binCode, LastPIDate = OriginalCountDate};
        Helios = helios;
        UserGenerated = true;
    }

    public BinVM(NAVBin bin, Helios helios)
    {
        Bin = bin;
        Helios = helios;
        UserGenerated = false;
        OriginalCountDate = Bin.LastPIDate;
    }

    public async Task<bool> Count()
    {
        Counted = true;
        LastPIDate = DateTime.Today;
        if (UserGenerated) return true;
        Bin.Empty = true;
        var i = await Helios.InventoryUpdater.NAVBinAsync(Bin);
        return i > 0;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}