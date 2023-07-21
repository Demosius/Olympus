using Cadmus.Annotations;
using Olympus.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Olympus.ViewModels;

public class LogInVM : INotifyPropertyChanged
{
    private string userID;
    public string UserID
    {
        get => userID;
        set
        {
            userID = value;
            OnPropertyChanged();
        }
    }

    private string password = "";
    public string Password
    {
        get => password;
        set
        {
            password = value;
            OnPropertyChanged();
        }
    }

    public LogInCommand LogInCommand { get; set; }

    public LogInVM()
    {
        userID = string.Empty;

        LogInCommand = new LogInCommand(this);
    }

    public async Task<bool> LogIn()
    {
        return int.TryParse(UserID, out var id) && await App.Charon.LogInAsync(id, Password);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}