using Olympus.ViewModels.Commands;
using Olympus.Views;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Olympus.ViewModels.Components;

public class UserHandlerVM : INotifyPropertyChanged
{
    public OlympusVM ParentVM { get; set; }
    private string userGreeting;
    public string UserGreeting
    {
        get => userGreeting;
        set
        {
            userGreeting = value;
            OnPropertyChanged(nameof(UserGreeting));
        }
    }
    private string buttonString;
    public string ButtonString
    {
        get => buttonString;
        set
        {
            buttonString = value;
            OnPropertyChanged(nameof(ButtonString));
        }
    }

    public UserCommand UserCommand { get; set; }

    public UserHandlerVM()
    {
        CheckUser();
        UserCommand = new UserCommand(this);
    }

    public UserHandlerVM(OlympusVM olympusVM) : this()
    {
        ParentVM = olympusVM;
        Task.Run(LogIn);
    }

    public void CheckUser()
    {
        if (App.Charon.User is null)
        {
            UserGreeting = "Who are you, and what are you doing here?";
            ButtonString = App.Helios.UserReader.UserCount() == 0 ? "Register" : "Log In";
        }
        else
        {
            UserGreeting = $"Hello, {App.Charon.Employee?.DisplayName ?? "... you?"}.";
            ButtonString = "Log Out";
        }
    }

    public void Register()
    {
        var user = App.Charon.User;
        AlphaRegistrationWindow alpha = new();
        _ = alpha.ShowDialog();
        CheckUser();
        if (user != App.Charon.User) ParentVM.ClearRunningProjects();
    }

    public void LogIn()
    {
        var user = App.Charon.User;
        LoginWindow login = new();
        _ = login.ShowDialog();
        CheckUser();
        if (user != App.Charon.User) ParentVM.ClearRunningProjects();
    }

    public void LogOut()
    {
        var user = App.Charon.User;
        App.Charon.LogOut();
        CheckUser();
        if (user != App.Charon.User) ParentVM.ClearRunningProjects();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}