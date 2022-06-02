using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Inventory.Models;

namespace Hydra.ViewModels.Controls;

public class ItemVM : INotifyPropertyChanged
{
    public NAVItem Item { get; set; }
    public int Number => Item.Number;
    public string VolumeString => $"{Item.Length} X {Item.Width} X {Item.Height} = {Item.Cube}cm³ : {Item.Weight}kg";
     
    #region INotifyPropertyChanged Members

    public bool UseLevelTargets
    {
        get => Item.SiteLevelTarget;
        set
        {
            Item.SiteLevelTarget = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => Item.Description;
        set
        {
            Item.Description = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ItemVM(NAVItem item)
    {
        Item = item;
        item.Extension ??= new ItemExtension(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}