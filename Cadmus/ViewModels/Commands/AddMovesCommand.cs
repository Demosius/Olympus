using System;
using System.Windows.Input;
using Cadmus.ViewModels.Controls;

namespace Cadmus.ViewModels.Commands;


public class AddMovesCommand : ICommand
{
    public RefOrgeDisplayVM VM { get; set; }

    public AddMovesCommand(RefOrgeDisplayVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public async void Execute(object? parameter)
    {
        await VM.AddMoves();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}