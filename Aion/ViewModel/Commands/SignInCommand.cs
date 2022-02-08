using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class SignInCommand : ICommand
    {
        public SignInVM VM { get; set; }

        public SignInCommand(SignInVM vm) { VM = vm; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return VM.Code?.Length > 0 && VM.SelectedManager != null;
        }

        public void Execute(object parameter)
        {
            var w = (Window)parameter;
            if (VM.SignIn())
            {
                if (w == null) return;
                w.DialogResult = true;
                w.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Code for Selected Manager", "Fail", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
