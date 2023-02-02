using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hermes.Views.Controls;
using Styx;
using Uranus;
using Uranus.Annotations;

namespace Hermes.ViewModels;

public class HermesVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    #region INotifypropertyChanged Members
    
    private bool canMessage;
    public bool CanMessage
    {
        get => canMessage;
        set
        {
            canMessage = value;
            OnPropertyChanged();
        }
    }
    
    private NotificationsView notificationsView;
    public NotificationsView NotificationsView
    {
        get => notificationsView;
        set
        {
            notificationsView = value;
            OnPropertyChanged();
        }
    }

    private FAQView faqView;
    public FAQView FAQView
    {
        get => faqView;
        set
        {
            faqView = value;
            OnPropertyChanged();
        }
    }

    private ReportingView reportingView;
    public ReportingView ReportingView
    {
        get => reportingView;
        set
        {
            reportingView = value;
            OnPropertyChanged();
        }
    }

    private MessagingView? messagingView;
    public MessagingView? MessagingView
    {
        get => messagingView;
        set
        {
            messagingView = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public HermesVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanMessage = Charon.User is not null;

        notificationsView = new NotificationsView(helios, charon);
        faqView = new FAQView(helios, charon);
        reportingView = new ReportingView(helios, charon);
        messagingView = CanMessage ? new MessagingView(helios, charon) : null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}