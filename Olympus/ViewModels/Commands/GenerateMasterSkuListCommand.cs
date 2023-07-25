using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Olympus.ViewModels.Commands;

public class GenerateMasterSkuListCommand : ICommand
{
    public OlympusVM VM { get; set; }

    public GenerateMasterSkuListCommand(OlympusVM vm)
    {
        VM = vm;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _ = Task.Run(OlympusVM.GenerateMasterSkuList);
    }
}