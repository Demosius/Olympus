using Olympus.ViewModels;
using Styx;
using System.Windows;

namespace Olympus.Views;

/// <summary>
/// Interaction logic for ChangePasswordWindow.xaml
/// </summary>
public partial class ChangePasswordWindow
{
    public ChangePasswordWindow(Charon charon)
    {
        InitializeComponent();
        DataContext = new ChangePasswordVM(charon);
    }

    private void CurrentPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        ((ChangePasswordVM)DataContext).CurrentPassword = CurrentPassword.Password;
    }

    private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {

        ((ChangePasswordVM)DataContext).NewPassword = NewPassword.Password;
    }

    private void ConfirmPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        ((ChangePasswordVM)DataContext).ConfirmPassword = ConfirmPassword.Password;
    }
}