using System;
using System.Windows;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands;

public class AlphaRegisterCommand : ICommand
{
    public AlphaRegistrationVM VM { get; set; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public AlphaRegisterCommand(AlphaRegistrationVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object parameter)
    {
        return VM.PasswordGood &&
               int.TryParse(VM.EmployeeID, out _) &&
               VM.DisplayName != "" &&
               VM.DepartmentName != "" && 
               VM.RoleName != "";
    }

    public void Execute(object parameter)
    {
        if (VM.Register(out var message))
        {
            var window = parameter as Window;
            window?.Close();
        }
        // TODO: Improve this message process.
        MessageBox.Show(message, "Failure To Register", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}