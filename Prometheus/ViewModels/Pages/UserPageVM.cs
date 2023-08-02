using Prometheus.Views.Pages.Users;
using Styx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModels.Pages;

internal class UserPageVM : INotifyPropertyChanged
{
    public enum EUserPage
    {
        Users,
        Activate,
        Roles
    }

    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public Dictionary<EUserPage, Page?> PageDict { get; set; }

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

    public UserPageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        Pages = new ObservableCollection<EUserPage>
        {
            EUserPage.Users,
            EUserPage.Activate,
            EUserPage.Roles
        };

        PageDict = new Dictionary<EUserPage, Page?>();
    }
    
    public void SetPage(EUserPage? page)
    {
        if (page is null) return;

        var ePage = (EUserPage)page;

        if (!PageDict.TryGetValue(ePage, out var newPage))
        {
            newPage = page switch
            {
                EUserPage.Users => new UserViewPage(Helios, Charon),
                EUserPage.Activate => new UserActivatePage(Helios, Charon),
                EUserPage.Roles => new UserDeactivatePage(Helios, Charon),
                _ => currentPage
            };
            PageDict.Add(ePage, newPage);
        }

        CurrentPage = newPage;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}