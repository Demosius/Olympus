using System.ComponentModel;
using System.Runtime.CompilerServices;
using Styx;
using Styx.Interfaces;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModel.Pages.Users;

internal class RolesVM : INotifyPropertyChanged, IDataSource
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    public RolesVM() { }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}