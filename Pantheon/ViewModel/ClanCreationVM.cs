using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel;

internal class ClanCreationVM : INotifyPropertyChanged
{
    #region Notifiable Properties

    public Clan Clan { get; set; }

    #endregion

    public ClanCreationVM()
    {
        Clan = new Clan();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        throw new System.NotImplementedException();
    }
}