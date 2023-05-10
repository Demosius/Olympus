using Hydra.ViewModels.Controls;
using System;
using System.Windows.Input;

namespace Hydra.ViewModels.Commands;


public class ManageSiteCommand : ICommand
{
    public ItemLevelsVM VM { get; set; }

    public ManageSiteCommand(ItemLevelsVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.SelectedObject is SiteItemLevelVM;
    }

    public async void Execute(object? parameter)
    {
        await VM.ManageSite();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}