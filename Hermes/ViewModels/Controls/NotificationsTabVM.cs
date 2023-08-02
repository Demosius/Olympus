using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Users.Models;

namespace Hermes.ViewModels.Controls;

public class NotificationsTabVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public List<DevNotification> Notifications { get; set; }

    public NotificationsTabVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        Notifications = new List<DevNotification>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}