using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Panacea.Interfaces;
using Panacea.ViewModels.Components;

namespace Panacea.ViewModels.Commands;


public class BinsToClipboardCommand : ICommand
{
    public IBinData VM { get; set; }

    public BinsToClipboardCommand(IBinData vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.BinsToClipboard();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}