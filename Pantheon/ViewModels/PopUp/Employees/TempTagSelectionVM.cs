using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Generic;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Pantheon.ViewModels.PopUp.Employees;

public class TempTagSelectionVM : INotifyPropertyChanged, ISelector, IFilters
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public bool CanCreate { get; }
    public bool CanDelete => CanCreate && SelectedTag is not null && !SelectedTag.IsAssigned;
    public bool CanConfirm => SelectedTag is not null && !SelectedTag.IsAssigned;

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
        }
    }

    #endregion

    #region Commands

    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }
    public ApplyFiltersCommand ApplyFiltersCommand { get; set; }
    public ClearFiltersCommand ClearFiltersCommand { get; set; }

    #endregion

    public TempTagSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        AllTags = Helios.StaffReader.TempTags().Select(tag => new TempTagVM(tag)).ToList();

        TempTags = new ObservableCollection<TempTagVM>();
        
        CanCreate = charon.CanCreateEmployee();

        filterString = string.Empty;

        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
        ApplyFiltersCommand = new ApplyFiltersCommand(this);
        ClearFiltersCommand = new ClearFiltersCommand(this);
    }

    public void ClearFilters()
    {
        throw new System.NotImplementedException();
    }

    public void ApplyFilters()
    {
        throw new System.NotImplementedException();
    }

    public void Create()
    {
        throw new System.NotImplementedException();
    }

    public void Delete()
    {
        throw new System.NotImplementedException();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}