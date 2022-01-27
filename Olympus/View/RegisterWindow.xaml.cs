using System.Windows;

namespace Olympus.View
{
    /// <summary>
    /// Interaction logic for Registerxaml.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }

}

