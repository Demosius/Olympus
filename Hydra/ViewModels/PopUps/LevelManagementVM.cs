using Hydra.ViewModels.Controls;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;

namespace Hydra.ViewModels.PopUps;

public class LevelManagementVM : INotifyPropertyChanged
{
    public ItemLevelsVM ItemLevelsVM { get; set; }
    public SiteItemLevelVM SiteItemLevelVM { get; set; }
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members

    #endregion

    #region Commands

    #endregion

    public LevelManagementVM(ItemLevelsVM parentVM, SiteItemLevelVM siteItemLevelVM)
    {
        ItemLevelsVM = parentVM;
        SiteItemLevelVM = siteItemLevelVM;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}