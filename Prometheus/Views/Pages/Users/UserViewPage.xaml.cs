using System;
using System.Windows;
using Prometheus.ViewModels.Pages.Users;
using Styx;
using Uranus;

namespace Prometheus.Views.Pages.Users;

/// <summary>
/// Interaction logic for UserViewPage.xaml
/// </summary>
public partial class UserViewPage
{
    public UserViewVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public UserViewPage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void UserViewPage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await UserViewVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }
}