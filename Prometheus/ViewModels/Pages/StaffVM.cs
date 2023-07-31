using System.Collections.Generic;
using System.Collections.ObjectModel;
using Styx;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Morpheus.Views.Controls.Staff;
using Uranus;
using Uranus.Annotations;

namespace Prometheus.ViewModels.Pages;

public class StaffVM : INotifyPropertyChanged
{
    public enum EStaffControl
    {
        Employee,
        Role,
        Department,
        Clan,
    }

    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public Dictionary<EStaffControl, Control?> ControlDict { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<EStaffControl> Controls { get; set; }

    private EStaffControl? selectedControl;
    public EStaffControl? SelectedControl
    {
        get => selectedControl;
        set
        {
            selectedControl = value;
            OnPropertyChanged();
            SetPage(selectedControl);
        }
    }

    private Control? currentControl;
    public Control? CurrentControl
    {
        get => currentControl;
        set
        {
            currentControl = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public StaffVM(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;

        ControlDict = new Dictionary<EStaffControl, Control?>();

        Controls = new ObservableCollection<EStaffControl>
        {
            EStaffControl.Employee,
            EStaffControl.Role,
            EStaffControl.Department,
            EStaffControl.Clan,
        };
    }

    public void SetPage(EStaffControl? control)
    {
        if (control is null) return;

        var valueControl = (EStaffControl)control;

        if (!ControlDict.TryGetValue(valueControl, out var newControl))
        {
            newControl = valueControl switch
            {
                EStaffControl.Employee => new EmployeeHandlerView(Helios, Charon),
                EStaffControl.Department => new DepartmentHandlerView(Helios, Charon),
                EStaffControl.Clan => new ClanHandlerView(Helios, Charon),
                EStaffControl.Role =>  new StaffRoleHandlerView(Helios, Charon),
                _ => currentControl
            };
            ControlDict.Add(valueControl, newControl);
        }

        CurrentControl = newControl;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}