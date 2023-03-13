using Pantheon.ViewModels.Commands;
using Pantheon.Views.Pages;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Pantheon.Annotations;
using Uranus;

namespace Pantheon.ViewModels;

internal class PantheonVM : INotifyPropertyChanged
{
    public Charon? Charon { get; set; }
    public Helios? Helios { get; set; }

    private EmployeePage? EmployeePage { get; set; }
    private ShiftPage? ShiftPage { get; set; }
    private RosterPage? RosterPage { get; set; }

    #region INotifyPropertyChanged Members

    private Page currentPage;
    public Page CurrentPage
    {
        get => currentPage;
        set
        {
            currentPage = value;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }

    #endregion

    #region Commands

    public ShowShiftPageCommand ShowShiftPageCommand { get; set; }
    public ShowEmployeePageCommand ShowEmployeePageCommand { get; set; }
    public ShowRosterPageCommand ShowRosterPageCommand { get; set; }
    public RefreshPageCommand RefreshPageCommand { get; set; }

    #endregion

    public PantheonVM()
    {
        currentPage = new Page();

        ShowEmployeePageCommand = new ShowEmployeePageCommand(this);
        ShowShiftPageCommand = new ShowShiftPageCommand(this);
        ShowRosterPageCommand = new ShowRosterPageCommand(this);
        RefreshPageCommand = new RefreshPageCommand(this);
    }

    public void SetDataSources(Charon charon, Helios helios)
    {
        Charon = charon;
        Helios = helios;
    }

    public void ShowEmployeePage()
    {
        CurrentPage = EmployeePage ??= new EmployeePage(Charon!, Helios!);
    }

    public void ShowShiftPage()
    {
        CurrentPage = ShiftPage ??= new ShiftPage(Helios!, Charon!);
    }

    public void ShowRosterPage()
    {
        CurrentPage = RosterPage ??= new RosterPage(Helios!, Charon!);
    }

    public void RefreshPage()
    {
        if (CurrentPage is EmployeePage) EmployeePage!.VM.RefreshData();
        if (CurrentPage is ShiftPage) ShiftPage!.VM.RefreshData();
        if (CurrentPage is RosterPage) RosterPage!.VM.RefreshData();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}