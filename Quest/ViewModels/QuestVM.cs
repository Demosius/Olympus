using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Quest.ViewModels;

public class QuestVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public QuestVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public async Task RefreshDataAsync()
    {
        await Task.Run(() => {});
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}