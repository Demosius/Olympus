using System;
using System.Windows.Input;
using Pantheon.ViewModels.Interfaces;

namespace Pantheon.ViewModels.Commands.Employees;

public class ClearClanCommand : ICommand
{
    public IClans VM { get; set; }

    public ClearClanCommand(IClans vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ClearClan();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}