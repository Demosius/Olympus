using System;
using System.Windows.Input;
using Argos.ViewModels.PopUps;

namespace Argos.ViewModels.Commands;

public class SetLanTechExportPathCommand : ICommand
{
    public FileLocationMenuVM VM { get; set; }

    public SetLanTechExportPathCommand(FileLocationMenuVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        VM.SetLanTechExportPath();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}