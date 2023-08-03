using Pantheon.ViewModels.Commands;
using Pantheon.Views.Pages;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Morpheus.ViewModels.Controls;
using Uranus;

namespace Pantheon.ViewModels;

public class PantheonVM : INotifyPropertyChanged
{
    public Charon Charon { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

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
            OnPropertyChanged();
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

    public PantheonVM(Helios helios, Charon charon, ProgressBarVM progressBar)
    {
        Helios = helios;
        Charon = charon;
        ProgressBar = progressBar;

        currentPage = new Page();

        ShowEmployeePageCommand = new ShowEmployeePageCommand(this);
        ShowShiftPageCommand = new ShowShiftPageCommand(this);
        ShowRosterPageCommand = new ShowRosterPageCommand(this);
        ShowTempTagPageCommand = new ShowTempTagPageCommand(this);
        RefreshPageCommand = new RefreshPageCommand(this);
    }
    
    public void ShowEmployeePage()
    {
        CurrentPage = EmployeePage ??= new EmployeePage(Helios, Charon, ProgressBar);
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
        if (EmployeePage is not null) await EmployeePage.VM.RefreshDataAsync();
        if (ShiftPage is not null) await ShiftPage.VM.RefreshDataAsync();
        if (RosterPage is not null) await RosterPage.VM.RefreshDataAsync();
        if (TempTagPage is not null) await TempTagPage.VM.RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator] 
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}