using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Olympus;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;

namespace Olympus.Components
{
    /// <summary>
    /// Interaction logic for DB_Selection.xaml
    /// </summary>
    public partial class DatabaseSelection : UserControl
    {
        public string DBString { get; set; }

        public DatabaseSelection()
        {
            InitializeComponent();
            SetDisplay();
        }

        private void SetDisplay()
        {
            DBString = $"DB Location: {Toolbox.GetSol()}";
        }

        private void ChangeDBCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ChangeDBCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog { SelectedPath = Toolbox.GetSol() };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                DBString = dialog.SelectedPath;
                MessageBox.Show(DBString);
            }
        }
    }

    public class DBSViewModel : INotifyPropertyChanged
    {
        public string DBString { get; set; }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }

    public static partial class Commands
    {
        public static readonly RoutedUICommand ChangeDatabase = new RoutedUICommand
            (
                "Change Database",
                "ChangeDatabase",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D, ModifierKeys.Control)
                }
            );
    }
}
