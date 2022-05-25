using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hydra.ViewModel.Controls;

namespace Hydra.ViewModel.Commands;

public class SaveZonesCommand : ICommand 
{
    public ZoneHandlerVM VM { get; set; }

    public SaveZonesCommand(ZoneHandlerVM vm)
    {
        VM = vm;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SaveZones();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}