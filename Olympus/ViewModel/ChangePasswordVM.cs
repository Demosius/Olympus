using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Aion.Properties;
using Olympus.ViewModel.Commands;
using Styx;

namespace Olympus.ViewModel;

internal class ChangePasswordVM : INotifyPropertyChanged
{
    private Charon charon;
    public Charon Charon
    {
        get => charon;
        set
        {
            charon = value;
            OnPropertyChanged(nameof(Charon));
        }
    }
        
    private string currentPassword;
    public string CurrentPassword
    {
        get => currentPassword;
        set
        {
            currentPassword = value;
            OnPropertyChanged(nameof(CurrentPassword));
        }
    }

    private string newPassword;
    public string NewPassword
    {
        get => newPassword;
        set
        {
            newPassword = value;
            OnPropertyChanged(nameof(NewPassword));
        }
    }

    private string confirmPassword;
    public string ConfirmPassword
    {
        get => confirmPassword;
        set
        {
            confirmPassword = value;
            OnPropertyChanged(nameof(ConfirmPassword));
        }
    }

    public ConfirmPasswordChangeCommand ConfirmPasswordChangeCommand { get; set; }

    public ChangePasswordVM()
    {
        ConfirmPasswordChangeCommand = new ConfirmPasswordChangeCommand(this);
    }

    public bool CheckPassword()
    {
        return NewPassword is not null && ConfirmPassword is not null && 
               NewPassword == ConfirmPassword &&
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

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}