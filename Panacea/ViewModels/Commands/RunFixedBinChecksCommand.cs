using Panacea.ViewModels.Components;
using System;
using System.Windows.Input;

namespace Panacea.ViewModels.Commands;

public class RunFixedBinChecksCommand : ICommand
{
    public FixedBinCheckerVM VM { get; set; }

    public RunFixedBinChecksCommand(FixedBinCheckerVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.FixedZoneString != string.Empty && VM.FromZoneString != string.Empty &&
               (VM.CheckCase || VM.CheckPack || VM.CheckEach || VM.CheckExclusiveEach);
    }

    public async void Execute(object? parameter)
    {
        await VM.RunChecksAsync();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}