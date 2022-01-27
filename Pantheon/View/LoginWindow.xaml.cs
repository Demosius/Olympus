using System.Windows;
using System.Windows.Controls;

namespace Pantheon.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (VM is not null) { VM.Password = ((PasswordBox)sender).Password; }
        }
    }
}
