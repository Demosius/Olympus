using Aion.ViewModels.Commands;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus;
using Uranus.Annotations;
using Uranus.Staff.Models;

namespace Aion.ViewModels;

public class EmployeeCreationVM : INotifyPropertyChanged
{
    public Helios Helios { get; set; }

    public Employee? NewEmployee { get; set; }

    private readonly List<int> existingCodes;

    private string newCode;
    public string NewCode
    {
        get => newCode;
        set
        {
            newCode = value;
            OnPropertyChanged(nameof(NewCode));
            CheckCode();
        }
    }

    private bool isFiveChars;
    public bool IsFiveChars
    {
        get => isFiveChars;
        set
        {
            isFiveChars = value;
            OnPropertyChanged(nameof(IsFiveChars));
        }
    }

    private bool isUnique;
    public bool IsUnique
    {
        get => isUnique;
        set
        {
            isUnique = value;
            OnPropertyChanged(nameof(IsUnique));
        }
    }

    private bool isNumeric;
    public bool IsNumeric
    {
        get => isNumeric;
        set
        {
            isNumeric = value;
            OnPropertyChanged(nameof(isNumeric));
        }
    }

    public ConfirmEmployeeCreationCommand ConfirmEmployeeCreationCommand { get; set; }

    public EmployeeCreationVM(Helios helios)
    {
        Helios = helios;
        existingCodes = Helios.StaffReader.EmployeeIDs();
        ConfirmEmployeeCreationCommand = new ConfirmEmployeeCreationCommand(this);

        newCode = string.Empty;
    }
    
    private void CheckCode()
    {
        IsFiveChars = NewCode.Length == 5;
        IsNumeric = int.TryParse(NewCode, out var intCode) && intCode is >= 0 and <= 99999;
        IsUnique = existingCodes.BinarySearch(intCode) < 0;
    }

    public void ConfirmCreation()
    {
        NewEmployee = new Employee { ID = int.Parse(NewCode) };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}