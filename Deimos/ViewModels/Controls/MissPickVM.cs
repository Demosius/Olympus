using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Uranus;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Deimos.ViewModels.Controls;

public class MissPickVM : INotifyPropertyChanged
{
    public MissPick MissPick { get; set; }

    public Helios Helios { get; set; }

    #region Direct MissPick Access

    public DateTime ShipmentDate => MissPick.ShipmentDate;
    public DateTime ReceivedDate => MissPick.ReceivedDate;
    public DateTime PostedDate => MissPick.PostedDate;
    public string CartonID => MissPick.CartonID;
    public int ItemNumber => MissPick.ItemNumber;
    public string ItemDescription => MissPick.ItemDescription;
    public string ActionNotes => MissPick.ActionNotes;
    public int OriginalQty => MissPick.OriginalQty;
    public int ReceivedQty => MissPick.ReceivedQty;
    public int VarianceQty => MissPick.VarianceQty;

    public string Comments
    {
        get => MissPick.Comments;
        set
        {
            MissPick.Comments = value;
            OnPropertyChanged();
            DBUpdate();
        }
    }

    public bool Checked => MissPick.Checked;
    public string AssignedRF_ID => MissPick.AssignedRF_ID;
    public string AssignedDematicID => MissPick.AssignedDematicID;

    public bool NoCarton => MissPick.NoCarton;
    public bool NoItem => MissPick.NoItem;
    public bool NoMatch => MissPick.NoMatch;

    #endregion

    #region Other INotifyPropertyChanged Members



    #endregion

    #region Commands

    

    #endregion

    public MissPickVM(MissPick missPick, Helios helios)
    {
        MissPick = missPick;
        Helios = helios;
    }

    public Task DBUpdate()
    { 
        return Task.Run(() => Helios.StaffUpdater.MissPickAsync(MissPick));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}