using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Morpheus.Views.Windows;
using Styx;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Morpheus.ViewModels.Controls.Staff;

public class ClanHandlerVM : INotifyPropertyChanged, IDBInteraction, IFilters, ICreateDelete<ClanVM>
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public List<ClanVM> AllClans { get; set; }

    public bool CanCreateClan { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<ClanVM> Clans { get; set; }

    public bool CanDeleteClan => SelectedItem?.CanDelete ?? false;

    private ClanVM? selectedItem;
    public ClanVM? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDeleteClan));
        }
    }

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

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public CreateNewItemCommand CreateNewItemCommand { get; set; }
    public DeleteSelectedItemCommand<ClanVM> DeleteSelectedItemCommand { get; set; }

    #endregion

    private ClanHandlerVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        CanCreateClan = Charon.CanCreateClan();
        filterString = string.Empty;

        AllClans = new List<ClanVM>();
        Clans = new ObservableCollection<ClanVM>();

        RefreshDataCommand = new RefreshDataCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        CreateNewItemCommand = new CreateNewItemCommand(this);
        DeleteSelectedItemCommand = new DeleteSelectedItemCommand<ClanVM>(this);
    }

    private async Task<ClanHandlerVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<ClanHandlerVM> CreateAsync(Helios helios, Charon charon)
    {
        var ret = new ClanHandlerVM(helios, charon);
        return ret.InitializeAsync();
    }

    public static ClanHandlerVM CreateEmpty(Helios helios, Charon charon) => new(helios, charon);

    public async Task RefreshDataAsync()
    {
        var clans = await Helios.StaffReader.ClansAsync();
        var empDict = (await Helios.StaffReader.EmployeesAsync()).ToDictionary(e => e.ID, e => e);
        foreach (var clan in clans)
        {
            if (empDict.TryGetValue(clan.LeaderID, out var head))
                clan.Leader = head;
        }
        AllClans = clans.Select(r => new ClanVM(r, Helios, Charon)).ToList();

        ApplyFilters();
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
    }

    public void ApplyFilters()
    {
        var clans = AllClans.Where(r =>
            Regex.IsMatch(r.Name, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.Leader, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.LeaderID, FilterString, RegexOptions.IgnoreCase) ||
            Regex.IsMatch(r.DepartmentName, FilterString, RegexOptions.IgnoreCase));

        Clans.Clear();
        foreach (var staffClanVM in clans)
            Clans.Add(staffClanVM);
    }

    public async Task CreateNewItemAsync()
    {
        if (!CanCreateClan) return;

        var input = new InputWindow("Enter new Clan Name:", "New Clan");

        if (input.ShowDialog() != true) return;

        var newClanName = input.InputText;

        if (await Helios.StaffReader.ClanExistsAsync(newClanName))
        {
            MessageBox.Show($"Clan - {newClanName} - already exists.", "Failed to Create Clan", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var clan = new Clan(newClanName);

        await Helios.StaffCreator.ClanAsync(clan);

        var clanVM = new ClanVM(clan, Helios, Charon);

        AllClans.Add(clanVM);
        Clans.Add(clanVM);
        SelectedItem = clanVM;
    }

    public async Task DeleteSelectedItemAsync()
    {
        if (SelectedItem is null || !CanDeleteClan) return;
        if (MessageBox.Show($"Are you sure that you want to delete {SelectedItem.Clan}?", "Confirm Clan Deletion",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        await Helios.StaffDeleter.ClanAsync(SelectedItem.Clan);
        await RefreshDataAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}