using System.Collections.Generic;
using Pantheon.ViewModels.Commands;
using Pantheon.Views.Pages;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Uranus;

namespace Pantheon.ViewModels;

public class PantheonVM : INotifyPropertyChanged
{
    public Charon Charon { get; set; }
    public Helios Helios { get; set; }

    private EmployeePage? EmployeePage { get; set; }
    private ShiftPage? ShiftPage { get; set; }
    private RosterPage? RosterPage { get; set; }
    private TempTagPage? TempTagPage { get; set; }

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
    public ShowTempTagPageCommand ShowTempTagPageCommand { get; set; }
    public RefreshPageCommand RefreshPageCommand { get; set; }

    #endregion

    public PantheonVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        currentPage = new Page();

        ShowEmployeePageCommand = new ShowEmployeePageCommand(this);
        ShowShiftPageCommand = new ShowShiftPageCommand(this);
        ShowRosterPageCommand = new ShowRosterPageCommand(this);
        ShowTempTagPageCommand = new ShowTempTagPageCommand(this);
        RefreshPageCommand = new RefreshPageCommand(this);
    }

    public void SetDataSources(Charon charon, Helios helios)
    {
        Charon = charon;
        Helios = helios;
    }

    public void ShowEmployeePage()
    {
        CurrentPage = EmployeePage ??= new EmployeePage(Charon, Helios);
    }

    public void ShowShiftPage()
    {
        CurrentPage = ShiftPage ??= new ShiftPage(Helios, Charon);
    }

    public void ShowRosterPage()
    {
        CurrentPage = RosterPage ??= new RosterPage(Helios, Charon);
    }

    public void ShowTempTagPage()
    {
        CurrentPage = TempTagPage ??= new TempTagPage(Helios, Charon);
    }

    public async Task RefreshPage()
    {
        var tasks = new List<Task>();
        if (CurrentPage is EmployeePage) tasks.Add(EmployeePage!.VM.RefreshDataAsync());
        if (CurrentPage is ShiftPage) tasks.Add(ShiftPage!.VM.RefreshDataAsync());
        if (CurrentPage is RosterPage) tasks.Add(RosterPage!.VM.RefreshDataAsync());
        if (CurrentPage is TempTagPage) tasks.Add(TempTagPage!.VM.RefreshDataAsync());
        await Task.WhenAll(tasks);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator] 
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}