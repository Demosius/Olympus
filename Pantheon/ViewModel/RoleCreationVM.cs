using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Styx;
using Uranus;
using Uranus.Staff.Model;

namespace Pantheon.ViewModel;

internal class RoleCreationVM : INotifyPropertyChanged
{
    #region Notifiable Properties

    public Role Role { get; set; }

    #endregion

    public RoleCreationVM()
    {
        Role = new Role();
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