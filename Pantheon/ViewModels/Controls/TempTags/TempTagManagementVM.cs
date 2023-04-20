using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Interfaces;
using Morpheus.Views.Windows;
using Pantheon.ViewModels.Commands.TempTags;
using Pantheon.ViewModels.Interfaces;
using Pantheon.ViewModels.Pages;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.Controls.TempTags;

public class TempTagManagementVM : INotifyPropertyChanged, IFilters, ISelector, ITempTags
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public TempTagPageVM ParentVM { get; set; }

    public EmployeeDataSet DataSet { get; set; }
    private List<TempTagVM> allTags;

    public bool CanCreate { get; }
    public bool CanDelete => CanCreate && SelectedTag is not null && !SelectedTag.IsAssigned && !SelectedTag.TagUses.Any();
    public bool CanConfirm => SelectedTag is not null && !SelectedTag.IsAssigned;
    public bool CanUnassign => SelectedTag is not null && SelectedTag.IsAssigned;
    public bool CanAssign => false;

    #region INotifyPropertyChanged Members

    public ObservableCollection<TempTagVM> TempTags { get; set; }
    public ObservableCollection<TagUseVM> TagUse { get; set; }

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

    private bool? filterAssigned;
    public bool? FilterAssigned
    {
        get => filterAssigned;
        set
        {
            filterAssigned = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private TempTagVM? selectedTag;
    public TempTagVM? SelectedTag
    {
        get => selectedTag;
        set
        {
            selectedTag = value;
            OnPropertyChanged();
            SetTagUse();
            ParentVM.SelectedTag = selectedTag;
        }
    }

    private TagUseVM? selectedUse;
    public TagUseVM? SelectedUse
    {
        get => selectedUse;
        set
        {
            selectedUse = value;
            OnPropertyChanged();
        }
    }
    #endregion

    #region Commands

    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }
    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }
    public AssignTempTagCommand AssignTempTagCommand { get; set; }

    #endregion

    public TempTagManagementVM(EmployeeDataSet dataSet, TempTagPageVM tempPageVM)
    {
        ParentVM = tempPageVM;

        Helios = ParentVM.Helios;
        Charon = ParentVM.Charon;

        DataSet = dataSet;

        CanCreate = Charon.CanCreateEmployee();

        allTags = new List<TempTagVM>();

        TempTags = new ObservableCollection<TempTagVM>();
        TagUse = new ObservableCollection<TagUseVM>();

        filterString = string.Empty;

        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        SelectTempTagCommand = new SelectTempTagCommand(this);
        UnassignTempTagCommand = new UnassignTempTagCommand(this);
        AssignTempTagCommand = new AssignTempTagCommand(this);

        RefreshData();
    }

    private void RefreshData()
    {
        allTags = DataSet.TagDict.Values.Select(tag => new TempTagVM(tag, this)).ToList();

        ApplyFilters();
    }

    public void RefreshData(EmployeeDataSet dataSet)
    {
        DataSet = dataSet;
        TempTags.Clear();
        TagUse.Clear();
        SelectedTag = null;
        SelectedUse = null;
        RefreshData();
    }

    public void SetTagUse()
    {
        TagUse.Clear();
        if (SelectedTag is null) return;
        foreach (var tagUseVM in SelectedTag.Usage)
            TagUse.Add(tagUseVM);
    }

    public void ClearFilters()
    {
        FilterString = string.Empty;
        FilterAssigned = null;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        TempTags.Clear();

        var tags = allTags
            .Where(tag => Regex.IsMatch(tag.RF_ID, FilterString, RegexOptions.IgnoreCase) &&
                          (FilterAssigned is null || 
                           (FilterAssigned == true && tag.IsAssigned) ||
                           (FilterAssigned == false && !tag.IsAssigned)));

        foreach (var tempTagVM in tags)
            TempTags.Add(tempTagVM);
    }

    public void RefreshTagView()
    {
        SetTagUse();
        ParentVM.TagEmployeeVM.ReOrderEmployees();
    }

    public void Create()
    {
        if (!CanCreate) return;

        // Prompt for new ID
        var input = new InputWindow("Enter RF ID for new Temp Tag.", "New RFID");
        if (input.ShowDialog() != true) return;

        var newRFID = input.InputText;

        if (allTags.Select(vm => vm.RF_ID).Contains(newRFID))
        {
            MessageBox.Show($"Cannot create new tag, as tag with RF ID '{newRFID}' already exists.",
                "ID Already Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var currentSelected = SelectedTag;
        var newTag = new TempTag { RF_ID = newRFID };

        Helios.StaffCreator.TempTag(newTag);

        var newTagVM = new TempTagVM(newTag);
        allTags.Add(newTagVM);

        ApplyFilters();

        SelectedTag = TempTags.Contains(newTagVM) ? newTagVM : currentSelected;
    }

    public void Delete()
    {
        if (SelectedTag is null) return;

        // Confirm
        if (MessageBox.Show($"Are you sure that you would like to delete the temp tag: {SelectedTag.RF_ID}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        if (!Helios.StaffDeleter.TempTag(SelectedTag.TempTag))
        {
            MessageBox.Show(
                "Cannot delete this Temp Tag as it is either currently assigned, or has historic use data that may be still be relevant.",
                "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        TempTags.Remove(SelectedTag);
        allTags.Remove(SelectedTag);
        SelectedTag = null;
    }

    public void SelectTempTag()
    {
        throw new System.NotImplementedException();
    }

    public void UnassignTempTag()
    {
        if (SelectedTag is null || !SelectedTag.IsAssigned) return;

        // Confirm
        if (MessageBox.Show($"Unassign {SelectedTag.Employee} from tag {SelectedTag.RF_ID}?", "Confirm Unassignment", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

        Helios.StaffUpdater.UnassignTempTag(SelectedTag.TempTag);

        ApplyFilters();
    }

    public void AssignTempTag()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Updates the usage in the database for the given tag.
    /// </summary>
    /// <param name="tag"></param>
    public int UpdateTagUse(TempTagVM tag) => Helios.StaffUpdater.TagUsage(tag.TempTag);

    /// <summary>
    /// Updates the given usage in the database.
    /// </summary>
    public int UpdateTagUse(TagUseVM tagUse) => Helios.StaffUpdater.TagUsage(tagUse.TagUse);

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}