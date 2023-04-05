﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Commands.Generic;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;
using Uranus.Staff.Models;

namespace Pantheon.ViewModels.PopUp.Employees;

public class TempTagSelectionVM : INotifyPropertyChanged, ISelector, IFilters, ITempTags
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public bool UserCanCreate { get; }
    public bool CanCreate => UserCanCreate && NewRF_ID != string.Empty;
    public bool CanDelete => UserCanCreate && SelectedTag is not null && !SelectedTag.IsAssigned;
    public bool CanConfirm => SelectedTag is not null && !SelectedTag.IsAssigned;
    public bool CanUnassign => SelectedTag is not null && SelectedTag.IsAssigned;

    public List<TempTagVM> AllTags { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<TempTagVM> TempTags { get; set; }

    private TempTagVM? selectedTag;
    public TempTagVM? SelectedTag
    {
        get => selectedTag;
        set
        {
            selectedTag = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanConfirm));
            OnPropertyChanged(nameof(CanDelete));
            OnPropertyChanged(nameof(CanUnassign));
            OnPropertyChanged(nameof(CanConfirm));
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

    private bool showAssigned;
    public bool ShowAssigned
    {
        get => showAssigned;
        set
        {
            showAssigned = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    private string newRF_ID;
    public string NewRF_ID
    {
        get => newRF_ID;
        set
        {
            newRF_ID = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }
    public SelectTempTagCommand SelectTempTagCommand { get; set; }
    public UnassignTempTagCommand UnassignTempTagCommand { get; set; }

    #endregion

    public TempTagSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        AllTags = Helios.StaffReader.TempTags().Select(tag => new TempTagVM(tag)).ToList();

        TempTags = new ObservableCollection<TempTagVM>();

        UserCanCreate = charon.CanCreateEmployee();

        newRF_ID = string.Empty;

        filterString = string.Empty;
        
        ApplyFilters();

        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
        SelectTempTagCommand = new SelectTempTagCommand(this);
        UnassignTempTagCommand = new UnassignTempTagCommand(this);
    }

    public void ClearFilters()
    {
        filterString = string.Empty;
        showAssigned = false;
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        TempTags.Clear();

        var tags = AllTags;

        tags = tags.Where(t => (ShowAssigned || !t.IsAssigned) && Regex.IsMatch(t.RF_ID, FilterString, RegexOptions.IgnoreCase)).ToList();

        foreach (var tempTagVM in tags)
            TempTags.Add(tempTagVM);
    }

    public void Create()
    {
        if (!CanCreate) return;
        if (AllTags.Select(vm => vm.RF_ID).Contains(NewRF_ID))
        {
            MessageBox.Show($"Cannot create new tag, as tag with RF ID '{NewRF_ID}' already exists.",
                "ID Already Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var currentSelected = SelectedTag;
        var newTag = new TempTag {RF_ID = NewRF_ID};

        Helios.StaffCreator.TempTag(newTag);

        var newTagVM = new TempTagVM(newTag);
        AllTags.Add(newTagVM);
        ApplyFilters();

        SelectedTag = TempTags.Contains(newTagVM) ? newTagVM : currentSelected;

        NewRF_ID = string.Empty;
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
        AllTags.Remove(SelectedTag);
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}