using System;
using Prometheus.ViewModels.Controls;
using Prometheus.ViewModels.PopUp.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.PopUp.Users;

/// <summary>
/// Interaction logic for SetUserRoleView.xaml
/// </summary>
public partial class SetUserRoleView
{
    public SetUserRoleVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }
    public UserVM UserVM { get; set; }

    public SetUserRoleView(Helios helios, Charon charon, UserVM user)
    {
        Helios = helios;
        Charon = charon;
        UserVM = user;
        VM = SetUserRoleVM.CreateEmpty(helios, charon, user);
        InitializeComponent();
        DataContext = VM;
    }


    private async void SetUserRoleView_OnInitialized(object? sender, EventArgs e)
    {
        VM = await SetUserRoleVM.CreateAsync(Helios, Charon, UserVM);
        DataContext = VM;
    }
}