using System.Collections.ObjectModel;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModel.Pages;

internal class UserVM : INotifyPropertyChanged
{
    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }


    #region INotifyPropertyChanged Members

    public ObservableCollection<string> PageNames { get; set; }

    private string? selectedPage;
    public string? SelectedPage
    {
        get => selectedPage;
        set
        {
            selectedPage = value;
            OnPropertyChanged();
        }
    }

    private Page? currentPage;
    public Page? CurrentPage
    {
        get => currentPage;
        set
        {
            currentPage = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public UserVM()
    {
        PageNames = new ObservableCollection<string>()
        {
            "View",
            "Activate",
            "Deactivate"
        };
    }

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