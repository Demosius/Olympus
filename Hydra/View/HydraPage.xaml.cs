using System;
using System.Windows;
using Styx;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Hydra.View;

/// <summary>
/// Interaction logic for HydraPage.xaml
/// </summary>
public partial class HydraPage : IProject
{
    public HydraPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        
    }

    public EProject Project => EProject.Hydra;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    private void ActionToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ZoneToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
    }

    private void ZoneToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ActionToggle.IsChecked = false;
        SiteToggle.IsChecked = false;
    }

    private void SiteToggle_OnChecked(object sender, RoutedEventArgs e)
    {
        ZoneToggle.IsChecked = false;
        ActionToggle.IsChecked = false;
    }
}