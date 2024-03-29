﻿using Cadmus.Annotations;
using Olympus.ViewModels.Commands;
using Olympus.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application = System.Windows.Application;

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
            OnPropertyChanged();
        }
    }
    private string buttonString;
    public string ButtonString
    {
        get => buttonString;
        set
        {
            buttonString = value;
            OnPropertyChanged();
        }
    }

    public UserCommand UserCommand { get; set; }

    public UserHandlerVM(OlympusVM olympusVM)
    {
        ParentVM = olympusVM;
        userGreeting = string.Empty;
        buttonString = string.Empty;
        CheckUser();
        UserCommand = new UserCommand(this);
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
        LoginWindow login = new()
        {
            Owner = Application.Current.MainWindow
        };
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

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}