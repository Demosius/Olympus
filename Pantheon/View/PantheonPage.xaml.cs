using Uranus.Staff;
using System;
using System.Windows.Navigation;
using Styx;
using Uranus;
using Uranus.Interfaces;

namespace Pantheon.View;

/// <summary>
/// Interaction logic for PantheonPage.xaml
/// </summary>
public partial class PantheonPage : IProject

{
    public PantheonPage(Charon charon, Helios helios)
    {
        InitializeComponent();
        VM.SetDataSources(charon, helios);
    }

    public EProject Project => EProject.Pantheon;

    public static bool RequiresUser => true;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
        if (e.NavigationMode is NavigationMode.Back or NavigationMode.Forward) e.Cancel = true;
    }
}