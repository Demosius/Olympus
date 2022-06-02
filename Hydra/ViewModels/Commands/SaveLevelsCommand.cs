using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hydra.ViewModels.Controls;

namespace Hydra.ViewModels.Commands;


public class SaveLevelsCommand : ICommand
{
    public ItemLevelsVM VM { get; set; }

    public SaveLevelsCommand(ItemLevelsVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SaveLevels();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}