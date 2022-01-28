using System.Windows;
using System.Windows.Controls;

namespace Olympus.View
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (VM != null) { VM.Password = ((PasswordBox)sender).Password; }
        }

    }

}
