using System.Windows;
using System.Windows.Controls;
using Olympus.Properties;
using Styx;


namespace Olympus.View.Components
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class UserHandler : UserControl
    {
        public Charon Charon { get; set; }

        public UserHandler()
        {
            InitializeComponent();
            Charon = new(Settings.Default.SolLocation);
        }
         
        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new();
            _ = login.ShowDialog();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var userCount = App.Helios.UserReader.UserCount();
            if (userCount == 0)
            {
                AlphaRegistrationWindow regForm = new();
                _ = regForm.ShowDialog();
            }
            else
            {
                RegisterWindow regForm = new();
                _ = regForm.ShowDialog();
            }
        }
    }
}
