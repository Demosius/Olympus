using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cadmus.Interfaces;

namespace Cadmus.ViewModels.Commands;


public class ClearLinesCommand : ICommand
{
    public IDataLines VM { get; set; }

    public ClearLinesCommand(IDataLines vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.ClearLines();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}