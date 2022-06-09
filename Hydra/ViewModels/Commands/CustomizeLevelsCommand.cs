using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class CustomizeLevelsCommand : ICommand
{
    public ItemLevelsVM VM { get; set; }

    public CustomizeLevelsCommand(ItemLevelsVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedObject is SiteItemLevelVM;
    }

    public void Execute(object? parameter)
    {
        VM.CustomizeLevels();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
