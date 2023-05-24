using System.ComponentModel;
using System.Runtime.CompilerServices;
using Quest.Views;
using Uranus.Annotations;

namespace Quest.ViewModels;

public class AppVM : INotifyPropertyChanged
{
    public QuestPage QuestPage { get; set; }

    public AppVM()
    {
        QuestPage = new QuestPage(App.Helios, App.Charon);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}