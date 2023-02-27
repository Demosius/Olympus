using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public void Execute(object? parameter)
    {
        VM.AddMoves();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}