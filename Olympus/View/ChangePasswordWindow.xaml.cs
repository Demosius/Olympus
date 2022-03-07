using System.Windows;
using Styx;

namespace Olympus.View;

/// <summary>
/// Interaction logic for ChangePasswordWindow.xaml
/// </summary>
public partial class ChangePasswordWindow
{
    public ChangePasswordWindow(Charon charon)
    {
        InitializeComponent();
        VM.Charon = charon;
    }

    private void CurrentPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        VM.CurrentPassword = CurrentPassword.Password;
    }

    private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {

        VM.NewPassword = NewPassword.Password;
    }

    private void ConfirmPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        VM.ConfirmPassword = ConfirmPassword.Password;
    }
}