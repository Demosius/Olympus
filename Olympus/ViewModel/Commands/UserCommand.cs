using Olympus.ViewModel.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Olympus.ViewModel.Commands
{
    public class UserCommand : ICommand
    {
        public UserHandlerVM VM { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public UserCommand(UserHandlerVM vm)
        {
            VM = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string commandName = parameter as string;
            if (commandName == "Register")
                VM.Register();
            else if (commandName == "Log In")
                VM.LogIn();
            else
                VM.LogOut();
        }
    }
}
