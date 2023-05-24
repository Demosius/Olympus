using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class MispickVM : INotifyPropertyChanged
{
    public Mispick Mispick { get; set; }

    public Helios Helios { get; set; }

    #region Direct Mispick Access

    public DateTime ShipmentDate => Mispick.ShipmentDate;
    public DateTime ReceivedDate => Mispick.ReceivedDate;
    public DateTime PostedDate => Mispick.PostedDate;
    public string CartonID => Mispick.CartonID;
    public int ItemNumber => Mispick.ItemNumber;
    public string ItemDescription => Mispick.ItemDescription;
    public string ActionNotes => Mispick.ActionNotes;
    public int OriginalQty => Mispick.OriginalQty;
    public int ReceivedQty => Mispick.ReceivedQty;
    public int VarianceQty => Mispick.VarianceQty;

    public string Comments
    {
        get => Mispick.Comments;
        set
        {
            Mispick.Comments = value;
            OnPropertyChanged();
            DBUpdate();
        }
    }

    public bool Checked => Mispick.Checked;
    public string AssignedRF_ID => Mispick.AssignedRF_ID;
    public string AssignedDematicID => Mispick.AssignedDematicID;

    public bool NoCarton => Mispick.NoCarton;
    public bool NoItem => Mispick.NoItem;
    public bool NoMatch => Mispick.NoMatch;

    #endregion

    #region Other INotifyPropertyChanged Members



    #endregion

    #region Commands

    

    #endregion

    public MispickVM(Mispick mispick, Helios helios)
    {
        Mispick = mispick;
        Helios = helios;
    }

    public Task DBUpdate()
    { 
        return Task.Run(() => Helios.StaffUpdater.MispickAsync(Mispick));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}