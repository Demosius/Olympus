using System.Collections.ObjectModel;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Prometheus.View.Pages.Users;
using Styx.Interfaces;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModel.Pages;

internal class UserVM : INotifyPropertyChanged, IDataSource
{
    public enum EUserPage
    {
        Users,
        Activate, 
        Roles
    }

    public Helios? Helios { get; set; }
    public Charon? Charon { get; set; }


    #region INotifyPropertyChanged Members

    public ObservableCollection<EUserPage> Pages { get; set; }

    private EUserPage? selectedPage;
    public EUserPage? SelectedPage
    {
        get => selectedPage;
        set
        {
            selectedPage = value;
            OnPropertyChanged();
            SetPage(selectedPage);
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
        Pages = new ObservableCollection<EUserPage>()
        {
            EUserPage.Users,
            EUserPage.Activate,
            EUserPage.Roles
        };
    }

    public void SetDataSources(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
    }

    public void SetPage(EUserPage? page)
    {
        if (Helios is null || Charon is null || page is null) return;

        CurrentPage = page switch
        {
            // TODO: Compare/convert to dictionary to hold loaded pages.
            // Load each time for now. Will mean data is always up to date on each page, and there shouldn't 
            // be an excessive amount of data to load... hopefully.
            EUserPage.Users => new UserViewPage(Helios, Charon),
            EUserPage.Activate => new UserActivatePage(Helios, Charon),
            EUserPage.Roles => new UserDeactivatePage(Helios, Charon),
            _ => currentPage
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}