﻿using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class LaunchShiftCreatorCommand : ICommand
    {
        public EntryViewPageVM VM { get; set; }

        public LaunchShiftCreatorCommand(EntryViewPageVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            VM.LaunchShiftCreator();
        }
    }
}
