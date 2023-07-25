using Olympus.ViewModels.Commands;
using Styx;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Olympus.ViewModels;

internal class ChangePasswordVM : INotifyPropertyChanged
{
    public Charon Charon { get; set; }

    private string currentPassword;
    public string CurrentPassword
    {
        get => currentPassword;
        set
        {
            currentPassword = value;
            OnPropertyChanged();
        }
    }

    private string newPassword;
    public string NewPassword
    {
        get => newPassword;
        set
        {
            newPassword = value;
            OnPropertyChanged();
        }
    }

    private string confirmPassword;
    public string ConfirmPassword
    {
        get => confirmPassword;
        set
        {
            confirmPassword = value;
            OnPropertyChanged();
        }
    }

    public ConfirmPasswordChangeCommand ConfirmPasswordChangeCommand { get; set; }

    public ChangePasswordVM(Charon charon)
    {
        Charon = charon;

        currentPassword = string.Empty;
        newPassword = string.Empty;
        confirmPassword = string.Empty;

        ConfirmPasswordChangeCommand = new ConfirmPasswordChangeCommand(this);
    }

    public bool CheckPassword()
    {
        return NewPassword == ConfirmPassword &&
               NewPassword.Length >= 6 &&
               !NewPassword.Any(char.IsWhiteSpace);
    }

    public bool ChangePassword()
    {
        var success = Charon.ChangePassword(NewPassword, ConfirmPassword, out var message);

        if (!success)
            MessageBox.Show(message, "Password Change Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        else
        {
            if (message is null or "") message = "Success!";
            MessageBox.Show(message, "Password Change Successful", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        return success;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [Cadmus.Annotations.NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}