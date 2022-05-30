using Pantheon.ViewModels.PopUp.Employees;
using System;
using System.Windows;
using System.Windows.Input;

namespace Pantheon.ViewModels.Commands.Employees;

internal class ConfirmClanCreationCommand : ICommand
{
    public ClanCreationVM VM { get; set; }

    public ConfirmClanCreationCommand(ClanCreationVM vm) { VM = vm; }

    public bool CanExecute(object? parameter)
    {
        return VM.Clan.Name != string.Empty &&
               VM.Clan.Department is not null &&
               VM.Clan.Leader is not null &&
               !VM.ClanNames.Contains(VM.Clan.Name);
    }

    public void Execute(object? parameter)
    {

        if (parameter is not Window w) return;
        w.DialogResult = VM.ConfirmClanCreation();
        w.Close();
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}