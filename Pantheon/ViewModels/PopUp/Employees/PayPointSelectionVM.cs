using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Pantheon.Annotations;
using Pantheon.ViewModels.Commands.Employees;
using Pantheon.ViewModels.Controls.Employees;
using Styx;
using Uranus;

namespace Pantheon.ViewModels.PopUp.Employees;

public class PayPointSelectionVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ObservableCollection<PayPointVM> PayPoints { get; set; }

    public bool CanCreatePayPoints { get; set; }

    #region INotifyPropertyChanged Members
    
    private PayPointVM? selectedPayPoint;
    public PayPointVM? SelectedPayPoint
    {
        get => selectedPayPoint;
        set
        {
            selectedPayPoint = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanDelete));
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
        }
    }

    public bool CanDelete => SelectedPayPoint?.Count == 0;

    #endregion

    #region Commands

    public AddNewPayPointCommand AddNewPayPointCommand { get; set; }
    public DeletePayPointCommand DeletePayPointCommand { get; set; }
    public ConfirmPayPointSelectionCommand ConfirmPayPointSelectionCommand { get; set; }

    #endregion

    public PayPointSelectionVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        PayPoints = new ObservableCollection<PayPointVM>(
            Helios.StaffReader.Employees()
                .GroupBy(e => e.PayPoint)
                .ToDictionary(g => g.Key, g => g.Count())
                .Select(i => new PayPointVM(i.Key, i.Value))
            );

        CanCreatePayPoints = Charon.CanCreateEmployee();
        newPayPointName = string.Empty;

        AddNewPayPointCommand = new AddNewPayPointCommand(this);
        DeletePayPointCommand = new DeletePayPointCommand(this);
        ConfirmPayPointSelectionCommand = new ConfirmPayPointSelectionCommand(this);
    }

    public void AddNewPayPoint()
    {
        PayPoints.Add(new PayPointVM(NewPayPointName, 0));
    }

    public void DeletePayPoint()
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