using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Morpheus.ViewModels.Commands;
using Morpheus.ViewModels.Controls;
using Morpheus.ViewModels.Interfaces;
using Styx;
using Uranus;

namespace Pantheon.ViewModels.PopUp.Employees;

public class LocationSelectionVM : INotifyPropertyChanged, ISelector
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ObservableCollection<StringCountVM> Locations { get; set; }

    public bool UserCanCreate { get; }
    public bool CanCreate => UserCanCreate && NewLocationName.Length > 0 &&
                             !Locations.Select(l => l.Name).Contains(NewLocationName);
    public bool CanDelete => SelectedLocation?.Count == 0;
    public bool CanConfirm => SelectedLocation is not null;

    #region INotifyPropertyChanged Members

    private StringCountVM? selectedLocation;
    public StringCountVM? SelectedLocation
    {
        get => selectedLocation;
        set
        {
            selectedLocation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDelete));
        }
    }

    private string newLocationName;
    public string NewLocationName
    {
        get => newLocationName;
        set
        {
            newLocationName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanCreate));
        }
    }

    #endregion

    #region Commands
    
    public CreateCommand CreateCommand { get; set; }
    public DeleteCommand DeleteCommand { get; set; }
    public ConfirmSelectionCommand ConfirmSelectionCommand { get; set; }

    #endregion

    public LocationSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        Locations = new ObservableCollection<StringCountVM>(
            Helios.StaffReader.Employees()
                .GroupBy(e => e.Location)
                .ToDictionary(g => g.Key, g => g.Count())
                .Select(i => new StringCountVM(i.Key, i.Value))
                .OrderBy(p => p.Name)
            );

        UserCanCreate = Charon.CanCreateEmployee();
        newLocationName = string.Empty;

        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);   
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
    }

    public void Create()
    {
        var newPP = new StringCountVM(NewLocationName, 0);
        Locations.Add(newPP);
        SelectedLocation = newPP;
        NewLocationName = string.Empty;
    }

    public void Delete()
    {
        if (SelectedLocation is null || SelectedLocation.Count > 0) return;

        Locations.Remove(SelectedLocation);
        SelectedLocation = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}