using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class MoveVM : INotifyPropertyChanged
{
    public Move Move { get; }

    public int ItemNumber => Move.ItemNumber;
    public string ItemDescription => Move.Item?.Description ?? "";

    public string TakeSiteName => Move.TakeBin?.Zone?.SiteName ?? "";
    public string TakeZoneCode => Move.TakeBin?.Zone?.Code ?? "";
    public string TakeBinCode => Move.TakeBin?.Code ?? "";
    public string TakeQtyString => $"{Move.TakeCases}C, {Move.TakePacks}P, {Move.TakeEaches}E";

    public string PlaceSiteName => Move.PlaceBin?.Zone?.SiteName ?? "";
    public string PlaceZoneCode => Move.PlaceBin?.Zone?.Code ?? "";
    public string PlaceBinCode => Move.PlaceBin?.Code ?? "";
    public string PlaceQtyString => $"{Move.PlaceCases}C, {Move.PlacePacks}P, {Move.PlaceEaches}E";


    #region INotifyPropertyChanged Members


    #endregion

    public MoveVM(Move move)
    {
        Move = move;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}