using Aion.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Aion.ViewModels;

public class SignInVM : INotifyPropertyChanged
{
    public ObservableCollection<Employee> Managers { get; set; }

    private Employee? selectedManager;
    public Employee? SelectedManager
    {
        get => selectedManager;
        set
        {
            selectedManager = value;
            OnPropertyChanged(nameof(SelectedManager));
            Code = "";
        }
    }

    private string code;
    public string Code
    {
        get => code;
        set
        {
            code = value;
            OnPropertyChanged(nameof(Code));
        }
    }

    // Command(s)
    public SignInCommand SignInCommand { get; set; }

    public SignInVM()
    {
        Managers = new ObservableCollection<Employee>(AsyncHelper.RunSync(() => App.Helios.StaffReader.GetManagersAsync()));
        SignInCommand = new SignInCommand(this);

        code = string.Empty;
    }

    /// <summary>
    /// Compares input code to Selected manager.
    /// </summary>
    /// <returns>Bool: true if Code is correct, otherwise - false.</returns>
    public bool SignIn()
    {
        if (SelectedManager is null || Code.Length <= 0) return false;
        _ = int.TryParse(Code, out var tryID);
        return SelectedManager.ID == tryID;
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}