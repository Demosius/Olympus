using System;
using Hydra.ViewModels;
using Styx;
using System.Threading.Tasks;
using System.Windows;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Hydra.Views;

/// <summary>
/// Interaction logic for HydraPage.xaml
/// </summary>
public partial class HydraPage : IProject
{
    public HydraVM? VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public HydraPage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        InitializeComponent();
    }

    private async void HydraPage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await HydraVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }

    public EProject Project => EProject.Hydra;

    public static bool RequiresUser => false;

    public async Task RefreshDataAsync()
    {
        await Task.Run(() => {});
    }

    private void ActionToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ZoneToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
        LevelsToggle.IsChecked = false;
    }

    private void ZoneToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ActionToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
        LevelsToggle.IsChecked = false;
    }

    private void SiteToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ZoneToggle.IsChecked = false;
        ActionToggle.IsChecked = false;
        LevelsToggle.IsChecked = false;
    }

    private void LevelsToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ActionToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
        ZoneToggle.IsChecked = false;
    }
}