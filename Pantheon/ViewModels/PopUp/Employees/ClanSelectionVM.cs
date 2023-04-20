using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interfaces;
using Pantheon.Views.PopUp.Employees;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class ClanSelectionVM : INotifyPropertyChanged, ICreationMode, ISelector, IDepartments, IFilters, IManagers
{
    private const string ANY_DEP_STR = "<ANY>";

    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    
    public bool UserCanCreate { get; }
    public bool CanCreate => UserCanCreate &&
                             NewClanName.Length > 0 &&
                             !FullClans.Select(c => c.Name).Contains(NewClanName) &&
                             ClanDepartment is not null;

    public bool UserCanDelete { get; }
    public bool CanDelete => UserCanDelete && SelectedClan is not null;
    public bool CanConfirm => SelectedClan is not null;

    public bool ShowCreationOption => !InCreation && UserCanCreate;
    public bool ShowNew => InCreation && UserCanCreate;

    public List<Clan> FullClans { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<Clan> Clans { get; set; }


    private Clan? selectedClan;
    public Clan? SelectedClan
    {
        get => selectedClan;
        set
        {
            selectedClan = value;
            OnPropertyChanged();
        }
    }

    // Filters
    public ObservableCollection<string> DepartmentNames { get; set; }

    private string filterString;
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private string selectedDepartmentName;
    public string SelectedDepartmentName
    {
        get => selectedDepartmentName;
        set
        {
            selectedDepartmentName = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    // Create New Clan

    private bool inCreation;
    public bool InCreation
    {
        get => inCreation;
        set
        {
            inCreation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ShowCreationOption));
            OnPropertyChanged(nameof(ShowNew));
        }
    }

    private string newClanName;
    public string NewClanName
    {
        get => newClanName;
        set
        {
            newClanName = value;
            OnPropertyChanged();
        }
    }

    private Department? clanDepartment;
    public Department? ClanDepartment
    {
        get => clanDepartment;
        set
        {
            clanDepartment = value;
            OnPropertyChanged();
        }
    }

    private Employee? clanLeader;
    public Employee? ClanLeader
    {
        get => clanLeader;
        set
        {
            clanLeader = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public SelectDepartmentCommand SelectDepartmentCommand { get; set; }
    public ClearDepartmentCommand ClearDepartmentCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }
    public ActivateCreationCommand ActivateCreationCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public SelectManagerCommand SelectManagerCommand { get; set; }
    public ClearManagerCommand ClearManagerCommand { get; set; }

    #endregion

    public ClanSelectionVM(Helios helios, Charon charon, string? departmentName = null)
    {
        Helios = helios;
        Charon = charon;

        UserCanCreate = Charon.CanCreateClan();
        UserCanDelete = Charon.CanDeleteClan();

        FullClans = Helios.StaffReader.Clans();

        Clans = new ObservableCollection<Clan>(FullClans);

        filterString = string.Empty;

        DepartmentNames = new ObservableCollection<string> { ANY_DEP_STR };
        foreach (var department in FullClans.Select(c => c.DepartmentName).Distinct())
        {
            DepartmentNames.Add(department);
        }

        selectedDepartmentName = departmentName ?? ANY_DEP_STR;
        newClanName = string.Empty;

        SelectDepartmentCommand = new SelectDepartmentCommand(this);
        ClearDepartmentCommand = new ClearDepartmentCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        ActivateCreationCommand = new ActivateCreationCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        SelectManagerCommand = new SelectManagerCommand(this);
        ClearManagerCommand = new ClearManagerCommand(this);
    }

    public void SelectDepartment()
    {
        var departmentSelector = new DepartmentSelectionWindow(Helios, Charon);
        if (departmentSelector.ShowDialog() != true) return;

        ClanDepartment = departmentSelector.VM.SelectedDepartment;
    }

    public void ClearDepartment()
    {
        ClanDepartment = null;
    }

    public void SelectManager()
    {
        var fullEmployeeList = Helios.StaffReader.Employees().OrderBy(e => e.FullName).Select(e => new EmployeeVM(e, Charon, Helios)).ToList();

        var leaderSelector = new EmployeeSelectionWindow(fullEmployeeList, department: ClanDepartment?.Name);
        if (leaderSelector.ShowDialog() != true) return;

        ClanLeader = leaderSelector.VM.SelectedEmployee?.Employee;
    }

    public void ClearManager()
    {
        ClanLeader = null;
    }

    public void ActivateCreation()
    {
        InCreation = !InCreation;
    }

    public void Create()
    {
        if (ClanDepartment is null || !CanCreate) return;

        var newClan = new Clan
        {
            Name = NewClanName,
            DepartmentName = ClanDepartment.Name,
            LeaderID = ClanLeader?.ID ?? 0,
        };

        // Create in Database
        Helios.StaffCreator.Clan(newClan);

        // Refresh all data.
        FullClans = Helios.StaffReader.Clans();
        
        DepartmentNames.Clear();
        DepartmentNames.Add(ANY_DEP_STR);
        foreach (var department in FullClans.Select(c => c.DepartmentName).Distinct())
            DepartmentNames.Add(department);

        ClearFilters();

        // Select newly created clan.
        SelectedClan = Clans.FirstOrDefault(c => c.Name == NewClanName);

        // Clear creation data.
        NewClanName = string.Empty;
        ClanDepartment = null;
        ClanLeader = null;
    }

    public void Delete()
    {
        if (SelectedClan is null || !CanDelete) return;
        // Confirm Deletion
        if (MessageBox.Show($"Are you sure that you would like to delete the {SelectedClan.Name} clan?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        Helios.StaffDeleter.Clan(SelectedClan);
    }

    public void ClearFilters()
    {
        selectedDepartmentName = ANY_DEP_STR;
        filterString = string.Empty;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        var clans = SelectedDepartmentName == ANY_DEP_STR
            ? FullClans
            : FullClans.Where(c => c.DepartmentName == SelectedDepartmentName);

        clans = clans.Where(c => Regex.IsMatch(c.Name, FilterString));

        Clans.Clear();
        foreach (var clan in clans) Clans.Add(clan);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}