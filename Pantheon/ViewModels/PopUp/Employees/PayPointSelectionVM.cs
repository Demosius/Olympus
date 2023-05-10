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

public class PayPointSelectionVM : INotifyPropertyChanged, ISelector
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ObservableCollection<StringCountVM> PayPoints { get; set; }

    public bool UserCanCreate { get; }
    public bool CanCreate => UserCanCreate && NewPayPointName.Length > 0;
    public bool CanDelete => SelectedPayPoint?.Count == 0;
    public bool CanConfirm => SelectedPayPoint is not null;

    #region INotifyPropertyChanged Members
    
    private StringCountVM? selectedPayPoint;
    public StringCountVM? SelectedPayPoint
    {
        get => selectedPayPoint;
        set
        {
            selectedPayPoint = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDelete));
            OnPropertyChanged(nameof(CanConfirm));
        }
    }

    private string newPayPointName;
    public string NewPayPointName
    {
        get => newPayPointName;
        set
        {
            newPayPointName = value;
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

    public PayPointSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        PayPoints = new ObservableCollection<StringCountVM>(
            AsyncHelper.RunSync(() =>Helios.StaffReader.EmployeesAsync())
                .GroupBy(e => e.PayPoint)
                .ToDictionary(g => g.Key, g => g.Count())
                .Select(i => new StringCountVM(i.Key, i.Value))
                .OrderBy(p => p.Name)
            );

        UserCanCreate = Charon.CanCreateEmployee();
        newPayPointName = string.Empty;

        CreateCommand = new CreateCommand(this);
        DeleteCommand = new DeleteCommand(this);
        ConfirmSelectionCommand = new ConfirmSelectionCommand(this);
    }

    public void Create()
    {
        var newPP = new StringCountVM(NewPayPointName, 0);
        PayPoints.Add(newPP);
        SelectedPayPoint = newPP;   
        NewPayPointName = string.Empty;
    }

    public void Delete()
    {
        if (SelectedPayPoint is null || SelectedPayPoint.Count > 0) return;

        PayPoints.Remove(SelectedPayPoint);
        SelectedPayPoint = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}