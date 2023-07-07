using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class QAStatReportsVM : INotifyPropertyChanged, IDBInteraction
{
    public DeimosVM Deimos { get; set; }
    public QAToolVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    #region INotifyPropertyChanged Members



    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    public QAStatReportsVM(QAToolVM parentVM)
    {
        ParentVM = parentVM;
        Deimos = ParentVM.ParentVM;
        Helios = ParentVM.Helios;
        ProgressBar = ParentVM.ProgressBar;

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    public Task RefreshDataAsync()
    {
        throw new System.NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}