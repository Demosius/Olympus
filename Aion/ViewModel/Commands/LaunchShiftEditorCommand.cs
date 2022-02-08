﻿using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class LaunchShiftEditorCommand : ICommand
    {
        public EntryViewPageVM VM { get; set; }

        public LaunchShiftEditorCommand(EntryViewPageVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.SelectedEntry != null;
        }

        public void Execute(object parameter)
        {
            VM.LaunchShiftEditor();
        }
    }
}