using Pantheon.ViewModel.Pages;
using System;
using System.Windows.Input;

namespace Pantheon.ViewModel.Commands;

public class AddClanCommand : ICommand
{
    public EmployeePageVM VM { get; set; }

    public AddClanCommand(EmployeePageVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return VM.Charon?.CanCreateClan() ?? false;
    }

    public void Execute(object? parameter)
    {
        VM.AddClan();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

}