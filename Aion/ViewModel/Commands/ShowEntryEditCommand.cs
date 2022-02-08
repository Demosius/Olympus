using Aion.View;
using System;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ShowEntryEditCommand : ICommand
    {
        public AionVM VM { get; set; }

        public ShowEntryEditCommand(AionVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.CurrentPage != VM.ShiftEntryPage || VM.CurrentPage is null;
        }

        public void Execute(object parameter)
        {
            SignInWindow signIn = new();
            if (signIn.ShowDialog() == true)
            {
                VM.ShowEntryPage();
            }
        }
    }
}
