using System;
using System.Windows;
using System.Windows.Input;

namespace Aion.ViewModel.Commands
{
    public class ConfirmEmployeeEditCommand : ICommand
    {
        public EmployeeEditorVM EditorVM { get; set; }

        public ConfirmEmployeeEditCommand(EmployeeEditorVM editorVM) { EditorVM = editorVM; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return EditorVM.ReportsTo != null;
        }

        public void Execute(object parameter)
        {
            var w = (Window)parameter;
            EditorVM.ConfirmEdit();
            if (w == null) return;
            w.DialogResult = true;
            w.Close();
        }
    }
}
