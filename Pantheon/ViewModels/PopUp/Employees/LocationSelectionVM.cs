using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.Interface;
using Styx;
using Uranus;

namespace Pantheon.ViewModels.PopUp.Employees;

public class LocationSelectionVM : INotifyPropertyChanged, IStringCount
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ObservableCollection<StringCountVM> Locations { get; set; }

    public bool CanCreateLocations { get; set; }

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
        }
    }

    public bool CanDelete => SelectedLocation?.Count == 0;
    public bool CanAdd => CanCreateLocations && NewLocationName.Length > 0;
    public bool CanConfirm => SelectedLocation is not null;

    #endregion

    #region Commands

    public AddNewStringCountCommand AddNewStringCountCommand { get; set; }
    public DeleteStringCountCommand DeleteStringCountCommand { get; set; }
    public ConfirmStringCountSelectionCommand ConfirmStringCountSelectionCommand { get; set; }

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

        CanCreateLocations = Charon.CanCreateEmployee();
        newLocationName = string.Empty;

        AddNewStringCountCommand = new AddNewStringCountCommand(this);
        DeleteStringCountCommand = new DeleteStringCountCommand(this);
        ConfirmStringCountSelectionCommand = new ConfirmStringCountSelectionCommand(this);
    }

    public void AddNewPayPoint()
    {
        var newPP = new StringCountVM(NewLocationName, 0);
        Locations.Add(newPP);
        SelectedLocation = newPP;
        NewLocationName = string.Empty;
    }

    public void DeletePayPoint()
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