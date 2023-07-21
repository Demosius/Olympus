using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Pantheon.ViewModels.Commands.TempTags;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Controls.TempTags;
using Pantheon.ViewModels.Interfaces;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Pages;

public class TempTagPageVM : INotifyPropertyChanged, IDBInteraction, ITempTags, ISelector
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public TempTagManagementVM ManagementVM { get; set; }
    public TempTagEmployeeVM TagEmployeeVM { get; set; }

    public EmployeeDataSet DataSet { get; set; }

    public bool CanCreate { get; }

    public bool CanDelete => CanCreate && SelectedTag is not null &&
                             SelectedTag.IsAssigned && !SelectedTag.TagUses.Any();

    public bool CanConfirm => false;

    public bool CanUnassign => CanCreate && SelectedTag is not null && SelectedEmployee is not null &&
                               SelectedTag.EmployeeID == SelectedEmployee.ID;

    public bool CanAssign => CanCreate && SelectedTag is not null &&
                             !SelectedTag.IsAssigned && SelectedEmployee is not null &&
                             !SelectedEmployee.HasTempTag;

    #region INotifyPropertyChanged Members

    private TempTagVM? selectedTag;
    public TempTagVM? SelectedTag
    {
        get => selectedTag;
        set
        {
            selectedTag = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanAssign));
            OnPropertyChanged(nameof(CanUnassign));
            OnPropertyChanged(nameof(CanDelete));
        }
    }

    private EmployeeVM? selectedEmployee;
    public EmployeeVM? SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            selectedEmployee = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanAssign));
            OnPropertyChanged(nameof(CanUnassign));
            OnPropertyChanged(nameof(CanDelete));
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }
    public AssignTempTagCommand AssignTempTagCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }

    #endregion

    private TempTagPageVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanCreate = Charon.CanCreateEmployee();

        DataSet = new EmployeeDataSet();
        ManagementVM = new TempTagManagementVM(DataSet, this);
        TagEmployeeVM = new TempTagEmployeeVM(this, DataSet);

        RefreshDataCommand = new RefreshDataCommand(this);
        SelectTempTagCommand = new SelectTempTagCommand(this);
        UnassignTempTagCommand = new UnassignTempTagCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        AssignTempTagCommand = new AssignTempTagCommand(this);
    }

    private async Task<TempTagPageVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<TempTagPageVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new TempTagPageVM(helios, charon);
        return ret.InitializeAsync();
    }

    public async Task RefreshDataAsync()
    {
        DataSet = await Helios.StaffReader.EmployeeDataSetAsync();
        ManagementVM.RefreshData(DataSet);
        TagEmployeeVM.RefreshData(DataSet);
    }

    public void ReOrderEmployees()
    {
        TagEmployeeVM.ReOrderEmployees();
    }
    
    public async Task SelectTempTagAsync()
    {
        await Task.Run(() => { });
    }

    public void UnassignTempTag()
    {
        if (!CanUnassign) return;
        Helios.StaffUpdater.UnassignTempTag(ManagementVM.SelectedTag!.TempTag);
        // Make sure objects are up to date.
        SelectedTag!.RefreshEmployee();
        SelectedEmployee!.RefreshTempTag();
        TagEmployeeVM.ReOrderEmployees();
    }

    public async Task AssignTempTagAsync()
    {
        if (!CanAssign) return;
        await Helios.StaffUpdater.AssignTempTagAsync(ManagementVM.SelectedTag!.TempTag, TagEmployeeVM.SelectedEmployee!.Employee);
        // Make sure objects are up to date.
        SelectedTag!.RefreshEmployee();
        SelectedEmployee!.RefreshTempTag();
        TagEmployeeVM.ReOrderEmployees();
    }

    public void Create() => ManagementVM.Create();

    public void Delete() => ManagementVM.Delete();

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}