using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Users.Models;

namespace Hermes.ViewModels.Controls;

public class NotificationVM : INotifyPropertyChanged
{
    private readonly DevNotification notification;

    #region Notfication Access

    public string Text
    {
        get => notification.Text;
        set
        {
            notification.Text = value;
            OnPropertyChanged();
        }
    } 


    #endregion

    public NotificationVM(DevNotification notification)
    {
        this.notification = notification;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}