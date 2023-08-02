using System;
using Prometheus.ViewModels.Pages.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages.Users;

/// <summary>
/// Interaction logic for UserActivatePage.xaml
/// </summary>
public partial class UserActivatePage
{
    public UserActivateVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public UserActivatePage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void UserActivatePage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await UserActivateVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}