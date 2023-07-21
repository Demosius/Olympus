using System.Collections.Generic;
using Pantheon.ViewModels.Commands;
using Pantheon.Views.Pages;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using Pantheon.ViewModels.Pages;
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

    private EmployeePageVM EmployeePageVM { get; set; } = null!;
    private ShiftPageVM ShiftPageVM { get; set; } = null!;
    private RosterPageVM RosterPageVM { get; set; } = null!;
    private TempTagPageVM TempTagPageVM { get; set; } = null!;

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

    private PantheonVM(Helios helios, Charon charon)
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

    private async Task<PantheonVM> InitializeAsync()
    {
        EmployeePageVM = await EmployeePageVM.CreateAsync(Helios, Charon);
        ShiftPageVM = await ShiftPageVM.CreateAsync(Helios, Charon);
        RosterPageVM = await RosterPageVM.CreateAsync(Helios, Charon);
        TempTagPageVM = await TempTagPageVM.CreateAsync(Helios, Charon);

        return this;
    }

    public static Task<PantheonVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new PantheonVM(helios, charon);
        return ret.InitializeAsync();
    }
    
    public void ShowEmployeePage()
    {
        CurrentPage = EmployeePage ??= new EmployeePage(Helios, Charon);
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
        if (EmployeePage is not null) tasks.Add(EmployeePage.VM!.RefreshDataAsync());
        if (ShiftPage is not null) tasks.Add(ShiftPage.VM!.RefreshDataAsync());
        if (RosterPage is not null) tasks.Add(RosterPage.VM!.RefreshDataAsync());
        if (TempTagPage is not null) tasks.Add(TempTagPage.VM!.RefreshDataAsync());
        await Task.WhenAll(tasks);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator] 
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}